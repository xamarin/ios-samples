//
// Button sample in C#v
//

using System;
using UIKit;
using Foundation;
using CoreGraphics;

namespace MonoCatalog
{


	public partial class ButtonsViewController : UITableViewController
	{

		//
		// This datasource describes how the UITableView should render the
		// contents.   We have a number of sections determined by the
		// samples in our container class, and 2 rows per section:
		//
		//   Row 0: the actual styled button
		//   Row 1: the text information about the button
		//
		class DataSource : UITableViewDataSource
		{
			ButtonsViewController bvc;
			static NSString kDisplayCell_ID = new NSString ("DisplayCellID");
			static NSString kSourceCell_ID = new NSString ("SourceCellID");

			public DataSource (ButtonsViewController bvc)
			{
				this.bvc = bvc;
			}

			public override nint NumberOfSections (UITableView tableView)
			{
				return bvc.samples.Length;
			}

			public override string TitleForHeader (UITableView tableView, nint section)
			{
				return bvc.samples[section].Title;
			}

			public override nint RowsInSection (UITableView tableView, nint section)
			{
				return 2;
			}

			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				UITableViewCell cell;

				if (indexPath.Row == 0) {
					cell = tableView.DequeueReusableCell (kDisplayCell_ID);
					if (cell == null) {
						cell = new UITableViewCell (UITableViewCellStyle.Default, kDisplayCell_ID);
						cell.SelectionStyle = UITableViewCellSelectionStyle.None;
					} else {
						// The cell is being recycled, remove the old content

						UIView viewToRemove = cell.ContentView.ViewWithTag (kViewTag);
						if (viewToRemove != null)
							viewToRemove.RemoveFromSuperview ();
					}
					cell.TextLabel.Text = bvc.samples[indexPath.Section].Label;
					cell.ContentView.AddSubview (bvc.samples[indexPath.Section].Button);
				} else {
					cell = tableView.DequeueReusableCell (kSourceCell_ID);
					if (cell == null) {
						// Construct the cell with reusability (the second argument is not null)
						cell = new UITableViewCell (UITableViewCellStyle.Default, kSourceCell_ID);
						cell.SelectionStyle = UITableViewCellSelectionStyle.None;
						var label = cell.TextLabel;

						label.Opaque = false;
						label.TextAlignment = UITextAlignment.Center;
						label.TextColor = UIColor.Gray;
						label.Lines = 2;
						label.HighlightedTextColor = UIColor.Black;
						label.Font = UIFont.SystemFontOfSize (12f);
					}
					cell.TextLabel.Text = bvc.samples[indexPath.Section].Source;
				}

				return cell;
			}
		}

		class TableDelegate : UITableViewDelegate
		{
			//
			// Override to provide the sizing of the rows in our table
			//
			public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
			{
				// First row is always 50 pixes, second row 38
				return indexPath.Row == 0 ? 50f : 38f;
			}
		}

		// Load our definition from the NIB file
		public ButtonsViewController () : base("ButtonsViewController", null)
		{
		}

		// For tagging our embedded controls at cell recylce time.
		const int kViewTag = 1;

		//
		// Utility function that configures the various buttons that we create
		//
		static UIButton ButtonWithTitle (string title, CGRect frame, UIImage image, UIImage imagePressed, bool darkTextColor)
		{
			var button = new UIButton (frame) {
				VerticalAlignment = UIControlContentVerticalAlignment.Center,
				HorizontalAlignment = UIControlContentHorizontalAlignment.Center,
				BackgroundColor = UIColor.Clear
			};

			button.SetTitle (title, UIControlState.Normal);
			if (darkTextColor)
				button.SetTitleColor (UIColor.Black, UIControlState.Normal); else
				button.SetTitleColor (UIColor.White, UIControlState.Normal);

			var newImage = image.StretchableImage (12, 0);
			button.SetBackgroundImage (newImage, UIControlState.Normal);
			var newPressedImage = image.StretchableImage (12, 0);
			button.SetBackgroundImage (newPressedImage, UIControlState.Highlighted);

			button.Tag = kViewTag;
			// To support reusable cells
			button.TouchDown += delegate { Console.WriteLine ("The button has been touched"); };

			return button;
		}

		UIButton GrayButton ()
		{
			var background = UIImage.FromFile ("images/whiteButton.png");
			var backgroundPressed = UIImage.FromFile ("images/blueButton.png");
			var frame = new CGRect (182f, 5f, 106f, 40f);
			//button.TouchDown += delegate { Console.WriteLine ("The button has been touched"); };
			return ButtonWithTitle ("Gray", frame, background, backgroundPressed, true);
		}

		UIButton ImageButton ()
		{
			var background = UIImage.FromFile ("images/whiteButton.png");
			var backgroundPressed = UIImage.FromFile ("images/blueButton.png");
			var frame = new CGRect (182f, 5f, 106f, 40f);

			var button = ButtonWithTitle ("", frame, background, backgroundPressed, true);
			button.TouchDown += delegate { Console.WriteLine ("The button has been touched"); };
			button.SetImage (UIImage.FromFile ("images/UIButton_custom.png"), UIControlState.Normal);
			return button;
		}

		UIButton RoundedButtonType ()
		{
			var button = UIButton.FromType (UIButtonType.RoundedRect);
			button.Frame = new CGRect (182f, 5f, 106f, 40f);
			button.BackgroundColor = UIColor.Clear;
			button.SetTitle ("Rounded", UIControlState.Normal);
			button.TouchDown += delegate { Console.WriteLine ("The button has been touched"); };
			button.Tag = kViewTag;
			// To support reusable cells
			return button;
		}

		UIButton DetailDisclosureButton ()
		{
			var button = UIButton.FromType (UIButtonType.DetailDisclosure);
			button.Frame = new CGRect (250f, 8f, 25f, 25f);
			button.BackgroundColor = UIColor.Clear;
			button.SetTitle ("Detail Disclosure", UIControlState.Normal);
			button.TouchDown += delegate { Console.WriteLine ("The button has been touched"); };
			button.Tag = kViewTag;
			// To support reusable cells
			return button;
		}


		UIButton InfoDarkButtonType ()
		{
			var button = UIButton.FromType (UIButtonType.InfoDark);
			button.Frame = new CGRect (250, 8f, 25f, 25f);
			button.BackgroundColor = UIColor.Clear;
			button.SetTitle ("Detail Disclosure", UIControlState.Normal);
			button.TouchDown += delegate { Console.WriteLine ("The button has been touched"); };
			button.Tag = kViewTag;
			button.AccessibilityLabel = "InfoDark";
			// To support reusable cells
			return button;
		}

		UIButton InfoLightButtonType ()
		{
			var button = UIButton.FromType (UIButtonType.InfoLight);
			button.Frame = new CGRect (250, 8f, 25f, 25f);
			button.BackgroundColor = UIColor.Gray;
			button.SetTitle ("Detail Disclosure", UIControlState.Normal);
			button.TouchDown += delegate { Console.WriteLine ("The button has been touched"); };
			button.Tag = kViewTag;
			button.AccessibilityLabel = "InfoLight";
			// To support reusable cells
			return button;
		}

		UIButton ContactAddButtonType ()
		{
			var button = UIButton.FromType (UIButtonType.ContactAdd);
			button.Frame = new CGRect (250, 8f, 25f, 25f);
			button.BackgroundColor = UIColor.Clear;
			button.SetTitle ("Detail Disclosure", UIControlState.Normal);
			button.TouchDown += delegate { Console.WriteLine ("The button has been touched"); };
			button.Tag = kViewTag;
			button.AccessibilityLabel = "ContactAdd";
			// To support reusable cells
			return button;
		}

		struct ButtonSample
		{
			public string Title, Label, Source;
			public UIButton Button;

			public ButtonSample (string t, string l, string s, UIButton b)
			{
				Title = t;
				Label = l;
				Source = s;
				Button = b;
			}
		}

		ButtonSample[] samples;

		public override void ViewDidLoad ()
		{
			Console.WriteLine ("Buttons: View Did Load - FIrst");
			base.ViewDidLoad ();
			Title = "Buttons";

			samples = new ButtonSample[] {
				new ButtonSample ("UIButton", "Background image", "buttons.cs:\rUIButton GrayButton ()", GrayButton ()),
				new ButtonSample ("UIButton", "Button with image", "buttons.cs:\rUIButton ImageButton ()", ImageButton ()),
				new ButtonSample ("UIButtonRoundedRect", "Rounded Button", "buttons.cs:\rUIButton RoundedButtonType ()", RoundedButtonType ()),
				new ButtonSample ("UIButtonTypeDetailDisclosure", "Detail disclosure", "buttons.cs:\rUIButton DetailDisclosureButton ()", DetailDisclosureButton ()),
				new ButtonSample ("UIButtonTypeInfoLight", "Info light", "buttons.cs:\rUIButton InfoLightButtonType ()", InfoLightButtonType ()),
				new ButtonSample ("UIButtonTypeInfoDark", "Info dark", "buttons.cs:\rUIButton InfoLightButtonType ()", InfoDarkButtonType ()),
				new ButtonSample ("UIButtonTypeContactAdd", "Contact Add", "buttons.cs:\rUIButton ContactAddButtonType ()", ContactAddButtonType ())
			};

			TableView.DataSource = new DataSource (this);
			TableView.Delegate = new TableDelegate ();
		}
	}
}
