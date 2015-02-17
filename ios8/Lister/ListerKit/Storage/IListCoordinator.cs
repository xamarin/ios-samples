using System;
using Foundation;

namespace ListerKit
{
	// TODO: add comments
	public interface IListCoordinator
	{
		IListCoordinatorDelegate Delegate { get; set; }

		void StartQuery ();

		void StopQuery ();

		void RemoveListAtUrl(NSUrl url);

		void CreateUrlForList(List list, string name);

		bool CanCreateListWithName(string name);
	}
}

