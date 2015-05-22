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
	class CustomAnimation : Animation
	{
		View view;

		protected override void ApplyTransformation (float interpolatedTime, Transformation t)
		{
			base.ApplyTransformation (interpolatedTime, t);

			//view.LayoutParameters = LinearLayout.LayoutParams.FillParent
		}
	}

}


