using Android.Content;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.Maui.NativeCalendar
{
    public class NativeCalendarImplementation : FrameLayout
    {
        private readonly CalendarView calendarView;
        private readonly NativeCalendarView nativeCalendarView;

        public NativeCalendarImplementation(Context context, NativeCalendarView nativeCalendarView) : base(context)
        {
            this.nativeCalendarView = nativeCalendarView;

            calendarView = new CalendarView(context);

            // Set layout parameters, e.g., match parent in both dimensions
            var layoutParams = new CoordinatorLayout.LayoutParams(
                LayoutParams.MatchParent,
                LayoutParams.MatchParent
            );

            // Add the CalendarView to the CoordinatorLayout
            calendarView.LayoutParameters = layoutParams;
            AddView(calendarView);

            calendarView.DateChange += CalendarView_DateChange;
        }

        private void CalendarView_DateChange(object? sender, CalendarView.DateChangeEventArgs e)
        {
            nativeCalendarView.SelectedDate = new DateTime(e.Year, e.Month + 1, e.DayOfMonth);
        }

        public void UpdateSelectedDate(NativeCalendarView nativeCalendarView)
        {
            calendarView.Date = new DateTimeOffset(nativeCalendarView.SelectedDate).ToUnixTimeMilliseconds();
        }

        public void UpdateMaximumDate(NativeCalendarView nativeCalendarView)
        {
            calendarView.MaxDate = new DateTimeOffset(nativeCalendarView.MaximumDate).ToUnixTimeMilliseconds();
        }

        public void UpdateMinimumDate(NativeCalendarView nativeCalendarView)
        {
            calendarView.MinDate = new DateTimeOffset(nativeCalendarView.MinimumDate).ToUnixTimeMilliseconds(); 
        }

        public void UpdateEvents(NativeCalendarView nativeCalendarView)
        {
            // Update the events
        }

    }
}
