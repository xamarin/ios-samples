using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace GreatPlays {
	public partial class SceneViewController : UIViewController, IUITextViewDelegate {

		Scene scene;
		public Scene Scene {
			get => scene;
			set {
				scene = value;
				NavigationItem.Title = scene?.Identifier ?? "Scene";
				if (scene?.Quiz != null)
					NavigationItem.RightBarButtonItem = new UIBarButtonItem ("Take Quiz", UIBarButtonItemStyle.Done, HandleTapQuiz);
				else
					NavigationItem.RightBarButtonItem = null;
			}
		}

		protected SceneViewController (IntPtr handle) : base (handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();
			sceneText.ContentOffset = CGPoint.Empty;
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			scene?.BeginActivity ();
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			scene?.EndActivity ();
		}

		[Export ("scrollViewDidScroll:")]
		public virtual void Scrolled (UIScrollView scrollView)
		{
			var position = sceneText.ContentOffset.Y + sceneText.Frame.Size.Height;
			var total = sceneText.ContentSize.Height;

			var progress = Math.Max (0, Math.Min (1, position / total));

			scene?.Update (progress);
		}

		void HandleTapQuiz (object sender, EventArgs e)
		{
			var quiz = Scene?.Quiz;
			if (quiz is null)
				return;
			PresentQuiz (quiz);
		}

		public void PresentQuiz (Quiz quiz)
		{
			var quizVC = Storyboard?.InstantiateViewController ("QuizViewController") as QuizViewController;
			if (quizVC is null)
				return;

			quiz.Reset ();
			quizVC.Quiz = quiz;
			PresentViewController (new UINavigationController (quizVC), true, quiz.Start);
		}
	}
}
