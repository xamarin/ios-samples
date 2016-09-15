using Foundation;
using System;
using UIKit;
using WatchKit;

namespace Watchkit2Extension
{
	public partial class CrownDetailController : WKInterfaceController, IWKCrownDelegate
    {
        public CrownDetailController (IntPtr handle) : base (handle)
        {
        }

		public override void Awake(NSObject context)
		{
			base.Awake(context);

			var pickerItems = new WKPickerItem[]
			{
				new WKPickerItem {Title = "Item 1", Caption="Red"},	
				new WKPickerItem {Title = "Item 2", Caption="Green"},
				new WKPickerItem {Title = "Item 3", Caption="Blue"}
			};

			pickerView.SetItems(pickerItems);

			CrownSequencer.Delegate = this;
		}

		public override void WillActivate()
		{
			base.WillActivate();
			CrownSequencer.Focus();
		}

		void UpdateCrownLabels()
		{
			velocityLabel.SetText("RPS:" + CrownSequencer.RotationsPerSecond);
			stateLabel.SetText(CrownSequencer.Idle ? "Idle:true" : "Idle: false");
		}

		[Export("crownDidRotate:rotationalDelta:")]
		public void CrownDidRotate(WKCrownSequencer crownSequencer, double rotationalDelta)
		{
			UpdateCrownLabels();
		}

		[Export("crownDidBecomeIdle:")]
		public void CrownDidBecomeIdle(WKCrownSequencer crownSequencer)
		{
			UpdateCrownLabels();
		}
    }
}