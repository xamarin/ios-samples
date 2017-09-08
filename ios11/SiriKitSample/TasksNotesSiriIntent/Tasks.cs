using System;
using System.Collections.Generic;
using Foundation;
using Intents;

namespace TasksNotesSiriIntent
{
    /// <summary>
    /// Helper class
    /// </summary>
    public class TaskList
    {
        //public TaskList()
        //{
        //}

        //public INSpeakableString GroupName { get; set; }
        //public INSpeakableString Title { get; set; }

        //public INSpatialEventTrigger Place { get; set; }
        //public INTemporalEventTrigger Time { get; set; }

        //public List<INTask> Tasks { get; set; }

        //public INTaskList ForResponse()
        //{
        //    return new INTaskList(Title, Tasks.ToArray(), GroupName, null, null, "id");
        //}

		public static INTaskList FromIntent(INCreateTaskListIntent intent)
		{
			var tasks = new List<INTask>();
			if (intent.TaskTitles != null)
			{
				foreach (var t in intent.TaskTitles)
				{
                    tasks.Add(new INTask(t, INTaskStatus.NotCompleted, INTaskType.Completable, null, null, null, null, "mytask"));
				}
			}
            return new INTaskList(intent.Title, tasks.ToArray(), intent.GroupName, null, null, "mylist");
		}

        public static INTaskList FromIntent (INAddTasksIntent intent)
        {
			var tasks = new List<INTask>();
            INSpeakableString title = new INSpeakableString("");
            INSpeakableString groupname = new INSpeakableString("");
			
			if (intent.TargetTaskList != null)
			{
                title = intent.TargetTaskList.Title;
			}
			if (intent.TaskTitles != null)
			{
				foreach (var t in intent.TaskTitles)
				{
					tasks.Add(new INTask (t, INTaskStatus.NotCompleted, INTaskType.Completable, null, null, null, null, "mytask"));
				}
			}
			
			return new INTaskList(title, tasks.ToArray(), groupname, null, null, "mylist");
        }
    }

	/// <summary>
	/// Helper class
	/// </summary>
	public class Task
    {
        public Task () 
        {
        }

        public INSpeakableString Title { get; set; }
        public INTaskStatus Status { get; set; } = INTaskStatus.NotCompleted;
        public INTaskType Type { get; set; } = INTaskType.Completable;

        public INTask ForResponse()
        {
            return new INTask(Title, Status, Type, null, null, null, null, "id");
        }

        public static Task FromIntent (INSetTaskAttributeIntent intent)
        {
            var task = new Task();
            task.Title = intent.TargetTask.Title;
            task.Status = intent.TargetTask.Status;
            task.Type = intent.TargetTask.TaskType;
            return task;
        }
    }
}
