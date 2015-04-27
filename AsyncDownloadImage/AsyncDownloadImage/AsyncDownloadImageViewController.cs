using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;

using CoreGraphics;
using CoreImage;

using Foundation;
using UIKit;

namespace AsyncDownloadImage
{
	public partial class AsyncDownloadImageViewController : UIViewController
	{
		int clickNumber = 0;
		WebClient webClient;
		public AsyncDownloadImageViewController () : base ("AsyncDownloadImageViewController", null)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			if (UIDevice.CurrentDevice.CheckSystemVersion (7, 0)) {
				this.EdgesForExtendedLayout = UIRectEdge.None;
			}
			this.downloadButton.SetTitleColor (UIColor.Blue, UIControlState.Normal);
			this.clickButton.SetTitleColor (UIColor.Blue, UIControlState.Normal);
			this.clickButton.TouchUpInside += (sender, e) => {
				clickNumber++;
				this.clickButton.SetTitle( "Click Me:" + clickNumber, UIControlState.Normal);
			};

			this.downloadButton.TouchUpInside += downloadAsync;

			this.downloadProgress.Progress = 0.0f;

		}

		//Change Status Bar colour to light
		public override UIStatusBarStyle PreferredStatusBarStyle ()
		{
			if (UIDevice.CurrentDevice.CheckSystemVersion (7, 0)) {
				return UIStatusBarStyle.LightContent;
			}
			return UIStatusBarStyle.Default;
		}

		async void downloadAsync(object sender, System.EventArgs ea)
		{
			webClient = new WebClient ();
			//An large image url
			var url = new Uri ("http://photojournal.jpl.nasa.gov/jpeg/PIA15416.jpg");
			byte[] bytes = null;

			webClient.DownloadProgressChanged += HandleDownloadProgressChanged;

			this.downloadButton.SetTitle ("Cancel",UIControlState.Normal);
			this.downloadButton.TouchUpInside -= downloadAsync;
			this.downloadButton.TouchUpInside += cancelDownload;
			infoLabel.Text = "Downloading...";

			//Start download data using DownloadDataTaskAsync
			try{
				bytes = await webClient.DownloadDataTaskAsync(url);
			}
			catch(OperationCanceledException){
				Console.WriteLine ("Task Canceled!");
				return;
			}
			catch(Exception e) {
				Console.WriteLine (e.ToString());
				return;
			}
			string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			string localFilename = "downloaded.png";
 			string localPath = Path.Combine (documentsPath, localFilename);
			infoLabel.Text = "Download Complete";

			//Save the image using writeAsync
			FileStream fs = new FileStream (localPath, FileMode.OpenOrCreate);
			await fs.WriteAsync (bytes, 0, bytes.Length);

			Console.WriteLine("localPath:"+localPath);

			//Resizing image is time costing, using async to avoid blocking the UI thread
			UIImage image = null;
			CGSize imageViewSize = imageView.Frame.Size;

			infoLabel.Text = "Resizing Image...";
			await Task.Run( () => { image = UIImage.FromFile(localPath).Scale(imageViewSize); } );
			Console.WriteLine ("Loaded!");

			imageView.Image = image;

			infoLabel.Text = "Click Dowload button to download the image";

			this.downloadButton.TouchUpInside -= cancelDownload;
			this.downloadButton.TouchUpInside += downloadAsync;
			this.downloadButton.SetTitle ("Download", UIControlState.Normal);
			this.downloadProgress.Progress = 0.0f;
		}

		void HandleDownloadProgressChanged (object sender, DownloadProgressChangedEventArgs e)
		{
			this.downloadProgress.Progress = e.ProgressPercentage / 100.0f;
		}

		void cancelDownload(object sender, System.EventArgs ea)
		{
			Console.WriteLine ("Cancel clicked!");
			if(webClient!=null)
				webClient.CancelAsync ();

			webClient.DownloadProgressChanged -= HandleDownloadProgressChanged;

			this.downloadButton.TouchUpInside -= cancelDownload;
			this.downloadButton.TouchUpInside += downloadAsync;
			this.downloadButton.SetTitle ("Download", UIControlState.Normal);
			this.downloadProgress.Progress = 0.0f;

			new UIAlertView ("Canceled"
			                 , "Download has been canceled."
			                 , null
			                 , "OK"
			                 , null).Show();
			infoLabel.Text = "Click Dowload button to download the image";
		}

		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
		}
	}
}

