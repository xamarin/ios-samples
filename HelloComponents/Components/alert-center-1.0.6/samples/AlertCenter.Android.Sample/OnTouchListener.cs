using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using Android.Views.Animations;

namespace AndroidAlertCenter
{
	class OnTouchListener : Activity, View.IOnTouchListener
	{
		//Context ApplicationContext;

		public OnTouchListener (/*Context applicationContext*/)
		{
			//ApplicationContext = applicationContext;
		}

//		public IntPtr Handle {
//			get {} ;
//		}



		public bool OnTouch (View v, MotionEvent e)
		{
//			var toast = Toast.MakeText(Application.Context,"Yoopieeee!!!", ToastLength.Short);
//			
//			toast.SetGravity(GravityFlags.Top | GravityFlags.Left,15,15);
//			
//			toast.Show();

			v.Visibility = ViewStates.Invisible;

			Console.WriteLine("Notification banner clicked.");

			return false;
		}
	}

}


