using System;
using UIKit;
using Foundation;

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