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
            [nameof(NativeCalendarView.MinimumDate)] = MapMinimumDate,
            [nameof(NativeCalendarView.SelectedDate)] = MapSelectedDate,
            [nameof(NativeCalendarView.Events)] = MapEvents,
            [nameof(NativeCalendarView.EventIndicatorColor)] = MapEventIndicatorColor,
            [nameof(NativeCalendarView.TitleTextColor)] = MapTitleTextColor,
            [nameof(NativeCalendarView.HeaderColor)] = MapHeaderColor,
            [nameof(NativeCalendarView.TintColor)] = MapTintColor
        };


        public static CommandMapper<NativeCalendarView, NativeCalendarHandler> CommandMapper = new(ViewCommandMapper)
        {
            [nameof(NativeCalendarView.DateChangedCommand)] = MapDateChangedCommand
        };

        public NativeCalendarHandler() : base(PropertyMapper, CommandMapper)
        {
        }

        public static void MapSelectedDate(NativeCalendarHandler handler, NativeCalendarView view)
        {
            handler.PlatformView?.UpdateSelectedDate(view);
        }

        public static void MapMaximumDate(NativeCalendarHandler handler, NativeCalendarView view)
        {
            handler.PlatformView?.UpdateMaximumDate(view);
        }

        public static void MapMinimumDate(NativeCalendarHandler handler, NativeCalendarView view)
        {
            handler.PlatformView?.UpdateMinimumDate(view);
        }
        public static void MapEvents(NativeCalendarHandler handler, NativeCalendarView view)
        {
            handler.PlatformView?.UpdateEvents(view);
        }

        public static void MapEventIndicatorColor(NativeCalendarHandler handler, NativeCalendarView view)
        {
            handler.PlatformView?.UpdateEvents(view);
        }

        private static void MapTitleTextColor(NativeCalendarHandler handler, NativeCalendarView view)
        {
            handler.PlatformView?.UpdateTitleTextColor(view);
        }
        private static void MapHeaderColor(NativeCalendarHandler handler, NativeCalendarView view)
        {
            handler.PlatformView?.UpdateHeaderColor(view);
        }
        private static void MapTintColor(NativeCalendarHandler handler, NativeCalendarView view)
        {
            handler.PlatformView?.UpdateTintColor(view);
        }

        private static void MapDateChangedCommand(NativeCalendarHandler handler, NativeCalendarView view, object? args)
        {
            // no-op
        }



    }
}
