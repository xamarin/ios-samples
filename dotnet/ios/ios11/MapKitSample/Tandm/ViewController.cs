using System;
using Foundation;
using UIKit;
using MapKit;
using CoreLocation;

namespace Tandm;

public partial class ViewController : UIViewController
{
	#region Private Variables
	private CLLocationManager LocationManager = new CLLocationManager();
	#endregion

	#region Constructors
	protected ViewController(IntPtr handle) : base(handle)
	{
		// Note: this .ctor should not contain any initialization logic.
	}
	#endregion

	#region Private Methods
	private void SetupCompassButton() {

		var compass = MKCompassButton.FromMapView(MapView);
		compass.CompassVisibility = MKFeatureVisibility.Visible;
		NavigationItem.RightBarButtonItem = new UIBarButtonItem(compass);
		MapView.ShowsCompass = false;
	}

	private void SetupUserTrackingAndScaleView() {

		var button = MKUserTrackingButton.FromMapView(MapView);
		button.Layer.BackgroundColor = UIColor.FromRGBA(255,255,255,80).CGColor;
		button.Layer.BorderColor = UIColor.White.CGColor;
		button.Layer.BorderWidth = 1;
		button.Layer.CornerRadius = 5;
		button.TranslatesAutoresizingMaskIntoConstraints = false;
		View.AddSubview(button);

		var scale = MKScaleView.FromMapView(MapView);
		scale.LegendAlignment = MKScaleViewAlignment.Trailing;
		scale.TranslatesAutoresizingMaskIntoConstraints = false;
		View.AddSubview(scale);

		NSLayoutConstraint.ActivateConstraints(new NSLayoutConstraint[]{
			button.BottomAnchor.ConstraintEqualTo(View.BottomAnchor, -10),
			button.TrailingAnchor.ConstraintEqualTo(View.TrailingAnchor, -10),
			scale.TrailingAnchor.ConstraintEqualTo(button.LeadingAnchor, -10),
			scale.CenterYAnchor.ConstraintEqualTo(button.CenterYAnchor)
		});
	}

	private void RegisterAnnotationViewClasses() {
		MapView.Register(typeof(BikeView), MKMapViewDefault.AnnotationViewReuseIdentifier);
		MapView.Register(typeof(ClusterView), MKMapViewDefault.ClusterAnnotationViewReuseIdentifier);
	}

	private void LoadDataForMapRegionAndBikes()
	{
		// var plist = NSDictionary.FromFile(NSBundle.MainBundle.PathForResource("Data", "plist"));
		var plistFile = NSBundle.MainBundle.PathForResource("Data", "plist");

		var plistPath = Path.GetFullPath(plistFile);

		//var url = new NSUrl (plistPath);
		var url = NSUrl.FromFilename(plistPath);

		var urlCheck = url.CheckPromisedItemIsReachable(out var error1);
		Console.WriteLine(error1);

		//var plist = new NSDictionary (new NSUrl (plistPath), out _);
		var plist = NSDictionary.FromUrl(url, out var error);
		Console.WriteLine(error);

		// FIXME TJ
		if (plist is null)
			return;

		var region = plist["region"] as NSArray;
		if (region !=null) {
			var coordinate = new CLLocationCoordinate2D(region.GetItem<NSNumber>(0).NFloatValue, region.GetItem<NSNumber>(1).NFloatValue);
			var span = new MKCoordinateSpan(region.GetItem<NSNumber>(2).NFloatValue, region.GetItem<NSNumber>(3).NFloatValue);
			MapView.Region = new MKCoordinateRegion(coordinate, span);
		}
		var bikes = plist["bikes"] as NSArray;
		if (bikes !=null) {
			MapView.AddAnnotations(Bike.FromDictionaryArray(bikes));
		}
	}

	private MKAnnotationView HandleMKMapViewAnnotation(MKMapView mapView, IMKAnnotation annotation)
	{
		if (annotation is Bike) {
			var marker = annotation as Bike;

			var view = mapView.DequeueReusableAnnotation(MKMapViewDefault.AnnotationViewReuseIdentifier) as BikeView;
			if (view == null) {
				view = new BikeView(marker, MKMapViewDefault.AnnotationViewReuseIdentifier);
			}
			return view;
		}
		else if (annotation is MKClusterAnnotation) {
			var cluster = annotation as MKClusterAnnotation;

			var view = mapView.DequeueReusableAnnotation(MKMapViewDefault.ClusterAnnotationViewReuseIdentifier) as ClusterView;
			if (view == null) {
				view = new ClusterView(cluster, MKMapViewDefault.ClusterAnnotationViewReuseIdentifier);
			}
			return view;
		}
		else if (annotation != null) {
			var unwrappedAnnotation = MKAnnotationWrapperExtensions.UnwrapClusterAnnotation(annotation);

			return HandleMKMapViewAnnotation(mapView, unwrappedAnnotation);
		}
		return null;
	}
	#endregion

	#region Override Methods
	public override void ViewDidLoad()
	{
		base.ViewDidLoad();

		SetupCompassButton();
		SetupUserTrackingAndScaleView();
		RegisterAnnotationViewClasses();
		LoadDataForMapRegionAndBikes();

		MapView.GetViewForAnnotation = HandleMKMapViewAnnotation;
	}
	#endregion
}
