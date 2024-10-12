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
    public class NativeCalendarImplementation : CoordinatorLayout
    {
        public NativeCalendarImplementation(Context context, NativeCalendarView nativeCalendarView) : base(context)
        {
            var calendarView = new CalendarView(context);

            // Set layout parameters, e.g., match parent in both dimensions
            var layoutParams = new CoordinatorLayout.LayoutParams(
                LayoutParams.MatchParent,
                LayoutParams.MatchParent
            );

            // Add the CalendarView to the CoordinatorLayout
            calendarView.LayoutParameters = layoutParams;
            AddView(calendarView);
        }
    }
}
