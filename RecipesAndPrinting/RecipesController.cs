// 
// RecipesController.cs
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
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using Foundation;
using UIKit;

namespace RecipesAndPrinting
{
	public class RecipesController
	{
		Recipe[] recipes;
		
		public RecipesController ()
		{
			recipes = CreateDemoData ();
		}
		
		public Recipe[] Recipes {
			get { return recipes; }
		}
		
		static Recipe[] CreateDemoData ()
		{
			Assembly assembly = typeof (Recipe).Assembly;
			Recipe[] recipes = new Recipe [2];
			
			recipes[0] = new Recipe () {
				Name = "Cherry Cobbler",
				Description = "Cherry cobbler with homemade whipped cream",
				PrepTime = "1.5 hours",
				Image = UIImage.FromResource (assembly, "RecipesAndPrinting.Images.CherryCobbler.png"),
				ThumbnailImage = UIImage.FromResource (assembly, "RecipesAndPrinting.Images.CherryCobblerThumbnail.png"),
				Instructions = "Mix the ingredients, bake and voilà!",
				Ingredients = new Ingredient [10] {
					new Ingredient () { Name = "Cherries (fresh)", Amount = "3 cups" },
					new Ingredient () { Name = "White Sugar", Amount = "1/2 cup" },
					new Ingredient () { Name = "Brown Sugar", Amount = "1/2 cup" },
					new Ingredient () { Name = "Cinnamon", Amount = "1/8 tsp" },
					new Ingredient () { Name = "Nutmeg", Amount = "1/8 tsp" },
					new Ingredient () { Name = "Lemon Juice", Amount = "1 tsp" },
					new Ingredient () { Name = "Flour", Amount = "1 cup" },
					new Ingredient () { Name = "Baking Powder", Amount = "1 tsp" },
					new Ingredient () { Name = "Salt", Amount = "1/8 tsp" },
					new Ingredient () { Name = "Butter", Amount = "6 tbsp" },
				},
			};
			
			recipes[1] = new Recipe () {
				Name = "Chocolate Cake",
				Description = "Chocolate cake with chocolate frosting",
				PrepTime = "1 hour",
				Image = UIImage.FromResource (assembly, "RecipesAndPrinting.Images.ChocolateCake.png"),
				ThumbnailImage = UIImage.FromResource (assembly, "RecipesAndPrinting.Images.ChocolateCakeThumbnail.png"),
				Instructions = "Mix the ingredients, bake and voilà!",
				Ingredients = new Ingredient [5] {
					new Ingredient () { Name = "Chocolate", Amount = "1 cup" },
					new Ingredient () { Name = "Flour", Amount = "1 cup" },
					new Ingredient () { Name = "Eggs", Amount = "2" },
					new Ingredient () { Name = "Sugar", Amount = "1 cup" },
					new Ingredient () { Name = "Salt", Amount = "pinch" }
				},
			};
			
			return recipes;
		}
	}
}

