using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.Util;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using Google.Android.Material.DatePicker;
using Java.Util;
using Microsoft.Maui.Platform;
using Plugin.Maui.NativeCalendar.Droid;
using Plugin.Maui.NativeCalendar.Extensions;
using System.Xml;
using static Android.Widget.CalendarView;

namespace Plugin.Maui.NativeCalendar
{
    public class NativeCalendarImplementation : FrameLayout
    {
        private ExtendedCalendarView calendarView;
        private readonly NativeCalendarView nativeCalendarView;

        public NativeCalendarImplementation(Context context, NativeCalendarView nativeCalendarView) : base(context)
        {
            this.nativeCalendarView = nativeCalendarView;

            calendarView = new ExtendedCalendarView(context);
            calendarView.SetOnDateChangeListener(new DateChangeListener(nativeCalendarView));

            var layoutParams = new CoordinatorLayout.LayoutParams(
                LayoutParams.MatchParent,
                LayoutParams.MatchParent
            );


            // Add the CalendarView to the CoordinatorLayout
            calendarView.LayoutParameters = layoutParams;
            //AddView(calendarView);

            Id = GenerateViewId();

            GenerateCalendarFragmentAndRender();
        }

        public void UpdateTintColor(NativeCalendarView nativeCalendarView)
        {
            calendarView.EventIndicatorColor = nativeCalendarView.TintColor.ToPlatform();
        }

        public void UpdateSelectedDate(NativeCalendarView nativeCalendarView)
        {
            calendarView.Date = nativeCalendarView.SelectedDate.ToLongInteger();
        }

        public void UpdateMaximumDate(NativeCalendarView nativeCalendarView)
        {
            calendarView.MaxDate = nativeCalendarView.MaximumDate.ToLongInteger();
        }

        public void UpdateMinimumDate(NativeCalendarView nativeCalendarView)
        {
            calendarView.MinDate = nativeCalendarView.MinimumDate.ToLongInteger();
        }

        public void UpdateEvents(NativeCalendarView nativeCalendarView)
        {
            calendarView.Events = nativeCalendarView.Events;
        }

        private void GenerateCalendarFragmentAndRender()
        {
            // Create a CalendarConstraints object to provide a valid date range for the MaterialCalendar
            var calendar = Java.Util.Calendar.Instance;
            long startMillis = calendar.TimeInMillis; // Start from today

            calendar.Add(Java.Util.CalendarField.Year, 1);
            long endMillis = calendar.TimeInMillis; // One year from today
            CalendarConstraints.Builder constraintsBuilder = new CalendarConstraints.Builder();
            constraintsBuilder.SetStart(startMillis);
            constraintsBuilder.SetEnd(endMillis);
            constraintsBuilder.SetOpenAt(startMillis);
            constraintsBuilder.SetValidator(DateValidatorPointForward.From(startMillis));

            var t = MaterialCalendar.NewInstance(new SingleDateSelector(), 0, constraintsBuilder.Build());
            Post(() =>
            {
                var transaction = Context.GetFragmentManager().BeginTransaction();
                transaction.Add(Id, t, "MaterialCalendar");
                transaction.Commit();
            });
        }

        private class DateChangeListener : Java.Lang.Object, CalendarView.IOnDateChangeListener
        {
            private readonly NativeCalendarView nativeCalendarView;

            public DateChangeListener(NativeCalendarView nativeCalendarView)
            {
                this.nativeCalendarView = nativeCalendarView;
            }

            public void OnSelectedDayChange(CalendarView view, int year, int month, int dayOfMonth)
            {
                // Handle the date change
                this.nativeCalendarView.SelectedDate = new DateTime(year, month + 1, dayOfMonth);
            }
        }

    }
}
