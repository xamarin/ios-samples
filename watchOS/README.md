watchOS 2 Preview
=================

Xamarin now has a [watchOS 2 preview](https://blog.xamarin.com/watchos-2-preview-and-updates/) - currently only for simulator build & test (will not work on devices).

1. You must add a new **WatchKit for watchOS 2** project to an iOS 9 solution.
2. Right-click the **Watch OS 2 app** project and select **Set as startup project**
3. The simulator list should change to display 38mm and 42mm watch options. If it doesn't change, restart Xamarin Studio and try again.
4. With a *watch simulator* selected, start debugging.


### Simulator Only

Not that this preview of Xamarin watchOS 2 support only works on the simulator. *You cannot compile for or deploy to devices.*

### Xamarin Studio Only

Preview does not work with Visual Studio at this time.

Preview Download / Requirements
--------------------------

* Xcode 7.0 or newer (currently in beta!)
* Xamarin Studio 5.10.0.786 or newer - a PREVIEW which must be downloaded separately ([download here](http://forums.xamarin.com/discussion/50055/watchos-2-preview))
* Xamarin.iOS 8.99 or newer - a PREVIEW which must be downloaded separately ([download here](http://forums.xamarin.com/discussion/50055/watchos-2-preview))
 