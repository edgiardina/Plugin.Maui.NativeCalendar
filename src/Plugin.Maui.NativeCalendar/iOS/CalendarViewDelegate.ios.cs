using Foundation;
using UIKit;

namespace Plugin.Maui.NativeCalendar.iOS
{
    public class CalendarViewDelegate : NSObject, IUICalendarViewDelegate
    {
        private readonly EventDayIndex eventDays;
        private readonly UIColor eventIndicatorColor;

        public CalendarViewDelegate(IEnumerable<NativeCalendarEvent> events, UIColor eventIndicatorColor)
        {
            // The calendar asks for a decoration for each visible day, so index the events by day.
            this.eventDays = new EventDayIndex(events);
            this.eventIndicatorColor = eventIndicatorColor;
        }

        // Decoration method for UICalendarView
        [Export("calendarView:decorationForDateComponents:")]
        public UICalendarViewDecoration? GetDecoration(UICalendarView calendarView, NSDateComponents dateComponents)
        {
            if (!eventDays.HasEvents(dateComponents.Year.ToInt32(), dateComponents.Month.ToInt32(), dateComponents.Day.ToInt32()))
            {
                return null; // No decoration if no event found for that date
            }

            return UICalendarViewDecoration.Create(eventIndicatorColor, UICalendarViewDecorationSize.Medium);
        }
    }
}
