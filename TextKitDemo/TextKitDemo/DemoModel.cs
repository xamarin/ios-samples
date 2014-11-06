using System;
using System.Collections.Generic;

using Foundation;
using UIKit;
using CoreText;

namespace TextKitDemo
{
	/*
	 * Our simple data model, contains the various demos that we will display.
	 */
	public class DemoModel
	{
		public static List<DemoModel> Demos = new List<DemoModel> () {
			new DemoModel ("Basic Interaction", "BasicInteractionViewController", "Basic Interaction.rtf"),
			new DemoModel ("Exclusion Paths", "ExclusionPathsViewController", "Exclusion Paths.rtf"),
			new DemoModel ("View Layout", "PersonViewController", "View Layout.rtf"),
			new DemoModel ("Dynamic Coloring", "DynamicColoringViewController", "Dynamic Coloring.rtf")
		};

		public string Title { get; set; }
		public string ViewControllerIdentifier { get; set;}

		string TextStoragePath;
		NSAttributedString AttributedText;
		bool StrRichFormatting = false;

		public DemoModel (string title, string identifier, string path)
		{
			Title = title;
			ViewControllerIdentifier = identifier;
			TextStoragePath = path;
		}

		public static DemoModel GetDemo (NSIndexPath indexPath)
		{
			return Demos[indexPath.Row];
		}

		public NSAttributedString GetAttributedText ()
		{
			string path = null;
			NSError error = new NSError ();

			if (TextStoragePath != null)
				path = NSBundle.MainBundle.PathForResource ("TextFiles/" + TextStoragePath, "");
			else
				path = NSBundle.MainBundle.PathForResource ("TextFiles/" + Title, "rtf");

			if (path == null)
				return new NSAttributedString ("");

			if (StrRichFormatting) {
				//  Load the file from disk
				var attributedString = new NSAttributedString (new NSUrl (path, false), null, ref error);

				// Make a copy we can alter
				var attributedTextHolder = new NSMutableAttributedString (attributedString);

				attributedTextHolder.AddAttributes (new UIStringAttributes () { Font = UIFont.PreferredBody },
													new NSRange (0, attributedTextHolder.Length));
				AttributedText = (NSAttributedString)attributedTextHolder.Copy ();
			} else {
				string newFlatText = new NSAttributedString (new NSUrl (path, false), null, ref error).Value;
				AttributedText = new NSAttributedString (newFlatText, font: UIFont.PreferredBody);
			}

			return AttributedText;
		}
	}
}

 