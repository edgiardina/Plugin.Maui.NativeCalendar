# Plugin.Maui.NativeCalendar

The `Plugin.Maui.NativeCalendar` project allows you to implement native calendar functionality in your .NET MAUI app.

In order to enable the plugin, you need to call the `UseNativeCalendar` method in the `MauiProgram.cs` file of your .NET MAUI app.

```csharp
  var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseNativeCalendar()   // <--- Add this line
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
```

You'll need to add a xmlns namespace to your XAML page:

```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="Plugin.Maui.NativeCalendar.Sample.MainPage"
             xmlns:nativecalendar="clr-namespace:Plugin.Maui.NativeCalendar;assembly=Plugin.Maui.NativeCalendar"
             Title="Native Calendar Plugin">
```

And then consume your calendar in the XAML page:

```xml

 <nativecalendar:NativeCalendarView MaximumDate="{Binding MaximumDate}"
                                           MinimumDate="{Binding MinimumDate}"
                                           SelectedDate="{Binding SelectedDate}"
                                           Events="{Binding Events}"
                                           EventIndicatorColor="{Binding EventIndicatorColor}"
                                           HeightRequest="500"
                                           DateChanged="NativeCalendarView_DateChanged" />
```