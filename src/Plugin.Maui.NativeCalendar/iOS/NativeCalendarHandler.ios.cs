using Microsoft.Maui.Handlers;

namespace Plugin.Maui.NativeCalendar
{
    public partial class NativeCalendarHandler : ViewHandler<NativeCalendarView, NativeCalendarImplementation>
    {
        protected override NativeCalendarImplementation CreatePlatformView() => new NativeCalendarImplementation(VirtualView);

        protected override void ConnectHandler(NativeCalendarImplementation platformView)
        {
            base.ConnectHandler(platformView);
        }

        protected override void DisconnectHandler(NativeCalendarImplementation platformView)
        {
            platformView.Dispose();
            base.DisconnectHandler(platformView);
        }
    }
}
