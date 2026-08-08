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

- **iOS** reports an intrinsic size, so the calendar sizes itself. A `HeightRequest` is optional and only needed to override the natural height.
- **Android** fills the height it is given but does not report an intrinsic height. The underlying `MaterialCalendar` is a fragment whose size is not known until it renders. Give the control a `HeightRequest`, or place it in a slot with a definite height, for example a `Grid` row with height `*` or `VerticalOptions="Fill"` inside a bounded container.

### Platform feature matrix

| Feature | iOS (`UICalendarView`) | Android (`MaterialCalendar`) |
|---------|------------------------|------------------------------|
| Single date selection | Yes | Yes |
| Event indicator dots (`Events`, `EventIndicatorColor`) | Yes | Yes |
| `TintColor` for today and selected day | Yes | Yes |
| `MinimumDate` / `MaximumDate` | Yes | Yes |
| Self-sizing without a `HeightRequest` | Yes | No, give it a height or a fill slot |
| Minimum OS | iOS 16 | API 21 |

The `Events` collection updates the calendar when the property is reassigned and when an `ObservableCollection` bound to it is changed in place.

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

 <!-- HeightRequest is necessary on Android. On iOS the calendar sizes itself, so it is optional. -->
 <nativecalendar:NativeCalendarView MaximumDate="{Binding MaximumDate}"
                                           MinimumDate="{Binding MinimumDate}"
                                           SelectedDate="{Binding SelectedDate}"
                                           Events="{Binding Events}"
                                           EventIndicatorColor="{Binding EventIndicatorColor}"
                                           HeightRequest="500"
                                           DateChanged="NativeCalendarView_DateChanged" />
```

#### Events

##### `DateChanged`

Occurs when Date is selected via user interaction.

#### Commands

##### `DateChangedCommand`

Command that is executed when Date is selected via user interaction.

#### Properties

##### `TintColor`

Bindable property indicating the color of the current day and selected day on the calendar for iOS

##### `EventIndicatorColor`

Color of the Event Indicator, a dot that appears below the date number indicating there is an event on that date. Currently only allows single date selection.

##### `MinimumDate`

Lowest date that can be selected on the calendar.

##### `MaximumDate`

Greatest date that can be selected on the calendar.

##### `SelectedDate`

Date that is currently selected on the calendar.

##### `Events`

List of dates that have events. The calendar will display a dot below the date number to indicate there is an event on that date.
