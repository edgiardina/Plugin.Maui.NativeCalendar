using Android.Content;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using Google.Android.Material.DatePicker;
using Microsoft.Maui.Platform;
using Plugin.Maui.NativeCalendar.Extensions;
using static Google.Android.Material.DatePicker.CalendarConstraints;
using Color = Android.Graphics.Color;
using ImageButton = Android.Widget.ImageButton;
using Paint = Android.Graphics.Paint;
using ShapeDrawable = Android.Graphics.Drawables.ShapeDrawable;

namespace Plugin.Maui.NativeCalendar
{
    // TODO Center text
    // TODO Month and Year Picker and Next/Prev buttons are inverse color

    public class NativeCalendarImplementation : FrameLayout
    {
        private const string FragmentTag = "MaterialCalendar";

        private MaterialCalendar materialCalendarFragment;
        private SingleDateSelector DateSelector;
        private CalendarConstraints calendarConstraints;
        private EventIndicatorDayViewDecorator eventIndicatorDayViewDecorator;

        private readonly NativeCalendarView nativeCalendarView;

        public NativeCalendarImplementation(Context context, NativeCalendarView nativeCalendarView) : base(context)
        {
            this.nativeCalendarView = nativeCalendarView;

            Id = GenerateViewId();

            GenerateCalendarFragmentAndRender();
        }

        public void UpdateTintColor(NativeCalendarView nativeCalendarView)
        {
            GenerateCalendarFragmentAndRender();
        }

        public void UpdateSelectedDate(NativeCalendarView nativeCalendarView)
        {
            materialCalendarFragment?.DateSelector?.Select(nativeCalendarView.SelectedDate.ToLongInteger());
            // TODO: ensure we are inspecting the correct month.
            GenerateCalendarFragmentAndRender();
        }

        public void UpdateMaximumDate(NativeCalendarView nativeCalendarView)
        {
            GenerateCalendarFragmentAndRender();
        }

        public void UpdateMinimumDate(NativeCalendarView nativeCalendarView)
        {
            GenerateCalendarFragmentAndRender();
        }

        public void UpdateEvents(NativeCalendarView nativeCalendarView)
        {
            GenerateCalendarFragmentAndRender();
        }

        private void GenerateCalendarFragmentAndRender()
        {
            // Create a CalendarConstraints object to provide a valid date ran
            Builder constraintsBuilder = new Builder();
            constraintsBuilder.SetOpenAt(nativeCalendarView.SelectedDate.ToLongInteger());

            var dateValidatorMin = DateValidatorPointForward.From(nativeCalendarView.MinimumDate.ToLongInteger());
            var dateValidatorMax = DateValidatorPointBackward.Before(nativeCalendarView.MaximumDate.ToLongInteger());

            var dateValidators = new List<IDateValidator>();

            if (nativeCalendarView.MinimumDate != DateTime.MinValue)
            {
                dateValidators.Add(dateValidatorMin);
                constraintsBuilder.SetStart(nativeCalendarView.MinimumDate.ToLongInteger());
            }

            if (nativeCalendarView.MaximumDate != DateTime.MaxValue)
            {
                dateValidators.Add(dateValidatorMax);
                constraintsBuilder.SetEnd(nativeCalendarView.MaximumDate.ToLongInteger());
            }

            var compositeDateValidator = CompositeDateValidator.AllOf(dateValidators);

            constraintsBuilder.SetValidator(compositeDateValidator);

            calendarConstraints = constraintsBuilder.Build();

            DateSelector = new SingleDateSelector();

            // create dayviewdecorator to add event indicators (small circles)
            eventIndicatorDayViewDecorator = new EventIndicatorDayViewDecorator(nativeCalendarView);

            //int customThemeResId = Resource.Style.CustomMaterialCalendarTheme;

            materialCalendarFragment = MaterialCalendar.NewInstance(DateSelector, 0, calendarConstraints, eventIndicatorDayViewDecorator);

            // TODO: I don't know how to do the event listener for date change, so we are doing it in the DayDecorator, sorry.
            //materialCalendarFragment.AddOnSelectionChangedListener(new MaterialCalendarOnSelectionChangedListener(nativeCalendarView, materialCalendarFragment));

            // Post MaterialCalendar fragment as actual view
            Post(() =>
            {
                Context.GetFragmentManager().BeginTransaction().Replace(Id, materialCalendarFragment, FragmentTag).CommitNow();
            });

            // Wait for the fragment to be fully created before adjusting alignment
            // TODO: replace Center logic with real styles
            PostDelayed(() =>
            {
                CenterCalendarText();

                materialCalendarFragment.View.ViewTreeObserver.GlobalLayout += (sender, args) =>
                {
                    // Trigger centering logic after the layout is updated
                    Post(() =>
                    {
                        CenterCalendarText();
                    });
                };
            }, 25); // Delay in milliseconds to give time for fragment initialization
        }

        private void CenterCalendarText()
        {
            if (materialCalendarFragment?.View is ViewGroup viewGroup)
            {
                for (int i = 0; i < viewGroup.ChildCount; i++)
                {
                    var child = viewGroup.GetChildAt(i);

                    if (child is TextView textView)
                    {
                        // Center the text horizontally and vertically
                        textView.Gravity = GravityFlags.Center;
                    }
                    else if (child is ViewGroup childGroup)
                    {
                        // If the child is a ViewGroup, iterate over its children
                        CenterChildViews(childGroup);
                    }
                }
            }
        }

        private void CenterChildViews(ViewGroup viewGroup)
        {
            for (int i = 0; i < viewGroup.ChildCount; i++)
            {
                var child = viewGroup.GetChildAt(i);

                if (child is TextView textView)
                {
                    // Center the text horizontally and vertically
                    textView.Gravity = GravityFlags.Center;
                }
                else if (child is ViewGroup childGroup)
                {
                    // If the child is a ViewGroup, iterate over its children
                    CenterChildViews(childGroup);
                }
            }
        }

        private class EventIndicatorDayViewDecorator : DayViewDecorator
        {
            public NativeCalendarView NativeCalendarView;

            public EventIndicatorDayViewDecorator(NativeCalendarView nativeCalendarView)
            {
                this.NativeCalendarView = nativeCalendarView;
            }

            public override int DescribeContents()
            {
                throw new NotImplementedException();
            }

            public override Drawable? GetCompoundDrawableBottom(Context context, int year, int month, int day, bool valid, bool selected)
            {
                // Since I can't figure out how to do event listeners on date change, just check here instead
                if (selected)
                {
                    NativeCalendarView.SelectedDate = new DateTime(year, month + 1, day);
                }

                // Check if the current date has an event
                if (NativeCalendarView.Events.Any(e => e.StartDate.Year == year && e.StartDate.Month == month + 1 && e.StartDate.Day == day))
                {
                    // Draw circle
                    ShapeDrawable drawable = new ShapeDrawable(new OvalShape());
                    drawable.Paint.Color = NativeCalendarView.EventIndicatorColor.ToPlatform();
                    drawable.Paint.SetStyle(Paint.Style.Fill);

                    // Set the bounds of the drawable to make sure it appears within the calendar cell
                    int size = 20; // You can adjust this size to control the size of the indicator circle
                    drawable.SetBounds(0, 0, size, size);
                    return drawable;
                }

                return base.GetCompoundDrawableTop(context, year, month, day, valid, selected);
            }

            public override ColorStateList? GetBackgroundColor(Context context, int year, int month, int day, bool valid, bool selected)
            {
                // if its today, draw a similar circle but with a 50% alpha and using the TintColor
                if (year == DateTime.Now.Year && month + 1 == DateTime.Now.Month && day == DateTime.Now.Day)
                {
                    ShapeDrawable drawable = new ShapeDrawable(new OvalShape());
                    drawable.Paint.Color = NativeCalendarView.TintColor.ToPlatform();
                    drawable.Paint.SetStyle(Paint.Style.Fill);
                    drawable.Paint.Alpha = 128;

                    // Set the bounds of the drawable to make sure it appears within the calendar cell
                    int size = 20; // You can adjust this size to control the size of the indicator circle
                    drawable.SetBounds(0, 0, size, size);
                    return new ColorStateList(new int[][] { new int[] { Android.Resource.Attribute.StateSelected }, new int[] { } }, new int[] { drawable.Paint.Color, drawable.Paint.Color });
                }
                //else show same Tinted Color but 100% opaque
                else if(selected)
                {
                    ShapeDrawable drawable = new ShapeDrawable(new OvalShape());
                    drawable.Paint.Color = NativeCalendarView.TintColor.ToPlatform();
                    drawable.Paint.SetStyle(Paint.Style.Fill);
                    drawable.Paint.Alpha = 255;

                    // Set the bounds of the drawable to make sure it appears within the calendar cell
                    int size = 20; // You can adjust this size to control the size of the indicator circle
                    drawable.SetBounds(0, 0, size, size);
                    return new ColorStateList(new int[][] { new int[] { Android.Resource.Attribute.StateSelected }, new int[] { } }, new int[] { drawable.Paint.Color, drawable.Paint.Color });
                }

                return base.GetBackgroundColor(context, year, month, day, valid, selected);
            }

            public override void WriteToParcel(Parcel? dest, [GeneratedEnum] ParcelableWriteFlags flags)
            {
                throw new NotImplementedException();
            }
        }

    }
}
