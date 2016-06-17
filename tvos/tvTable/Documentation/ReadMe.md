# tvTable

The tvTable sample app presents a collection of Cities, each with a collection of Attractions, in a Table View inside of a Split View Controller.

The table of Cities and Attractions is presented as the Master view of the Split View. When an Attraction is highlighted in the Table, the details of the Attraction will be displayed in the Details view of the Split View.

This is the typical usage pattern for Table Views in a tvOS app according to Apple.

# Implementation

The following sections will briefly discuss the key source code components and how they were used to implement tvTable's functionality.

## Generic Classes

The following generic classes are defined:

* **AlertViewController.cs** - Provides a [helper class](https://developer.xamarin.com/guides/ios/tvos/user-interface/alerts/#Alert_View_Controller_Helper_Class) to make working with Alerts easier in a Xamarin.tvOS app.
* **AttractionInformation.cs** - Holds information about a given attraction that can be attached to a City. Attractions are displayed as individual rows in the Table View.
* **AttractionTableDatasource.cs** - Provides the data for the attraction table as a collection of `CityInformation` and `AttractionInformation` objects.
* **AttractionTableDelegate.cs** - Handles user interaction with the Attraction Table.


## Assets.xcassets

Provides the [images and icons](https://developer.xamarin.com/guides/ios/tvos/application-fundamentals/icons-images/) used in tvTable. This includes the **Launch Image**, **Top Shelf Image** and **App Icons** required by Apple.

## App, Windows and Controllers

The following classes provide app support, View and View Controller Support in the application:

* **AppDelegate.cs** - The [AppDelegate.cs](https://developer.xamarin.com/guides/ios/tvos/getting-started/hello-tvos/#AppDelegate.cs) file contains the AppDelegate class, which is responsible for creating the window and listening to OS events.
* **AtractionTableCell.cs** - Defines the prototype Cell used to display the rows of `AttractionInformation` in the Attraction Table.
* **AttractionTableView.cs** - Defines the Table View used to present the Attraction Table.
* **AttractionTableViewController.cs** - Controls the Attraction Table View, initializes it with data and attaches the Attraction Table Delegate used to respond to user events.
* **AttractionView.cs** - The View presented as the Detail view in the Split View used to present the details of the highlighted row in the Attraction Table.
* **AttractionViewController.cs** - Controls the Attraction View and updates it with the current `AttractionInformation` object attached to the currently highlighted row in the Attraction Table.
* **Entitlements.plist** - Defines any entitlements for the app such as iCloud support.
* **Info.plist** - The `Info.plist` file defines information for the app such as its Title, Type and Icon.
SourceWriter is released under the [MIT License](#License).
* **Main.cs** - The [`Main.cs`](https://developer.xamarin.com/guides/ios/tvos/getting-started/hello-tvos/#Main.cs) file starts the App.
* **Main.storyboard** - This Storyboard defines the User Interface for the app. 
* **MasterNavigationController.cs** - Controls the Navigation for the Master side of the Split View that houses the Attraction Table.
* **MasterSplitView.cs** - Controls the Split View that houses the Master and Details views and provides shortcuts to allow the Master and Detail views to communicate.

For more detailed descriptions, please see the [API Documentation](html/index.html) for the SourceWriter source code.