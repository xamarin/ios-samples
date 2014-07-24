// 
// Recipe.cs
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
using System.Text;
using System.Collections;
using System.Collections.Generic;

using Foundation;
using UIKit;

namespace RecipesAndPrinting
{
	public class Ingredient {
		public string Name;
		public string Amount;
	}
	
	public class Recipe : IDisposable
	{
		public Recipe ()
		{
		}
		
		public string Name {
			get; set;
		}
		
		public string Description {
			get; set;
		}
		
    	public string PrepTime {
			get; set;
		}
		
		public string Instructions {
			get; set;
		}
		
		public UIImage Image {
			get; set;
		}
		
		public UIImage ThumbnailImage {
			get; set;
		}
		
		public Ingredient[] Ingredients {
			get; set;
		}
		
		public string HtmlRepresentation {
			get {
				StringBuilder body = new StringBuilder ("<!DOCTYPE html><html><body>");
				
				if (Ingredients != null && Ingredients.Length > 0) {
					body.Append ("<h2>Ingredients</h2>");
					body.Append ("<ul>");
					foreach (var ingredient in Ingredients)
						body.AppendFormat ("<li>{0} {1}</li>", ingredient.Amount, ingredient.Name);
					body.Append ("</ul>");
				}
				
				if (Instructions != null && Instructions.Length > 0) {
					body.Append ("<h2>Instructions</h2>");
					body.AppendFormat ("<p>{0}</p>", Instructions);
				}
				
				body.Append ("</body></html>");
				
				return body.ToString ();
			}
		}
		
		public string AggregatedInfo {
			get {
				StringBuilder info = new StringBuilder ();
				
				if (Description != null && Description.Length > 0)
					info.Append (Description);
				
				if (PrepTime != null && PrepTime.Length > 0) {
					if (info.Length > 0)
						info.Append ('\n');
					
					info.AppendFormat ("Preparation Time: {0}", PrepTime);
				}
				
				return info.ToString ();
			}
		}
		
		public void Dispose ()
		{
			if (ThumbnailImage != null) {
				ThumbnailImage.Dispose ();
				ThumbnailImage = null;
			}
			
			if (Image != null) {
				Image.Dispose ();
				Image = null;
			}
		}
	}
}

