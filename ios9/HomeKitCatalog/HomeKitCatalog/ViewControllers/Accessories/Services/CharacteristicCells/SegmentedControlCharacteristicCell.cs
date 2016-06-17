using System;

using CoreGraphics;
using Foundation;
using HomeKit;
using UIKit;

namespace HomeKitCatalog
{
	// A `CharacteristicCell` subclass that contains a `UISegmentedControl`.
	// Used for `HMCharacteristic`s which have associated, non-numeric values, like Lock Management State.
	[Register ("SegmentedControlCharacteristicCell")]
	public class SegmentedControlCharacteristicCell : CharacteristicCell
	{
		[Outlet ("segmentedControl")]
		UISegmentedControl SegmentedControl { get; set; }

		// Calls the super class's didSet, and also computes a list of possible values.
		public override HMCharacteristic Characteristic {
			get {
				return base.Characteristic;
			}
			set {
				base.Characteristic = value;

				SegmentedControl.Alpha = Enabled ? 1 : DisabledAlpha;
				SegmentedControl.UserInteractionEnabled = Enabled;

				var possibleValues = value.AllPossibleValues ();
				if (possibleValues != null)
					PossibleValues = possibleValues;
			}
		}

		// The possible values for this characteristic.
		// When this is set, adds localized descriptions to the segmented control.
		int[] possibleValues = new int[0];
		int[] PossibleValues {
			get {
				return possibleValues;
			}
			set {
				possibleValues = value;

				SegmentedControl.RemoveAllSegments ();
				for (int i = 0; i < possibleValues.Length; i++) {
					var v = possibleValues [i];
					var title = Characteristic.DescriptionForValue (v);
					SegmentedControl.InsertSegment (title, i, false);
				}
				ResetSelectedIndex ();
			}
		}

		public SegmentedControlCharacteristicCell (IntPtr handle)
			: base (handle)
		{
		}

		[Export ("initWithFrame:")]
		public SegmentedControlCharacteristicCell (CGRect frame)
			: base (frame)
		{
		}

		[Export ("initWithCoder:")]
		public SegmentedControlCharacteristicCell (NSCoder coder)
			: base (coder)
		{
		}

		// Responds to the segmented control's segment changing.
		// Sets the value and notifies its delegate.
		[Export("segmentedControlDidChange:")]
		void segmentedControlDidChange(UISegmentedControl sender)
		{
			var value = possibleValues [(int)sender.SelectedSegment];
			SetValue (new NSNumber(value), true);
		}

		// If notify is false, then this is an external change,
		// so update the selected segment on the segmented control.
		public override void SetValue (NSObject newValue, bool notify)
		{
			base.SetValue (newValue, notify);
			if (!notify)
				ResetSelectedIndex ();
		}

		// Sets the segmented control based on the set value.
		void ResetSelectedIndex ()
		{
			var number = Value as NSNumber;
			if (number == null)
				return;

			var index = Array.IndexOf (PossibleValues, number.Int32Value);
			if (index < 0)
				return;

			SegmentedControl.SelectedSegment = index;
		}
	}
}