// 
// RootViewController.cs
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
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;

namespace SimpleDrillDown {

	public partial class RootViewController : UITableViewController {
		
		public RootViewController (string nibName, NSBundle bundle) : base (nibName, bundle)
		{
			Title = "Plays";
			TableView.Source = new DataSource (this);
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
		}
		
		class DataSource : UITableViewSource {
			
			List<Play> Plays { get; set; }
			
			DetailViewController PlayView { get; set; }
			
			RootViewController controller;

			public DataSource (RootViewController controller)
			{
				this.controller = controller;
				
				// Create the ViewController which will display play specific information
				// and re-use it for each play.
				PlayView = new DetailViewController ();
				
				// Create the data we need to display
				Plays = Play.CreateDemoPlays ();
			}
			
			// Customize the number of sections in the table view.
			public override int NumberOfSections (UITableView tableView)
			{
				return 1;
			}
			
			public override int RowsInSection (UITableView tableview, int section)
			{
				return Plays.Count;
			}
			
			// Customize the appearance of table view cells.
			public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				string cellIdentifier = "Cell";
				var cell = tableView.DequeueReusableCell (cellIdentifier);
				if (cell == null) {
					cell = new UITableViewCell (UITableViewCellStyle.Default, cellIdentifier);
					cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
				}
				
				// Set the title of the play as the text.
				cell.TextLabel.Text = Plays [indexPath.Row].Title;
				return cell;
			}

			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				// Update the PlayView with the information from the selected play
				PlayView.Play = Plays [indexPath.Row];
				
				// Make the PlayView the active View.
				controller.NavigationController.PushViewController (PlayView, true);
			}
		}
	}
}
