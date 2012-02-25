// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace avTouch
{
	[Register ("avTouchViewController")]
	partial class avTouchViewController
	{
		[Outlet]
		avTouch.avTouchController controller { get; set; }
	}

	[Register ("avTouchController")]
	partial class avTouchController
	{
		[Outlet]
		MonoTouch.UIKit.UIButton _playButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton _rewButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton _ffwButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel _currentTime { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel _duration { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISlider _volumeSlider { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISlider _progressBar { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel _fileName { get; set; }

		[Outlet]
		avTouch.CALevelMeter _lvlMeter_in { get; set; }

		[Action ("volumeSliderMoved:")]
		partial void volumeSliderMoved (MonoTouch.UIKit.UISlider sender);

		[Action ("progressSliderMoved:")]
		partial void progressSliderMoved (MonoTouch.UIKit.UISlider sender);

		[Action ("playButtonPressed:")]
		partial void playButtonPressed (MonoTouch.UIKit.UIButton sender);

		[Action ("ffwButtonReleased:")]
		partial void ffwButtonReleased (MonoTouch.UIKit.UIButton sender);

		[Action ("rewButtonReleased:")]
		partial void rewButtonReleased (MonoTouch.UIKit.UIButton sender);

		[Action ("rewButtonPressed:")]
		partial void rewButtonPressed (MonoTouch.UIKit.UIButton sender);

		[Action ("ffwButtonPressed:")]
		partial void ffwButtonPressed (MonoTouch.UIKit.UIButton sender);
	}

	[Register ("CALevelMeter")]
	partial class CALevelMeter
	{
	}
}
