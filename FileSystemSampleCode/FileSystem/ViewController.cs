using System;
using UIKit;

namespace FileSystem {
	public partial class ViewController : UIViewController {
		private bool writeJson = true;

		protected ViewController (IntPtr handle) : base (handle) { }

		partial void OpenReadMe (UIButton sender)
		{
			textView.Text = DataManager.ReadText ();
		}

		partial void OpenTestFile (UIButton sender)
		{
			textView.Text = DataManager.ReadXml ();
		}

		partial void ListDirectories (UIButton sender)
		{
			textView.Text = DataManager.ReadDirectories ();
		}

		partial void ListAll (UIButton sender)
		{
			textView.Text = DataManager.ReadAll ();
		}

		partial void WriteFile (UIButton sender)
		{
			var text = string.Empty;
			if (writeJson) {
				text = DataManager.WriteJson ();
			} else {
				text = DataManager.WriteFile ();
			}

			textView.Text = text;
		}

		partial void CreateDirectory (UIButton sender)
		{
			textView.Text = DataManager.CreateDirectory ();
		}
	}
}
