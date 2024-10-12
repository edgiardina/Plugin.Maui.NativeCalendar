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
            //[nameof(Video.AreTransportControlsEnabled)] = MapAreTransportControlsEnabled,
            //[nameof(Video.Source)] = MapSource,
            //[nameof(Video.IsLooping)] = MapIsLooping,
            //[nameof(Video.Position)] = MapPosition
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

        

    }
}
