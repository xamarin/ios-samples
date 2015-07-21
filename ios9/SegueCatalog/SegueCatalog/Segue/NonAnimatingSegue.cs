using System;

using UIKit;

namespace SegueCatalog
{
	public partial class NonAnimatingSegue : UIStoryboardSegue
	{
		public NonAnimatingSegue (IntPtr handle)
			: base (handle)
		{
		}

		public override void Perform ()
		{
			UIView.PerformWithoutAnimation (base.Perform);
		}
	}
}
