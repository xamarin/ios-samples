// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace MTGKTapper
{
	[Register ("MTGKTapperViewController")]
	partial class MTGKTapperViewController
	{
		[Outlet]
		UIKit.UILabel currentScoreTextField { get; set; }

		[Outlet]
		UIKit.UILabel globalHighestScoreTextField { get; set; }

		[Outlet]
		UIKit.UIButton incrementScoreButton { get; set; }

		[Outlet]
		UIKit.UILabel playerBestScoreTextField { get; set; }

		[Outlet]
		UIKit.UIButton resetButton { get; set; }

		[Outlet]
		UIKit.UIButton selectLeaderBoardButton { get; set; }

		[Outlet]
		UIKit.UIButton showAchievementButton { get; set; }

		[Outlet]
		UIKit.UIButton showLeaderboardButton { get; set; }

		[Outlet]
		UIKit.UIButton submitScoreButton { get; set; }
		
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
