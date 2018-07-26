using System;
using System.Collections.Generic;
using Foundation;
using Intents;

namespace TasksNotesSiriIntent
{
    public partial class IntentHandler
    {
		public void HandleAppendToNote(INAppendToNoteIntent intent, Action<INAppendToNoteIntentResponse> completion)
		{
			throw new NotImplementedException();
		}

		public void HandleCreateNote(INCreateNoteIntent intent, Action<INCreateNoteIntentResponse> completion)
		{
			throw new NotImplementedException();
		}



		public void HandleSearchForNotebookItems(INSearchForNotebookItemsIntent intent, Action<INSearchForNotebookItemsIntentResponse> completion)
		{
			Console.WriteLine("HandleSearchForNotebookItems");
			throw new NotImplementedException();
		}
    }
}
