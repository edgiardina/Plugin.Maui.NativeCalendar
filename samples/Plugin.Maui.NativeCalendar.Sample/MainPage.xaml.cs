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
}
