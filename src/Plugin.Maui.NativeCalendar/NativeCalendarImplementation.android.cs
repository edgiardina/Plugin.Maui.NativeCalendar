using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
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
        private EventIndicatorDayViewDecorator eventIndicatorDayViewDecorator;

        private readonly NativeCalendarView nativeCalendarView;

        public NativeCalendarImplementation(Context context, NativeCalendarView nativeCalendarView) : base(context)
        {
            this.nativeCalendarView = nativeCalendarView;

            Id = GenerateViewId();

            GenerateCalendarFragmentAndRender();
        }

        public void UpdateTintColor(NativeCalendarView nativeCalendarView)
        {
            // TODO: What to update on Android?
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
            // TODO: force MaterialCalendar to redraw

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
            eventIndicatorDayViewDecorator = new EventIndicatorDayViewDecorator(nativeCalendarView);

            materialCalendarFragment = MaterialCalendar.NewInstance(DateSelector, 0, calendarConstraints, eventIndicatorDayViewDecorator);
            
 
            //materialCalendarFragment.AddOnSelectionChangedListener(new MaterialCalendarOnSelectionChangedListener(nativeCalendarView, materialCalendarFragment));

            //materialCalendarFragment.DateSelector.
            Post(() =>
            {
                var transaction = Context.GetFragmentManager().BeginTransaction();
                transaction.Add(Id, materialCalendarFragment, "MaterialCalendar");
                transaction.Commit();
            });

            // Wait for the fragment to be fully created before adjusting alignment
            PostDelayed(() =>
            {
                CenterCalendarText();

                materialCalendarFragment.View.ViewTreeObserver.GlobalLayout += (sender, args) =>
                {
                    // Trigger centering logic after the layout is updated
                    PostDelayed(() =>
                    {
                        CenterCalendarText();
                    }, 100);
                };
            }, 500); // Delay in milliseconds to give time for fragment initialization


        }

        private void CenterCalendarText()
        {
            if (materialCalendarFragment?.View is ViewGroup viewGroup)
            {
                for (int i = 0; i < viewGroup.ChildCount; i++)
                {
                    var child = viewGroup.GetChildAt(i);

                    if (child is TextView textView)
                    {
                        // Center the text horizontally and vertically
                        textView.Gravity = GravityFlags.Center;
                    }
                    else if (child is ViewGroup childGroup)
                    {
                        // If the child is a ViewGroup, iterate over its children
                        CenterChildViews(childGroup);
                    }
                }
            }
        }

        private void CenterChildViews(ViewGroup viewGroup)
        {
            for (int i = 0; i < viewGroup.ChildCount; i++)
            {
                var child = viewGroup.GetChildAt(i);

                if (child is TextView textView)
                {
                    // Center the text horizontally and vertically
                    textView.Gravity = GravityFlags.Center;
                }
                else if (child is ViewGroup childGroup)
                {
                    // If the child is a ViewGroup, iterate over its children
                    CenterChildViews(childGroup);
                }
            }
        }


        private class EventIndicatorDayViewDecorator : DayViewDecorator
        {
            public NativeCalendarView NativeCalendarView;           
      

            public EventIndicatorDayViewDecorator(NativeCalendarView nativeCalendarView)
            {
                this.NativeCalendarView = nativeCalendarView;
            }

            public override int DescribeContents()
            {
                throw new NotImplementedException();
            }

            public override Drawable? GetCompoundDrawableBottom(Context context, int year, int month, int day, bool valid, bool selected)
            {
                // Check if the current date has an event
                if (NativeCalendarView.Events.Any(e => e.StartDate.Year == year && e.StartDate.Month == month + 1 && e.StartDate.Day == day))
                {
                    // Draw circle
                    ShapeDrawable drawable = new ShapeDrawable(new OvalShape());
                    drawable.Paint.Color = NativeCalendarView.EventIndicatorColor.ToPlatform();
                    drawable.Paint.SetStyle(Paint.Style.Fill);

                    // Set the bounds of the drawable to make sure it appears within the calendar cell
                    int size = 20; // You can adjust this size to control the size of the indicator circle
                    drawable.SetBounds(0, 0, size, size);
                    return drawable;
                }

                return base.GetCompoundDrawableTop(context, year, month, day, valid, selected);
            }

            public override void WriteToParcel(Parcel? dest, [GeneratedEnum] ParcelableWriteFlags flags)
            {
                throw new NotImplementedException();
            }
        }

    }
}
