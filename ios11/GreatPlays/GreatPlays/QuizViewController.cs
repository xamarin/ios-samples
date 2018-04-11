using System;
using Foundation;
using UIKit;

namespace GreatPlays {
	public partial class QuizViewController : UITableViewController {

		Quiz quiz;
		public Quiz Quiz {
			get => quiz;
			set {
				quiz = value;
				if (Quiz is null)
					return;

				if (quiz.IsOver) {
					NavigationItem.Title = $"{quiz.Title} Results";
					NavigationItem.RightBarButtonItem = new UIBarButtonItem (UIBarButtonSystemItem.Done, TapDoneHandler);
					TableView.AllowsSelection = false;

					quiz.Record ();
				} else {
					NavigationItem.Title = $"{quiz.Title} Question {quiz.QuestionIndex + 1}";
					NavigationItem.RightBarButtonItem = new UIBarButtonItem ("Hint", UIBarButtonItemStyle.Plain, TapHintHandler);
				}

				NavigationItem.HidesBackButton = true;
			}
		}

		protected QuizViewController (IntPtr handle) : base (handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}

		public override nint NumberOfSections (UITableView tableView) => 1;

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			if (Quiz is null)
				return 0;
			return Quiz.IsOver ? quiz.Questions.Count : Quiz.CurrentQuestion.Answers.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell ("QuizCellIdentifier", indexPath);
			if (Quiz is null)
				return cell;

			if (Quiz.IsOver) {
				var question = Quiz.Questions [indexPath.Row];

				cell.TextLabel.Text = question.Text;
				cell.DetailTextLabel.Text = question.Response;

				cell.DetailTextLabel.TextColor = question.IsCorrect ? UIColor.Green : UIColor.Red;
			} else {
				var ascii = indexPath.Row + 65;
				var letter = Char.ConvertFromUtf32 (ascii);

				cell.TextLabel.Text = $"{letter}. {Quiz.CurrentQuestion.Answers [indexPath.Row]}";
				cell.DetailTextLabel.Text = string.Empty;
			}

			return cell;
		}

		public override UIView GetViewForHeader (UITableView tableView, nint section)
		{
			var view = new UILabel ();
			if (Quiz is null)
				return view;

			view.Text = quiz.IsOver ? $"Your Score: {Quiz.CorrectCount} / {Quiz.Questions.Count}" : Quiz.CurrentQuestion.Text;
			view.TextAlignment = UITextAlignment.Center;
			view.Lines = 0;
			view.Font = UIFont.SystemFontOfSize (32, UIFontWeight.Light);

			return view;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			if (Quiz is null)
				return;
			Quiz.SetAnswer (indexPath.Row);

			var quizVC = Storyboard?.InstantiateViewController ("QuizViewController") as QuizViewController;
			if (quizVC != null) {
				quizVC.Quiz = Quiz;
				NavigationController.PushViewController (quizVC, true);
			}
		}

		void TapDoneHandler (object sender, EventArgs e) => DismissViewController (true, null);

		void TapHintHandler (object sender, EventArgs e)
		{
			var button = sender as UIBarButtonItem;
			if (Quiz is null || button is null)
				return;

			button.Enabled = false;

			Quiz.CurrentQuestion.Hints += 1;

			var hintIndex = (quiz.CurrentQuestion.CorrectAnswerIndex + 1) % quiz.CurrentQuestion.Answers.Count;
			var indexPath = NSIndexPath.FromRowSection (hintIndex, 0);

			var cell = TableView.CellAt (indexPath);
			if (cell != null) {
				UIView.Animate (0.2, () => cell.Alpha = 0, () => {
					UIView.Animate (0.2, 1, UIViewAnimationOptions.CurveEaseInOut, () => cell.Alpha = 1, () => button.Enabled = true);
				});
			}
		}
	}
}

