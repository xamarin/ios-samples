using System;
using System.Threading.Tasks;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using System.Drawing;
using System.Collections.Generic;
using MonoTouch.Dialog;
using System.Linq;

namespace coreimage
{
	class TestImagesViewController : VisitFilterViewController
	{
		List<TestResult> ResultList { get; set;}

		public TestImagesViewController (FilterHolder [] filterList) : base (filterList)
		{
			BarButton.Title = "Test";
		}

		protected override async Task VisitFiltersWithAction ()
		{
			ResultList = new List<TestResult> ();

			await base.VisitFiltersWithAction ();

			if (ShouldStop)
				return;

			var query = ResultList.Where (l => !l.Pass);

			if (query.Any ()) {
				var root = new RootElement ("Failed Tests");
				var section = new Section ();
				root.Add (section);

				section.AddAll (query.Select (l => new StringElement (l.FilterName)));

				NavigationItem.BackBarButtonItem = new UIBarButtonItem ("Back", UIBarButtonItemStyle.Bordered, null);
				NavigationController.PushViewController (new DialogViewController (root, true), true);
			}
			else
				Title = "All Tests Passed";
		}

		protected override async Task PerformActionOnTestImage (UIImage image, FilterHolder filterHolder)
		{
			await Task.Factory.StartNew (() => {
				var cgImage = image.CGImage;

				// Get the Image data for the current Image
				byte[] testRawData = new byte[cgImage.Width * cgImage.Height * 4];
				var testColorSpace = CGColorSpace.CreateDeviceRGB();

				var testCGContext = new CGBitmapContext(testRawData, cgImage.Width, cgImage.Height, 8, cgImage.BytesPerRow, testColorSpace, CGImageAlphaInfo.PremultipliedLast);
				testCGContext.DrawImage(new RectangleF(0, 0, cgImage.Width, cgImage.Height), cgImage);

				// Get the base image
				var baseImage = GetTestImage(filterHolder.Name);

				// Get the image data for the base image.
				byte[] baseRawData = new byte[baseImage.Width * baseImage.Height * 4];
				var baseColorSpace = CGColorSpace.CreateDeviceRGB();

				var baseCGContext = new CGBitmapContext(baseRawData, baseImage.Width, baseImage.Height, 8, baseImage.BytesPerRow, baseColorSpace, CGImageAlphaInfo.PremultipliedLast);
				baseCGContext.DrawImage(new RectangleF(0, 0, baseImage.Width, baseImage.Height), baseImage);

				if (baseRawData.Length != testRawData.Length ) {
					ResultList.Add(new TestResult() { FilterName = filterHolder.Name, Pass = false});
					return;
				}

				// Compare each Pixel
				for(var i = 0; i < baseRawData.Length; i++)
				{
					if(testRawData[i] != baseRawData[i])
					{
						ResultList.Add(new TestResult() { FilterName = filterHolder.Name, Pass = false});
						return;
					}
				}

				ResultList.Add(new TestResult() { FilterName = filterHolder.Name, Pass = true});
			});
		}

		CGImage GetTestImage(string imageName)
		{
			var image = UIImage.FromFile(ImagePath(imageName));
			return image.CGImage;
		}

		struct TestResult
		{
			public bool Pass {get; set;}
			public string FilterName {get; set;}
		}
	}
}

