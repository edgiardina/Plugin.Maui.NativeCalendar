![nuget.png](https://raw.githubusercontent.com/edgiardina/Plugin.Maui.NativeCalendar/main/nuget.png)
# Plugin.Maui.NativeCalendar

`Plugin.Maui.NativeCalendar` provides the ability to implement native calendar functionality in your .NET MAUI app.

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

##### `ReadingChanged`

Occurs when feature reading changes.

#### Properties

##### `IsSupported`

Gets a value indicating whether reading the feature is supported on this device.

##### `IsMonitoring`

Gets a value indicating whether the feature is actively being monitored.

#### Methods

##### `Start()`

Start monitoring for changes to the feature.

##### `Stop()`

Stop monitoring for changes to the feature.

# Acknowledgements

This project could not have came to be without these projects and people, thank you! <3
