using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using Android.Views.Animations;
using Android.Graphics.Drawables;

namespace AndroidAlertCenter
{
	class NotificationView : LinearLayout
	{
		PaintDrawable drawable;

		public NotificationView (Context context):base(context)
		{
			drawable = new PaintDrawable();
			drawable.Paint.Color = Color.White;
			drawable.SetCornerRadius(5);

			SetBackgroundDrawable(drawable);

			var imageView = new ImageView(context);
			imageView.SetImageResource(Resource.Drawable.Icon);

			AddView(imageView);

			var textView = new TextView(context);
			textView.Text = "hello machina";
			textView.SetTextColor(Color.Black);
			//textView.Layout(150,0,50,0);
			//textView.OffsetLeftAndRight(50);

			//var prms = new RelativeLayout(context);
			//prms.off = 50;
			AddView(textView/*,prms*/);


		}
	}

}


