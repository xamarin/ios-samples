using Foundation;
using System;
using UIKit;
using CoreFoundation;
using CoreGraphics;

namespace PhotoAlbum
{
    public partial class PhotoPlaceholderCollectionViewCell : UICollectionViewCell
    {
        public static string Identifier = "PhotoPlaceholderCollectionViewCell";
        public static NSString localKeyPath = new NSString("progress.fractionCompleted");


        public PhotoPlaceholderCollectionViewCell (IntPtr handle) : base (handle)
        {
            progressView = new UIProgressView();
			progressView.Frame = new CGRect(0, 95, 200, 10);
            progressView.SetProgress(0.1f, false);
			AddSubview(progressView);
        }

        NSProgress progress;

        [Export("progress")]
        public NSProgress Progress {
            
            get
            {
                return progress; 
            }

            set
            {
                progress = value;

                if (progress == null) return;
                progressView.SetProgress((float)progress.FractionCompleted, false);

                Console.WriteLine("Dropping images from external applications doesn't work due to a bug with NSItemProviderReading and UIImage");
                // HACK: fix KVO once the LoadObject bug is resolved
                //Progress.AddObserver(this,
                                     //localKeyPath,
                                     //NSKeyValueObservingOptions.Initial | NSKeyValueObservingOptions.New,
                                     //IntPtr.Zero);
            }
        }

		public void Configure(NSProgress progress)
		{
            Progress = progress;
		}

        public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
        {
            var @object = ofObject as NSProgress;

            if (@object == progress && keyPath == localKeyPath)
            {
                var fractionCompleted = Convert.ToDouble(change?[ChangeNewKey]);
                DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    progressView.SetProgress((float)fractionCompleted, true);
                });
            }
        }
    }
}