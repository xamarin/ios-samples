using System;
using System.IO;
using System.Drawing;
using UIKit;
using CoreImage;
using Foundation;
using ObjCRuntime;
using System.Threading.Tasks;

namespace coreimage
{
	class VisitFilterViewController : UIViewController
	{
		protected UIImageView ImageView { get; set; }
		protected UIBarButtonItem BarButton { get; set; }
		protected FilterHolder [] FilterList { get; set; }
		protected bool ShouldStop { get; set; }

		public VisitFilterViewController (FilterHolder [] filterList)
		{
			FilterList = filterList;
			ShouldStop = false;

			BarButton = new UIBarButtonItem ("Run", UIBarButtonItemStyle.Plain, async (sender, e) => {
				await VisitFiltersWithAction ();
			});
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			View.BackgroundColor = UIColor.Black;

			if (UIDevice.CurrentDevice.CheckSystemVersion (7, 0))
				EdgesForExtendedLayout = UIRectEdge.None;
			View.AutosizesSubviews = true;

			ImageView = new UIImageView (View.Bounds);
			ImageView.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
			ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;

			View.AddSubview (ImageView);

			NavigationItem.RightBarButtonItem = BarButton;
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			ShouldStop = true;
		} 

		protected virtual async Task VisitFiltersWithAction ()
		{
			try {
				BarButton.Enabled = false;
				ShouldStop = false;

				using (var context = CIContext.FromOptions (null)) {

					foreach(var filter in FilterList)
					{
						Title = filter.Name;

						if (ShouldStop) 
							break;

						var resultImageTask = Task.Factory.StartNew ( () =>{
							var output = filter.Callback ();
							
								using (var result = context.CreateCGImage (output, output.Extent)) {
									return UIImage.FromImage (result);
							}
						});

						var resultImage = await resultImageTask;

						if (ShouldStop) 
							break;

						if (ImageView.Image != null)
							ImageView.Image.Dispose ();

						ImageView.Image = resultImage;

						await PerformActionOnTestImage (resultImage, filter);
					}
				}

			}
			finally {
				BarButton.Enabled = true;
			}
		}

		protected string ImageDirectory()
		{
			var directory = Path.Combine(Directory.GetCurrentDirectory(), "TestImages");
			return directory;
		}

		protected string ImagePath(string imageName)
		{	
			var fileName = Path.Combine (ImageDirectory(), imageName);
			fileName = Path.ChangeExtension(fileName, ".png");

			return fileName;
		}

		protected virtual async Task PerformActionOnTestImage (UIImage image, FilterHolder filterHolder)
		{
			await Task.Factory.StartNew (() => {});
		}
	}
}

