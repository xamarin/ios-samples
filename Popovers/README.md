---
name: Xamarin.iOS - Popovers
description: This sample demonstrates proper use of UIPopoverController in iOS. UIPopoverController presentation, dismissing, and rotation handling are covered....
page_type: sample
languages:
- csharp
products:
- xamarin
urlFragment: popovers
---
# Popovers

This sample demonstrates proper use of UIPopoverController in iOS.
UIPopoverController presentation, dismissing, and rotation
handling are covered. The sample is provided using a UISplitViewController
in order to show proper handling of UIPopoverControllers being presented
from UIBarButtonItems. Additional handling ensures that multiple
UIPopoverControllers are never presented at the same time.

Popovers are only supported on the iPad.

![Popovers application screenshot](Screenshots/Popovers1.png "Popovers application screenshot")

## Build Requirements

iOS 4.1 or later

## Runtime Requirements

iOS 3.2 or later

## Packaging List

PopoversAppDelegate
The application delegate sets up the initial view and makes the window visible.

RootViewController
Acts as the master list view controller for the split view controller and adds rows of placeholder items to the table view.

DetailViewController
Displays the detail view of the split view controller. This also contains buttons and a bar button item that all present popovers.
The detail view responds to orientation changes when popovers are visible and re-displays them in the new orientation.
The detail view is also responsible for ensuring that there are never multiple popovers visible at the same time.

PopoverContentViewController
A view controller that is the contents of the popovers in this sample. In this example, the view controller only contains a label with text.

## License

Xamarin port changes are released under the MIT license
