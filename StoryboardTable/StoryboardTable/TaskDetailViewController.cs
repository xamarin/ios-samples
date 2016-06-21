using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;

namespace StoryboardTable
{
	partial class TaskDetailViewController : UITableViewController
	{
		Chore currentTask {get;set;}
		public MasterViewController Delegate {get;set;} // will be used to Save, Delete later

		public TaskDetailViewController (IntPtr handle) : base (handle)
		{

		}

		// when displaying, set-up the properties
		public override void ViewDidLoad()
		{
			base.ViewDidLoad ();

			/*SaveButton.TouchUpInside += (sender, e) => {
				currentTask.Name = TitleText.Text;
				currentTask.Notes = NotesText.Text;
				currentTask.Done = DoneSwitch.On;
				Delegate.SaveTask(currentTask);
			};
			DeleteButton.TouchUpInside += (sender, e) => {
				Delegate.DeleteTask(currentTask);
			};*/
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			TitleText.Text = currentTask.Name;
			NotesText.Text = currentTask.Notes;
			DoneSwitch.On = currentTask.Done;
		}

		// this will be called before the view is displayed
		public void SetTask (MasterViewController d, Chore task) {
			Delegate = d;
			currentTask = task;
		}

		//create a New Task
		partial void SaveButtonTouchUpInside (UIButton sender)
		{
			currentTask.Name = TitleText.Text;
			currentTask.Notes = NotesText.Text;
			currentTask.Done = DoneSwitch.On;
			Delegate.SaveTask(currentTask);
		}

		//Delete a Task
		partial void DeleteButtonTouchUpInside (UIButton sender)
		{
			Delegate.DeleteTask(currentTask);
		}
	}
}
