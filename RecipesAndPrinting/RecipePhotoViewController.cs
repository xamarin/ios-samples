// 
// RecipePhotoViewController.cs
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

using Foundation;
using UIKit;

namespace RecipesAndPrinting
{
	public class RecipePhotoViewController : UIViewController
	{
		UIImageView imageView;
		
		public RecipePhotoViewController ()
		{
			Title = "Photo";
		}
		
		public Recipe Recipe {
			get; set;
		}
		
		public override void LoadView ()
		{
			base.LoadView ();
			
			imageView = new UIImageView (UIScreen.MainScreen.ApplicationFrame);
			imageView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
			imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			imageView.BackgroundColor = UIColor.Black;
			imageView.Image = Recipe.Image;
			View = imageView;
		}
		
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			
			imageView.Dispose ();
			imageView = null;
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
		}
	}
}

