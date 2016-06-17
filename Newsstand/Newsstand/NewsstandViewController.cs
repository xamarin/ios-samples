using CoreGraphics;
using UIKit;

namespace Newsstand {
	/// <summary>
	/// View containing Buttons and TextView to show off the samples
	/// </summary>
	public class NewsstandViewController : UIViewController
	{
		UIButton btnGetLibrary, btnSetReading, btnPopulate, btnDownload, btnRead, btnIcon;
		UITextView txtView;

		public override void ViewDidLoad ()
		{	
			base.ViewDidLoad ();
			
			// Create the buttons and TextView to run the sample code
			btnGetLibrary = UIButton.FromType (UIButtonType.RoundedRect);
			btnGetLibrary.Frame = new CGRect (10f,10f,145f, 50f);
			btnGetLibrary.SetTitle ("Get Library", UIControlState.Normal);
			
			btnPopulate = UIButton.FromType (UIButtonType.RoundedRect);
			btnPopulate.Frame = new CGRect (165f, 10f, 145f, 50f);
			btnPopulate.SetTitle ("Populate Library", UIControlState.Normal);

			btnSetReading = UIButton.FromType (UIButtonType.RoundedRect);
			btnSetReading.Frame = new CGRect(10f, 70f, 145f, 50f);
			btnSetReading.SetTitle("Set Reading", UIControlState.Normal);
			
			btnDownload = UIButton.FromType (UIButtonType.RoundedRect);
			btnDownload.Frame = new CGRect (165f, 70f, 145f, 50f);
			btnDownload.SetTitle ("Download", UIControlState.Normal);

			btnRead = UIButton.FromType (UIButtonType.RoundedRect);
			btnRead.Frame = new CGRect (10f, 130f, 145f, 50f);
			btnRead.SetTitle ("Read", UIControlState.Normal);

			btnIcon = UIButton.FromType (UIButtonType.RoundedRect);
			btnIcon.Frame = new CGRect (165f, 130f, 145f, 50f);
			btnIcon.SetTitle ("Update Icon", UIControlState.Normal);

			txtView = new UITextView(new CGRect(10f, 190f, 300f, 250f)) {
				Editable = false,
				ScrollEnabled = true
			};
			
			// Wire up the buttons to the SamplCode class methods
			btnGetLibrary.TouchUpInside += (sender, e) => {
				SampleCode.GetLibrary (txtView);
			};

			btnPopulate.TouchUpInside += (sender, e) => {
				SampleCode.PopulateLibrary (txtView);
			};

			btnSetReading.TouchUpInside += (sender, e) => {
				SampleCode.SetReading (txtView);
			};

			btnDownload.TouchUpInside += (sender, e) => {
				SampleCode.Download (txtView);
			};

			btnRead.TouchUpInside += (sender, e) => {
				SampleCode.Read (txtView);
			};
			
			btnIcon.TouchUpInside += (sender, e) => {
				SampleCode.UpdateIcon (txtView);
			};

			// Add the controls to the view
			Add (btnGetLibrary);
			Add (btnSetReading);
			Add (btnPopulate);
			Add (btnDownload);
			Add (btnRead);
			Add (btnIcon);
			Add (txtView);
		}
	}
}