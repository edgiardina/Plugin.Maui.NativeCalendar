using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Plugin.Maui.NativeCalendar
{
    public class NativeCalendarView : View
    {
        #region Bindable Properties

        public static readonly BindableProperty EventsProperty = BindableProperty.Create(
            propertyName: nameof(Events),
            returnType: typeof(IEnumerable<NativeCalendarEvent>),
            declaringType: typeof(NativeCalendarView),
            // A defaultValueCreator gives every instance its own collection. A plain
            // defaultValue is a single shared instance across all NativeCalendarView objects.
            defaultValueCreator: _ => new ObservableCollection<NativeCalendarEvent>(),
            defaultBindingMode: BindingMode.OneWay,
            propertyChanged: OnEventsPropertyChanged
        );
        public IEnumerable<NativeCalendarEvent> Events
        {
            get => (IEnumerable<NativeCalendarEvent>)GetValue(EventsProperty);
            set => SetValue(EventsProperty, value);
        }

        // Keep the platform view in sync when the bound collection itself is mutated
        // (items added or removed), not only when the Events property is reassigned.
        static void OnEventsPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is not NativeCalendarView view)
                return;

            if (oldValue is INotifyCollectionChanged oldObservable)
                oldObservable.CollectionChanged -= view.OnEventsCollectionChanged;

            if (newValue is INotifyCollectionChanged newObservable)
                newObservable.CollectionChanged += view.OnEventsCollectionChanged;
        }

        void OnEventsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // The Events reference did not change, so re-run the platform mapper by hand.
            Handler?.UpdateValue(nameof(Events));
        }

        // Selected Date Bindable Property
        public static readonly BindableProperty SelectedDateProperty = BindableProperty.Create(
            propertyName: nameof(SelectedDate),
            returnType: typeof(DateTime),
            declaringType: typeof(NativeCalendarView),
            defaultValue: DateTime.Now,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                if (bindable is NativeCalendarView nativeCalendarView)
                {
                    nativeCalendarView.DateChanged?.Invoke(nativeCalendarView, new DateChangedEventArgs((DateTime)oldValue, (DateTime)newValue));
                    nativeCalendarView.DateChangedCommand?.Execute(new DateChangedEventArgs((DateTime)oldValue, (DateTime)newValue));
                }
            }
        );

        public DateTime SelectedDate
        {
            get => (DateTime)GetValue(SelectedDateProperty);
            set => SetValue(SelectedDateProperty, value);
        }

        // Maximum Date Bindable Property
        public static readonly BindableProperty MaximumDateProperty = BindableProperty.Create(
            propertyName: nameof(MaximumDate),
            returnType: typeof(DateTime),
            declaringType: typeof(NativeCalendarView),
            defaultValue: DateTime.MaxValue,
            defaultBindingMode: BindingMode.TwoWay
        );

        public DateTime MaximumDate
        {
            get => (DateTime)GetValue(MaximumDateProperty);
            set => SetValue(MaximumDateProperty, value);
        }

        // Minimum Date Bindable Property
        public static readonly BindableProperty MinimumDateProperty = BindableProperty.Create(
            propertyName: nameof(MinimumDate),
            returnType: typeof(DateTime),
            declaringType: typeof(NativeCalendarView),
            defaultValue: DateTime.MinValue,
            defaultBindingMode: BindingMode.TwoWay
        );
        public DateTime MinimumDate
        {
            get => (DateTime)GetValue(MinimumDateProperty);
            set => SetValue(MinimumDateProperty, value);
        }

        public static readonly BindableProperty EventIndicatorColorProperty = BindableProperty.Create(
            propertyName: nameof(EventIndicatorColor),
            returnType: typeof(Color),
            declaringType: typeof(NativeCalendarView),
            defaultValue: Colors.Blue,
            defaultBindingMode: BindingMode.TwoWay
        );

        public Color EventIndicatorColor
        {
            get => (Color)GetValue(EventIndicatorColorProperty);
            set => SetValue(EventIndicatorColorProperty, value);
        }

        public static readonly BindableProperty DateChangedCommandProperty = BindableProperty.Create(
            propertyName: nameof(DateChangedCommand),
            returnType: typeof(ICommand),
            declaringType: typeof(NativeCalendarView),
            defaultValue: null
        );        

        public ICommand DateChangedCommand
        {
            get => (ICommand)GetValue(DateChangedCommandProperty);
            set => SetValue(DateChangedCommandProperty, value);
        }

        public static readonly BindableProperty TintColorProperty = BindableProperty.Create(
            propertyName: nameof(TintColor),
            returnType: typeof(Color),
            declaringType: typeof(NativeCalendarView),
            defaultValue: Colors.Blue,
            defaultBindingMode: BindingMode.TwoWay
        );
        public Color TintColor
        {
            get => (Color)GetValue(TintColorProperty);
            set => SetValue(TintColorProperty, value);
        }


        #endregion



        public event EventHandler<DateChangedEventArgs> DateChanged;
    }
}
