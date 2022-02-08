namespace Tandm;

public partial class ViewController : UIViewController
{
	#region Private Variables
	CLLocationManager LocationManager = new CLLocationManager ();
	#endregion

	#region Constructors
	protected ViewController (IntPtr handle) : base (handle)
	{
		// Note: this .ctor should not contain any initialization logic.
	}
	#endregion

	#region Private Methods
	private void SetupCompassButton () {
		var compass = MKCompassButton.FromMapView (MapView);
		compass.CompassVisibility = MKFeatureVisibility.Visible;
		NavigationItem.RightBarButtonItem = new UIBarButtonItem (compass);
		MapView.ShowsCompass = false;
	}

	void SetupUserTrackingAndScaleView ()
	{
		if (View is null)
			throw new InvalidOperationException (nameof (View));

		var button = MKUserTrackingButton.FromMapView (MapView);
		button.Layer.BackgroundColor = UIColor.FromRGBA (255,255,255,80).CGColor;
		button.Layer.BorderColor = UIColor.White.CGColor;
		button.Layer.BorderWidth = 1;
		button.Layer.CornerRadius = 5;
		button.TranslatesAutoresizingMaskIntoConstraints = false;
		View.AddSubview (button);

		var scale = MKScaleView.FromMapView (MapView);
		scale.LegendAlignment = MKScaleViewAlignment.Trailing;
		scale.TranslatesAutoresizingMaskIntoConstraints = false;
		View.AddSubview (scale);

		NSLayoutConstraint.ActivateConstraints (new NSLayoutConstraint[] {
			button.BottomAnchor.ConstraintEqualTo (View.BottomAnchor, -10),
			button.TrailingAnchor.ConstraintEqualTo (View.TrailingAnchor, -10),
			scale.TrailingAnchor.ConstraintEqualTo (button.LeadingAnchor, -10),
			scale.CenterYAnchor.ConstraintEqualTo (button.CenterYAnchor)
		});
	}

	void RegisterAnnotationViewClasses () {
		MapView.Register (typeof (BikeView), MKMapViewDefault.AnnotationViewReuseIdentifier);
		MapView.Register (typeof (ClusterView), MKMapViewDefault.ClusterAnnotationViewReuseIdentifier);
	}

	void LoadDataForMapRegionAndBikes ()
	{
		var plistFile = NSBundle.MainBundle.PathForResource ("Data", "plist");
		var plistPath = Path.GetFullPath (plistFile);
		var url = NSUrl.FromFilename (plistPath);

		if (NSDictionary.FromUrl (url, out var err) is NSDictionary plist) {
			if (err is not null)
				throw new Exception ($"Error forming NSDictionary from Data.plist: {err.LocalizedDescription}");

			var region = plist["region"] as NSArray;
			if (region is not null) {
				var coordinate = new CLLocationCoordinate2D (region.GetItem<NSNumber> (0).NFloatValue, region.GetItem<NSNumber> (1).NFloatValue);
				var span = new MKCoordinateSpan (region.GetItem<NSNumber> (2).NFloatValue, region.GetItem<NSNumber> (3).NFloatValue);
				MapView.Region = new MKCoordinateRegion (coordinate, span);
			}
			var bikes = plist["bikes"] as NSArray;
			if (bikes is not null) {
				MapView.AddAnnotations (Bike.FromDictionaryArray (bikes));
			}
		} else {
			throw new InvalidOperationException ("plist could not be created from Data.plist");
		}
	}

	MKAnnotationView HandleMKMapViewAnnotation (MKMapView mapView, IMKAnnotation annotation)
	{
		switch (annotation){
			case Bike marker:
				return mapView.DequeueReusableAnnotation (MKMapViewDefault.AnnotationViewReuseIdentifier) ?? new BikeView (marker, MKMapViewDefault.AnnotationViewReuseIdentifier);
			case MKClusterAnnotation cluster:
				return mapView.DequeueReusableAnnotation (MKMapViewDefault.ClusterAnnotationViewReuseIdentifier) ?? new ClusterView (cluster, MKMapViewDefault.ClusterAnnotationViewReuseIdentifier);
			default:
				var unwrappedAnnotation = MKAnnotationWrapperExtensions.UnwrapClusterAnnotation (annotation);
				return HandleMKMapViewAnnotation (mapView, unwrappedAnnotation);
		};
	}
	#endregion

	#region Override Methods
	public override void ViewDidLoad ()
	{
		base.ViewDidLoad ();

		SetupCompassButton ();
		SetupUserTrackingAndScaleView ();
		RegisterAnnotationViewClasses ();
		LoadDataForMapRegionAndBikes ();

		MapView.GetViewForAnnotation = HandleMKMapViewAnnotation;
	}
	#endregion
}
