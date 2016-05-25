using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using Android.Views.Animations;

using Xamarin.Controls;

namespace AndroidAlertCenter
{
	[Activity (Label = "AndroidAlertCenter", MainLauncher = true)]
	public class Activity1 : Activity
	{
		int count = 1;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.myButton);

			//We need to initialize AlertCenter 
			AlertCenter.Default.Init (Application);

			AlertCenter.Default.TimeToClose = new TimeSpan (0, 0, 0, 3, 0);

			AlertCenter.Default.BackgroundColor = Color.White;

			AlertCenter.Default.PostMessage ("title", "A message from Earth has arrived", Resource.Drawable.Icon);

			button.Click += delegate {
				AlertCenter.Default.PostMessage ("Title"
				                                 , string.Format ("{0} clicks!", count++)
				                                 , Resource.Drawable.Icon
				                                 , () => {
															Console.WriteLine (string.Format ("Message #{0} was clicked", count));
														 });
			};

		}
	}
}


