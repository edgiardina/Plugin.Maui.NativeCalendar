![nuget.png](https://raw.githubusercontent.com/edgiardina/Plugin.Maui.NativeCalendar/main/nuget.png)
# Plugin.Maui.NativeCalendar

`Plugin.Maui.NativeCalendar` provides the ability to implement native calendar functionality in your .NET MAUI app.

iOS

<img src="https://raw.githubusercontent.com/edgiardina/Plugin.Maui.NativeCalendar/main/ios-example.png" width=200>

Android

<img src="https://raw.githubusercontent.com/edgiardina/Plugin.Maui.NativeCalendar/main/android-example.png" width=200>


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

The calendar view on iOS is implemented using `UICalendarView`.

The calendar view on Android is implemented using `MaterialCalendar`, a class used in the MaterialDatePicker from the Android Material library.

### Sizing

The calendar sizes itself on iOS and Android. A `HeightRequest` is optional. Use it only to override the natural height.

- **iOS** reports the intrinsic size of `UICalendarView`.
- **Android** reports the height of the Material date picker calendar: the month navigation, the weekday names and six weeks. The height is the same for each month, so the layout does not move when the user changes the month.

### Platform feature matrix

| Feature | iOS (`UICalendarView`) | Android (`MaterialCalendar`) |
|---------|------------------------|------------------------------|
| Single date selection | Yes | Yes |
| Event indicator dots (`Events`, `EventIndicatorColor`) | Yes | Yes |
| `TintColor` for today and selected day | Yes | Yes |
| `MinimumDate` / `MaximumDate` | Yes | Yes |
| Self-sizing without a `HeightRequest` | Yes | Yes |
| Shows the month of `SelectedDate` when you set it in code | Yes | Yes |
| Light and dark mode, also when the mode changes while the app runs | Yes | Yes |
| Year picker from the month title | Yes | Yes |
| Event titles in the screen reader description of a day | No | Yes |
| Minimum OS | iOS 16 | API 21 |

The `Events` collection updates the calendar when the property is reassigned and when an `ObservableCollection` bound to it is changed in place.

#### Android notes

- The calendar uses the Material theme of the app. If `TintColor` is not set, the selected day uses `colorPrimary` of the theme.
- `TintColor` changes the selected day, the number of today, and the navigation arrows. The ring around today and the year picker keep the colors of the theme.
- The calendar does not support fragment state restore. Keep the default of .NET MAUI, where `AllowFragmentRestore` of the activity is `false`.
- `MaterialCalendar` is a part of the Material library that Google does not document as public API. An update of the Material library can change it. Test the calendar when you update `Xamarin.Google.Android.Material` or .NET MAUI.

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
                                           DateChanged="NativeCalendarView_DateChanged" />
```

#### Events

##### `DateChanged`

Occurs when `SelectedDate` changes. This includes a tap on a day and a change from code or from a binding.

#### Commands

##### `DateChangedCommand`

Command that is executed when `SelectedDate` changes. The parameter is the `DateChangedEventArgs`.

#### Properties

##### `TintColor`

Color of the selected day and of today. The default is `null`, which uses the accent color of the platform: the tint color on iOS and `colorPrimary` of the app theme on Android.

##### `EventIndicatorColor`

Color of the Event Indicator, a dot that appears below the date number indicating there is an event on that date. The default is `null`, which uses the `TintColor`.

##### `MinimumDate`

Lowest date that can be selected on the calendar. The date itself can be selected.

##### `MaximumDate`

Greatest date that can be selected on the calendar. The date itself can be selected.

##### `SelectedDate`

Date that is currently selected on the calendar. The calendar selects full days, so the time of day is removed. A date outside `MinimumDate` and `MaximumDate` moves to the nearest date in the range. The default is today.

##### `Events`

List of dates that have events. The calendar will display a dot below the date number to indicate there is an event on that date. An event shows on each day from its `StartDate` to its `EndDate`. An event with an `EndDate` before its `StartDate` shows on its `StartDate` only.
