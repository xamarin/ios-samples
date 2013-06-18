// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace GameCenterSample
{
	[Register ("MainViewController")]
	partial class MainViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIButton resetAchievementsButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField scoreTextField { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton showAchievementsButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton showLeaderBoardButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton submitAchievementButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton submitScoreButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (scoreTextField != null) {
				scoreTextField.Dispose ();
				scoreTextField = null;
			}

			if (showAchievementsButton != null) {
				showAchievementsButton.Dispose ();
				showAchievementsButton = null;
			}

			if (showLeaderBoardButton != null) {
				showLeaderBoardButton.Dispose ();
				showLeaderBoardButton = null;
			}

			if (submitScoreButton != null) {
				submitScoreButton.Dispose ();
				submitScoreButton = null;
			}

			if (submitAchievementButton != null) {
				submitAchievementButton.Dispose ();
				submitAchievementButton = null;
			}

			if (resetAchievementsButton != null) {
				resetAchievementsButton.Dispose ();
				resetAchievementsButton = null;
			}
		}
	}
}
