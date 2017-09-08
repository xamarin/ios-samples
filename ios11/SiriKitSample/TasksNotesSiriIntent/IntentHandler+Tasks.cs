using System;
using System.Collections.Generic;
using Foundation;
using Intents;

namespace TasksNotesSiriIntent
{
    public partial class IntentHandler
    {
		/*
            https://developer.apple.com/documentation/sirikit/resolving_and_handling_intents/handling_an_intent
        */

		public void HandleAddTasks(INAddTasksIntent intent, Action<INAddTasksIntentResponse> completion)
		{
			Console.WriteLine("Add a task");
			var userActivity = new NSUserActivity("INAddTasksIntent"); // https://developer.apple.com/documentation/sirikit/resolving_and_handling_intents/confirming_the_details_of_an_intent

            var tasklist = TaskList.FromIntent(intent);

			//if tasks array is empty but a group name is specified, maybe convert it into a task?
			
            // TODO: have to create the list and tasks... in your app data store
			
            var response = new INAddTasksIntentResponse(INAddTasksIntentResponseCode.Success, userActivity)
            {
                AddedTasks = tasklist.Tasks,
			};
            if (intent.TargetTaskList != null)
            {
                response.ModifiedTaskList = tasklist;
            }
			completion(response);
		}

		/// <summary>
		/// Handles the create task list.
		/// </summary>
		/// <remarks>
		/// "Make a grocery list with apples, bananas, and pears in TasksNotes"
        /// </remarks>
        public void HandleCreateTaskList(INCreateTaskListIntent intent, Action<INCreateTaskListIntentResponse> completion)
		{
			Console.WriteLine("Create a task list");
			var userActivity = new NSUserActivity("INCreateTaskListIntent");
            var list = TaskList.FromIntent(intent);
            // TODO: have to create the list and tasks... in your app data store
            var response = new INCreateTaskListIntentResponse(INCreateTaskListIntentResponseCode.Success, userActivity)
            {
                CreatedTaskList = list
			};
			completion(response);
		}


		/// <summary>
		/// Handles the set task attribute.
		/// </summary>
		/// <remarks>
		/// "Mark buy iPhone as completed in TasksNotes"
		/// </remarks>
		public void HandleSetTaskAttribute(INSetTaskAttributeIntent intent, Action<INSetTaskAttributeIntentResponse> completion)
		{
			Console.WriteLine("Set task attribute");
			var userActivity = new NSUserActivity("INSetTaskAttributeIntent");

            var task = Task.FromIntent(intent);
			// TODO: have to actually confirm the task exists and then change its status... in your app data store
			task.Status = intent.Status;

            var response = new INSetTaskAttributeIntentResponse(INSetTaskAttributeIntentResponseCode.Success, userActivity)
            {
                ModifiedTask = task.ForResponse()
            };
			completion(response);
		}
    }
}
