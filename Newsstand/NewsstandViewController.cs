using System;
using CoreGraphics;
using System.Linq;
using UIKit;
using Foundation;

namespace Newsstand
{
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
			btnGetLibrary = UIButton.FromType(UIButtonType.RoundedRect);
			btnGetLibrary.Frame = new CGRect(10,10,145,50);
			btnGetLibrary.SetTitle("Get Library", UIControlState.Normal);
			
			btnPopulate = UIButton.FromType(UIButtonType.RoundedRect);
			btnPopulate.Frame = new CGRect(165,10,145,50);
			btnPopulate.SetTitle("Populate Library", UIControlState.Normal);

			btnSetReading = UIButton.FromType(UIButtonType.RoundedRect);
			btnSetReading.Frame = new CGRect(10,70,145,50);
			btnSetReading.SetTitle("Set Reading", UIControlState.Normal);
			
			btnDownload = UIButton.FromType(UIButtonType.RoundedRect);
			btnDownload.Frame = new CGRect(165,70,145,50);
			btnDownload.SetTitle("Download", UIControlState.Normal);

			btnRead = UIButton.FromType(UIButtonType.RoundedRect);
			btnRead.Frame = new CGRect(10,130,145,50);
			btnRead.SetTitle("Read", UIControlState.Normal);

			btnIcon = UIButton.FromType(UIButtonType.RoundedRect);
			btnIcon.Frame = new CGRect(165,130,145,50);
			btnIcon.SetTitle("Update Icon", UIControlState.Normal);

			txtView = new UITextView(new CGRect(10, 190, 300, 250));
			txtView.Editable = false;
			txtView.ScrollEnabled = true;
			
			// Wire up the buttons to the SamplCode class methods
			btnGetLibrary.TouchUpInside += (sender, e) => {
				SampleCode.GetLibrary(txtView);
			};

			btnPopulate.TouchUpInside += (sender, e) => {
				SampleCode.PopulateLibrary (txtView);
			};

			btnSetReading.TouchUpInside += (sender, e) => {
				SampleCode.SetReading(txtView);
			};

			btnDownload.TouchUpInside += (sender, e) => {
				SampleCode.Download(txtView);
			};

			btnRead.TouchUpInside += (sender, e) => {
				SampleCode.Read(txtView);
			};
			
			btnIcon.TouchUpInside += (sender, e) => {
				SampleCode.UpdateIcon(txtView);
			};

			// Add the controls to the view
			this.Add(btnGetLibrary);
			this.Add(btnSetReading);
			this.Add(btnPopulate);
			this.Add(btnDownload);
			this.Add(btnRead);
			this.Add(btnIcon);
			
			this.Add(txtView);
		}
	}
}