using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System;
using System.Collections.Generic;
using System.Drawing;

[Register]
public class CustomCell : UITableViewCell {
	
	public string Title { get; set; }
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
			Frame = RectangleF.Empty,
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
		
		this.TextLabel.Frame = new RectangleF (
			this.ContentView.Bounds.Left + 40f, 
			8f, 
			this.ContentView.Bounds.Width, 
			30f);
		
		// layout the check button image
		UIImage checkedImage = UIImage.FromFile ("checked.png");
		CheckButton.Frame = new RectangleF (
			this.ContentView.Bounds.Left + 10f, 
			12f, 
			checkedImage.Size.Width, 
			checkedImage.Size.Height);
		
		UIImage image = this.Checked ? checkedImage : UIImage.FromFile ("unchecked.png"); 
		UIImage newImage = image.StretchableImage (12, 0);
		CheckButton.SetBackgroundImage(newImage, UIControlState.Normal); 
	}

	public void CheckButtonTouchDown (object sender, EventArgs e)
	{
		this.Checked = !this.Checked;
		UIImage checkImage = this.Checked ? UIImage.FromFile("checked.png") : UIImage.FromFile("unchecked.png");
		this.CheckButton.SetImage(checkImage, UIControlState.Normal);
	}

/*
// called when the checkmark button is touched 
- (void)checkAction:(id)sender
{
	// note: we don't use 'sender' because this action method can be called separate from the button (i.e. from table selection)
	self.checked = !self.checked;
	UIImage *checkImage = (self.checked) ? [UIImage imageNamed:@"checked.png"] : [UIImage imageNamed:@"unchecked.png"];
	[checkButton setImage:checkImage forState:UIControlStateNormal];
}
 */
}