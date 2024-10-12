using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.Maui.NativeCalendar
{
    public class NativeCalendarView : View
    {
        public DateTime SelectedDate { get; set; }
        public event EventHandler<DateChangedEventArgs> DateChanged;
    }
}
