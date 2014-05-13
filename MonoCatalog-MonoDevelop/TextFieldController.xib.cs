//
// Textfields sample ported to C#
//
using System;
using UIKit;
using Foundation;
using CoreGraphics;

namespace MonoCatalog {
	
	public partial class TextFieldController : UITableViewController {
	
		public TextFieldController () : base ("TextFieldController", null) {
		}
		
		const int kViewTag = 1;
		
		class DataSource : UITableViewDataSource {
			TextFieldController tvc;
			static NSString kDisplayCell_ID = new NSString ("CellTextField_ID");
			static NSString kSourceCell_ID = new NSString ("SourceCell_ID");
			
			public DataSource (TextFieldController tvc)
			{
				this.tvc = tvc;
			}
	
			public override nint NumberOfSections (UITableView tableView)
			{
				return tvc.samples.Length;
			}
	
			public override string TitleForHeader (UITableView tableView, nint section)
			{
				return tvc.samples [section].Title;
			}
	
			public override nint RowsInSection (UITableView tableView, nint section)
			{
				return 2;
			}
	
			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				UITableViewCell cell = null;
				nint row = indexPath.Row;
	
				if (row == 0){
					cell = tableView.DequeueReusableCell (kDisplayCell_ID);
					if (cell == null){
						cell = new UITableViewCell (UITableViewCellStyle.Default, kDisplayCell_ID);
						cell.SelectionStyle = UITableViewCellSelectionStyle.None;
					} else {
						// The cell is being recycled, remove the old content
						UIView viewToRemove = cell.ContentView.ViewWithTag (kViewTag);
						if (viewToRemove != null)
							viewToRemove.RemoveFromSuperview ();
					}
					cell.ContentView.AddSubview (tvc.samples [indexPath.Section].View);
				} else {
					cell = tableView.DequeueReusableCell (kSourceCell_ID);
					if (cell == null){
						// Construct the cell with reusability (the second argument is not null)
						cell = new UITableViewCell (UITableViewCellStyle.Default, kSourceCell_ID);
						cell.SelectionStyle = UITableViewCellSelectionStyle.None;
						var label = cell.TextLabel;
						
						label.Opaque = false;
						label.TextAlignment = UITextAlignment.Center;
						label.TextColor = UIColor.Gray;
						label.Lines = 1;
						label.HighlightedTextColor = UIColor.Black;
						label.Font = UIFont.SystemFontOfSize (12f);
					}
					cell.TextLabel.Text = tvc.samples [indexPath.Section].Source;
				}
	
				return cell;
			}
		}
	
		class TableDelegate : UITableViewDelegate {
			// Override to provide the sizing of the rows in our table
			public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
			{
				return indexPath.Row == 0 ? 50f : 22f;
			}
		}
	
		bool DoReturn (UITextField tf)
		{
			tf.ResignFirstResponder ();
			return true;
		}
		
		UITextField TextFieldNormal ()
		{
			return new UITextField (new CGRect (30f, 8f, 260f, 30f)){
				BorderStyle = UITextBorderStyle.Bezel,
				TextColor = UIColor.Black,
				Font = UIFont.SystemFontOfSize (17f),
				Placeholder = "<enter text>",
				BackgroundColor = UIColor.White,
				AutocorrectionType = UITextAutocorrectionType.No,
				KeyboardType = UIKeyboardType.Default,
				ReturnKeyType = UIReturnKeyType.Done,
				ClearButtonMode = UITextFieldViewMode.WhileEditing,
				Tag = kViewTag,
				ShouldReturn = DoReturn,
				AccessibilityLabel = "Normal"
			};
		}
	
		UITextField TextFieldRounded ()
		{
			return new UITextField (new CGRect (30f, 8f, 260f, 30f)){
				BorderStyle = UITextBorderStyle.RoundedRect,
				TextColor = UIColor.Black,
				Font = UIFont.SystemFontOfSize (17f),
				Placeholder = "<enter text>",
				BackgroundColor = UIColor.White,
				AutocorrectionType = UITextAutocorrectionType.No,
				KeyboardType = UIKeyboardType.Default,
				ReturnKeyType = UIReturnKeyType.Done,
				ClearButtonMode = UITextFieldViewMode.WhileEditing,
				Tag = kViewTag,
				ShouldReturn = DoReturn,
				AccessibilityLabel = "Rounded"
			};
		}
	
		UITextField TextFieldSecure ()
		{
			return new UITextField (new CGRect (30f, 8f, 260f, 30f)){
				BorderStyle = UITextBorderStyle.Bezel,
				TextColor = UIColor.Black,
				Font = UIFont.SystemFontOfSize (17f),
				Placeholder = "<enter text>",
				BackgroundColor = UIColor.White,
				AutocorrectionType = UITextAutocorrectionType.No,
				KeyboardType = UIKeyboardType.Default,
				ReturnKeyType = UIReturnKeyType.Done,
				ClearButtonMode = UITextFieldViewMode.WhileEditing,
				SecureTextEntry = true,
				Tag = kViewTag,
				ShouldReturn = DoReturn,
				AccessibilityLabel = "Secure"
			};
		}
	
	
		UITextField TextFieldLeftView ()
		{
			return new UITextField (new CGRect (30f, 8f, 260f, 30f)){
				BorderStyle = UITextBorderStyle.Bezel,
				TextColor = UIColor.Black,
				Font = UIFont.SystemFontOfSize (17f),
				Placeholder = "<enter text>",
				BackgroundColor = UIColor.White,
				AutocorrectionType = UITextAutocorrectionType.No,
				KeyboardType = UIKeyboardType.Default,
				ReturnKeyType = UIReturnKeyType.Done,
				ClearButtonMode = UITextFieldViewMode.WhileEditing,
				Tag = kViewTag,
				ShouldReturn = DoReturn,
				LeftView = new UIImageView (UIImage.FromFile ("images/segment_check.png")),
				LeftViewMode = UITextFieldViewMode.Always,
				AccessibilityLabel = "LeftView"
			};
		}
		
		struct TextFieldSample {
			public string Title, Source;
			public UITextField View;
	
			public TextFieldSample (string t, string s, UITextField b)
			{
				Title = t;
				Source = s;
				View = b;
			}
		}
	
		TextFieldSample [] samples;
	
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			Title = "Text Field";
	
			samples = new TextFieldSample [] {
				new TextFieldSample ("UITextField", "textfield.cs: TextFieldNormal()", TextFieldNormal ()),
				new TextFieldSample ("UITextField Rounded", "textfield.cs: TextFieldRounded ()", TextFieldRounded ()),
				new TextFieldSample ("UITextField Secure", "textfield.cs: TextFieldSecure ()", TextFieldSecure ()),
				new TextFieldSample ("UITextField (with LeftView)", "textfield.cs: TextFieldLeftView ()", TextFieldLeftView ()),
			};
	
			TableView.DataSource = new DataSource (this);
			TableView.Delegate = new TableDelegate ();
			Editing = false;
		}
	}
}
