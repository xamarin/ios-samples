using Foundation;
using System;
using UIKit;

namespace CustomTransitions
{
    public partial class AdaptativePresentationSegue : UIStoryboardSegue
    {
        public AdaptativePresentationSegue (IntPtr handle) : base (handle)
        {
        }

		public void perform()
		{
			UIViewController sourceViewController = SourceViewController;
			UIViewController destinationViewController = DestinationViewController;
		}
    }
}