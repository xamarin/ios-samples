// 
// RecipeDetailViewController.cs
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
using CoreGraphics;
using System.Collections.Generic;

using Foundation;
using UIKit;

namespace RecipesAndPrinting
{
	public partial class RecipeDetailViewController : UITableViewController
	{
		RecipePhotoViewController recipePhotoViewController;
		RecipeDetailSource tableSource;
		
		public RecipeDetailViewController (Recipe recipe) : base (UITableViewStyle.Grouped)
		{
			tableSource = new RecipeDetailSource (this);
			NavigationItem.Title = "Recipe";
			Recipe = recipe;
		}
		
		public Recipe Recipe {
			get; private set;
		}
		
		class RecipeDetailSource : UITableViewSource {
			static NSString InstructionsCellId = new NSString ("InstructionsCell");
			static NSString IngredientsCellId = new NSString ("IngredientsCell");
			const int AmountTag = 1;
			
			enum RecipeSection {
				Ingredients,
				Instructions,
				TotalSections,
			}
			
			RecipeDetailViewController controller;
			
			public RecipeDetailSource (RecipeDetailViewController controller)
			{
				this.controller = controller;
			}
			
			Recipe Recipe {
				get { return controller.Recipe; }
			}
			
			public override nint NumberOfSections (UITableView tableView)
			{
				return (int) RecipeSection.TotalSections;
			}
			
			public override string TitleForHeader (UITableView tableView, nint section)
			{
				if (section == (int) RecipeSection.Instructions)
					return "Instructions";
				
				return null;
			}
			
			public override nint RowsInSection (UITableView tableView, nint section)
			{
				switch ((RecipeSection) (int)section) {
				case RecipeSection.Instructions: return 1;
				case RecipeSection.Ingredients: return Recipe.Ingredients.Length;
				default: return 0;
				}
			}
			
			UITableViewCell GetIngredientsCellAtIndex (UITableView tableView, nint index)
			{
				UITableViewCell cell = tableView.DequeueReusableCell (IngredientsCellId);
				CGRect computedFrame;
				UILabel amountLabel;
				
				if (cell == null) {
					cell = new UITableViewCell (UITableViewCellStyle.Default, IngredientsCellId);
					
					amountLabel = new UILabel (CGRect.Empty);
					amountLabel.AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin;
					amountLabel.TextColor = UIColor.FromRGBA (50.0f / 255.0f, 79.0f / 255.0f, 133.0f / 255.0f, 1.0f);
					amountLabel.TextAlignment = UITextAlignment.Right;
					amountLabel.HighlightedTextColor = UIColor.White;
					amountLabel.BackgroundColor = UIColor.Clear;
					amountLabel.Tag = AmountTag;
					
					cell.ContentView.InsertSubviewAbove (amountLabel, cell.TextLabel);
				} else {
					amountLabel = cell.ViewWithTag (AmountTag) as UILabel;
				}
				
				cell.TextLabel.Text = Recipe.Ingredients[index].Name;
				amountLabel.Text = Recipe.Ingredients[index].Amount;   
				
				CGSize desiredSize = amountLabel.SizeThatFits (new CGSize (160.0f, 32.0f));
				nfloat midY = (cell.ContentView.Bounds.Y + cell.ContentView.Bounds.Height) / 2.0f;
				nfloat maxX = cell.ContentView.Bounds.Right;
				
				computedFrame = new CGRect (new CGPoint (maxX - desiredSize.Width - 10.0f, midY - desiredSize.Height / 2.0f), desiredSize);
				amountLabel.Frame = computedFrame;
				
				return cell;
			}
			
			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				RecipeSection section = (RecipeSection)(int) indexPath.Section;
				
				if (section == (int) RecipeSection.Ingredients)
					return GetIngredientsCellAtIndex (tableView, indexPath.Row);
				
				UITableViewCell cell = tableView.DequeueReusableCell (InstructionsCellId);
				if (cell == null)
					cell = new UITableViewCell (UITableViewCellStyle.Default, InstructionsCellId);
				
				switch (section) {
				case RecipeSection.Instructions:
					cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
					cell.TextLabel.Text = "Instructions";
					break;
				default:
					cell.Accessory = UITableViewCellAccessory.None;
					break;
				}
				
				return cell;
			}
			
			public override NSIndexPath WillSelectRow (UITableView tableView, NSIndexPath indexPath)
			{
				RecipeSection section = (RecipeSection)(int) indexPath.Section;
				
				// Don't allow ingredients to be selected
				if (section == RecipeSection.Ingredients) {
					tableView.DeselectRow (indexPath, true);
					return null;
				}
				
				return indexPath;
			}
			
			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				// If the intructions section is tapped, navigate to the instructions view controller
				if (indexPath.Section == (int) RecipeSection.Instructions) {
					InstructionsViewController nextViewController = new InstructionsViewController ();
					
					// pass the recipe to the instructions view controller
					nextViewController.Recipe = Recipe;
					
					controller.NavigationController.PushViewController (nextViewController, true);
				}
			}
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			if (tableHeaderView == null) {
				NSBundle.MainBundle.LoadNib ("DetailHeaderView", this, null);
				tableHeaderView.BackgroundColor = UIColor.GroupTableViewBackgroundColor;
			}
			
			TableView.TableHeaderView = tableHeaderView;
			TableView.AllowsSelectionDuringEditing = true;
			TableView.Source = tableSource;
		}
		
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			
			tableHeaderView.Dispose ();
			tableHeaderView = null;
			
			photoButton.Dispose ();
			photoButton = null;
			
			nameLabel.Dispose ();
			nameLabel = null;
		}
		
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			
			// to update recipe type and ingredients on return
			TableView.ReloadData ();
			
			// Skin the photo button with the recipe thumbnail - displayed up in the header view
			photoButton.SetImage (Recipe.ThumbnailImage, UIControlState.Normal);
			
			// Set the header view text
			nameLabel.Text = Recipe.Name;
			
			// Set the recipe name as the nav bar title
			Title = Recipe.Name;
			
			// Provide a back button to go back to the recipes
			UIBarButtonItem backButton = new UIBarButtonItem ("Recipe", UIBarButtonItemStyle.Plain, null);
			NavigationItem.BackBarButtonItem = backButton;
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
		}
		
		partial void ShowPhoto (NSObject sender)
		{
			// Navigate to the recipe photo view controller to show a large photo for the recipe.
			recipePhotoViewController = new RecipePhotoViewController ();
			recipePhotoViewController.Recipe = Recipe;
			
			NavigationController.PushViewController (recipePhotoViewController, true);
		}
		
		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			
			if (recipePhotoViewController != null) {
				recipePhotoViewController.Dispose ();
				recipePhotoViewController = null;
			}
			
			if (tableSource != null) {
				tableSource.Dispose ();
				tableSource = null;
			}
			
			if (tableHeaderView != null) {
				tableHeaderView.Dispose ();
				tableHeaderView = null;
			}
			
			if (photoButton != null) {
				photoButton.Dispose ();
				photoButton = null;
			}
			
			if (nameLabel != null) {
				nameLabel.Dispose ();
				nameLabel = null;
			}
		}
	}
}
