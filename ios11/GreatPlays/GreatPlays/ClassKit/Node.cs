using System;
using System.Collections.Generic;
using System.Linq;
using ClassKit;

namespace GreatPlays {

	public interface INode {
		INode Parent { get; }
		List<INode> Children { get; }
		string Identifier { get; }
		CLSContextType ContextType { get; }
	}

	public static class NodeIdentifiers_Extensions
	{
		public static List<string> GetIdentifierPath (this INode node)
		{
			var pathComponents = node?.Parent?.GetIdentifierPath ();
			if (pathComponents is null)
				pathComponents = new List<string> { node.Identifier };
			else
				pathComponents.Add (node.Identifier);

			return pathComponents;
		}

		public static INode FindDescendant (this INode node, List<string> identifierPath)
		{
			var identifier = identifierPath?.FirstOrDefault ();
			if (identifier != null) {
				var child = node?.Children?.FirstOrDefault (c => c.Identifier == identifier);
				if (child != null) {
					return child.FindDescendant (identifierPath.AsEnumerable ().Reverse ().Take (identifierPath.Count - 1).Reverse ().ToList ());
				}
				return null;
			}
			return node;
		}
	}

	public static class NodeActivity_Extensions {

		public static void BeginActivity (this INode node, bool asNew = false)
		{
			var identifierPath = node.GetIdentifierPath ();
			PrintActivity ("Start", identifierPath);

			CLSDataStore.Shared.MainAppContext.FindDescendantMatching (identifierPath.ToArray (), (context, err) => {
			context?.BecomeActive ();

				var activity = context?.CurrentActivity;
				if (asNew == false && activity != null)
					activity.Start ();
				else
					context?.CreateNewActivity ()?.Start ();

				CLSDataStore.Shared.Save (err2 => {
					if (err2 == null) return;
					Console.WriteLine ($"***Save error: {err2?.LocalizedDescription}");
				});
			});
		}

		public static void Update (this INode node, double progress)
		{
			var identifierPath = node.GetIdentifierPath ();
			CLSDataStore.Shared.MainAppContext.FindDescendantMatching (identifierPath.ToArray (), (context, err) => {
				var activity = context?.CurrentActivity;
				if (activity != null && progress > activity.Progress && activity.Started) {
					activity.AddProgressRange (0, progress);

					PrintActivity ("Progress", identifierPath, progress * 100);
				}
			});
		}

		public static void AddScore (this INode node, double score, string title, bool primary = false)
		{
			var identifierPath = node.GetIdentifierPath ();
			PrintActivity ("Score", identifierPath);

			CLSDataStore.Shared.MainAppContext.FindDescendantMatching (identifierPath.ToArray (), (context, err) => {
				var activity = context?.CurrentActivity;
				if (activity != null && activity.Started) {
					var item = new CLSScoreItem ("score", title, score, 1);
					if (primary)
						activity.PrimaryActivityItem = item;
					else
						activity.AddAdditionalActivityItem (item);
					Console.WriteLine ($"  => {title}: {item}");
				}
			});
		}

		public static void AddQuantity (this INode node, double quantity, string title, bool primary = false)
		{
			var identifierPath = node.GetIdentifierPath ();
			PrintActivity ("Quantity", identifierPath);

			CLSDataStore.Shared.MainAppContext.FindDescendantMatching (identifierPath.ToArray (), (context, err) => {
				var activity = context?.CurrentActivity;
				if (activity != null && activity.Started) {
					var item = new CLSQuantityItem ("quantity", title);
					if (primary)
						activity.PrimaryActivityItem = item;
					else
						activity.AddAdditionalActivityItem (item);
					Console.WriteLine ($"  => {title}: {quantity}");
				}
			});
		}

		public static void EndActivity (this INode node)
		{
			var identifierPath = node.GetIdentifierPath ();
			PrintActivity ("End", identifierPath);

			CLSDataStore.Shared.MainAppContext.FindDescendantMatching (identifierPath.ToArray (), (context, err) => {
				var activity = context?.CurrentActivity;
				if (activity != null) {
					Console.WriteLine ($"  => {activity.Duration} seconds elapsed.");

					activity.Stop ();
					context?.ResignActive ();
				}

				CLSDataStore.Shared.Save (err2 => {
					if (err2 == null) return;
					Console.WriteLine ($"***Save error: {err2?.LocalizedDescription}");
				});
			});
		}

		public static void PrintActivity (string activity, List<string> identifierPath, double? progress = null)
		{
			Console.WriteLine ();
			Console.Write ($"{activity}: [");
			for (int i = 0; i < identifierPath.Count; i++) {
				Console.Write ($"{identifierPath [i]}");
				if (i != identifierPath.Count - 1)
					Console.Write (", ");
			}
			if (progress is null)
				Console.WriteLine ("]");
			else
				Console.WriteLine ($"]: {progress.Value}%");
		}
	}
}
