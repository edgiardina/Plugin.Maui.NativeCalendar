using Foundation;
using UIKit;

namespace Plugin.Maui.NativeCalendar.iOS
{
    public class CalendarViewDelegate : NSObject, IUICalendarViewDelegate
    {
        private readonly List<NativeCalendarEvent> events;

        public CalendarViewDelegate(List<NativeCalendarEvent> events)
        {
            // List of calendar events
            this.events = events;
        }

        // Decoration method for UICalendarView
        [Export("calendarView:decorationForDateComponents:")]
        public UICalendarViewDecoration GetDecoration(UICalendarView calendarView, NSDateComponents dateComponents)
        {
            // Find the matching event for the given date components
            var matchingEvent = FindEventForDate(dateComponents);

            if (matchingEvent == null)
            {
                return null; // No decoration if no event found for that date
            }
            else
            {
                return new UICalendarViewDecoration();
            }
        }

        // Helper method to find an event for the given date
        private NativeCalendarEvent FindEventForDate(NSDateComponents dateComponents)
        {
            foreach (var calendarEvent in events)
            {
                if (calendarEvent.StartDate.Year == dateComponents.Year &&
                    calendarEvent.StartDate.Month == dateComponents.Month &&
                    calendarEvent.StartDate.Day == dateComponents.Day)
                {
                    return calendarEvent;
                }
            }

            return null; // No matching event found
        }
    }
}
