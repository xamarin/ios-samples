using System;
using Foundation;
using UIKit;

namespace MySingleView
{
	public partial class ViewController : UIViewController
	{
		#region Computed Properties
		public nint PageNumber { get; set; } = 0;
		#endregion

		#region Constructors
		public ViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Override Methods
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Initialize
			PageView.Pages = 6;
			ShowCat ();
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
		#endregion

		#region Custom Actions
		partial void NextCat (Foundation.NSObject sender) {

			// Display next Cat
			if (++PageNumber > 5) {
				PageNumber = 5;
			}
			ShowCat();

		}

		partial void PreviousCat (Foundation.NSObject sender) {
			// Display previous cat
			if (--PageNumber < 0) {
				PageNumber = 0;
			}
			ShowCat();
		}
		#endregion

		#region Private Methods
		private void ShowCat() {

			// Adjust UI
			PreviousButton.Enabled = (PageNumber > 0);
			NextButton.Enabled = (PageNumber < 5);
			PageView.CurrentPage = PageNumber;

			// Display new cat
			CatView.Image = UIImage.FromFile(string.Format("Cat{0:00}.jpg",PageNumber+1));
		}
		#endregion
	}
}


