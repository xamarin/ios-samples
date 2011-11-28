
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Example_TableAndCellStyles.Code.CustomCells
{
	public partial class CustomCellController1 : UIViewController
	{
		public UITableViewCell Cell
		{
			get { return this.celMain; }
		}
		public string Heading
		{
			get { return this.lblHeading.Text; }
			set { this.lblHeading.Text = value; }
		}
		public string SubHeading
		{
			get { return this.lblSubHeading.Text; }
			set { this.lblSubHeading.Text = value; }
		}
		
		public UIImage Image
		{
			get { return this.imgMain.Image; }
			set { this.imgMain.Image = value; }
		}
	
		
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for items that need 
		// to be able to be created from a xib rather than from managed code

		public CustomCellController1 (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public CustomCellController1 (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public CustomCellController1 ()// : base("CustomCellController1", null)
		{
			// this next line forces the loading of the xib file to be synchronous
			MonoTouch.Foundation.NSBundle.MainBundle.LoadNib ("CustomCellController1", this, null);
			Initialize ();
		}
		
		void Initialize ()
		{
		}
		
		#endregion
	}
}

