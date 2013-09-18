using System;
using System.IO;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using System.Threading.Tasks;

namespace coreimage
{
	class RebaseImagesViewController : VisitFilterViewController
	{
		public RebaseImagesViewController (FilterHolder [] filterList) : base (filterList)
		{
			BarButton.Title = "Rebase";
		}

		protected override async Task PerformActionOnTestImage (UIImage image, FilterHolder filterHolder)
		{
			await Task.Factory.StartNew (() => {
				var directory = ImageDirectory();
				var fileName = ImagePath(filterHolder.Name);

				if(!Directory.Exists(directory))
					Directory.CreateDirectory(directory);

				if (Runtime.Arch == Arch.SIMULATOR) {
					if(File.Exists(fileName))
						File.Delete(fileName);

					NSError err;
					image.AsPNG().Save(fileName, NSDataWritingOptions.FileProtectionNone, out err);

					if(err != null)
						Console.WriteLine("Could not write image File. " + Environment.NewLine + err.LocalizedDescription);
				}
			});
		}
	}
}

