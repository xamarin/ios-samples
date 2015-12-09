Lister
==============

Lister is a productivity sample code project for iOS. In this sample,
the user can create lists, add items to lists, and track the progress
of items in the lists.  The solution includes iOS app and iOS Today
extensions app (Widget).

In sample, the user manages multiple lists using a table view
implemented in the `DocumentsViewController` class. In addition to
vending rows in the table view, the list documents controller observes
changes to the lists, as well as the status of `iCloud`. Tapping on a
list brings the user to the `ListViewController`. This class displays
and manages a single document. The `NewListDocumentController` class
allows a user to create a new list.

The `ListCoordinator` class tracks the user's storage choice — local
or `iCloud` and moves the user's documents between the two storage
locations. The `ListDocument` class, a subclass of `UIDocument`,
represents an individual list document that is responsible for
serialization and deserialization.

`List` and `ListItem` are the two main model objects. The `ListItem`
class represents a single item in a list. It contains only three
stored properties: the text of the item, a Boolean indicating whether
the user has completed it, and a unique identifier. Along with these
properties, the `ListItem` class also implements the functionality
required to compare, archive, and unarchive a `ListItem` object. The
`List` class holds an array of these `ListItem` objects, as well as
the color the desired list color. The `List` class also supports
indexer, archiving and unarchiving.

In addition to model code, by subclassing `CALayer`, Lister shares
checkbox drawing code with both app and it's extension.

References
----------

For understanding Widgets (Today extension) read [Apple's
documentation](https://developer.apple.com/library/prerelease/mac/documentation/General/Conceptual/ExtensibilityPG/NotificationCenter.html#//apple_ref/doc/uid/TP40014214-CH11-SW1)

Setup
-----

For this sample you need create ApplicationID and setup
iCloudContainer for it. For complete instructions read `Enabling
iCloud in Xamarin` section from
[tutorial](http://developer.xamarin.com/guides/ios/platform_features/introduction_to_the_document_picker/).

Instructions
------------

Before running the app you shoud signin to your `iCloud` account via
`Settings` application. Also enable `iCloud Drive`.  After that run
the app.

During launching process application will copy resource files (which
represents serialized `List` objects) to `Documents` folder within
sandbox.

You will be promted about storage choice (`iCloud` or local). You will
not be able to change you choice in future, so please choose
`iCloud`. You need `iCloud` storage, because this is a way to share
data between application and it's extension.

When you choose `iCloud` app will move resource files (`*.lister`)
from `Documents` folder to `iCloud` container. After that these files
will available to `ListerToday` extension app (`Widget`).
`ListerToady` (Widget) displays only items from `Today` list, because
of this add some items to this list. Now you are ready to run your
widget.

Go to `Today view` (`Notification center`). Here you will see built in
widgets like `Calendar`, `Remainders`, etc. Now we need to enable
`ListerLoday` Widget (add it to `Today view`). At the button you will
see `Edit` button. Tap on it, then enable `ListerToday` widget and
press `Done`. At this point you will see `Today view` with
`ListerToday` widget, also you will see your Today items.

When you change your `Today list` within Lister app these changes will
be displayed inside ListerToday Widget and vice versa.

Build Requirements
------------------

Building this sample requires Xcode 6.0 and iOS 8.0 SDK

Target
------
This sample runnable on iPhone/iPad

Author
------ 
IOS:
Copyright (C) 2014 Xamarin Inc. All rights reserved.

Ported to Xamarin.iOS by Rustam Zaitov
