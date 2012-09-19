using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace MediaNotes
{
	public interface PNDataSourceProtocol
	{
		UIImage ImageForCurrentItem ();
		NSUrl UrlForCurrentItem ();
		void ProceedToNextItem ();
		void ProceedToPreviousItem ();
	}
}