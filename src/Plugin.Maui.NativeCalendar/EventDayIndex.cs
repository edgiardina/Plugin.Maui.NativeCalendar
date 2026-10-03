namespace Plugin.Maui.NativeCalendar
{
    /// <summary>
    /// Maps each calendar day to the events that cover it. The platform views ask about every
    /// visible day each time a month is drawn, so a lookup is cheaper than a scan of the
    /// event list per day.
    /// </summary>
    internal sealed class EventDayIndex
    {
        // A longer event is cut off at this number of days so that one bad EndDate cannot
        // fill the index.
        const int MaximumEventLengthInDays = 3660;

        public static readonly EventDayIndex Empty = new(null);

        readonly Dictionary<int, List<NativeCalendarEvent>> eventsByDay = new();

        public EventDayIndex(IEnumerable<NativeCalendarEvent>? events)
        {
            foreach (var calendarEvent in Snapshot(events))
            {
                if (calendarEvent is null)
                    continue;

                var start = calendarEvent.StartDate.Date;
                var end = calendarEvent.EndDate.Date;

                // An event with no EndDate, or an EndDate before its StartDate, is a one-day event.
                if (end < start)
                    end = start;

                var day = start;
                for (var length = 0; day <= end && length < MaximumEventLengthInDays; length++)
                {
                    var key = ToKey(day.Year, day.Month, day.Day);

                    if (!eventsByDay.TryGetValue(key, out var dayEvents))
                        eventsByDay[key] = dayEvents = new List<NativeCalendarEvent>(1);

                    dayEvents.Add(calendarEvent);

                    if (day.Date == DateTime.MaxValue.Date)
                        break;

                    day = day.AddDays(1);
                }
            }
        }

        // The source collection can change on another thread while it is copied. The copy then
        // throws. The platform views call this constructor from native callbacks, where an
        // exception stops the app, so try again and then give up.
        static NativeCalendarEvent[] Snapshot(IEnumerable<NativeCalendarEvent>? events)
        {
            if (events is null)
                return Array.Empty<NativeCalendarEvent>();

            for (var attempt = 0; attempt < 3; attempt++)
            {
                try
                {
                    return events.ToArray();
                }
                catch (Exception exception) when (exception is InvalidOperationException or ArgumentException)
                {
                    // The collection changed during the copy.
                }
            }

            // The change raises CollectionChanged, which builds a new index.
            return Array.Empty<NativeCalendarEvent>();
        }

        /// <param name="month">The month, where January is 1.</param>
        public bool HasEvents(int year, int month, int day) => eventsByDay.ContainsKey(ToKey(year, month, day));

        /// <param name="month">The month, where January is 1.</param>
        public IReadOnlyList<NativeCalendarEvent> GetEvents(int year, int month, int day)
        {
            return eventsByDay.TryGetValue(ToKey(year, month, day), out var dayEvents)
                ? dayEvents
                : Array.Empty<NativeCalendarEvent>();
        }

        static int ToKey(int year, int month, int day) => (year * 10000) + (month * 100) + day;
    }
}
