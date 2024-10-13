using Microsoft.Maui.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.Maui.NativeCalendar
{
    public partial class NativeCalendarHandler : ViewHandler<NativeCalendarView, NativeCalendarImplementation>
    {
        public static IPropertyMapper<NativeCalendarView, NativeCalendarHandler> PropertyMapper = new PropertyMapper<NativeCalendarView, NativeCalendarHandler>(ViewHandler.ViewMapper)
        {
            [nameof(NativeCalendarView.MaximumDate)] = MapMaximumDate,
            [nameof(NativeCalendarView.MinimumDate)] = MapMinimumDate
        };

        public static CommandMapper<NativeCalendarView, NativeCalendarHandler> CommandMapper = new(ViewCommandMapper)
        {
            //[nameof(Video.UpdateStatus)] = MapUpdateStatus,
            //[nameof(Video.PlayRequested)] = MapPlayRequested,
            //[nameof(Video.PauseRequested)] = MapPauseRequested,
            //[nameof(Video.StopRequested)] = MapStopRequested
        };

        public NativeCalendarHandler() : base(PropertyMapper, CommandMapper)
        {
        }

        public static void MapMaximumDate(NativeCalendarHandler handler, NativeCalendarView view)
        {
            handler.PlatformView?.UpdateMaximumDate(view);
        }

        public static void MapMinimumDate(NativeCalendarHandler handler, NativeCalendarView view)
        {
            handler.PlatformView?.UpdateMinimumDate(view);
        }



    }
}
