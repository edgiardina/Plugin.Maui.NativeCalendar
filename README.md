![nuget.png](https://raw.githubusercontent.com/edgiardina/Plugin.Maui.NativeCalendar/main/nuget.png)
# Plugin.Maui.NativeCalendar

`Plugin.Maui.NativeCalendar` provides the ability to implement native calendar functionality in your .NET MAUI app.


![iOS](https://raw.githubusercontent.com/edgiardina/Plugin.Maui.NativeCalendar/main/ios-example.png)

![Android](https://raw.githubusercontent.com/edgiardina/Plugin.Maui.NativeCalendar/main/android-example.png)

## Install Plugin

[![NuGet](https://img.shields.io/nuget/v/Plugin.Maui.NativeCalendar.svg?label=NuGet)](https://www.nuget.org/packages/Plugin.Maui.NativeCalendar/)

Available on [NuGet](http://www.nuget.org/packages/Plugin.Maui.NativeCalendar).

Install with the dotnet CLI: `dotnet add package Plugin.Maui.NativeCalendar`, or through the NuGet Package Manager in Visual Studio.

### Supported Platforms

| Platform | Minimum Version Supported |
|----------|---------------------------|
| iOS      | 16+                       |
| Android  | 5.0 (API 21)              |

## API Usage

`Plugin.Maui.NativeCalendar` provides the `NativeCalendar` class that displays a native calendar view in your .NET MAUI app. 

The calendar view on iOS is implemented using `UICalendarView`.  NOTE: iOS requires a declared height for the NativeCalendarView to appear.

The calendar view on Android is implemented using `MaterialCalendar`, a class used in the MaterialDatePicker from the Android Material library

### Permissions

#### iOS

No permissions are needed for iOS.

#### Android

No permissions are needed for Android.

### Dependency Injection

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


### Native Calendar Implementation


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

#### Events

##### `DateSelected`

Occurs when Date is selected via user interaction.

#### Properties

##### `TintColor`

Bindable property indicating the color of the current day and selected day on the calendar for iOS

For android, the color of the current day and selected day is the primary color of the app, defined in the `colors.xml` file.

##### `EventIndicatorColor`

Color of the Event Indicator, a dot that appears below the date number indicating there is an event on that date. Currently only allows single date selection.

##### `MinimumDate`

Lowest date that can be selected on the calendar.

##### `MaxminumDate`

Greatest date that can be selected on the calendar.

##### `SelectedDate`

Date that is currently selected on the calendar.

##### `Events`

List of dates that have events. The calendar will display a dot below the date number to indicate there is an event on that date. (iOS only)
