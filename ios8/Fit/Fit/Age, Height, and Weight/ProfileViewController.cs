using System;
using System.Collections.Generic;
using Foundation;
using HealthKit;
using UIKit;

namespace Fit
{
	public partial class ProfileViewController : UITableViewController, IHealthStore
	{
		NSNumberFormatter numberFormatter;

		public HKHealthStore HealthStore { get; set; }

		NSSet DataTypesToWrite {
			get {
				return NSSet.MakeNSObjectSet <HKObjectType> (new HKObjectType[] {
					HKQuantityType.GetQuantityType (HKQuantityTypeIdentifierKey.DietaryEnergyConsumed),
					HKQuantityType.GetQuantityType (HKQuantityTypeIdentifierKey.ActiveEnergyBurned),
					HKQuantityType.GetQuantityType (HKQuantityTypeIdentifierKey.Height),
					HKQuantityType.GetQuantityType (HKQuantityTypeIdentifierKey.BodyMass)
				});
			}
		}

		NSSet DataTypesToRead {
			get {
				return NSSet.MakeNSObjectSet <HKObjectType> (new HKObjectType[] {
					HKQuantityType.GetQuantityType (HKQuantityTypeIdentifierKey.DietaryEnergyConsumed),
					HKQuantityType.GetQuantityType (HKQuantityTypeIdentifierKey.ActiveEnergyBurned),
					HKQuantityType.GetQuantityType (HKQuantityTypeIdentifierKey.Height),
					HKQuantityType.GetQuantityType (HKQuantityTypeIdentifierKey.BodyMass),
					HKCharacteristicType.GetCharacteristicType (HKCharacteristicTypeIdentifierKey.DateOfBirth)
				});
			}
		}

		public ProfileViewController (IntPtr handle) : base (handle)
		{
		}

		public override async void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (HKHealthStore.IsHealthDataAvailable) {

				var success = await HealthStore.RequestAuthorizationToShareAsync (DataTypesToWrite, DataTypesToRead);

				if (!success.Item1) {
					Console.WriteLine ("You didn't allow HealthKit to access these read/write data types. " +
					"In your app, try to handle this error gracefully when a user decides not to provide access. " +
					"If you're using a simulator, try it on a device.");
					return;
				}

				numberFormatter = new NSNumberFormatter ();

				UpdateUsersAge ();
				UpdateUsersHeight ();
				UpdateUsersWeight ();
			}
		}

		void UpdateUsersAge ()
		{
			NSError error;
			NSDate dateOfBirth = HealthStore.GetDateOfBirth (out error);

			if (error != null) {
				Console.WriteLine ("An error occured fetching the user's age information. " +
				"In your app, try to handle this gracefully. The error was: {0}", error);
				return;
			}

			if (dateOfBirth == null)
				return;

			var now = NSDate.Now;

			NSDateComponents ageComponents = NSCalendar.CurrentCalendar.Components (NSCalendarUnit.Year, dateOfBirth, now,
				                                 NSCalendarOptions.WrapCalendarComponents);

			nint usersAge = ageComponents.Year;

			ageHeightValueLabel.Text = string.Format ("{0} years", usersAge);
		}

		void UpdateUsersHeight ()
		{
			var heightType = HKQuantityType.GetQuantityType (HKQuantityTypeIdentifierKey.Height);

			FetchMostRecentData (heightType, (mostRecentQuantity, error) => {
				if (error != null) {
					Console.WriteLine ("An error occured fetching the user's height information. " +
					"In your app, try to handle this gracefully. The error was: {0}.", error.LocalizedDescription);
					return;
				}

				double usersHeight = 0.0;

				if (mostRecentQuantity != null) {
					var heightUnit = HKUnit.Inch;
					usersHeight = mostRecentQuantity.GetDoubleValue (heightUnit);
				}

				InvokeOnMainThread (delegate {
					heightValueLabel.Text = numberFormatter.StringFromNumber (new NSNumber (usersHeight));
				});
			});
		}

		void UpdateUsersWeight ()
		{
			var weightType = HKQuantityType.GetQuantityType (HKQuantityTypeIdentifierKey.BodyMass);

			FetchMostRecentData (weightType, (mostRecentQuantity, error) => {
				if (error != null) {
					Console.WriteLine ("An error occured fetching the user's age information. " +
					"In your app, try to handle this gracefully. The error was: {0}", error.LocalizedDescription);
					return;
				}

				double usersWeight = 0.0;

				if (mostRecentQuantity != null) {
					var weightUnit = HKUnit.Pound;
					usersWeight = mostRecentQuantity.GetDoubleValue (weightUnit);
				}

				InvokeOnMainThread (delegate {
					weightValueLabel.Text = numberFormatter.StringFromNumber (new NSNumber (usersWeight));
				});
			}
			);
		}

		void FetchMostRecentData (HKQuantityType quantityType, Action <HKQuantity, NSError> completion)
		{
			var timeSortDescriptor = new NSSortDescriptor (HKSample.SortIdentifierEndDate, false);
			var query = new HKSampleQuery (quantityType, null, 1, new NSSortDescriptor[] { timeSortDescriptor },
				            (HKSampleQuery resultQuery, HKSample[] results, NSError error) => {
					if (completion != null && error != null) {
						completion (null, error);
						return;
					}

					HKQuantity quantity = null;
					if (results.Length != 0) {
						var quantitySample = (HKQuantitySample)results [results.Length - 1];
						quantity = quantitySample.Quantity;
					}

					if (completion != null)
						completion (quantity, error);
				});

			HealthStore.ExecuteQuery (query);
		}

		void SaveHeightIntoHealthStore (double value)
		{
			var heightQuantity = HKQuantity.FromQuantity (HKUnit.Inch, value);
			var heightType = HKQuantityType.GetQuantityType (HKQuantityTypeIdentifierKey.Height);
			var heightSample = HKQuantitySample.FromType (heightType, heightQuantity, NSDate.Now, NSDate.Now, new NSDictionary ());

			HealthStore.SaveObject (heightSample, (success, error) => {
				if (!success) {
					Console.WriteLine ("An error occured saving the height sample {0}. " +
					"In your app, try to handle this gracefully. The error was: {1}.", heightSample, error);
					return;
				}

				UpdateUsersHeight ();
			});
		}

		void SaveWeightIntoHealthStore (double value)
		{
			var weightQuantity = HKQuantity.FromQuantity (HKUnit.Pound, value);
			var weightType = HKQuantityType.GetQuantityType (HKQuantityTypeIdentifierKey.BodyMass);
			var weightSample = HKQuantitySample.FromType (weightType, weightQuantity, NSDate.Now, NSDate.Now, new NSDictionary ());

			HealthStore.SaveObject (weightSample, (success, error) => {
				if (!success) {
					Console.WriteLine ("An error occured saving the weight sample {0}. " +
						"In your app, try to handle this gracefully. The error was: {1}.", weightSample, error.LocalizedDescription);
					return;
				}

				UpdateUsersWeight ();
			});
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			string title = string.Empty;
			Action<double> valueChangedHandler = null;
			if (indexPath.Row == 0)
				return;

			switch (indexPath.Row) {
			case 1:
				title = "Your Height";
				valueChangedHandler = SaveHeightIntoHealthStore;
				break;
			case 2:
				title = "Your Weight";
				valueChangedHandler = SaveWeightIntoHealthStore;
				break;
			}

			var alertController = UIAlertController.Create (title, string.Empty, UIAlertControllerStyle.Alert);
			alertController.AddTextField ((textField) => {
				textField.KeyboardType = UIKeyboardType.DecimalPad;
			});

			var okAction = UIAlertAction.Create ("OK", UIAlertActionStyle.Default, (alertAction) => {
				var textField = alertController.TextFields [0];
				double value;
				Double.TryParse (textField.Text, out value);
				if(valueChangedHandler != null)
					valueChangedHandler (value);
				TableView.DeselectRow (indexPath, true);
			});

			alertController.AddAction (okAction);

			var cancelAction = UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, (alertAction) => {
				TableView.DeselectRow (indexPath, true);
			});

			alertController.AddAction (cancelAction);
			PresentViewController (alertController, true, null);
		}
	}
}
