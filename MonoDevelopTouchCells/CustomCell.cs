using Foundation;
using UIKit;
using System;
using System.Collections.Generic;
using CoreGraphics;

namespace MonoDevelopTouchCells
{
	
	[Register]
	public class CustomCell : UITableViewCell {
		
		public string Title { 
			get { return this.TextLabel.Text; }
			set { this.TextLabel.Text = value; }
		}
		
		public bool Checked { get; set; }
		public UIButton CheckButton { get; set; }
	
		public CustomCell (UITableViewCellStyle style, string reuseIdentifier) : base(style, reuseIdentifier) 
		{    
			this.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
			
			// cell's title label
			this.TextLabel.BackgroundColor = this.BackgroundColor;
			this.TextLabel.Opaque = false;
			this.TextLabel.TextColor = UIColor.Black;
			this.TextLabel.HighlightedTextColor = UIColor.White;
			this.TextLabel.Font = UIFont.BoldSystemFontOfSize(18f);
			
			// cell's check button
			CheckButton = new UIButton () {
				Frame = CGRect.Empty,
				VerticalAlignment = UIControlContentVerticalAlignment.Center,
				HorizontalAlignment = UIControlContentHorizontalAlignment.Center,
				BackgroundColor = this.BackgroundColor,
			};
			
			CheckButton.TouchDown += CheckButtonTouchDown;
			ContentView.AddSubview (this.CheckButton);
		}
	
		public override void LayoutSubviews () 
		{
			base.LayoutSubviews ();
			
			this.TextLabel.Frame = new CGRect (
				this.ContentView.Bounds.Left + 40f, 
				8f, 
				this.ContentView.Bounds.Width, 
				30f);

			// layout the check button image
			UIImage checkedImage = UIImage.FromFile ("images/checked.png");
			
			CheckButton.Frame = new CGRect (
				this.ContentView.Bounds.Left + 10f, 
				12f, 
				checkedImage.Size.Width, 
				checkedImage.Size.Height);
			
			UIImage image = this.Checked ? checkedImage : UIImage.FromFile ("images/unchecked.png"); 
			UIImage newImage = image.StretchableImage (12, 0);
			
			CheckButton.SetBackgroundImage(newImage, UIControlState.Normal); 
		}
	
		public void CheckButtonTouchDown (object sender, EventArgs e)
		{
			this.Checked = !this.Checked;
			UIImage checkImage = this.Checked ? UIImage.FromFile("images/checked.png") : UIImage.FromFile("images/unchecked.png");
			this.CheckButton.SetImage(checkImage, UIControlState.Normal);
		}
	
	}
}