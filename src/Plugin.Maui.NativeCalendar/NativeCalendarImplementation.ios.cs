using CoreGraphics;
using Foundation;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Platform;
using Plugin.Maui.NativeCalendar.iOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace Plugin.Maui.NativeCalendar
{
    public class NativeCalendarImplementation : UIView
    {
        private readonly UICalendarView calendarView;

        private NSDate MaxDate { get; set; } = NSDate.DistantFuture;
        private NSDate MinDate { get; set; } = NSDate.DistantPast;

        private NativeCalendarView nativeCalendarView;

        private readonly CalendarSelectionSingleDateDelegate calendarSelectionSingleDateDelegate;

        public NativeCalendarImplementation(NativeCalendarView nativeCalendarView)
        {
            // only add a calendar on iOS 16.0 or later
            if (UIDevice.CurrentDevice.CheckSystemVersion(16, 0))
            {
                calendarView = new UICalendarView();
                calendarView.Calendar = new NSCalendar(NSCalendarType.Gregorian);

                calendarSelectionSingleDateDelegate = new CalendarSelectionSingleDateDelegate(nativeCalendarView);

                calendarView.SelectionBehavior = new UICalendarSelectionSingleDate(calendarSelectionSingleDateDelegate);

                // Enable Auto Layout
                calendarView.TranslatesAutoresizingMaskIntoConstraints = false;

                AddSubview(calendarView);

                // Set constraints to fill the parent view
                NSLayoutConstraint.ActivateConstraints(new[]
                {
                    calendarView.LeadingAnchor.ConstraintEqualTo(this.LeadingAnchor),
                    calendarView.TrailingAnchor.ConstraintEqualTo(this.TrailingAnchor),
                    calendarView.TopAnchor.ConstraintEqualTo(this.TopAnchor),
                    calendarView.BottomAnchor.ConstraintEqualTo(this.BottomAnchor)
                });

                // TODO: is this needed? it seems in iOS the background color bleeds through
                if(nativeCalendarView.BackgroundColor != null)  
                    calendarView.BackgroundColor = nativeCalendarView.BackgroundColor.ToPlatform();

                // Set the delegate for calendarView
                calendarView.Delegate = new CalendarViewDelegate(nativeCalendarView.Events, nativeCalendarView.EventIndicatorColor.ToPlatform());

            }
            else
            {
                throw new Exception("iOS 16.0 or later is required to use the NativeCalendarView");
            }

            this.nativeCalendarView = nativeCalendarView;
        }

        public void UpdateTintColor(NativeCalendarView nativeCalendarView)
        {
            calendarView.TintColor = nativeCalendarView.TintColor.ToPlatform();
        }

        public void UpdateTitleTextColor(NativeCalendarView nativeCalendarView)
        {
            // TODO: Set the title text color
            //calendarView = nativeCalendarView.TitleTextColor.ToPlatform();
        }

        public void UpdateHeaderColor(NativeCalendarView nativeCalendarView)
        {
            // No-op?
        }

        public void UpdateSelectedDate(NativeCalendarView nativeCalendarView)
        {
            // Create NSDateComponents from the DateTime
            var dateTime = nativeCalendarView.SelectedDate;

            NSDateComponents dateComponents = new NSDateComponents
            {
                Year = dateTime.Year,
                Month = dateTime.Month,
                Day = dateTime.Day
            };

            // Cast to UICalendarSelectionSingleDate and set the SelectedDate
            if (calendarView.SelectionBehavior is UICalendarSelectionSingleDate singleDateSelection)
            {
                singleDateSelection.SelectedDate = dateComponents;
            }
        }

        public void UpdateMaximumDate(NativeCalendarView nativeCalendarView)
        {
            if(nativeCalendarView.MaximumDate != null && nativeCalendarView.MaximumDate != DateTime.MinValue)
                MaxDate = (NSDate)nativeCalendarView.MaximumDate;

            // Update the maximum date of the CalendarView
            calendarView.AvailableDateRange = new Foundation.NSDateInterval(MinDate, MaxDate);
        }

        public void UpdateMinimumDate(NativeCalendarView nativeCalendarView)
        {
            if (nativeCalendarView.MinimumDate != null && nativeCalendarView.MinimumDate != DateTime.MinValue)
                MinDate = (NSDate)nativeCalendarView.MinimumDate;

            // Update the minimum date of the CalendarView
            calendarView.AvailableDateRange = new Foundation.NSDateInterval(MinDate, MaxDate);
        }

        public void UpdateEvents(NativeCalendarView nativeCalendarView)
        {
            // Remove and reset the delegate to force a decoration re-evaluation
            calendarView.Delegate = null;

            // TODO: is this enough?
            calendarView.Delegate = new CalendarViewDelegate(nativeCalendarView.Events, nativeCalendarView.EventIndicatorColor.ToPlatform());

            // Trigger a layout update to redraw the decorations
            calendarView.SetNeedsLayout();
            calendarView.LayoutIfNeeded();
        }

    }
}
