using System;
using System.Collections.Generic;
using Foundation;
using Intents;

namespace TasksNotesSiriIntent {
	// As an example, this class is set up to handle Tasks and Lists intents.
	//
	// The intents you wish to handle must be declared in the extension's Info.plist.

	[Register ("IntentHandler")]
	public partial class IntentHandler : INExtension, IINNotebookDomainHandling {
		/*  The IINNotebookDomainHandling interface includes all the following, which you could alternatively implement seperately:
		    - IINAddTasksIntentHandling, 
            - IINAppendToNoteIntentHandling, 
            - IINCreateNoteIntentHandling, 
            - IINCreateTaskListIntentHandling, 
            - IINSearchForNotebookItemsIntentHandling, 
            - IINSetTaskAttributeIntentHandling

            Refer to the partial classes:
            - IntentHandler+Notes.cs
            - IntentHandler+Tasks.cs
            for the implmementations of these interfaces
            */

		protected IntentHandler (IntPtr handle) : base (handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override NSObject GetHandler (INIntent intent)
		{
			// This is the default implementation.  If you want different objects to handle different intents,
			// you can override this and return the handler you want for that particular intent.
			return this;
		}
	}
}
