using System;

using UIKit;
using Foundation;

namespace CloudKitAtlas
{
	public partial class ResultsViewController : UIViewController
	{
		public CodeSample CodeSample { get; set; }
		public Results Results { get; set; }

		public ResultsViewController (IntPtr handle) : base (handle)
		{
		}

	}
}
