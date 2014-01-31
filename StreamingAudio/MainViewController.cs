using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace StreamingAudio
{
	public enum PlayerOption
	{
		Stream = 0,
		StreamAndSave
	}

	public partial class MainViewController : UIViewController
	{
		private const string LetsStopTheWarUrl = "http://ccmixter.org/content/bradstanfield/bradstanfield_-_People_Let_s_Stop_The_War.mp3";

		public MainViewController () : base ("MainViewController", null)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			Title = "Streaming MP3";

			streamAndPlayButton.TouchUpInside += (sender, e) => OpenPlayerView(PlayerOption.Stream);
			streamSaveAndPlayButton.TouchUpInside += (sender, e) => OpenPlayerView(PlayerOption.StreamAndSave);

			urlTextbox.Text = LetsStopTheWarUrl;
			urlTextbox.EditingDidEnd += (sender, e) => urlTextbox.ResignFirstResponder ();
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			statusLabel.Text = string.Empty;
		}

		private void OpenPlayerView(PlayerOption option)
		{
			statusLabel.Text = "Starting HTTP request";
			var playerViewController = new PlayerViewController (option, LetsStopTheWarUrl);
			playerViewController.ErrorOccurred += HandleError;
			NavigationController.PushViewController (playerViewController, true);
		}

		private void HandleError(string message)
		{
			InvokeOnMainThread (delegate {
				statusLabel.Text = message;
			});
		}
	}
}

