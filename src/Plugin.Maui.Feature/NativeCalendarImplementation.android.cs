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
            // Add a calendar view to the layout
            var calendarView = new CalendarView(context);
            AddView(calendarView);
        }
    }
}
