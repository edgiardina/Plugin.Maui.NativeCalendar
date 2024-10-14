using CoreGraphics;
using Foundation;
using Microsoft.Extensions.Logging;
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

        private bool isLayoutInitialized = false;

        private NativeCalendarView nativeCalendarView;

        private List<UIView> eventIndicators = new List<UIView>();

        public NativeCalendarImplementation(NativeCalendarView nativeCalendarView)
        {
            // only add a calendar on iOS 16.0 or later
            if (UIDevice.CurrentDevice.CheckSystemVersion(16, 0))
            {
                calendarView = new UICalendarView();
                calendarView.Calendar = new NSCalendar(NSCalendarType.Gregorian);
                calendarView.SelectionBehavior = new UICalendarSelectionSingleDate();

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

                // Set the delegate for calendarView
                var calendarDelegate = new CalendarViewDelegate(nativeCalendarView.Events);
                calendarView.Delegate = calendarDelegate;

            }
            else
            {
                throw new Exception("iOS 16.0 or later is required to use the NativeCalendarView");
            }

            this.nativeCalendarView = nativeCalendarView;
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
            MaxDate = (NSDate)nativeCalendarView.MaximumDate;

            // Update the maximum date of the CalendarView
            calendarView.AvailableDateRange = new Foundation.NSDateInterval(MinDate, MaxDate);
        }

        public void UpdateMinimumDate(NativeCalendarView nativeCalendarView)
        {
            MinDate = (NSDate)nativeCalendarView.MinimumDate;

            // Update the minimum date of the CalendarView
            calendarView.AvailableDateRange = new Foundation.NSDateInterval(MinDate, MaxDate);
        }

        public void UpdateEvents(NativeCalendarView nativeCalendarView)
        {
            // TODO: is this enough?
            //calendarView.ReloadDecorations()         

        }

    }
}
