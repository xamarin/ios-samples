using System;
using System.Linq;
using System.Collections.Generic;
using ClassKit;
using Foundation;

namespace GreatPlays {
	public partial class PlayLibrary : NSObject, ICLSDataStoreDelegate {

		void SetupClassKit () => CLSDataStore.Shared.Delegate = this;

		void SetupContext (Play play)
		{
			foreach (var act in play.Acts) {
				foreach (var scene in act.Scenes) {
					var path = scene.Quiz?.GetIdentifierPath () ?? scene.GetIdentifierPath ();
					CLSDataStore.Shared.MainAppContext.FindDescendantMatching (path.ToArray (), (context, error) => { });
				}
			}
		}

		public CLSContext CreateContext (string identifier, CLSContext parentContext, string [] parentIdentifierPath)
		{
			var identifierPath = parentIdentifierPath.ToList ();
			identifierPath.Add (identifier);

			var playIdentifier = identifierPath.FirstOrDefault ();
			var play = Shared.Plays?.FirstOrDefault (p => p.Identifier == playIdentifier);
			var node = play?.FindDescendant (identifierPath.AsEnumerable ().Reverse ().Take (identifierPath.Count - 1).Reverse ().ToList ());

			if (playIdentifier is null || play is null || node is null)
				return null;

			var context = new CLSContext (node.ContextType, identifier, node.Identifier) {
				Topic = CLSContextTopic.LiteracyAndWriting
			};

			LogContext (node.GetIdentifierPath ());
			return context;
		}

		public static void LogContext (List<string> identifierPath)
		{
			Console.WriteLine ();
			Console.Write ("Built context for [");
			for (int i = 0; i < identifierPath.Count; i++) {
				Console.Write ($"{identifierPath [i]}");
				if (i != identifierPath.Count - 1)
					Console.Write (", ");
			}
			Console.WriteLine ($"]");
		}
	}
}
