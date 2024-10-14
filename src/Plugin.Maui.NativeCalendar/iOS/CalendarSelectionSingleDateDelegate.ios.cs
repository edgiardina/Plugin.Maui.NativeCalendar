using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace Plugin.Maui.NativeCalendar.iOS
{
    public class CalendarSelectionSingleDateDelegate : UICalendarSelectionSingleDateDelegate
    {
        private readonly NativeCalendarView nativeCalendarView;

        public CalendarSelectionSingleDateDelegate(NativeCalendarView nativeCalendarView)        
        {
            this.nativeCalendarView = nativeCalendarView;
        }

        public override void DidSelectDate(UICalendarSelectionSingleDate selection, NSDateComponents? dateComponents)
        {
            if (dateComponents != null)
            {
                nativeCalendarView.SelectedDate = new DateTime(dateComponents.Year.ToInt32(), dateComponents.Month.ToInt32(), dateComponents.Day.ToInt32());
            }
        }
    }
}
