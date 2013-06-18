// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace MTGKTapper
{
	[Register ("MTGKTapperViewController")]
	partial class MTGKTapperViewController
	{
		[Outlet]
		MonoTouch.UIKit.UILabel currentScoreTextField { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel globalHighestScoreTextField { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton incrementScoreButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel playerBestScoreTextField { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton resetButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton selectLeaderBoardButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton showAchievementButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton showLeaderboardButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton submitScoreButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (currentScoreTextField != null) {
				currentScoreTextField.Dispose ();
				currentScoreTextField = null;
			}

			if (playerBestScoreTextField != null) {
				playerBestScoreTextField.Dispose ();
				playerBestScoreTextField = null;
			}

			if (globalHighestScoreTextField != null) {
				globalHighestScoreTextField.Dispose ();
				globalHighestScoreTextField = null;
			}

			if (incrementScoreButton != null) {
				incrementScoreButton.Dispose ();
				incrementScoreButton = null;
			}

			if (submitScoreButton != null) {
				submitScoreButton.Dispose ();
				submitScoreButton = null;
			}

			if (selectLeaderBoardButton != null) {
				selectLeaderBoardButton.Dispose ();
				selectLeaderBoardButton = null;
			}

			if (showLeaderboardButton != null) {
				showLeaderboardButton.Dispose ();
				showLeaderboardButton = null;
			}

			if (showAchievementButton != null) {
				showAchievementButton.Dispose ();
				showAchievementButton = null;
			}

			if (resetButton != null) {
				resetButton.Dispose ();
				resetButton = null;
			}
		}
	}
}
