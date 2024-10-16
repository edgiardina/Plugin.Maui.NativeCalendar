using Android.Content;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using Com.Applandeo.Materialcalendarview;
using Com.Applandeo.Materialcalendarview.Listeners;
using Java.Interop;
using Java.Util;
using static Android.Provider.CalendarContract;
using static Plugin.Maui.NativeCalendar.NativeCalendarImplementation;
using MaterialCalendar = Com.Applandeo.Materialcalendarview;

namespace Plugin.Maui.NativeCalendar
{
    public class NativeCalendarImplementation : FrameLayout
    {
        private readonly MaterialCalendar.CalendarView calendarView;
        private readonly NativeCalendarView nativeCalendarView;

        public NativeCalendarImplementation(Context context, NativeCalendarView nativeCalendarView) : base(context)
        {
            this.nativeCalendarView = nativeCalendarView;
            calendarView = new MaterialCalendar.CalendarView(context);

            // set to single selection?
            //calendarView.SetSelectionMode(MaterialCalendarView.SelectionMode.Single);

            // Set layout parameters, e.g., match parent in both dimensions
            var layoutParams = new CoordinatorLayout.LayoutParams(
                LayoutParams.MatchParent,
                LayoutParams.MatchParent
            );

            // Add the CalendarView to the CoordinatorLayout
            calendarView.LayoutParameters = layoutParams;
            AddView(calendarView);

            // TODO set event handler when date is changed
            //calendarView.SetOnCalendarDayClickListener(new MaterialCalendar.OnCalendarDayClickListener());
            //calendarView.DateChange += CalendarView_DateChange;
        }

        //private void CalendarView_DateChange(object? sender, CalendarView.DateChangeEventArgs e)
        //{
        //    nativeCalendarView.SelectedDate = new DateTime(e.Year, e.Month + 1, e.DayOfMonth);
        //}

        public void UpdateSelectedDate(NativeCalendarView nativeCalendarView)
        {
            Calendar date = Calendar.Instance;
            date.Set(nativeCalendarView.SelectedDate.Year, nativeCalendarView.SelectedDate.Month - 1, nativeCalendarView.SelectedDate.Day);
            calendarView.SetDate(date);
        }

        public void UpdateMaximumDate(NativeCalendarView nativeCalendarView)
        {
            Calendar maxDate = Calendar.Instance;
            maxDate.Set(nativeCalendarView.MaximumDate.Year, nativeCalendarView.MaximumDate.Month - 1, nativeCalendarView.MaximumDate.Day);
            calendarView.SetMinimumDate(maxDate);
        }

        public void UpdateMinimumDate(NativeCalendarView nativeCalendarView)
        {
            // calendarView.SetMinimumDate();// = new DateTimeOffset(nativeCalendarView.MinimumDate).ToUnixTimeMilliseconds(); 
            Calendar minDate = Calendar.Instance;
            minDate.Set(nativeCalendarView.MinimumDate.Year, nativeCalendarView.MinimumDate.Month - 1, nativeCalendarView.MinimumDate.Day);
            calendarView.SetMinimumDate(minDate);
        }

        public void UpdateEvents(NativeCalendarView nativeCalendarView)
        {
            // Update the events
            List<CalendarDay> calendarDays = new List<CalendarDay>();

            foreach (var date in nativeCalendarView.Events)
            {
                // Convert DateTime to Calendar
                Calendar calendar = Calendar.Instance;
                calendar.Set(date.StartDate.Year, date.StartDate.Month - 1, date.StartDate.Day);  // Month is 0-based in Calendar

                // Create CalendarDay
                CalendarDay calendarDay = new CalendarDay(calendar);
                //calendarDay.SetLabelColor(Color.ParseColor("#228B22"));  // Set a custom label color (optional)
                calendarDays.Add(calendarDay);
            }

            // Set the calendar days to the CalendarView
            calendarView.SetCalendarDays(calendarDays);
        }

    }
}
