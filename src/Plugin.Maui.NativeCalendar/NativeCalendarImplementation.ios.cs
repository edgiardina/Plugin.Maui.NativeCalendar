using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace Plugin.Maui.NativeCalendar
{
    public class NativeCalendarImplementation : UIView
    {
        public NativeCalendarImplementation(NativeCalendarView nativeCalendarHandler)
        {
            // only add a calendar on iOS 16.0 or later
            if (UIDevice.CurrentDevice.CheckSystemVersion(16, 0))
            {
                var calendarView = new UICalendarView();
                AddSubview(calendarView);
            }            
        }
    }
}
