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
        public static NSString localKeyPath = new NSString("Progress.FractionCompleted");

        public PhotoPlaceholderCollectionViewCell (IntPtr handle) : base (handle)
        {
            progressView = new UIProgressView();
			progressView.Frame = new CGRect(0, 0, 200, 200);
			AddSubview(progressView);
        }

        NSProgress progress;
        NSProgress Progress {
            get
            {
                return progress;
            }
            set
            {
                progress = value;

                if (progress == null) return;
                progressView.SetProgress((float)progress.FractionCompleted, false);

                progress.AddObserver(this,
                                     localKeyPath,
                                     NSKeyValueObservingOptions.Initial | NSKeyValueObservingOptions.New,
                                     IntPtr.Zero);
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