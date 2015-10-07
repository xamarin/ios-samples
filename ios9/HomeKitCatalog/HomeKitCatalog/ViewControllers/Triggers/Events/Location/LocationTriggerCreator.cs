using System.Collections.Generic;

using CoreLocation;
using HomeKit;

namespace HomeKitCatalog
{
	// An `EventTriggerCreator` subclass which allows for the creation of location triggers.
	public class LocationTriggerCreator : EventTriggerCreator, IMapViewControllerDelegate
	{
		HMLocationEvent LocationEvent { get; set; }

		public CLCircularRegion TargetRegion { get; set; }

		public int TargetRegionStateIndex { get; set; }

		public LocationTriggerCreator (HMTrigger trigger, HMHome home)
			: base (trigger, home)
		{
			var eventTrigger = EventTrigger;
			if (eventTrigger != null) {
				LocationEvent = eventTrigger.LocationEvent ();
				if (LocationEvent != null) {
					TargetRegion = LocationEvent.Region as CLCircularRegion;
					TargetRegionStateIndex = TargetRegion.NotifyOnEntry ? 0 : 1;
				}
			}
		}

		protected override void UpdateTrigger ()
		{
			var region = TargetRegion;
			if (region != null) {
				PrepareRegion ();
				var locationEvent = LocationEvent;
				if (locationEvent != null) {
					SaveTriggerGroup.Enter ();
					locationEvent.UpdateRegion (region, error => {
						if (error != null)
							Errors.Add (error);
						SaveTriggerGroup.Leave ();
					});
				}
			}

			SavePredicate ();
		}

		protected override HMTrigger NewTrigger ()
		{
			var events = new List<HMLocationEvent> ();
			var region = TargetRegion;
			if (region != null) {
				PrepareRegion ();
				events.Add (new HMLocationEvent (region));
			}
			return new HMEventTrigger (Name, events.ToArray (), NewPredicate ());
		}

		#region Helper Methods

		void PrepareRegion ()
		{
			var region = TargetRegion;
			if (region != null) {
				region.NotifyOnEntry = (TargetRegionStateIndex == 0);
				region.NotifyOnExit = !region.NotifyOnEntry;
			}
		}

		#endregion

		#region IMapViewControllerDelegate implementation

		public void MapViewDidUpdateRegion (CLCircularRegion region)
		{
			TargetRegion = region;
		}

		#endregion
	}
}