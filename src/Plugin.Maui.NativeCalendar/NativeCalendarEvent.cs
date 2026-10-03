namespace Plugin.Maui.NativeCalendar
{
    public class NativeCalendarEvent
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Location { get; set; }
    }
}
