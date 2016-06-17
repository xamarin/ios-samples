using System;

using CoreGraphics;
using Foundation;
using HomeKit;
using UIKit;

namespace HomeKitCatalog
{
	// A `UITableViewCell` subclass that displays the current value of an `HMCharacteristic` and
	// notifies its delegate of changes. Subclasses of this class will provide additional controls
	// to display different kinds of data.
	[Register ("CharacteristicCell")]
	public class CharacteristicCell : UITableViewCell
	{
		// An alpha percentage used when disabling cells.
		protected readonly nfloat DisabledAlpha = 0.4f;

		[Outlet ("typeLabel")]
		public UILabel TypeLabel { get; set; }

		[Outlet ("valueLabel")]
		public UILabel ValueLabel { get; set; }

		[Outlet ("favoriteButton")]
		public UIButton FavoriteButton { get; set; }

		[Outlet ("favoriteButtonWidthConstraint")]
		public NSLayoutConstraint FavoriteButtonWidthConstraint { get; set; }

		[Outlet ("favoriteButtonHeightContraint")]
		public NSLayoutConstraint FavoriteButtonHeightContraint { get; set; }

		// Show / hide the favoriteButton and adjust the constraints to ensure proper layout.
		bool showsFavorites;

		public bool ShowsFavorites {
			get {
				return showsFavorites;
			}
			set {
				showsFavorites = value;
				if (showsFavorites) {
					FavoriteButton.Hidden = false;
					FavoriteButtonWidthConstraint.Constant = FavoriteButtonHeightContraint.Constant;
				} else {
					FavoriteButton.Hidden = true;
					FavoriteButtonWidthConstraint.Constant = 15;
				}
			}
		}

		// returns: `true` if the represented characteristic is reachable; `false` otherwise.
		public bool Enabled {
			get {
				var c = Characteristic;
				if (c == null)
					return false;

				var s = c.Service;
				if (s == null)
					return false;

				var a = s.Accessory;
				return a != null && a.Reachable;
			}
		}

		/// Subclasses can return false if they have many frequent updates that should be deferred.
		protected virtual bool UpdatesImmediately {
			get {
				return true;
			}
		}

		protected NSObject Value { get; set; }

		public ICharacteristicCellDelegate Delegate { get; set; }

		// The characteristic represented by this cell.
		// When this is set, the cell populates based on
		// the characteristic's value and requests an initial value from its delegate.
		HMCharacteristic characteristic;

		public virtual HMCharacteristic Characteristic {
			get {
				return characteristic;
			}
			set {
				characteristic = value;
				TypeLabel.Text = characteristic.LocalizedCharacteristicType ();
				SelectionStyle = characteristic.IsIdentify () ? UITableViewCellSelectionStyle.Default : UITableViewCellSelectionStyle.None;
				SetValue (characteristic.Value, false);

				// Don't read the value for write-only characteristics.
				if (characteristic.IsWriteOnly ())
					return;

				// Set initial state of the favorite button
				FavoriteButton.Selected = characteristic.IsFavorite ();

				// "Enable" the cell if the accessory is reachable or we are displaying the favorites.
				// Configure the views.
				var alpha = Enabled ? 1 : DisabledAlpha;

				TypeLabel.Alpha = alpha;

				var lable = ValueLabel;
				if(lable != null)
					ValueLabel.Alpha = alpha;

				if (Enabled) {
					Delegate.CharacteristicCellReadInitialValueForCharacteristic (this, characteristic, (v, error) => {
						if (error != null)
							Console.WriteLine ("HomeKit: Error reading value for characteristic {0}: {1}.", Characteristic, error.LocalizedDescription);
						else
							SetValue (v, false);
					});
				}
			}
		}

		public CharacteristicCell (IntPtr handle)
			: base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public CharacteristicCell (NSCoder coder)
			: base (coder)
		{
		}

		[Export ("initWithFrame:")]
		public CharacteristicCell (CGRect frame)
			: base (frame)
		{
		}

		// Resets the value label to the localized description from HMCharacteristic+Readability.
		public void ResetValueLabel ()
		{
			var label = ValueLabel;
			if (Value != null && label != null)
				label.Text = characteristic.DescriptionForValue (Value);
		}

		// Toggles the star button and saves the favorite status
		// of the characteristic in the FavoriteManager.
		[Export ("didTapFavoriteButton:")]
		void DidTapFavoriteButton (UIButton sender)
		{
			sender.Selected = !sender.Selected;
			characteristic.IsFavorite (sender.Selected);
		}

		// Sets the cell's value and resets the label.
		public virtual void SetValue (NSObject newValue, bool notify)
		{
			Value = newValue;
			if (newValue != null) {
				ResetValueLabel ();
				// We do not allow the setting of nil values from the app, but we do have to deal with incoming nil values.
				if (notify && Delegate != null)
					Delegate.CharacteristicCellDidUpdateValueForCharacteristic (this, newValue, characteristic, UpdatesImmediately);
			}
		}
	}
}