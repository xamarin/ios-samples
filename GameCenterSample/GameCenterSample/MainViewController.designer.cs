// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace GameCenterSample
{
	[Register ("MainViewController")]
	partial class MainViewController
	{
		[Outlet]
		UIKit.UIButton resetAchievementsButton { get; set; }

		[Outlet]
		UIKit.UITextField scoreTextField { get; set; }

		[Outlet]
		UIKit.UIButton showAchievementsButton { get; set; }

		[Outlet]
		UIKit.UIButton showLeaderBoardButton { get; set; }

		[Outlet]
		UIKit.UIButton submitAchievementButton { get; set; }

		[Outlet]
		UIKit.UIButton submitScoreButton { get; set; }
		
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
