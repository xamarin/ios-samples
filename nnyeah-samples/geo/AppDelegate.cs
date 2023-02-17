using System.Threading.Tasks;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;

namespace geo;

[Register ("AppDelegate")]
public class AppDelegate : UIApplicationDelegate {
	public override UIWindow? Window {
		get;
		set;
	}

	UILabel? Label;

	public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
	{
		// create a new window instance based on the screen size
		Window = new UIWindow (UIScreen.MainScreen.Bounds);

		// create a UIViewController with a single UILabel
		var vc = new UIViewController ();
		Label = new UILabel (Window!.Frame) {
			BackgroundColor = UIColor.SystemBackground,
			TextAlignment = UITextAlignment.Center,
			AutoresizingMask = UIViewAutoresizing.All,
			Lines = 30,
		};
		vc.View!.AddSubview (Label);
		Window.RootViewController = vc;

		// make the window visible
		Window.MakeKeyAndVisible ();

		Task.Run (() => Query ());

		return true;
	}

	void SetTextOnMainThread (string text)
	{
		UIApplication.SharedApplication.BeginInvokeOnMainThread (() => {
			if (Label is not null) Label.Text = text;
		});
	}

	async void Query ()
	{
		SetTextOnMainThread ("waiting a bit.");
		await Task.Delay (5000);
		SetTextOnMainThread ("checking if we can get location");
		if (!CrossGeolocator.Current.IsGeolocationAvailable) {
			SetTextOnMainThread ("Does not have permissions!");
		} else {
			SetTextOnMainThread ("getting position.");
			var position = await GetPosition ();
			if (position is not null) {
				SetTextOnMainThread ($"Lat: {position.Latitude} x Long: {position.Longitude}");
			} else {
				SetTextOnMainThread ($"Could not find the location");
			}
		}
	}

	async Task<Position?> GetPosition ()
	{
		try {
			var current = CrossGeolocator.Current;
			current.DesiredAccuracy = 100;
			var position = await current.GetLastKnownLocationAsync ();

			if (position is null)
				position = await current.GetPositionAsync (TimeSpan.FromSeconds (20), null, true);

			return position;
		} catch (Exception e) {
			SetTextOnMainThread ($"Oops: {e.GetType ().Name} - {e.Message}\n{e.StackTrace}");
			return null;
		}
	}
}
