// 
// DetailViewController.cs
//  
// Author:
//       Alan McGovern <alan@xamarin.com>
// 
// Copyright 2011, Xamarin Inc.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Drawing;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace SimpleDrillDown {

	public partial class DetailViewController : UITableViewController {
		
		public Play Play { get; set; }
		
		public DetailViewController () : base (UITableViewStyle.Grouped)
		{
			TableView.Source = new DataSource (this);
		}
		
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			
			// Refresh the data before the view appears and ensure
			// it is scrolled to the top
			TableView.ReloadData ();
			TableView.ContentOffset = new PointF (0, 0);
			Title = Play.Title;
		}
		
		class DataSource : UITableViewSource {
			DetailViewController controller;
			
			public DataSource (DetailViewController controller)
			{
				this.controller = controller;
			}
			
			// Customize the number of sections in the table view.
			public override int NumberOfSections (UITableView tableView)
			{
				// One section for the Date, Genre and then Characters.
				return 3;
			}
			
			public override string TitleForHeader (UITableView tableView, int section)
			{
				if (section == 0) {
					return "Date";
				} else if (section == 1) {
					return "Genre";
				} else {
					return "Main Characters";
				}
			}
			
			public override int RowsInSection (UITableView tableview, int section)
			{
				// Date and Genre sections have one entry
				if (section == 0 || section == 1)
					return 1;
				else
					return controller.Play.Characters.Count;
			}
			
			// Customize the appearance of table view cells.
			public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				string cellIdentifier = "Cell";
				var cell = tableView.DequeueReusableCell (cellIdentifier);
				if (cell == null) {
					cell = new UITableViewCell (UITableViewCellStyle.Default, cellIdentifier);
					cell.SelectionStyle = UITableViewCellSelectionStyle.None;
				}
				
				if (indexPath.Section == 0) {
					cell.TextLabel.Text = controller.Play.Date.Year.ToString ();
				} else if (indexPath.Section == 1) {
					cell.TextLabel.Text = controller.Play.Genre;
				} else {
					cell.TextLabel.Text = controller.Play.Characters [indexPath.Row];
				}
				
				return cell;
			}
		}
	}
}
