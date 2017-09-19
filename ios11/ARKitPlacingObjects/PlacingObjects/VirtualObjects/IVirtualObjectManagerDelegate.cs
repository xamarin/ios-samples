namespace PlacingObjects
{
	public interface IVirtualObjectManagerDelegate
	{
		void WillLoad(VirtualObjectManager manager, VirtualObject virtualObject);
		void DidLoad(VirtualObjectManager manager, VirtualObject virtualObject);
		void CouldNotPlace(VirtualObjectManager manager, VirtualObject virtualObject);
	}

	static class VirtualObjectManagerDelegate_Extensions
	{
		public static void TransformDidChangeFor(this IVirtualObjectManagerDelegate self, VirtualObjectManager manager, VirtualObject virtualObject) { }
		public static void DidMoveObjectOntoNearbyPlane(this IVirtualObjectManagerDelegate self, VirtualObjectManager manager, VirtualObject virtualObject) { }
	}
}
