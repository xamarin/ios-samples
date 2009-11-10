using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouchCells;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Serialization;

[Register]
public class CustomTableViewController : UITableViewController {

	public List<Item> Data { get; set; }

	public CustomTableViewController (IntPtr handle) : base (handle) { }
	
	public override void ViewDidLoad ()
	{
		base.ViewDidLoad ();
		
		//TODO: need to replace with data loading from disk
		this.Data = new List<Item>() {
			new Item() { Title="Mono", Checked=false },
			new Item() { Title="MonoDevelop", Checked=false },
			new Item() { Title="MonoTouch", Checked=false },
			new Item() { Title="Mono Tools", Checked=true },
			new Item() { Title="Banshee", Checked=true },
			new Item() { Title="Tomboy", Checked=false },
		};
		/*
		using (System.IO.TextWriter tw = new System.IO.StreamWriter("Items.xml")) {
			XmlSerializer ser = new XmlSerializer(typeof(List<Item>));
			ser.Serialize(tw, this.Data);
		}*/
		this.TableView.Delegate = new TableDelegate();
		this.TableView.DataSource = new DataSource(this);
		
	}
	
	public override bool ShouldAutorotateToInterfaceOrientation (MonoTouch.UIKit.UIInterfaceOrientation toInterfaceOrientation)
	{
		return true;
	}

	//
	// The data source for our TableView
	//
	class DataSource : UITableViewDataSource {
		CustomTableViewController ctvc;
		
		public DataSource (CustomTableViewController ctvc)
		{
			this.ctvc = ctvc;
		}
		
		public override int RowsInSection (UITableView tableView, int section)
		{
			return this.ctvc.Data.Count;
		}
		
		public override int NumberOfSections (UITableView tableView)
		{
			return 1;
		}
		
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			const string kCustomCellID = "MyCellID";
	
			CustomCell cell = (CustomCell)tableView.DequeueReusableCell(kCustomCellID);
			
			if (cell == null)
			{
				System.Console.WriteLine(indexPath.Row);
				cell = new CustomCell(UITableViewCellStyle.Default, kCustomCellID);
			}
			
			Item item = this.ctvc.Data[indexPath.Row];
			cell.Title = item.Title;
			cell.TextLabel.Text = item.Title;
			cell.Checked = item.Checked;
		
			return cell;
		}

	}

	
	class TableDelegate : UITableViewDelegate {
		
		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			// find the cell being touched and update its checked/unchecked image
			CustomCell targetCustomCell = (CustomCell)tableView.CellAt(indexPath); 
			
			targetCustomCell.CheckButtonTouchDown(null, null);
	
			// don't keep the table selection
			tableView.DeselectRow(indexPath, true);
	/*
			// update our data source array with the new checked state
			targetCustomCell.Checked = !targetCustomCell.Checked;
			UIImage checkImage = targetCustomCell.Checked ? UIImage.FromFile("checked.png") : UIImage.FromFile("unchecked.png");
			targetCustomCell.CheckButton.SetImage(checkImage, UIControlState.Normal);
	*/
			
		}
		
		public override void AccessoryButtonTapped (UITableView tableView, NSIndexPath indexPath)
		{
			// called when the accessory view (disclosure button) is touched
			CustomCell cell = (CustomCell)tableView.CellAt(indexPath);
			
			AppDelegate appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
			
			appDelegate.ShowDetail(cell);
		}

		
	}

	
}
