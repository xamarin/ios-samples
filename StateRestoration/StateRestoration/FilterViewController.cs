
using System;

using Foundation;
using UIKit;

namespace StateRestoration
{
	public partial class FilterViewController : UIViewController
	{
		public ImageFilter filter;

		public FilterViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			ActiveSwitch.ValueChanged += HandleValueChanged;
			Slider.ValueChanged += SetFilterValue;
			Update ();
		}

		void Update ()
		{
			ActiveSwitch.On = filter.Active;
			Slider.Enabled = filter.Active;
			Slider.Value = filter.Value;
		}

		void SetFilterValue (object sender, EventArgs e)
		{
			filter.Value = Slider.Value;
			filter.Dirty = true;
		}

		void HandleValueChanged (object sender, EventArgs e)
		{
			filter.Active = ActiveSwitch.On;
			filter.Dirty = true;
			Slider.Enabled = filter.Active;
		}

		// State Restoration
		public override void EncodeRestorableState (NSCoder coder)
		{
			base.EncodeRestorableState (coder);
			coder.Encode (filter, "kImageFilterKey");
		}

		public override void DecodeRestorableState (NSCoder coder)
		{
			base.DecodeRestorableState (coder);
			filter = coder.DecodeObject ("kImageFilterKey") as ImageFilter;
		}

		public override void ApplicationFinishedRestoringState ()
		{
			base.ApplicationFinishedRestoringState ();
			Update ();
		}
	}
}
