using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.Maui.NativeCalendar.Sample
{
    public partial class MainPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private DateTime maximumDate;

        [ObservableProperty]
        private DateTime minimumDate;

        [ObservableProperty]
        private DateTime selectedDate;

        [ObservableProperty]
        private List<NativeCalendarEvent> events;

        public MainPageViewModel()
        {
            maximumDate = DateTime.Now.AddYears(1);
            minimumDate = DateTime.Now.AddYears(-1);
            selectedDate = DateTime.Now.AddDays(1);

            events = new List<NativeCalendarEvent>
            {
                new NativeCalendarEvent
                {
                    Title = "Event 1",
                    Description = "Description 1",
                    StartDate = DateTime.Now.AddDays(1),
                    EndDate = DateTime.Now.AddDays(2),
                    Location = "Location 1"
                },
                new NativeCalendarEvent
                {
                    Title = "Event 2",
                    Description = "Description 2",
                    StartDate = DateTime.Now.AddDays(3),
                    EndDate = DateTime.Now.AddDays(4),
                    Location = "Location 2"
                }
            };


        }

    }
}
