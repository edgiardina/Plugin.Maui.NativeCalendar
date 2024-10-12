using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Widget;
using Android.Views;
using Microsoft.Maui.Handlers;

namespace Plugin.Maui.NativeCalendar
{
    public partial class NativeCalendarHandler : ViewHandler<NativeCalendarView, NativeCalendarImplementation>
    {
        protected override NativeCalendarImplementation CreatePlatformView() => new NativeCalendarImplementation(Context, VirtualView);

        protected override void ConnectHandler(NativeCalendarImplementation platformView)
        {
            base.ConnectHandler(platformView);

            // Perform any control setup here
        }
        protected override void DisconnectHandler(NativeCalendarImplementation platformView)
        {
            platformView.Dispose();
            base.DisconnectHandler(platformView);
        }
    }
}
