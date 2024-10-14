using Microsoft.Maui.Controls;
using Plugin.Maui.NativeCalendar;

namespace Plugin.Maui.NativeCalendar.Sample;

public partial class MainPage : ContentPage
{

	public MainPage(MainPageViewModel mainPageViewModel)
	{
        InitializeComponent();
        this.BindingContext = mainPageViewModel;
    }

    private void NativeCalendarView_DateChanged(object sender, DateChangedEventArgs e)
    {
        DisplayAlert("Date Changed", $"Old Date: {e.OldDate.ToShortDateString()} New Date: {e.NewDate.ToShortDateString()}", "OK");
    }
}
