RegionDefiner
=============

This is a MonoTouch port of the WWDC2012 sample of the same name
showing how to get around with MapKit.

Description:
============

This sample is an example to generate GeoJSON coverage files for use
with the Maps routing apps API.


RegionDefiner
=============

Sample app opens with MapView, Reset and Log Button.

On longpressing any point on the map an annotation is added as pin
point with map coordinate information.  A multipolygon is defined when
3 or more pinpoints are added to the map. On pressing reset button,
the annotations and region defined are erased.  Log button logs the
coordinate information. Logs can be checked in Device Log.


Packaging List:

AppDelegate
- A basic UIApplication delegate which sets up the application.

ViewController
- A UIViewController subclass which shows a map

MyAnnotation
- Represents a point on the map

Changes From Previous Version:
 
Version 1.0
- First version.

Ported By: GouriKumari
 








