using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        private Color eventIndicatorColor;

        [ObservableProperty]
        private Color tintColor;

        [ObservableProperty]
        private List<NativeCalendarEvent> events;

        private Random random = new Random();

        public MainPageViewModel()
        {
            MaximumDate = DateTime.Now.AddYears(1);
            MinimumDate = DateTime.Now.AddYears(-1);
            SelectedDate = DateTime.Now.AddDays(1);
            EventIndicatorColor = Colors.Red;
            TintColor = Colors.Green;

            Events = new List<NativeCalendarEvent>
            {
                new NativeCalendarEvent
                {
                    Title = "Event 1",
                    Description = "Description 1",
                    StartDate = DateTime.Now.AddDays(random.Next(31)),
                    EndDate = DateTime.Now.AddDays(2),
                    Location = "Location 1"
                },
                new NativeCalendarEvent
                {
                    Title = "Event 2",
                    Description = "Description 2",
                    StartDate = DateTime.Now.AddDays(random.Next(31)),
                    EndDate = DateTime.Now.AddDays(4),
                    Location = "Location 2"
                }
            };
        }

        [RelayCommand]
        public void ChangeEvents()
        {
            Events = new List<NativeCalendarEvent>
            {
                new NativeCalendarEvent
                {
                    Title = "Event 3",
                    Description = "Description 3",
                    StartDate = DateTime.Now.AddDays(random.Next(31)),
                    EndDate = DateTime.Now.AddDays(6),
                    Location = "Location 3"
                },
                new NativeCalendarEvent
                {
                    Title = "Event 4",
                    Description = "Description 4",
                    StartDate = DateTime.Now.AddDays(random.Next(31)),
                    EndDate = DateTime.Now.AddDays(8),
                    Location = "Location 4"
                }
            };

            // Randomize the EventIndicatorColor
            EventIndicatorColor = Color.FromRgb(random.Next(256), random.Next(256), random.Next(256));
        }

        [RelayCommand]
        public void DateChanged(DateChangedEventArgs dateChangedEventArgs)
        {
            Console.WriteLine($"Selected Date: {dateChangedEventArgs.NewDate}");
        }

        [RelayCommand]
        public void ChangeSelectedDate()
        {
            SelectedDate = DateTime.Now.AddDays(random.Next(31));
        }

        [RelayCommand]
        public void ChangeTintColor()
        {
            TintColor = Color.FromRgb(random.Next(256), random.Next(256), random.Next(256));
        }

    }
}
