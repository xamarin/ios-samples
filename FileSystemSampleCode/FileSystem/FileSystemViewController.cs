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

		public override void ViewDidLoad ()
		{	
			base.ViewDidLoad ();
			
			// Create the buttons and TextView to run the sample code
			btnFiles = UIButton.FromType(UIButtonType.RoundedRect);
			btnFiles.Frame = new CGRect(10,10,145,50);
			btnFiles.SetTitle("Open 'ReadMe.txt'", UIControlState.Normal);

			btnDirectories = UIButton.FromType(UIButtonType.RoundedRect);
			btnDirectories.Frame = new CGRect(10,70,145,50);
			btnDirectories.SetTitle("List Directories", UIControlState.Normal);
			
			btnXml = UIButton.FromType(UIButtonType.RoundedRect);
			btnXml.Frame = new CGRect(165,10,145,50);
			btnXml.SetTitle("Open 'Test.xml'", UIControlState.Normal);

			btnAll = UIButton.FromType(UIButtonType.RoundedRect);
			btnAll.Frame = new CGRect(165,70,145,50);
			btnAll.SetTitle("List All", UIControlState.Normal);
			
			btnWrite = UIButton.FromType(UIButtonType.RoundedRect);
			btnWrite.Frame = new CGRect(10,130,145,50);
			btnWrite.SetTitle("Write File", UIControlState.Normal);

			btnDirectory = UIButton.FromType(UIButtonType.RoundedRect);
			btnDirectory.Frame = new CGRect(165,130,145,50);
			btnDirectory.SetTitle("Create Directory", UIControlState.Normal);

			txtView = new UITextView(new CGRect(10, 190, 300, 250));
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
				SampleCode.WriteFile(txtView);
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