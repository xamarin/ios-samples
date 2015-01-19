// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

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
		UIKit.UIButton _playButton { get; set; }

		[Outlet]
		UIKit.UIButton _rewButton { get; set; }

		[Outlet]
		UIKit.UIButton _ffwButton { get; set; }

		[Outlet]
		UIKit.UILabel _currentTime { get; set; }

		[Outlet]
		UIKit.UILabel _duration { get; set; }

		[Outlet]
		UIKit.UISlider _volumeSlider { get; set; }

		[Outlet]
		UIKit.UISlider _progressBar { get; set; }

		[Outlet]
		UIKit.UILabel _fileName { get; set; }

		[Outlet]
		avTouch.CALevelMeter _lvlMeter_in { get; set; }

		[Action ("volumeSliderMoved:")]
		partial void volumeSliderMoved (UIKit.UISlider sender);

		[Action ("progressSliderMoved:")]
		partial void progressSliderMoved (UIKit.UISlider sender);

		[Action ("playButtonPressed:")]
		partial void playButtonPressed (UIKit.UIButton sender);

		[Action ("ffwButtonReleased:")]
		partial void ffwButtonReleased (UIKit.UIButton sender);

		[Action ("rewButtonReleased:")]
		partial void rewButtonReleased (UIKit.UIButton sender);

		[Action ("rewButtonPressed:")]
		partial void rewButtonPressed (UIKit.UIButton sender);

		[Action ("ffwButtonPressed:")]
		partial void ffwButtonPressed (UIKit.UIButton sender);
	}

	[Register ("CALevelMeter")]
	partial class CALevelMeter
	{
	}
}
