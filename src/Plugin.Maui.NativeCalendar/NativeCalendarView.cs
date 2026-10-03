using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;

namespace Plugin.Maui.NativeCalendar
{
    public class NativeCalendarView : View
    {
        WeakCollectionChangedProxy? eventsProxy;

        public NativeCalendarView()
        {
            // The property-changed callback does not run for the default collection, so
            // observe it here.
            if (Events is INotifyCollectionChanged defaultEvents)
                eventsProxy = new WeakCollectionChangedProxy(defaultEvents, this);
        }

        #region Bindable Properties

        public static readonly BindableProperty EventsProperty = BindableProperty.Create(
            propertyName: nameof(Events),
            returnType: typeof(IEnumerable<NativeCalendarEvent>),
            declaringType: typeof(NativeCalendarView),
            // A defaultValueCreator gives every instance its own collection. A plain
            // defaultValue is a single shared instance across all NativeCalendarView objects.
            defaultValueCreator: _ => new ObservableCollection<NativeCalendarEvent>(),
            defaultBindingMode: BindingMode.OneWay,
            propertyChanged: OnEventsPropertyChanged
        );
        public IEnumerable<NativeCalendarEvent> Events
        {
            get => (IEnumerable<NativeCalendarEvent>)GetValue(EventsProperty);
            set => SetValue(EventsProperty, value);
        }

        // Keep the platform view in sync when the bound collection itself is mutated
        // (items added or removed), not only when the Events property is reassigned.
        static void OnEventsPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is not NativeCalendarView view)
                return;

            view.eventsProxy?.Unsubscribe();
            view.eventsProxy = null;

            // The proxy holds this view weakly, so a long-lived collection (for example one
            // owned by a singleton view model) does not keep the view and its page alive.
            if (newValue is INotifyCollectionChanged newObservable)
                view.eventsProxy = new WeakCollectionChangedProxy(newObservable, view);
        }

        void OnEventsCollectionChanged()
        {
            // Collections can be changed from a background thread. The platform view is not
            // safe to touch from there.
            if (Dispatcher is { IsDispatchRequired: true } dispatcher)
            {
                dispatcher.Dispatch(OnEventsCollectionChanged);
                return;
            }

            // The Events reference did not change, so re-run the platform mapper by hand.
            Handler?.UpdateValue(nameof(Events));
        }

        // Selected Date Bindable Property
        public static readonly BindableProperty SelectedDateProperty = BindableProperty.Create(
            propertyName: nameof(SelectedDate),
            returnType: typeof(DateTime),
            declaringType: typeof(NativeCalendarView),
            // A plain defaultValue is evaluated once per process, so it goes stale in an app
            // that stays open past midnight.
            defaultValueCreator: _ => DateTime.Today,
            defaultBindingMode: BindingMode.TwoWay,
            coerceValue: CoerceSelectedDate,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                if (bindable is NativeCalendarView nativeCalendarView)
                {
                    var args = new DateChangedEventArgs((DateTime)oldValue, (DateTime)newValue);

                    nativeCalendarView.DateChanged?.Invoke(nativeCalendarView, args);

                    var command = nativeCalendarView.DateChangedCommand;
                    if (command?.CanExecute(args) == true)
                        command.Execute(args);
                }
            }
        );

        public DateTime SelectedDate
        {
            get => (DateTime)GetValue(SelectedDateProperty);
            set => SetValue(SelectedDateProperty, value);
        }

        // The calendar selects whole days, so drop the time of day, and keep the selection
        // inside the MinimumDate to MaximumDate range like the MAUI DatePicker does.
        static object CoerceSelectedDate(BindableObject bindable, object value)
        {
            var date = ((DateTime)value).Date;

            if (bindable is not NativeCalendarView view)
                return date;

            var minimum = view.MinimumDate.Date;
            var maximum = view.MaximumDate.Date;

            // An inverted range has no valid day. Leave the selection alone.
            if (minimum > maximum)
                return date;

            if (date < minimum)
                return minimum;

            if (date > maximum)
                return maximum;

            return date;
        }

        static void OnDateRangeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is not NativeCalendarView view)
                return;

            // BindableObject.CoerceValue does not store the result, so set the value here.
            var selectedDate = view.SelectedDate;
            var coercedDate = (DateTime)CoerceSelectedDate(view, selectedDate);

            if (coercedDate != selectedDate)
                view.SelectedDate = coercedDate;
        }

        // Maximum Date Bindable Property
        public static readonly BindableProperty MaximumDateProperty = BindableProperty.Create(
            propertyName: nameof(MaximumDate),
            returnType: typeof(DateTime),
            declaringType: typeof(NativeCalendarView),
            defaultValue: DateTime.MaxValue,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: OnDateRangeChanged
        );

        public DateTime MaximumDate
        {
            get => (DateTime)GetValue(MaximumDateProperty);
            set => SetValue(MaximumDateProperty, value);
        }

        // Minimum Date Bindable Property
        public static readonly BindableProperty MinimumDateProperty = BindableProperty.Create(
            propertyName: nameof(MinimumDate),
            returnType: typeof(DateTime),
            declaringType: typeof(NativeCalendarView),
            defaultValue: DateTime.MinValue,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: OnDateRangeChanged
        );
        public DateTime MinimumDate
        {
            get => (DateTime)GetValue(MinimumDateProperty);
            set => SetValue(MinimumDateProperty, value);
        }

        // A null color means "use the platform default": the TintColor for the event
        // indicator, and the app accent color for the tint.
        public static readonly BindableProperty EventIndicatorColorProperty = BindableProperty.Create(
            propertyName: nameof(EventIndicatorColor),
            returnType: typeof(Color),
            declaringType: typeof(NativeCalendarView),
            defaultValue: null,
            defaultBindingMode: BindingMode.TwoWay
        );

        public Color? EventIndicatorColor
        {
            get => (Color?)GetValue(EventIndicatorColorProperty);
            set => SetValue(EventIndicatorColorProperty, value);
        }

        public static readonly BindableProperty DateChangedCommandProperty = BindableProperty.Create(
            propertyName: nameof(DateChangedCommand),
            returnType: typeof(ICommand),
            declaringType: typeof(NativeCalendarView),
            defaultValue: null
        );

        public ICommand? DateChangedCommand
        {
            get => (ICommand?)GetValue(DateChangedCommandProperty);
            set => SetValue(DateChangedCommandProperty, value);
        }

        public static readonly BindableProperty TintColorProperty = BindableProperty.Create(
            propertyName: nameof(TintColor),
            returnType: typeof(Color),
            declaringType: typeof(NativeCalendarView),
            defaultValue: null,
            defaultBindingMode: BindingMode.TwoWay
        );
        public Color? TintColor
        {
            get => (Color?)GetValue(TintColorProperty);
            set => SetValue(TintColorProperty, value);
        }


        #endregion



        public event EventHandler<DateChangedEventArgs>? DateChanged;

        sealed class WeakCollectionChangedProxy
        {
            readonly INotifyCollectionChanged source;
            readonly WeakReference<NativeCalendarView> target;

            public WeakCollectionChangedProxy(INotifyCollectionChanged source, NativeCalendarView target)
            {
                this.source = source;
                this.target = new WeakReference<NativeCalendarView>(target);
                source.CollectionChanged += OnCollectionChanged;
            }

            public void Unsubscribe() => source.CollectionChanged -= OnCollectionChanged;

            void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
            {
                if (target.TryGetTarget(out var view))
                    view.OnEventsCollectionChanged();
                else
                    Unsubscribe();
            }
        }
    }
}
