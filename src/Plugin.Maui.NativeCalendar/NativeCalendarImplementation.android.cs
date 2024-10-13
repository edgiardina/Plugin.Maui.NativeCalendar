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

        public NativeCalendarImplementation(Context context, NativeCalendarView nativeCalendarView) : base(context)
        {
            calendarView = new CalendarView(context);

            // Set layout parameters, e.g., match parent in both dimensions
            var layoutParams = new CoordinatorLayout.LayoutParams(
                LayoutParams.MatchParent,
                LayoutParams.MatchParent
            );

            // Add the CalendarView to the CoordinatorLayout
            calendarView.LayoutParameters = layoutParams;
            AddView(calendarView);
        }

        public void UpdateMaximumDate(NativeCalendarView nativeCalendarView)
        {
            calendarView.MaxDate = new DateTimeOffset(nativeCalendarView.MaximumDate).ToUnixTimeMilliseconds();
        }

        public void UpdateMinimumDate(NativeCalendarView nativeCalendarView)
        {
            calendarView.MinDate = new DateTimeOffset(nativeCalendarView.MinimumDate).ToUnixTimeMilliseconds();
        }

        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            base.OnLayout(changed, left, top, right, bottom);

            // Now you can safely interact with the CalendarView
            // This ensures the view is fully measured before accessing properties
            if (calendarView != null)
            {
                // Access CalendarView's methods safely here
                Console.WriteLine("CalendarView layout completed.");
            }
        }
    }
}
