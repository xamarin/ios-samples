
namespace XamarinShot.Models.Interactions {
	using SceneKit;

	public interface IGrabbable {
		int GrabbableId { get; set; }
		Player Player { get; set; }
		bool IsGrabbed { get; set; }

		bool IsVisible { get; set; }
		bool IsHighlighted { get; }
		void DoHighlight (bool show, SFXCoordinator sfxCoordinator);

		bool CanGrab (Ray cameraRay);
		float DistanceFrom (SCNVector3 worldPos);

		void Move (CameraInfo cameraInfo);
	}
}
