using System.Runtime.CompilerServices;
using Android.Content;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.Fragment.App;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.Button;
using Google.Android.Material.Color;
using Google.Android.Material.DatePicker;
using Java.Lang;
using Microsoft.Maui.Platform;
using Plugin.Maui.NativeCalendar.Extensions;
using AView = Android.Views.View;
using AViewGroup = Android.Views.ViewGroup;
using Color = Android.Graphics.Color;
using Fragment = AndroidX.Fragment.App.Fragment;
using FragmentManager = AndroidX.Fragment.App.FragmentManager;

// MaterialCalendar and SingleDateSelector are public types that Google treats as internal API.
// This file is the only place that touches them, and each lookup into the fragment has a fallback.
#pragma warning disable XAOBS001

namespace Plugin.Maui.NativeCalendar
{
    public class NativeCalendarImplementation : FrameLayout
    {
        private const string FragmentTag = "MaterialCalendar";

        // Tags that MaterialCalendar sets on its own views.
        private const string MonthsViewGroupTag = "MONTHS_VIEW_GROUP_TAG";
        private const string NavigationNextTag = "NAVIGATION_NEXT_TAG";
        private const string NavigationPrevTag = "NAVIGATION_PREV_TAG";
        private const string SelectorToggleTag = "SELECTOR_TOGGLE_TAG";

        // The month range that MaterialCalendar shows when no minimum or maximum date is set.
        private static readonly DateTime DefaultRangeStart = new DateTime(1900, 1, 1);
        private static readonly DateTime DefaultRangeEnd = new DateTime(2100, 12, 31);

        private readonly NativeCalendarView nativeCalendarView;
        private readonly Handler mainHandler = new Handler(Looper.MainLooper!);
        private readonly EventIndicatorDayViewDecorator dayViewDecorator;
        private readonly Dictionary<MaterialButton, ColorStateList?> defaultNavigationIconTints = new();

        // Records which month grids show the current events, colors and selection.
        private ConditionalWeakTable<AView, object> refreshedMonthViews = new();

        private MaterialCalendar? materialCalendarFragment;
        private FragmentManager? fragmentManager;
        private TrackingDateSelector? dateSelector;
        private AView? fragmentView;
        private RecyclerView? monthsRecyclerView;
        private MonthAttachListener? monthAttachListener;
        private MonthScrollListener? monthScrollListener;

        // The first month of the current fragment. Month page N of the fragment is this month plus N.
        private DateTime rangeStartMonth = DefaultRangeStart;
        private DateTime? monthToOpen;

        // The month that ShowMonth asked for. The month list moves to it at its next layout,
        // so the layout position is not correct until then.
        private DateTime? requestedMonth;

        private bool isAttached;
        private bool isDisconnected;
        private bool isDisposed;
        private bool isReconcilePosted;
        private bool isRebuildNeeded;
        private bool isSelectingFromPlatform;

        // The Java peer is gone once the handle is zero, even if Dispose has not run yet.
        private bool IsDisposed => isDisposed || Handle == IntPtr.Zero;

        public NativeCalendarImplementation(Context context, NativeCalendarView nativeCalendarView) : base(context)
        {
            this.nativeCalendarView = nativeCalendarView;

            Id = GenerateViewId();

            dayViewDecorator = new EventIndicatorDayViewDecorator(nativeCalendarView);

            SetOnHierarchyChangeListener(new ChildViewListener(this));
        }

        public void UpdateTintColor(NativeCalendarView nativeCalendarView)
        {
            dayViewDecorator.Invalidate();
            ApplyNavigationTint();
            RefreshDays();
        }

        public void UpdateSelectedDate(NativeCalendarView nativeCalendarView)
        {
            // The change came from a tap on the calendar, which already shows it.
            if (isSelectingFromPlatform)
                return;

            var selectedDate = nativeCalendarView.SelectedDate;

            if (dateSelector is null)
            {
                // There is no fragment at this time. The next one opens at the new selection.
                monthToOpen = selectedDate;
                return;
            }

            ShowSelection(selectedDate);
        }

        private void ShowSelection(DateTime selectedDate)
        {
            dateSelector?.SelectSilently(selectedDate.ToLongInteger());
            ShowMonth(selectedDate);
            RefreshDays();
        }

        public void UpdateMaximumDate(NativeCalendarView nativeCalendarView)
        {
            RebuildFragment();
        }

        public void UpdateMinimumDate(NativeCalendarView nativeCalendarView)
        {
            RebuildFragment();
        }

        public void UpdateEvents(NativeCalendarView nativeCalendarView)
        {
            dayViewDecorator.Invalidate();
            RefreshDays();
        }

        /// <summary>
        /// Removes the calendar fragment. The handler calls this when it disconnects.
        /// </summary>
        public void Disconnect()
        {
            isDisconnected = true;

            // The handler can disconnect while a fragment transaction runs, for example during
            // navigation. A posted removal runs after that transaction.
            ScheduleReconcile();
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            // The month grid of MaterialCalendar shows one week if it does not get a definite
            // height. The date picker dialog gives it one, so do the same here.
            if (MeasureSpec.GetMode(heightMeasureSpec) != Android.Views.MeasureSpecMode.Exactly)
            {
                var height = System.Math.Max(GetNaturalHeight(), fragmentView?.MinimumHeight ?? 0);

                if (MeasureSpec.GetMode(heightMeasureSpec) == Android.Views.MeasureSpecMode.AtMost)
                    height = System.Math.Min(height, MeasureSpec.GetSize(heightMeasureSpec));

                heightMeasureSpec = MeasureSpec.MakeMeasureSpec(height, Android.Views.MeasureSpecMode.Exactly);
            }

            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
        }

        // The height of the month navigation, the weekday names and six weeks. This is the
        // height that MaterialDatePicker gives its calendar.
        private int GetNaturalHeight()
        {
            var resources = Resources;
            if (resources is null)
                return 0;

            try
            {
                const int maximumWeeks = 6;

                return resources.GetDimensionPixelSize(Resource.Dimension.mtrl_calendar_navigation_height)
                     + resources.GetDimensionPixelOffset(Resource.Dimension.mtrl_calendar_navigation_top_padding)
                     + resources.GetDimensionPixelOffset(Resource.Dimension.mtrl_calendar_navigation_bottom_padding)
                     + resources.GetDimensionPixelSize(Resource.Dimension.mtrl_calendar_days_of_week_height)
                     + (maximumWeeks * resources.GetDimensionPixelSize(Resource.Dimension.mtrl_calendar_day_height))
                     + ((maximumWeeks - 1) * resources.GetDimensionPixelOffset(Resource.Dimension.mtrl_calendar_month_vertical_padding))
                     + resources.GetDimensionPixelOffset(Resource.Dimension.mtrl_calendar_bottom_padding);
            }
            catch (Android.Content.Res.Resources.NotFoundException)
            {
                return 0;
            }
        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();

            isAttached = true;
            ScheduleReconcile();
        }

        protected override void OnDetachedFromWindow()
        {
            // Remember the month on screen. The view can attach again, for example when the
            // user comes back to a Shell tab or to a page below this one.
            monthToOpen ??= GetDisplayedMonth();

            isAttached = false;
            ScheduleReconcile();

            base.OnDetachedFromWindow();
        }

        protected override void OnConfigurationChanged(Configuration? newConfig)
        {
            base.OnConfigurationChanged(newConfig);

            // A MAUI activity handles a change of dark mode, locale or font scale without a
            // restart. The fragment keeps the colors and texts of the old configuration, so
            // make a new one.
            dayViewDecorator.Invalidate();
            RebuildFragment();
        }

        private void OnFragmentViewAdded(AView child)
        {
            // The fragment manager adds the fragment view to this container. This is the
            // earliest point where the views of the calendar exist.
            if (!IsDisposed)
                HookFragmentView(child);
        }

        private void OnFragmentViewRemoved(AView child)
        {
            if (!ReferenceEquals(child, fragmentView))
                return;

            // The host fragment destroyed the calendar view, for example for a page push.
            monthToOpen ??= GetDisplayedMonth();
            UnhookFragmentView();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !isDisposed)
            {
                isDisposed = true;
                RemoveFragment();
            }

            base.Dispose(disposing);
        }

        // The MAUI property mappers run one after another, and attach and detach can come in
        // quick succession. One posted pass looks at the final state and makes the fragment match it.
        private void ScheduleReconcile()
        {
            if (isReconcilePosted)
                return;

            isReconcilePosted = true;
            mainHandler.Post(Reconcile);
        }

        private void RebuildFragment()
        {
            isRebuildNeeded = true;
            ScheduleReconcile();
        }

        private void Reconcile()
        {
            isReconcilePosted = false;

            if (isDisposed)
                return;

            if (Handle == IntPtr.Zero || isDisconnected || !isAttached)
            {
                monthToOpen ??= GetDisplayedMonth();
                RemoveFragment();
                return;
            }

            if (!isRebuildNeeded && IsFragmentShown())
            {
                // The view went away and came back before this pass. The fragment is intact.
                monthToOpen = null;
                return;
            }

            ShowFragment();
        }

        private bool IsFragmentShown()
        {
            return materialCalendarFragment is { IsAdded: true }
                && fragmentView is not null
                && IndexOfChild(fragmentView) >= 0;
        }

        private void ShowFragment()
        {
            var manager = ResolveFragmentManager();
            if (manager is null || manager.IsDestroyed)
                return;

            // A rebuild keeps the month that is on screen, unless a caller asked for another one.
            var openAt = monthToOpen ?? GetDisplayedMonth() ?? nativeCalendarView.SelectedDate;
            monthToOpen = null;

            RemoveFragment();

            var selector = new TrackingDateSelector(OnDaySelectedByUser);
            selector.SelectSilently(nativeCalendarView.SelectedDate.ToLongInteger());

            var fragment = MaterialCalendar.NewInstance(selector, ResolveThemeResId(selector), GenerateCalendarConstraints(openAt), dayViewDecorator);

            dateSelector = selector;
            materialCalendarFragment = fragment;
            fragmentManager = manager;
            isRebuildNeeded = false;

            if (!Commit(manager.BeginTransaction().Replace(Id, fragment, FragmentTag + Id)))
            {
                materialCalendarFragment = null;
                fragmentManager = null;
                dateSelector = null;
            }
        }

        private void RemoveFragment()
        {
            var fragment = materialCalendarFragment;
            var manager = fragmentManager;

            UnhookFragmentView();

            materialCalendarFragment = null;
            fragmentManager = null;
            dateSelector = null;

            if (fragment is null || manager is null)
                return;

            try
            {
                // A destroyed manager has already removed its fragments.
                if (!manager.IsDestroyed && fragment.IsAdded)
                    Commit(manager.BeginTransaction().Remove(fragment));
            }
            catch (ObjectDisposedException)
            {
                // The manager or the fragment peer is gone. There is nothing to remove.
            }
        }

        // This runs from a posted callback, where an exception stops the app. A calendar that
        // does not show is the better failure.
        private static bool Commit(FragmentTransaction transaction)
        {
            try
            {
                transaction.CommitNowAllowingStateLoss();
                return true;
            }
            catch (IllegalStateException)
            {
                // The manager is in the middle of another transaction. Queue this one behind it.
                try
                {
                    transaction.CommitAllowingStateLoss();
                    return true;
                }
                catch (Java.Lang.Exception)
                {
                    // The host is gone, and its fragments with it.
                    return false;
                }
            }
            catch (Java.Lang.Exception)
            {
                // The container is not in the views of the host, or the theme of the app
                // cannot inflate the calendar.
                return false;
            }
        }

        // The fragment manager finds the container by its id, inside the views of its own host.
        // A page inside a Shell, a NavigationPage or a modal lives in a fragment, so the nearest
        // fragment is the correct host. The activity cannot see the views of a modal page.
        private FragmentManager? ResolveFragmentManager()
        {
            try
            {
                if (FragmentManager.FindFragment(this) is Fragment { IsAdded: true } host)
                    return host.ChildFragmentManager;
            }
            catch (IllegalStateException)
            {
                // This view is not inside a fragment.
            }

            return Context?.GetFragmentManager();
        }

        // MaterialDatePicker gives its calendar this theme overlay. Without it the day cells and
        // the navigation buttons have no calendar style.
        private int ResolveThemeResId(SingleDateSelector selector)
        {
            try
            {
                return selector.GetDefaultThemeResId(Context);
            }
            catch (Java.Lang.Exception)
            {
                // The app theme is not a Material theme. The calendar falls back to the host theme.
                return 0;
            }
        }

        private void HookFragmentView(AView view)
        {
            UnhookFragmentView();

            fragmentView = view;
            monthsRecyclerView = view.FindViewWithTag(MonthsViewGroupTag) as RecyclerView;

            if (monthsRecyclerView is not null)
            {
                monthAttachListener = new MonthAttachListener(this);
                monthsRecyclerView.AddOnChildAttachStateChangeListener(monthAttachListener);

                monthScrollListener = new MonthScrollListener(this);
                monthsRecyclerView.AddOnScrollListener(monthScrollListener);
            }

            ApplyNavigationTint();

            // The control had no content to measure until now.
            ((IView)nativeCalendarView).InvalidateMeasure();
        }

        private void UnhookFragmentView()
        {
            try
            {
                if (monthsRecyclerView is not null && monthAttachListener is not null)
                    monthsRecyclerView.RemoveOnChildAttachStateChangeListener(monthAttachListener);

                if (monthsRecyclerView is not null && monthScrollListener is not null)
                    monthsRecyclerView.RemoveOnScrollListener(monthScrollListener);
            }
            catch (ObjectDisposedException)
            {
                // The fragment view was already torn down.
            }

            monthAttachListener = null;
            monthScrollListener = null;
            monthsRecyclerView = null;
            requestedMonth = null;
            fragmentView = null;
            defaultNavigationIconTints.Clear();
            refreshedMonthViews = new ConditionalWeakTable<AView, object>();
        }

        private DateTime? GetDisplayedMonth()
        {
            if (requestedMonth is not null)
                return requestedMonth;

            if (monthsRecyclerView?.GetLayoutManager() is not LinearLayoutManager layoutManager)
                return null;

            var position = layoutManager.FindFirstVisibleItemPosition();

            return position < 0 ? null : rangeStartMonth.AddMonths(position);
        }

        private void ShowMonth(DateTime date)
        {
            if (materialCalendarFragment is null)
                return;

            var adapter = monthsRecyclerView?.GetAdapter();

            if (monthsRecyclerView?.GetLayoutManager() is not LinearLayoutManager layoutManager || adapter is null)
            {
                // The fragment has no month list that this class knows. Open a new fragment at the month.
                monthToOpen = date;
                RebuildFragment();
                return;
            }

            // A new fragment opens at this month if the range changes before the list moves.
            requestedMonth = date;

            var position = ((date.Year - rangeStartMonth.Year) * 12) + date.Month - rangeStartMonth.Month;
            position = System.Math.Clamp(position, 0, System.Math.Max(0, adapter.ItemCount - 1));

            // MaterialCalendar listens to the scroll and updates its month title from it.
            if (position != layoutManager.FindFirstVisibleItemPosition())
                monthsRecyclerView.ScrollToPosition(position);
        }

        // Makes each month grid ask the decorator about its days again.
        private void RefreshDays()
        {
            if (monthsRecyclerView is null)
                return;

            // Month views that are off screen stay in the cache of the list. They refresh when
            // they attach again, see OnMonthViewAttached.
            refreshedMonthViews = new ConditionalWeakTable<AView, object>();

            for (int i = 0; i < monthsRecyclerView.ChildCount; i++)
            {
                if (monthsRecyclerView.GetChildAt(i) is AView monthView)
                    RefreshMonthView(monthView);
            }
        }

        private void OnMonthViewAttached(AView monthView)
        {
            if (IsDisposed || refreshedMonthViews.TryGetValue(monthView, out _))
                return;

            RefreshMonthView(monthView);
        }

        private void RefreshMonthView(AView monthView)
        {
            refreshedMonthViews.AddOrUpdate(monthView, monthView);
            InvalidateGrids(monthView);
        }

        private static void InvalidateGrids(AView view)
        {
            if (view is GridView gridView)
            {
                gridView.InvalidateViews();
            }
            else if (view is AViewGroup viewGroup)
            {
                for (int i = 0; i < viewGroup.ChildCount; i++)
                {
                    if (viewGroup.GetChildAt(i) is AView child)
                        InvalidateGrids(child);
                }
            }
        }

        private void ApplyNavigationTint()
        {
            if (fragmentView is null)
                return;

            var tintColor = nativeCalendarView.TintColor;

            foreach (var tag in new[] { NavigationNextTag, NavigationPrevTag, SelectorToggleTag })
            {
                if (fragmentView.FindViewWithTag(tag) is not MaterialButton button)
                    continue;

                if (!defaultNavigationIconTints.TryGetValue(button, out var defaultTint))
                    defaultNavigationIconTints[button] = defaultTint = button.IconTint;

                button.IconTint = tintColor is null ? defaultTint : ColorStateList.ValueOf(tintColor.ToPlatform());
            }
        }

        private void OnDaySelectedByUser(long utcMilliseconds)
        {
            if (IsDisposed)
                return;

            isSelectingFromPlatform = true;

            var tappedDate = utcMilliseconds.ToCalendarDate();

            try
            {
                nativeCalendarView.SelectedDate = tappedDate;
            }
            finally
            {
                isSelectingFromPlatform = false;
            }

            // A DateChanged handler or a view model can set a different date in response.
            // The calendar must show the date that the view has.
            var selectedDate = nativeCalendarView.SelectedDate;

            if (selectedDate != tappedDate)
                ShowSelection(selectedDate);
        }

        private void OnMonthsScrolled()
        {
            // The user, or MaterialCalendar itself, moved the list to a different month.
            requestedMonth = null;
        }

        private CalendarConstraints GenerateCalendarConstraints(DateTime openAt)
        {
            var minimumDate = nativeCalendarView.MinimumDate.Date;
            var maximumDate = nativeCalendarView.MaximumDate.Date;

            var hasMinimum = minimumDate > DateTime.MinValue.Date;
            var hasMaximum = maximumDate < DateTime.MaxValue.Date;

            // An inverted range has no valid day. Show the calendar with no limits.
            if (hasMinimum && hasMaximum && minimumDate > maximumDate)
                hasMinimum = hasMaximum = false;

            openAt = openAt.Date;

            // CalendarConstraints throws when the start is after the end, or when the month to
            // open is outside the two. Make the range hold all three.
            if (hasMinimum && openAt < minimumDate)
                openAt = minimumDate;

            if (hasMaximum && openAt > maximumDate)
                openAt = maximumDate;

            var start = hasMinimum ? minimumDate : Earliest(DefaultRangeStart, openAt);
            var end = hasMaximum ? maximumDate : Latest(DefaultRangeEnd, openAt);

            if (hasMaximum && start > end)
                start = end;

            if (hasMinimum && end < start)
                end = start;

            rangeStartMonth = new DateTime(start.Year, start.Month, 1);

            var constraintsBuilder = new CalendarConstraints.Builder();
            constraintsBuilder.SetStart(start.ToLongInteger());
            constraintsBuilder.SetEnd(end.ToLongInteger());
            constraintsBuilder.SetOpenAt(openAt.ToLongInteger());

            // Both validators include their own date.
            var dateValidators = new List<CalendarConstraints.IDateValidator>();

            if (hasMinimum)
                dateValidators.Add(DateValidatorPointForward.From(minimumDate.ToLongInteger()));

            if (hasMaximum)
                dateValidators.Add(DateValidatorPointBackward.Before(maximumDate.ToLongInteger()));

            if (dateValidators.Count > 0)
                constraintsBuilder.SetValidator(CompositeDateValidator.AllOf(dateValidators));

            return constraintsBuilder.Build();
        }

        private static DateTime Earliest(DateTime first, DateTime second) => first < second ? first : second;

        private static DateTime Latest(DateTime first, DateTime second) => first > second ? first : second;

        // MaterialCalendar calls Select on its date selector when the user taps a valid day.
        private sealed class TrackingDateSelector : SingleDateSelector
        {
            private readonly Action<long>? daySelected;
            private bool isSilent;

            public TrackingDateSelector(Action<long> daySelected)
            {
                this.daySelected = daySelected;
            }

            // Java makes the managed peer through this constructor if the first one is collected.
            public TrackingDateSelector(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
            {
            }

            public void SelectSilently(long selection)
            {
                isSilent = true;

                try
                {
                    Select(selection);
                }
                finally
                {
                    isSilent = false;
                }
            }

            public override void Select(long selection)
            {
                base.Select(selection);

                if (!isSilent)
                    daySelected?.Invoke(selection);
            }
        }

        private sealed class ChildViewListener : Java.Lang.Object, IOnHierarchyChangeListener
        {
            private readonly WeakReference<NativeCalendarImplementation> owner;

            public ChildViewListener(NativeCalendarImplementation owner)
            {
                this.owner = new WeakReference<NativeCalendarImplementation>(owner);
            }

            public void OnChildViewAdded(AView? parent, AView? child)
            {
                if (child is not null && owner.TryGetTarget(out var calendar) && ReferenceEquals(parent, calendar))
                    calendar.OnFragmentViewAdded(child);
            }

            public void OnChildViewRemoved(AView? parent, AView? child)
            {
                if (child is not null && owner.TryGetTarget(out var calendar) && ReferenceEquals(parent, calendar))
                    calendar.OnFragmentViewRemoved(child);
            }
        }

        private sealed class MonthScrollListener : RecyclerView.OnScrollListener
        {
            private readonly WeakReference<NativeCalendarImplementation> owner;

            public MonthScrollListener(NativeCalendarImplementation owner)
            {
                this.owner = new WeakReference<NativeCalendarImplementation>(owner);
            }

            public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
            {
                // A jump from ScrollToPosition reports no distance. A real scroll does.
                if ((dx != 0 || dy != 0) && owner.TryGetTarget(out var calendar))
                    calendar.OnMonthsScrolled();
            }
        }

        private sealed class MonthAttachListener : Java.Lang.Object, RecyclerView.IOnChildAttachStateChangeListener
        {
            private readonly WeakReference<NativeCalendarImplementation> owner;

            public MonthAttachListener(NativeCalendarImplementation owner)
            {
                this.owner = new WeakReference<NativeCalendarImplementation>(owner);
            }

            public void OnChildViewAttachedToWindow(AView view)
            {
                if (owner.TryGetTarget(out var calendar))
                    calendar.OnMonthViewAttached(view);
            }

            public void OnChildViewDetachedFromWindow(AView view)
            {
            }
        }

        private class EventIndicatorDayViewDecorator : DayViewDecorator
        {
            private const float IndicatorSizeInDp = 5;

            // Lifts the indicator off the bottom edge of the day cell, so that it sits inside
            // the circle of a selected day.
            private const float IndicatorBottomInsetInDp = 3;

            public NativeCalendarView NativeCalendarView;

            private bool isStale = true;
            private EventDayIndex eventDays = EventDayIndex.Empty;
            private int indicatorSize;
            private int indicatorBottomInset;
            private Color indicatorColor;
            private Color selectedIndicatorColor;
            private ColorStateList? tintColors;
            private ColorStateList? selectedTextColors;

            public EventIndicatorDayViewDecorator(NativeCalendarView nativeCalendarView)
            {
                NativeCalendarView = nativeCalendarView;
            }

            // Reads the events and colors from the calendar view again at the next draw.
            public void Invalidate()
            {
                isStale = true;
            }

            private void Refresh(Context context)
            {
                if (!isStale)
                    return;

                isStale = false;

                eventDays = new EventDayIndex(NativeCalendarView.Events);
                var density = context.Resources?.DisplayMetrics?.Density ?? 1;
                indicatorSize = (int)System.Math.Round(IndicatorSizeInDp * density);
                indicatorBottomInset = (int)System.Math.Round(IndicatorBottomInsetInDp * density);

                var primary = new Color(MaterialColors.GetColor(context, Resource.Attribute.colorPrimary, Color.Black));
                var onPrimary = new Color(MaterialColors.GetColor(context, Resource.Attribute.colorOnPrimary, Color.White));

                if (NativeCalendarView.TintColor is { } tintColor)
                {
                    var tint = tintColor.ToPlatform();
                    var onTint = tintColor.GetLuminosity() > 0.6f ? Color.Black : Color.White;

                    tintColors = ColorStateList.ValueOf(tint);
                    selectedTextColors = ColorStateList.ValueOf(onTint);
                    selectedIndicatorColor = onTint;
                    indicatorColor = NativeCalendarView.EventIndicatorColor?.ToPlatform() ?? tint;
                }
                else
                {
                    // Leave the day colors to the Material theme.
                    tintColors = null;
                    selectedTextColors = null;
                    selectedIndicatorColor = onPrimary;
                    indicatorColor = NativeCalendarView.EventIndicatorColor?.ToPlatform() ?? primary;
                }
            }

            private static bool IsToday(int year, int month, int day)
            {
                var today = DateTime.Today;
                return year == today.Year && month + 1 == today.Month && day == today.Day;
            }

            public override int DescribeContents()
            {
                return 0;
            }

            public override Drawable? GetCompoundDrawableBottom(Context context, int year, int month, int day, bool valid, bool selected)
            {
                Refresh(context);

                // A day with no event gets a transparent indicator of the same size, so the day
                // numbers of a week stay on one line.
                var color = Color.Transparent;

                if (eventDays.HasEvents(year, month + 1, day))
                    color = selected ? selectedIndicatorColor : indicatorColor;

                var dot = new GradientDrawable();
                dot.SetShape(ShapeType.Oval);
                dot.SetColor(color);

                var drawable = new InsetDrawable(dot, 0, 0, 0, indicatorBottomInset);
                drawable.SetBounds(0, 0, indicatorSize, indicatorSize + indicatorBottomInset);

                return drawable;
            }

            public override ColorStateList? GetBackgroundColor(Context context, int year, int month, int day, bool valid, bool selected)
            {
                Refresh(context);

                if (selected && tintColors is not null)
                    return tintColors;

                return base.GetBackgroundColor(context, year, month, day, valid, selected);
            }

            public override ColorStateList? GetTextColor(Context context, int year, int month, int day, bool valid, bool selected)
            {
                Refresh(context);

                if (tintColors is not null && valid)
                {
                    if (selected)
                        return selectedTextColors;

                    if (IsToday(year, month, day))
                        return tintColors;
                }

                return base.GetTextColor(context, year, month, day, valid, selected);
            }

            public override ICharSequence? GetContentDescriptionFormatted(Context context, int year, int month, int day, bool valid, bool selected, ICharSequence? originalContentDescription)
            {
                Refresh(context);

                // Let a screen reader say the events of the day after the date.
                var titles = eventDays.GetEvents(year, month + 1, day)
                                      .Select(e => e.Title)
                                      .Where(title => !string.IsNullOrWhiteSpace(title))
                                      .ToList();

                if (titles.Count == 0)
                    return originalContentDescription;

                return new Java.Lang.String($"{originalContentDescription}, {string.Join(", ", titles)}");
            }

            public override void WriteToParcel(Parcel? dest, [GeneratedEnum] ParcelableWriteFlags flags)
            {
                // The decorator has no state of its own. It reads from the calendar view.
            }
        }
    }
}
