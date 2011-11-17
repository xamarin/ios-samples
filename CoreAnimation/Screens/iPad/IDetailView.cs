using System;
using MonoTouch.UIKit;
namespace Example_CoreAnimation.Screens.iPad
{
	public interface IDetailView
	{
		void AddContentsButton (UIBarButtonItem button);
		void RemoveContentsButton ();
	}
}

