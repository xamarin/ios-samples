using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace Example_TableAndCellStyles.Code.CustomCells
{
	public class CustomCellController2 : UIViewController
	{
		UILabel lblHeading = new UILabel (new RectangleF (11, 0, 195, 46));
		UILabel lblSubHeading = new UILabel (new RectangleF (20, 45, 186, 30));
		UIImageView imgMain = new UIImageView (new RectangleF (214, 5, 100, 75));
		
		public UITableViewCell Cell
		{
			get { return cell; }
		}
		UITableViewCell cell = new UITableViewCell(UITableViewCellStyle.Default, "CustomCell2");
		
		public string Heading
		{
			get { return lblHeading.Text; }
			set { lblHeading.Text = value; }
		}
		public string SubHeading
		{
			get { return lblSubHeading.Text; }
			set { lblSubHeading.Text = value; }
		}
		
		public UIImage Image
		{
			get { return imgMain.Image; }
			set { imgMain.Image = value; }
		}

		public CustomCellController2 () : base()
		{	
			base.View.AddSubview (cell);
			cell.AddSubview (lblHeading);
			cell.AddSubview (lblSubHeading);
			cell.AddSubview (imgMain);
			
			imgMain.AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin;
			lblHeading.TextColor = UIColor.Brown;
			lblHeading.Font = UIFont.SystemFontOfSize (32);
			lblSubHeading.TextColor = UIColor.DarkGray;
			lblSubHeading.Font = UIFont.SystemFontOfSize (13);
			
		}
	}
}

