using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using Google.Android.Material.DatePicker;
using Java.Util;
using Microsoft.Maui.Platform;
using Plugin.Maui.NativeCalendar.Extensions;
using System.Xml;
using static Android.Widget.CalendarView;
using Color = Android.Graphics.Color;
using Paint = Android.Graphics.Paint;
using ShapeDrawable = Android.Graphics.Drawables.ShapeDrawable;

namespace Plugin.Maui.NativeCalendar
{
    public class NativeCalendarImplementation : FrameLayout
    {

        private MaterialCalendar materialCalendarFragment;
        private SingleDateSelector DateSelector;
        private CalendarConstraints calendarConstraints;

        private readonly NativeCalendarView nativeCalendarView;

        public NativeCalendarImplementation(Context context, NativeCalendarView nativeCalendarView) : base(context)
        {
            this.nativeCalendarView = nativeCalendarView;

            Id = GenerateViewId();

            GenerateCalendarFragmentAndRender();
        }

        public void UpdateTintColor(NativeCalendarView nativeCalendarView)
        {
            
        }

        public void UpdateSelectedDate(NativeCalendarView nativeCalendarView)
        {
            //calendarView.Date = nativeCalendarView.SelectedDate.ToLongInteger();
            materialCalendarFragment?.DateSelector?.Select(nativeCalendarView.SelectedDate.ToLongInteger());
            
        }

        public void UpdateMaximumDate(NativeCalendarView nativeCalendarView)
        {
            //calendarConstraints. = nativeCalendarView.MaximumDate.ToLongInteger();
        }

        public void UpdateMinimumDate(NativeCalendarView nativeCalendarView)
        {
            
        }

        public void UpdateEvents(NativeCalendarView nativeCalendarView)
        {
            
        }

        private void GenerateCalendarFragmentAndRender()
        {
            // Create a CalendarConstraints object to provide a valid date ran
            CalendarConstraints.Builder constraintsBuilder = new CalendarConstraints.Builder();
            constraintsBuilder.SetStart(nativeCalendarView.MinimumDate.ToLongInteger());
            constraintsBuilder.SetEnd(nativeCalendarView.MaximumDate.ToLongInteger());
            constraintsBuilder.SetOpenAt(nativeCalendarView.SelectedDate.ToLongInteger());
            constraintsBuilder.SetValidator(DateValidatorPointForward.From(nativeCalendarView.MinimumDate.ToLongInteger()));

            calendarConstraints = constraintsBuilder.Build();

            DateSelector = new SingleDateSelector();

            // create dayviewdecorator to add event indicators (small circles)
            DayViewDecorator dayViewDecorator = new EventIndicatorDayViewDecorator(nativeCalendarView.EventIndicatorColor.ToPlatform());

            materialCalendarFragment = MaterialCalendar.NewInstance(DateSelector, 0, calendarConstraints, dayViewDecorator);
 
            //materialCalendarFragment.AddOnSelectionChangedListener(new MaterialCalendarOnSelectionChangedListener(nativeCalendarView, materialCalendarFragment));

            //materialCalendarFragment.DateSelector.
            Post(() =>
            {
                var transaction = Context.GetFragmentManager().BeginTransaction();
                transaction.Add(Id, materialCalendarFragment, "MaterialCalendar");
                transaction.Commit();
            });
        }

        private class EventIndicatorDayViewDecorator : DayViewDecorator
        {
            private readonly Color eventIndicatorColor;

            public EventIndicatorDayViewDecorator(Color eventIndicatorColor)
            {
                this.eventIndicatorColor = eventIndicatorColor;
            }

            public override int DescribeContents()
            {
                throw new NotImplementedException();
            }

            public override Drawable? GetCompoundDrawableBottom(Context context, int year, int month, int day, bool valid, bool selected)
            {
                //return base.GetCompoundDrawableBottom(context, year, month, day, valid, selected);

                // Draw circle
                ShapeDrawable drawable = new ShapeDrawable(new OvalShape());
                drawable.Paint.Color = eventIndicatorColor;
                drawable.Paint.SetStyle(Paint.Style.Fill);
                return drawable;
            }

            public override void WriteToParcel(Parcel? dest, [GeneratedEnum] ParcelableWriteFlags flags)
            {
                throw new NotImplementedException();
            }
        }

    }
}
