using CoreGraphics;
using System;
using UIKit;

namespace AdaptiveElements {
	/// <summary>
	/// ExampleContainerViewController is the core of this sample code project.
	/// It demonstrates:
	/// - How to decide which design to use depending on the view's size
	/// - How to apply the design to the UI
	/// - How to correctly add and remove child view controllers
	/// </summary>
	public partial class ExampleContainerViewController : UIViewController {
		/// <summary>
		/// It holds our 3 child view controllers.
		/// They will either be instances of SmallElementViewController or LargeElementViewController.
		/// </summary>
		private readonly UIViewController [] elementViewControllers = { null, null, null };

		/// <summary>
		/// It is the design that is currently displayed in the view controller.
		/// It is initially nil because no design has been displayed yet.
		/// </summary>
		private Design displayedDesign;

		public ExampleContainerViewController (IntPtr handle) : base (handle) { }

		public override void ViewWillLayoutSubviews ()
		{
			/*
             * In viewWillLayoutSubviews, we are guaranteed that our view's size, traits, etc. are up to date.
             * It's a good place to update anything that affects the layout of our subviews.
             * However, be careful, because this method is called frequently!
             * Do as little work as possible, and don't invalidate the layout of any superviews.
             */

			// Step 1: Find our size.
			var size = base.View.Bounds.Size;

			// Step 2: Decide what design to use, based on our rules.
			var newDesign = this.DecideDesign (size);

			// Step 3: If the design is different than what is displayed, change the UI.
			if (displayedDesign != newDesign) {
				this.ApplyDesign (newDesign);
				this.displayedDesign = newDesign;
			}
		}

		private Design DecideDesign (CGSize size)
		{
			/*
             * Decide which Design is appropriate, given the size of our view, by applying rules.
             *            
             * Note that these rules are _examples_, which produce good results in this particular sample app,
             * but they are not general rules that would automatically work in other apps.
             */

			/*
             * Decision #1: Should our elements be laid out horizontally or vertically?
             * Rule: If the width is greater that the height, be horizontal, otherwise be vertical.
             */

			var axis = size.Width > size.Height ? UILayoutConstraintAxis.Horizontal : UILayoutConstraintAxis.Vertical;

			/*
             * Decision #2: Should our elements be small or large?
             * Rule: If the width is less than a threshold value, be small, otherwise be large.
             * (We chose 750 as a threshold value since it produces reasonable results for this example, but there is nothing special about that number.)
             */

			const float widthThreshold = 750f;
			var elementKind = size.Width < widthThreshold ? Design.ElementKind.Small : Design.ElementKind.Large;

			// Return a Design encapsulating the results of those decisions.
			return new Design { Axis = axis, Kind = elementKind };
		}

		private void ApplyDesign (Design newDesign)
		{
			/*
             * Change the view controllers and views to display the new design.
             * Be careful to only change properties that need to be changed.
             */

			// Set the stack view's layout axis to horizontal or vertical.
			if (this.displayedDesign?.Axis != newDesign.Axis) {
				this.stackView.Axis = newDesign.Axis;
			}

			// Change the view controllers to the small or large kind.
			if (this.displayedDesign?.Kind != newDesign.Kind) {
				// Repeat these steps for each of the element view controllers:
				for (int i = 0; i < this.elementViewControllers.Length; i++) {
					var oldElementViewController = this.elementViewControllers [i];

					// If an old view controller exists, then remove it from this container's child view controllers.
					if (oldElementViewController != null) {
						this.RemoveOldElementViewController (oldElementViewController);
					}

					// Create the new view controller.
					var storyboard = UIStoryboard.FromName ("Main", null);
					var newElementViewController = storyboard.InstantiateViewController (newDesign.ElementIdentifier);

					// Add it as a child view controller of this container.
					this.AddNewElementViewController (newElementViewController);

					// And remember it, so we can remove it later.
					this.elementViewControllers [i] = newElementViewController;
				}
			}
		}

		/* Helper functions to be a well-behaved container view controller: */

		private void AddNewElementViewController (UIViewController elementViewController)
		{
			// Step 1: Add this view controller to our list of child view controllers.
			// This will call elementViewController.willMove(toParentViewController: self).
			base.AddChildViewController (elementViewController);

			// Step 2: Add the view controller's view to our view hierarchy.
			this.stackView.AddArrangedSubview (elementViewController.View);

			// Step 3: Tell the view controller that it has moved, and `self` is the new parent.
			elementViewController.DidMoveToParentViewController (this);
		}

		private void RemoveOldElementViewController (UIViewController elementViewController)
		{
			// Step 1: Tell the view controller that it will move to having no parent view controller.
			elementViewController.WillMoveToParentViewController (null);

			// Step 2: Remove the view controller's view from our view hierarchy.
			elementViewController.View.RemoveFromSuperview ();

			// Step 3: Remove the view controller from our list of child view controllers.
			// This will call elementViewController.didMove(toParentViewController: nil).
			elementViewController.RemoveFromParentViewController ();
		}
	}
}
