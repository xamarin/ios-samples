using System;
using CoreGraphics;
using System.Linq;
using UIKit;
using Foundation;

namespace FileSystem
{
	/// <summary>
	/// View containing Buttons and TextView to show off the samples
	/// </summary>
	public class FileSystemViewController : UIViewController
	{
		UIButton btnFiles, btnDirectories, btnXml, btnAll, btnWrite, btnDirectory;
		UITextView txtView;
		bool writeJson = true;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

            var topMargin = 50;
            var margin = 10;
            var width = View.Bounds.Width;
            var buttonwidth = width / 2 - 30;
            var buttonHeight = 50;
            var height = View.Bounds.Height;

			// Create the buttons and TextView to run the sample code
			btnFiles = UIButton.FromType(UIButtonType.RoundedRect);
			btnFiles.Frame = new CGRect(margin, topMargin, buttonwidth, buttonHeight);
			btnFiles.SetTitle("Open 'ReadMe.txt'", UIControlState.Normal);

			btnDirectories = UIButton.FromType(UIButtonType.RoundedRect);
			btnDirectories.Frame = new CGRect(margin, topMargin+buttonHeight+margin, buttonwidth, buttonHeight);
			btnDirectories.SetTitle("List Directories", UIControlState.Normal);

			btnXml = UIButton.FromType(UIButtonType.RoundedRect);
			btnXml.Frame = new CGRect(buttonwidth + 2 * margin, topMargin, buttonwidth, buttonHeight);
			btnXml.SetTitle("Open 'Test.xml'", UIControlState.Normal);

			btnAll = UIButton.FromType(UIButtonType.RoundedRect);
			btnAll.Frame = new CGRect(buttonwidth + 2 * margin, topMargin + buttonHeight + margin, buttonwidth, buttonHeight);
			btnAll.SetTitle("List All", UIControlState.Normal);

			btnWrite = UIButton.FromType(UIButtonType.RoundedRect);
			btnWrite.Frame = new CGRect(margin, topMargin + 2*(buttonHeight + margin), buttonwidth, buttonHeight);
			btnWrite.SetTitle("Write File", UIControlState.Normal);

			btnDirectory = UIButton.FromType(UIButtonType.RoundedRect);
			btnDirectory.Frame = new CGRect(buttonwidth + 2 * margin, topMargin + 2 * (buttonHeight + margin), buttonwidth, buttonHeight);
			btnDirectory.SetTitle("Create Directory", UIControlState.Normal);

            var topText = topMargin + 3 * (buttonHeight + margin);
            txtView = new UITextView(new CGRect(margin, topText, width - 2*margin, height - topText - margin));
			txtView.Editable = false;
			txtView.ScrollEnabled = true;

			// Wire up the buttons to the SamplCode class methods
			btnFiles.TouchUpInside += (sender, e) => {
				SampleCode.ReadText(txtView);
			};

			btnDirectories.TouchUpInside += (sender, e) => {
				SampleCode.ReadDirectories(txtView);
			};

			btnAll.TouchUpInside += (sender, e) => {
				SampleCode.ReadAll(txtView);
			};

			btnXml.TouchUpInside += (sender, e) => {
				SampleCode.ReadXml(txtView);
			};

			btnWrite.TouchUpInside += (sender, e) => {
				if (writeJson ) {
					SampleCode.WriteJson(txtView);
				} else {
					SampleCode.WriteFile(txtView);
				}
			};

			btnDirectory.TouchUpInside += (sender, e) => {
				SampleCode.CreateDirectory(txtView);
			};

			// Add the controls to the view
			this.Add(btnFiles);
			this.Add(btnDirectories);
			this.Add(btnXml);
			this.Add(btnAll);
			this.Add(btnWrite);
			this.Add(btnDirectory);
			this.Add(txtView);

			// Write out this special folder, just for curiousity's sake
			Console.WriteLine("MyDocuments:"+Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
		}
	}
}
