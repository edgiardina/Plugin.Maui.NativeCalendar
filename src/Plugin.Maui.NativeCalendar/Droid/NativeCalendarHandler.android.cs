using Microsoft.Maui.Handlers;

namespace Plugin.Maui.NativeCalendar
{
    public partial class NativeCalendarHandler : ViewHandler<NativeCalendarView, NativeCalendarImplementation>
    {
        protected override NativeCalendarImplementation CreatePlatformView() => new NativeCalendarImplementation(Context, VirtualView);

        protected override void DisconnectHandler(NativeCalendarImplementation platformView)
        {
            // Do not dispose the platform view here. Android can still call into it while it
            // leaves the window, and a disposed peer throws ObjectDisposedException.
            platformView.Disconnect();
            base.DisconnectHandler(platformView);
        }
    }
}
