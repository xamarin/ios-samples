using System;

using Foundation;
using UIKit;
using ObjCRuntime;

namespace SegueCatalog
{
	/// <summary>
	/// This segue subclass demonstrates how to adapt a view controller that has been
	/// modally presented by a segue. An instance of this segue is triggered by the 
	/// Form Sheet nav item owned by an OuterViewController. It is configured to show 
	/// a Form Sheet presentation, but Form Sheets are not permissible in Compact 
	/// size classes. By default, UIKit will adapt a Form Sheet presentation to a
	/// full-screen presentation, using the same view controller. We would like to 
	/// provide an alternative representation in the adapted case.
	/// </summary>
	public partial class AdaptableFormSheetSegue : UIStoryboardSegue, IUIAdaptivePresentationControllerDelegate
	{
		public AdaptableFormSheetSegue (IntPtr handle)
			: base (handle)
		{
		}

		public override void Perform ()
		{
			/*
			Because this class is used for a Present Modally segue, UIKit will 
			maintain a strong reference to this segue object for the duration of
			the presentation. That way, this segue object will live long enough 
			to act as the presentation controller's delegate and customize any 
			adaptation.
			*/

			DestinationViewController.PresentationController.Delegate = this;
			base.Perform ();
		}

		[Export ("adaptivePresentationStyleForPresentationController:traitCollection:")]
		public UIModalPresentationStyle GetAdaptivePresentationStyle (UIPresentationController controller, UITraitCollection traitCollection)
		{
			return traitCollection.HorizontalSizeClass == UIUserInterfaceSizeClass.Compact ? UIModalPresentationStyle.FullScreen : UIModalPresentationStyle.FormSheet;
		}

		[Export ("presentationController:viewControllerForAdaptivePresentationStyle:")]
		public UIKit.UIViewController GetViewControllerForAdaptivePresentation (UIPresentationController controller, UIModalPresentationStyle style)
		{
			/*
			Load and return the adapted view controller from the Detail storyboard. 
			That storyboard is stored within the same bundle that contains this 
			class.
			*/

			var adaptableFormSheetSegueBundle = NSBundle.FromClass (new Class (typeof(AdaptableFormSheetSegue)));
			return UIStoryboard.FromName ("Detail", adaptableFormSheetSegueBundle).InstantiateViewController ("Adapted");
		}
	}
}
