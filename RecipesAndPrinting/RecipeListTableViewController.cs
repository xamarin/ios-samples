// 
// RecipeListTableViewController.cs
//  
// Author: Jeffrey Stedfast <jeff@xamarin.com>
// 
// Copyright (c) 2011 Xamarin Inc.
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
// 

using System;
using System.Collections.Generic;

using Foundation;
using UIKit;

namespace RecipesAndPrinting
{
	public partial class RecipeListTableViewController : UITableViewController
	{
		RecipeDetailViewController details;
		UIBarButtonItem printButtonItem;
		RecipesController recipes;
		
		public RecipeListTableViewController (UITableViewStyle style, RecipesController recipes) : base (style)
		{
			Title = "Recipes";
			
			this.recipes = recipes;
			
			// Add a print button and use PrintSelectedRecipes as the pressed event handler
			printButtonItem = new UIBarButtonItem ("Print", UIBarButtonItemStyle.Bordered, PrintSelectedRecipes);
			NavigationItem.RightBarButtonItem = printButtonItem;
			
			// Increase the height of the table rows - 1 pixel higher than the recipe thumbnails displayed in the table cells
			TableView.RowHeight = 43.0f;
			
			TableView.Source = new RecipeListSource (this);
		}
		
		void PrintSelectedRecipes (object sender, EventArgs args)
		{
			// Get a reference to the singleton iOS printing concierge
			UIPrintInteractionController printController = UIPrintInteractionController.SharedPrintController;
			
			// Instruct the printing concierge to use our custom UIPrintPageRenderer subclass when printing this job
			printController.PrintPageRenderer = new RecipePrintPageRenderer (recipes.Recipes);
			
			// Ask for a print job object and configure its settings to tailor the print request
			UIPrintInfo info = UIPrintInfo.PrintInfo;
			
			// B&W or color, normal quality output for mixed text, graphics, and images
			info.OutputType = UIPrintInfoOutputType.General;
			
			// Select the job named this in the printer queue to cancel our print request.
			info.JobName = "Recipes";
			
			// Instruct the printing concierge to use our custom print job settings. 
			printController.PrintInfo = info;
			
			// Present the standard iOS Print Panel that allows you to pick the target Printer, number of pages, double-sided, etc.
			printController.Present (true, PrintingCompleted);
		}
		
		void PrintingCompleted (UIPrintInteractionController controller, bool completed, NSError error)
		{
			
		}
		
		void ShowRecipe (Recipe recipe, bool animated)
		{
			details = new RecipeDetailViewController (recipe);
			
			NavigationController.PushViewController (details, animated);
		}
		
		class RecipeListSource : UITableViewSource {
			static NSString RecipeCellId = new NSString ("RecipeCell");
			
			RecipeListTableViewController controller;
			
			public RecipeListSource (RecipeListTableViewController controller)
			{
				this.controller = controller;
			}
			
			Recipe[] Recipes {
				get { return controller.recipes.Recipes; }
			}
			
			public override nint NumberOfSections (UITableView tableView)
			{
				return 1;
			}
			
			public override nint RowsInSection (UITableView tableView, nint section)
			{
				return Recipes.Length;
			}
			
			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				// Ask for a cached cell that's been moved off the screen that we can therefore repurpose for a new cell coming onto the screen. 
				RecipeTableViewCell cell = tableView.DequeueReusableCell (RecipeCellId) as RecipeTableViewCell;
				
				// If no cached cells are available, create one. Depending on your table row height, we only need to create enough to fill one screen.
				// After that, the above call will start working to give us the cell that got scrolled off the screen.
				if (cell == null)
					cell = new RecipeTableViewCell (UITableViewCellStyle.Default, RecipeCellId);
				
				// Provide to the cell its corresponding recipe depending on the argument row
				cell.Recipe = Recipes[indexPath.Row];
				
				// Right arrow-looking indicator on the right side of the table view cell
				cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
				
				return cell;
			}
			
			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				Recipe recipe = Recipes[indexPath.Row];
				
				controller.ShowRecipe (recipe, true);
			}
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
		}
		
		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			
			if (printButtonItem != null) {
				printButtonItem.Dispose ();
				printButtonItem = null;
			}
			
			if (details != null) {
				details.Dispose ();
				details = null;
			}
		}
	}
}
