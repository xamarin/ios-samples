
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;

namespace MonoDevelopTouchCells
{
	public partial class DetailViewController : UIViewController
	{

		// This is required for controllers that need to be able to be
		// created from a xib rather than from managed code
		public DetailViewController (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		public DetailViewController ()
		{
			Initialize ();
		}

		void Initialize ()
		{
			this.LoadView();
		}
		
		public void ShowDetail (CustomCell cell) {
			this.itemTitle.Text = cell.Title;
			
			UIImage image = cell.Checked ? 
				UIImage.FromFile ("images/checked.png") : UIImage.FromFile ("images/unchecked.png");
			
			this.checkedImage.Image = image;
		}
	}
}
