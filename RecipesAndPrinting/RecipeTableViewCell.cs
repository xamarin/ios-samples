// 
// RecipeTableViewCell.cs
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
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace RecipesAndPrinting
{
	public class RecipeTableViewCell : UITableViewCell
	{
		const float ImageSize = 42.0f;
		const float EditingInset = 10.0f;
		const float TextLeftMargin = 8.0f;
		const float TextRightMargin = 5.0f;
		const float PrepTimeWidth = 80.0f;
		
		UILabel descriptionLabel;
		UILabel prepTimeLabel;
		UIImageView imageView;
		UILabel nameLabel;
		Recipe recipe;
		
		public RecipeTableViewCell (UITableViewCellStyle style, NSString identifier) : base (style, identifier)
		{
			imageView = new UIImageView (RectangleF.Empty);
			ContentView.AddSubview (imageView);
			
			descriptionLabel = new UILabel (RectangleF.Empty);
			descriptionLabel.Font = UIFont.SystemFontOfSize (12.0f);
			descriptionLabel.TextColor = UIColor.DarkGray;
			descriptionLabel.HighlightedTextColor = UIColor.White;
			ContentView.AddSubview (descriptionLabel);
			
			prepTimeLabel = new UILabel (RectangleF.Empty);
			prepTimeLabel.TextAlignment = UITextAlignment.Right;
			prepTimeLabel.Font = UIFont.SystemFontOfSize (12.0f);
			prepTimeLabel.TextColor = UIColor.Black;
			prepTimeLabel.HighlightedTextColor = UIColor.White;
			ContentView.AddSubview (prepTimeLabel);
			
			nameLabel = new UILabel (RectangleF.Empty);
			nameLabel.Font = UIFont.BoldSystemFontOfSize (14.0f);
			nameLabel.TextColor = UIColor.Black;
			nameLabel.HighlightedTextColor = UIColor.White;
			ContentView.AddSubview (nameLabel);
		}
		
		public Recipe Recipe {
			get { return recipe; }
			set {
				if (recipe == value)
					return;
				
				recipe = value;
				imageView.Image = recipe.ThumbnailImage;
				descriptionLabel.Text = recipe.Description;
				prepTimeLabel.Text = recipe.PrepTime;
				nameLabel.Text = recipe.Name;
			}
		}
		
		RectangleF ImageViewFrame {
			get {
				if (Editing) {
					return new RectangleF (EditingInset, 0.0f, ImageSize, ImageSize);
				} else {
					return new RectangleF (0.0f, 0.0f, ImageSize, ImageSize);
				}
			}
		}
		
		RectangleF NameLabelFrame {
			get {
				if (Editing) {
					return new RectangleF (ImageSize + EditingInset + TextLeftMargin, 4.0f, ContentView.Bounds.Width - ImageSize - EditingInset - TextLeftMargin, 16.0f);
				} else {
					return new RectangleF (ImageSize + TextLeftMargin, 4.0f, ContentView.Bounds.Width - ImageSize - TextLeftMargin - PrepTimeWidth, 16.0f);
				}
			}
		}

		RectangleF DescriptionLabelFrame {
			get {
				if (Editing) {
					return new RectangleF (ImageSize + EditingInset + TextLeftMargin, 22.0f, ContentView.Bounds.Width - ImageSize - EditingInset - TextLeftMargin, 16.0f);
				} else {
					return new RectangleF (ImageSize + TextLeftMargin, 22.0f, ContentView.Bounds.Width - ImageSize - TextLeftMargin, 16.0f);
				}
			}
		}

		RectangleF PrepTimeLabelFrame {
			get {
				return new RectangleF (ContentView.Bounds.Width - PrepTimeWidth - TextRightMargin, 4.0f, PrepTimeWidth, 16.0f);
			}
		}
		
		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			
			descriptionLabel.Frame = DescriptionLabelFrame;
			prepTimeLabel.Frame = PrepTimeLabelFrame;
			imageView.Frame = ImageViewFrame;
			nameLabel.Frame = NameLabelFrame;
			
			if (Editing) {
				prepTimeLabel.Alpha = 0.0f;
			} else {
				prepTimeLabel.Alpha = 1.0f;
			}
		}
		
		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			
			if (descriptionLabel != null) {
				descriptionLabel.Dispose ();
				descriptionLabel = null;
			}
			
			if (prepTimeLabel != null) {
				prepTimeLabel.Dispose ();
				prepTimeLabel = null;
			}
			
			if (imageView != null) {
				imageView.Dispose ();
				imageView = null;
			}
			
			if (nameLabel != null) {
				nameLabel.Dispose ();
				nameLabel = null;
			}
		}
	}
}

