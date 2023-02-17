using System;
using Foundation;
using UIKit;

namespace tvFocus {
	/// <summary>
	/// Controls the main View for the app and is responsible for setting up a Foucs Guide
	/// <c>UIFocusGuide</c> that allows the user to move between the More Info and Buy
	/// button in the User Interface.
	/// </summary>
	public partial class ViewController : UIViewController {
		#region Private Variables
		/// <summary>
		/// Private storage for the Focus Guide that will allow us to
		/// move between buttons.
		/// </summary>
		public UIFocusGuide FocusGuide = new UIFocusGuide ();
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="T:tvFocus.ViewController"/> class.
		/// </summary>
		/// <param name="handle">Handle.</param>
		public ViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Override Methods
		/// <summary>
		/// Called directly after the VIew has been loaded into memory and allows you
		/// to initialize the view.
		/// </summary>
		/// <remarks>We are building the Focus Guide here and attaching it to the view.</remarks>
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Add Focus Guide to layout
			View.AddLayoutGuide (FocusGuide);

			// Define Focus Guide that will allow the user to move
			// between the More Info and Buy buttons.
			FocusGuide.LeftAnchor.ConstraintEqualTo (BuyButton.LeftAnchor).Active = true;
			FocusGuide.TopAnchor.ConstraintEqualTo (MoreInfoButton.TopAnchor).Active = true;
			FocusGuide.WidthAnchor.ConstraintEqualTo (BuyButton.WidthAnchor).Active = true;
			FocusGuide.HeightAnchor.ConstraintEqualTo (MoreInfoButton.HeightAnchor).Active = true;
		}

		/// <summary>
		/// Called after the Focus Engine has been called to change the Focus from an existing item
		/// to a new item.
		/// </summary>
		/// <param name="context">The context of the movement.</param>
		/// <param name="coordinator">An Animation Coordinator that you can use to animat the focus change.</param>
		/// <remarks>We are telling the Focus Guide where to move focus to from this method.</remarks>
		public override void DidUpdateFocus (UIFocusUpdateContext context, UIFocusAnimationCoordinator coordinator)
		{
			base.DidUpdateFocus (context, coordinator);

			// Get next focusable item from context
			var nextFocusableItem = context.NextFocusedView;

			// Anything to process?
			if (nextFocusableItem == null) return;

			// Decide the next focusable item based on the current
			// item with focus
			if (nextFocusableItem == MoreInfoButton) {
				// Move from the More Info to Buy button
				FocusGuide.PreferredFocusedView = BuyButton;
			} else if (nextFocusableItem == BuyButton) {
				// Move from the Buy to the More Info button
				FocusGuide.PreferredFocusedView = MoreInfoButton;
			} else {
				// No valid move
				FocusGuide.PreferredFocusedView = null;
			}
		}
		#endregion

		#region Actions
		/// <summary>
		/// This action is triggered when the More Info button is clicked using the Apple Remote.
		/// </summary>
		/// <param name="sender">Sender.</param>
		partial void MoreInfoButton_PrimaryActionTriggered (UIButton sender)
		{
			AlertViewController.PresentOKAlert ("More Information", "Sorry but more information on the currently selected flower is not available", this);
		}

		/// <summary>
		/// This action is triggered when the Buy button is clicked using the Apple Remote.
		/// </summary>
		/// <param name="sender">Sender.</param>
		partial void BuyButton_PrimaryActionTriggered (UIButton sender)
		{
			AlertViewController.PresentOKCancelAlert ("Add Flower to Shopping Cart",
													 "Would you like to add the currently selected flower to your shopping cart?",
													 this,
													 (ok) => {

													 });
		}
		#endregion
	}
}


