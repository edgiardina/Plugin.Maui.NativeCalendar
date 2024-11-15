using Foundation;
using Microsoft.Extensions.Logging;
using System.Linq;
using UIKit;

namespace Plugin.Maui.NativeCalendar.iOS
{
    public class CalendarViewDelegate : NSObject, IUICalendarViewDelegate
    {
        private readonly IEnumerable<NativeCalendarEvent> events;
        private readonly UIColor eventIndicatorColor;

        public CalendarViewDelegate(IEnumerable<NativeCalendarEvent> events, UIColor eventIndicatorColor)
        {
            // List of calendar events
            this.events = events;
            this.eventIndicatorColor = eventIndicatorColor;
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
                return UICalendarViewDecoration.Create(eventIndicatorColor, UICalendarViewDecorationSize.Medium);
            }
        }

        // Helper method to find an event for the given date
        private NativeCalendarEvent? FindEventForDate(NSDateComponents dateComponents)
        {
            return events.FirstOrDefault(e => e.StartDate.Date <= new DateTime(dateComponents.Year.ToInt32(), dateComponents.Month.ToInt32(), dateComponents.Day.ToInt32())
                                           && e.EndDate.Date >= new DateTime(dateComponents.Year.ToInt32(), dateComponents.Month.ToInt32(), dateComponents.Day.ToInt32()));
        }
    }
}
