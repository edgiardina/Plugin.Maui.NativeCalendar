namespace Plugin.Maui.NativeCalendar
{
    public static class NativeCalendarExtensions
    {
        public static MauiAppBuilder UseNativeCalendar(this MauiAppBuilder builder)
        {
            // Register the custom handler for NativeCalendarView
            builder.ConfigureMauiHandlers(handlers =>
            {
                handlers.AddHandler(typeof(NativeCalendarView), typeof(NativeCalendarHandler));
            });

            return builder;
        }
    }
}
