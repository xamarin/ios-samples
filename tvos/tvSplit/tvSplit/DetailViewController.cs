using System;
using Foundation;
using UIKit;

namespace tvSplit
{
	public partial class DetailViewController : UIViewController
	{
		#region Computed Properties
		public MainSplitViewController SplitViewController { get; set;}

		public string Title {
			get {return PageTitle.Text; }
			set { PageTitle.Text = value; }
		}

		public string FirstChoice {
			get {return ButtonA.Title(UIControlState.Normal); }
			set { 
				ButtonA.SetTitle (value, UIControlState.Normal);
				BackgroundImage.Image = UIImage.FromFile ("default.jpg");
			}
		}

		public string SecondChoice {
			get {return ButtonB.Title(UIControlState.Normal); }
			set { ButtonB.SetTitle (value, UIControlState.Normal); }
		}
		#endregion

		#region Constructors
		public DetailViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Custom Actions
		partial void PlayPausePressed (Foundation.NSObject sender) {

			// Show hide split view
			if (SplitViewController.DisplayMode == UISplitViewControllerDisplayMode.PrimaryHidden) {
				SplitViewController.PreferredDisplayMode = UISplitViewControllerDisplayMode.AllVisible;
			} else {
				SplitViewController.PreferredDisplayMode = UISplitViewControllerDisplayMode.PrimaryHidden;
			}

		}

		partial void ChooseA (NSObject sender)
		{
			BackgroundImage.Image = UIImage.FromFile (string.Format("{0}.jpg",ButtonA.Title(UIControlState.Normal)));
		}

		partial void ChooseB (NSObject sender)
		{
			BackgroundImage.Image = UIImage.FromFile (string.Format("{0}.jpg",ButtonB.Title(UIControlState.Normal)));
		}
		#endregion
	}
}
