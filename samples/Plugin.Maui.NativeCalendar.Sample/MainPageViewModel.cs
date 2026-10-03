using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Plugin.Maui.NativeCalendar.Sample
{
    /// <summary>
    /// A color choice in the sample. A null <see cref="Color"/> is the platform default.
    /// </summary>
    public record ColorOption(string Name, Color Color)
    {
        public Color Swatch => Color ?? Colors.Transparent;
        public string Label => Color is null ? "Auto" : string.Empty;
    }

    public partial class MainPageViewModel : ObservableObject
    {
        private static readonly string[] EventTitles =
        {
            "Team standup", "Dentist", "Lunch with Sam", "Flight to Lisbon",
            "Gym", "Book club", "Release day", "Design review"
        };

        private static readonly string[] EventLocations =
        {
            "Room 4B", "Main Street", "Cafe Aurora", "Terminal 2", "Downtown", "Library", "Online", "Studio"
        };

        private readonly Random random = new Random();

        [ObservableProperty]
        private bool isCalendarVisible = true;

        [ObservableProperty]
        private bool isRangeLimited = true;

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
        private string selectedDateTitle;

        [ObservableProperty]
        private string lastChange = "Tap a day, or use the actions below.";

        [ObservableProperty]
        private bool hasNoSelectedDayEvents;

        // The calendar watches this collection, so changes in place show up without a new binding.
        public ObservableCollection<NativeCalendarEvent> Events { get; } = new();

        public ObservableCollection<NativeCalendarEvent> SelectedDayEvents { get; } = new();

        public IReadOnlyList<ColorOption> ColorOptions { get; } = new[]
        {
            new ColorOption("Auto", null),
            new ColorOption("Blue", Color.FromArgb("#3B6FF5")),
            new ColorOption("Green", Color.FromArgb("#1E9E6A")),
            new ColorOption("Orange", Color.FromArgb("#F08A24")),
            new ColorOption("Pink", Color.FromArgb("#E0458B")),
            new ColorOption("Purple", Color.FromArgb("#8B5CF6")),
        };

        public MainPageViewModel()
        {
            ApplyRange();

            AddEvent("Team standup", "Room 4B", DateTime.Today.AddDays(2), DateTime.Today.AddDays(2));
            AddEvent("Flight to Lisbon", "Terminal 2", DateTime.Today.AddDays(4), DateTime.Today.AddDays(6));
            AddEvent("Book club", "Library", DateTime.Today.AddDays(6), DateTime.Today.AddDays(6));
            AddEvent("Release day", "Online", DateTime.Today.AddDays(12), DateTime.Today.AddDays(12));

            SelectedDate = DateTime.Today.AddDays(4);
        }

        partial void OnSelectedDateChanged(DateTime value)
        {
            RefreshSelectedDay();
        }

        partial void OnIsRangeLimitedChanged(bool value)
        {
            ApplyRange();
        }

        [RelayCommand]
        public void DateChanged(DateChangedEventArgs dateChangedEventArgs)
        {
            LastChange = $"DateChanged: {dateChangedEventArgs.OldDate:MMM d} to {dateChangedEventArgs.NewDate:MMM d}";
        }

        [RelayCommand]
        public void SelectToday()
        {
            SelectedDate = DateTime.Today;
        }

        [RelayCommand]
        public void SelectRandomDay()
        {
            SelectedDate = DateTime.Today.AddDays(random.Next(-90, 91));
        }

        [RelayCommand]
        public void ShuffleEvents()
        {
            Events.Clear();

            for (int i = 0; i < 8; i++)
            {
                var start = DateTime.Today.AddDays(random.Next(-20, 45));
                var index = random.Next(EventTitles.Length);

                AddEvent(EventTitles[index], EventLocations[index], start, start.AddDays(random.Next(0, 3)));
            }

            RefreshSelectedDay();
        }

        [RelayCommand]
        public void AddEventOnSelectedDay()
        {
            var index = random.Next(EventTitles.Length);

            AddEvent(EventTitles[index], EventLocations[index], SelectedDate, SelectedDate);
            RefreshSelectedDay();
        }

        [RelayCommand]
        public void SetTint(ColorOption option)
        {
            TintColor = option?.Color;
        }

        [RelayCommand]
        public void SetEventIndicator(ColorOption option)
        {
            EventIndicatorColor = option?.Color;
        }

        private void AddEvent(string title, string location, DateTime start, DateTime end)
        {
            Events.Add(new NativeCalendarEvent
            {
                Title = title,
                Location = location,
                StartDate = start,
                EndDate = end
            });
        }

        private void ApplyRange()
        {
            MinimumDate = IsRangeLimited ? DateTime.Today.AddYears(-1) : DateTime.MinValue;
            MaximumDate = IsRangeLimited ? DateTime.Today.AddYears(1) : DateTime.MaxValue;
        }

        private void RefreshSelectedDay()
        {
            SelectedDateTitle = SelectedDate.ToString("dddd, MMMM d");

            SelectedDayEvents.Clear();

            foreach (var calendarEvent in Events.Where(e => e.StartDate.Date <= SelectedDate.Date && e.EndDate.Date >= SelectedDate.Date))
                SelectedDayEvents.Add(calendarEvent);

            HasNoSelectedDayEvents = SelectedDayEvents.Count == 0;
        }
    }
}
