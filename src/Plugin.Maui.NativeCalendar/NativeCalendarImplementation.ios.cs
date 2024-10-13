using Foundation;
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
        private readonly UICalendarView calendarView;

        private NSDate MaxDate { get; set; } 
        private NSDate MinDate { get; set; }    

        public NativeCalendarImplementation(NativeCalendarView nativeCalendarHandler)
        {
            // only add a calendar on iOS 16.0 or later
            if (UIDevice.CurrentDevice.CheckSystemVersion(16, 0))
            {
                calendarView = new UICalendarView();
                calendarView.SelectionBehavior = new UICalendarSelectionSingleDate();


                AddSubview(calendarView);
            }         
            else
            {
                throw new Exception("iOS 16.0 or later is required to use the NativeCalendarView");
            }
        }

        public void UpdateSelectedDate(NativeCalendarView nativeCalendarView)
        {
            // Create NSDateComponents from the DateTime
            var dateTime = nativeCalendarView.SelectedDate;

            NSDateComponents dateComponents = new NSDateComponents
            {
                Year = dateTime.Year,
                Month = dateTime.Month,
                Day = dateTime.Day
            };

            // Cast to UICalendarSelectionSingleDate and set the SelectedDate
            if (calendarView.SelectionBehavior is UICalendarSelectionSingleDate singleDateSelection)
            {
                singleDateSelection.SelectedDate = dateComponents;
            }
        }

        public void UpdateMaximumDate(NativeCalendarView nativeCalendarView)
        {
            MaxDate = (NSDate)nativeCalendarView.MaximumDate;

            // Update the maximum date of the CalendarView
            calendarView.AvailableDateRange = new Foundation.NSDateInterval(MinDate, MaxDate);
        }

        public void UpdateMinimumDate(NativeCalendarView nativeCalendarView)
        {
            MinDate = (NSDate)nativeCalendarView.MinimumDate;

            // Update the minimum date of the CalendarView
            calendarView.AvailableDateRange = new Foundation.NSDateInterval(MinDate, MaxDate);
        }
    }
}
