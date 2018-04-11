using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Foundation;
using UIKit;

namespace GreatPlays {
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate, IUISplitViewControllerDelegate {

		public override UIWindow Window { get; set; }

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			var splitViewController = Window?.RootViewController as UISplitViewController;
			var navigationController = splitViewController?.ViewControllers [splitViewController.ViewControllers.Length - 1] as UINavigationController;
			if (splitViewController != null && navigationController != null) {
				navigationController.TopViewController.NavigationItem.LeftBarButtonItem = splitViewController.DisplayModeButtonItem;
				splitViewController.Delegate = this;
			}

			var hamlet = new Play ("Hamlet");
			
			var sceneRangesByActNumber = new Dictionary<int, IEnumerable<int>> {
				{ 1, Enumerable.Range (1, 5) },
				{ 2, Enumerable.Range (1, 2) },
				{ 3, Enumerable.Range (1, 4) },
				{ 4, Enumerable.Range (1, 7) },
				{ 5, Enumerable.Range (1, 2) }
			};

			foreach (var actInfo in sceneRangesByActNumber.OrderBy (a => a.Key)) {
				var act = new Act (actInfo.Key, hamlet);
				foreach (var sceneNumber in actInfo.Value) {
					var scene = new Scene (sceneNumber, act);

					// We always create the same quiz here, but ideally we should
					// create a quiz specific to the given scene.
					var question1 = new Question (
						text: "Who is Hamlet?",
						answers: new List<string> {
							"The Prince of Norway",
							"The Prince of Denmark",
							"The Prince of Thieves"
						},
						correctAnswerIndex: 1
					);

					var question2 = new Question (
						text: "What is Hamlet's Quest?",
						answers: new List<string> {
							"To avenge his father's murder",
							"To put on a play",
							"To seek the Holy Grail"
						},
						correctAnswerIndex: 0
					);

					var question3 = new Question (
						text: "Where does Hamlet tell Ophelia to go?",
						answers: new List<string> {
							"A winery",
							"A nursery",
							"A nunnery"
						},
						correctAnswerIndex: 2
					);

					scene.Quiz = new Quiz (
						$"Act {act.Number} Scene {scene.Number} Quiz",
						new List<Question> { question1, question2, question3 },
						scene
					);
					act.Scenes.Add (scene);
				}
				hamlet.Acts.Add (act);
			}
			PlayLibrary.Shared.AddPlay (hamlet);

			return true;
		}

		public override bool ContinueUserActivity (UIApplication application, NSUserActivity userActivity, UIApplicationRestorationHandler completionHandler)
		{
			if (userActivity.IsClassKitDeepLink) {
				var identifierPath = userActivity.ContextIdentifierPath;

				// The first element of the identifier path is the main app context, which we don't need, so drop it.
				return NavigateTo (identifierPath.Skip (1).ToArray ());
			}

			return false;
		}

		public override bool OpenUrl (UIApplication app, NSUrl url, NSDictionary options)
		{
			Console.WriteLine (url);

			// Handle URLs with the `greatplays` scheme, and composed of node identifiers:
			// greatplays://play/act/scene/quiz

			var host = HttpUtility.UrlDecode (url.Host);
			if (!string.IsNullOrEmpty (host) && url.Scheme == "greatplays") {
				var identifierPath = url.PathComponents.Skip (1).Select (p => HttpUtility.UrlDecode (p)).ToList ();
				identifierPath.Insert (0, host);
				return NavigateTo (identifierPath.ToArray ());
			}

			return false;
		}

		bool NavigateTo (string [] identifierPath)
		{
			var identifier = identifierPath [0];
			var play = PlayLibrary.Shared.Plays.FirstOrDefault (p => p.Identifier == identifier);
			var node = play?.FindDescendant (identifierPath.AsEnumerable ().Reverse ().Take (identifierPath.Length - 1).Reverse ().ToList ());

			if (identifier is null || play is null || node is null)
				return false;

			switch (node) {
			case Play p:
				NavigateTo (p);
				break;
			case Act a:
				NavigateTo (a);
				break;
			case Scene s:
				NavigateTo (s);
				break;
			case Quiz q:
				NavigateTo (q);
				break;
			default:
				return false;
			}

			return true;
		}

		void NavigateTo (Play play)
		{
			var splitViewController = Window?.RootViewController as UISplitViewController;
			var masterNav = splitViewController?.ViewControllers?.FirstOrDefault () as UINavigationController;
			var detailNav = splitViewController?.ViewControllers?.LastOrDefault () as UINavigationController;
			var playList = masterNav?.ViewControllers?.FirstOrDefault () as PlaysTableViewController;

			if (splitViewController is null || masterNav is null || detailNav is null || playList is null)
				return;

			detailNav.DismissViewController (false, null);

			var row = playList.Plays?.FindIndex (p => p.Identifier == play.Identifier);
			if (row != null && row.HasValue)
				playList?.TableView?.SelectRow (NSIndexPath.FromRowSection (row.Value, 0), true, UITableViewScrollPosition.Top);

			detailNav.PopToRootViewController (true);
		}

		void NavigateTo (Act act)
		{
			NavigateTo (act.Play);

			var splitViewController = Window?.RootViewController as UISplitViewController;
			var detailNav = splitViewController?.ViewControllers?.LastOrDefault () as UINavigationController;
			var actsTable = detailNav?.ViewControllers?.LastOrDefault () as ActsTableViewController;
			var storyboard = actsTable?.Storyboard;
			var scenesTable = storyboard?.InstantiateViewController ("ScenesTableViewController") as ScenesTableViewController;

			if (splitViewController is null || detailNav is null || actsTable is null || storyboard is null || scenesTable is null)
				return;

			actsTable.Play = act.Play;
			scenesTable.Act = act;

			detailNav.PushViewController (scenesTable, false);
		}

		void NavigateTo (Scene scene)
		{
			NavigateTo (scene.Act);

			var splitViewController = Window?.RootViewController as UISplitViewController;
			var detailNav = splitViewController?.ViewControllers?.LastOrDefault () as UINavigationController;
			var scenesTable = detailNav?.ViewControllers?.LastOrDefault () as ScenesTableViewController;
			var storyboard = scenesTable?.Storyboard;
			var sceneView = storyboard?.InstantiateViewController ("SceneViewController") as SceneViewController;

			if (splitViewController is null || detailNav is null || scenesTable is null || storyboard is null || sceneView is null)
				return;

			sceneView.Scene = scene;
			detailNav.PushViewController (sceneView, false);
		}

		void NavigateTo (Quiz quiz)
		{
			NavigateTo (quiz.Scene);

			var splitViewController = Window?.RootViewController as UISplitViewController;
			var detailNav = splitViewController?.ViewControllers?.LastOrDefault () as UINavigationController;
			var sceneView = detailNav?.ViewControllers?.LastOrDefault () as SceneViewController;

			if (splitViewController is null || detailNav is null || sceneView is null)
				return;

			sceneView.PresentQuiz (quiz);
		}

		[Export ("splitViewController:collapseSecondaryViewController:ontoPrimaryViewController:")]
		public bool CollapseSecondViewController (UISplitViewController splitViewController, UIViewController secondaryViewController, UIViewController primaryViewController)
		{
			var secondaryAsNavController = secondaryViewController as UINavigationController;
			if (secondaryAsNavController is null)
				return false;

			var topAsDetailController = secondaryAsNavController?.TopViewController as ActsTableViewController;
			if (topAsDetailController is null)
				return false;

			if (topAsDetailController.Play is null)
				return true;

			return false;
		}
	}
}

