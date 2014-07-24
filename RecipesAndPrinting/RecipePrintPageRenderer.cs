// 
// RecipePrintPageRenderer.cs
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
using System.Collections;
using System.Collections.Generic;

using Foundation;
using UIKit;

namespace RecipesAndPrinting
{
	public class RecipePrintPageRenderer : UIPrintPageRenderer
	{
		const float RecipeInfoHeight = 150.0f;
		const float TitleSize = 24.0f;
		const float Padding = 10.0f;
		
		static UIFont SystemFont = UIFont.SystemFontOfSize (UIFont.SystemFontSize);
		
		Dictionary<UIPrintFormatter, Recipe> recipeFormatterMap = new Dictionary<UIPrintFormatter, Recipe> ();
		NSRange pageRange;
		Recipe[] recipes;
		
		public RecipePrintPageRenderer (Recipe[] recipes)
		{
			this.HeaderHeight = 20.0f;
			this.FooterHeight = 20.0f;
			this.recipes = recipes;
		}
		
		// Calculate the content area based on the printableRect, that is, the area in which
		// the printer can print content. a.k.a the imageable area of the paper.
		CGRect ContentArea {
			get {
				CGRect r = PrintableRect;
				r.Height -= HeaderHeight + FooterHeight;
				r.Y += HeaderHeight;
				return r;
			}
		}
		
		public override void PrepareForDrawingPages (NSRange range)
		{
			base.PrepareForDrawingPages (range);
			pageRange = range;
		}
		
		// This property must be overriden when doing custom drawing as we are. 
		// Since our custom drawing is really only for the borders and we are 
		// relying on a series of UIMarkupTextPrintFormatters to display the recipe
		// content, UIKit can calculate the number of pages based on informtation
		// provided by those formatters. 
		//
		// Therefore, setup the formatters, and ask super to count the pages.
		public override nint NumberOfPages {
			get {
				PrintFormatters = new UIPrintFormatter [0];
				SetupPrintFormatters ();
				return base.NumberOfPages;
			}
		}
		
		// Iterate through the recipes setting each of their html representations into 
		// a UIMarkupTextPrintFormatter and add that formatter to the printing job.
		void SetupPrintFormatters ()
		{
			CGRect contentArea = ContentArea;
			nfloat previousFormatterMaxY = contentArea.Top;
			nint page = 0;
			
			foreach (Recipe recipe in recipes) {
				string html = recipe.HtmlRepresentation;
				
				UIMarkupTextPrintFormatter formatter = new UIMarkupTextPrintFormatter (html);
				recipeFormatterMap.Add (formatter, recipe);
				
				// Make room for the recipe info
				UIEdgeInsets contentInsets = new UIEdgeInsets (0.0f, 0.0f, 0.0f, 0.0f);
				
				contentInsets.Top = previousFormatterMaxY + RecipeInfoHeight;
				if (contentInsets.Top > contentArea.Bottom) {
					// Move to the next page
					contentInsets.Top = contentArea.Top + RecipeInfoHeight;
					page++;
				}
				
				formatter.ContentInsets = contentInsets;
				
				// Add the formatter to the renderer at the specified page
				AddPrintFormatter (formatter, page);
				
				page = formatter.StartPage + formatter.PageCount - 1;

				previousFormatterMaxY = formatter.RectangleForPage (page).Bottom;
			}
		}
		
		// Custom UIPrintPageRenderer's may override this class to draw a custom print page header. 
		// To illustrate that, this class sets the date in the header.
		public override void DrawHeaderForPage (nint index, CGRect headerRect)
		{
			NSDateFormatter dateFormatter = new NSDateFormatter ();
			dateFormatter.DateFormat = "MMMM d, yyyy 'at' h:mm a";
			
			NSString dateString = new NSString (dateFormatter.ToString (NSDate.Now));
			dateFormatter.Dispose ();
			
			dateString.DrawString (headerRect, SystemFont, UILineBreakMode.Clip, UITextAlignment.Right);
			dateString.Dispose ();
		}
		
		public override void DrawFooterForPage (nint index, CGRect footerRect)
		{
			NSString footer = new NSString (string.Format ("Page {0} of {1}", index - pageRange.Location + 1, pageRange.Length));
			footer.DrawString (footerRect, SystemFont, UILineBreakMode.Clip, UITextAlignment.Center);
			footer.Dispose ();
		}
		
		// To intermix custom drawing with the drawing performed by an associated print formatter,
		// this method is called for each print formatter associated with a given page.
		//
		// We do this to intermix/overlay our custom drawing onto the recipe presentation.
		// We draw the upper portion of the recipe presentation by hand (image, title, desc), 
		// and the bottom portion is drawn via the UIMarkupTextPrintFormatter.
		public override void DrawPrintFormatterForPage (UIPrintFormatter printFormatter, nint page)
		{
			base.DrawPrintFormatterForPage (printFormatter, page);
			
			// To keep our custom drawing in sync with the printFormatter, base our drawing
			// on the formatters rect.
			CGRect rect = printFormatter.RectangleForPage (page);
			
			// Use a bezier path to draw the borders.
			// We may potentially choose not to draw either the top or bottom line
			// of the border depending on whether our recipe extended from the previous
			// page, or carries onto the subsequent page.
			UIBezierPath border = new UIBezierPath ();
			
			if (page == printFormatter.StartPage) {
				// For border drawing, get the rect that includes the formatter area plus the header area.
				// Move the formatter's rect up the size of the custom drawn recipe presentation
				// and essentially grow the rect's height by this amount.
				rect.Height += RecipeInfoHeight;
				rect.Y -= RecipeInfoHeight;
				
				border.MoveTo (rect.Location);
				border.AddLineTo (new CGPoint (rect.Right, rect.Top));
				
				Recipe recipe = recipeFormatterMap[printFormatter];
				
				// Run custom code to draw upper portion of the recipe presentation (image, title, desc)
				DrawRecipe (recipe, rect);
			}
			
			// Draw the left border
			border.MoveTo (rect.Location);
			border.AddLineTo (new CGPoint (rect.Left, rect.Bottom));
			
			// Draw the right border
			border.MoveTo (new CGPoint (rect.Right, rect.Top));
			border.AddLineTo (new CGPoint (rect.Right, rect.Bottom));
			
			if (page == printFormatter.StartPage + printFormatter.PageCount - 1)
				border.AddLineTo (new CGPoint (rect.Left, rect.Bottom));
			
			// Set the UIColor to be used by the current graphics context, and then stroke 
			// stroke the current path that is defined by the border bezier path.
			UIColor.Black.SetColor ();
			border.Stroke ();
		}
		
		// Custom code to draw upper portion of the recipe presentation (image, title, desc).
		// The argument rect is the full size of the recipe presentation.
		void DrawRecipe (Recipe recipe, CGRect rect)
		{
			DrawRecipeImage (recipe.Image, rect);
			DrawRecipeName (recipe.Name, rect);
			DrawRecipeInfo (recipe.AggregatedInfo, rect);
		}
		
		void DrawRecipeImage (UIImage image, CGRect rect)
		{
			// Create a new rect based on the size of the header area
			CGRect imageRect = CGRect.Empty;
			
			// Scale the image to fit in the infoRect
			nfloat maxImageDimension = RecipeInfoHeight - Padding * 2;
			nfloat largestImageDimension = (nfloat)Math.Max (image.Size.Width, image.Size.Height);
			nfloat scale = maxImageDimension / largestImageDimension;
			
			imageRect.Size = new CGSize (image.Size.Width * scale, image.Size.Height * scale);

			// Place the image rect at the x,y defined by the argument rect
			imageRect.Location = new CGPoint (rect.Left + Padding, rect.Top + Padding);
			
			// Ask the image to draw in the image rect
			image.Draw (imageRect);
		}
		
		// Custom drawing code to put the recipe name in the title section of the recipe presentation's header.
		void DrawRecipeName (string name, CGRect rect)
		{
			CGRect nameRect = CGRect.Empty;
			nameRect.X = rect.Left + RecipeInfoHeight;
			nameRect.Y = rect.Top + Padding;
			nameRect.Width = rect.Width - RecipeInfoHeight;
			nameRect.Height = RecipeInfoHeight;
			
			using (UIFont font = UIFont.BoldSystemFontOfSize (TitleSize)) {
				using (NSString str = new NSString (name)) {
					str.DrawString (nameRect, font, UILineBreakMode.Clip, UITextAlignment.Left);
				}
			}
		}
		
		// Custom drawing code to put the recipe recipe description, and prep time 
		// in the title section of the recipe presentation's header.
		void DrawRecipeInfo (string info, CGRect rect)
		{
			CGRect infoRect = CGRect.Empty;
			infoRect.X = rect.Left + RecipeInfoHeight;
			infoRect.Y = rect.Top + TitleSize * 2;
			infoRect.Width = rect.Width - RecipeInfoHeight;
			infoRect.Height = RecipeInfoHeight - TitleSize * 2;
			
			UIColor.DarkGray.SetColor ();
			using (NSString str = new NSString (info)) {
				str.DrawString (infoRect, SystemFont, UILineBreakMode.Clip, UITextAlignment.Left);
			}
		}
	}
}

