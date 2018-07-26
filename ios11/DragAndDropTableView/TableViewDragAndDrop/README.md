# Adopting Drag and Drop in a Table View

Demonstrates how to enable and implement drag and drop for a table view.

## Overview
This sample code project uses a [`UITableView`](https://developer.apple.com/documentation/uikit/uitableview) instance to show how to make a table view into a drag source and a drop destination. 

To enable drag and drop, you specify the table view as its own drag delegate and drop delegate. To provide or consume data, you implement the drag and drop delegate methods.

Adopting drag and drop in a table view differs in some important ways compared to the process you follow for a custom view. To compare the steps, see [Adopting Drag and Drop in a Custom View](https://developer.apple.com/documentation/uikit/drag_and_drop/adopting_drag_and_drop_in_a_custom_view).

## Get Started
Deploy this project on iPad, which supports drag and drop between apps. When you first launch this project’s built app, you see a table with several rows, each with a text string. Use this app along with a second app that supports editing of text strings, such as Reminders. For example, configure the iPad screen to Split View, with this app side by side with Reminders. Then drag a row from this app into Reminders, or drag a reminder into this app.

This app also supports rearranging rows in the table by dragging a row up or down. However, rearrangement in this app uses the traditional [`tableView(canMoveRowAt:)`](https://developer.apple.com/documentation/uikit/uitableviewdatasource/1614927-tableview) and [`tableView(moveRowAt:to:)`](https://developer.apple.com/documentation/uikit/uitableviewdatasource/1614867-tableview) methods rather than the drag and drop API.

## Enable Drag and Drop Interactions
To enable dragging, dropping, or both, specify a table view as its own drag or drop delegate. A convenient place for this code is in an app’s [`viewDidLoad()`](https://developer.apple.com/documentation/uikit/uiviewcontroller/1621495-viewdidload) method. This code enables both dragging and dropping:

``` swift
override func viewDidLoad() {
    super.viewDidLoad()
    
    tableView.dragDelegate = self
    tableView.dropDelegate = self
}
```

Unlike a custom view, a table view does not have an `interactions` property to which you add interactions. Instead, a table view uses a drag delegate and a drop delegate directly.

## Provide Data for a Drag Session
To provide data for dragging from a table view, implement the [`tableView(_:itemsForBeginning:at:)`](https://developer.apple.com/documentation/uikit/uitableviewdragdelegate/2897492-tableview) method. Here’s the model-agnostic portion of this code:

``` swift
func tableView(_ tableView: UITableView, itemsForBeginning session: UIDragSession, at indexPath: IndexPath) -> [UIDragItem] {
    return model.dragItems(for: indexPath)
}
```

The following helper function, used by the `tableView(_:itemsForBeginning:at:)` method, serves as an interface to the data model in this sample code project:

``` swift
func dragItems(for indexPath: IndexPath) -> [UIDragItem] {
    let placeName = placeNames[indexPath.row]

    let data = placeName.data(using: .utf8)
    let itemProvider = NSItemProvider()
    
    itemProvider.registerDataRepresentation(forTypeIdentifier: kUTTypePlainText as String, visibility: .all) { completion in
        completion(data, nil)
        return nil
    }

    return [
        UIDragItem(itemProvider: itemProvider)
    ]
}
```

## Consume Data from a Drop Session
To consume data from a drop session in a table view, you implement three delegate methods.

First, your app can refuse the drag items based on their class, the state of your app, or other requirements. This project’s implementation allows a user to drop only instances of the [`NSString`](https://developer.apple.com/documentation/foundation/nsstring) class. Here is the model-agnostic portion of this code:

``` swift
func tableView(_ tableView: UITableView, canHandle session: UIDropSession) -> Bool {
    return model.canHandle(session)
}
```

The following helper function, used by the [`tableView(_:canHandle:)`](https://developer.apple.com/documentation/uikit/uitableviewdropdelegate/2897434-tableview) method, serves as the interface to the data model:

``` swift
func canHandle(_ session: UIDropSession) -> Bool {
    return session.canLoadObjects(ofClass: NSString.self)
}
```

Second, you must tell the system how you want to consume the data, which is typically by copying it. You specify this choice by way of a drop proposal:

``` swift
func tableView(_ tableView: UITableView, dropSessionDidUpdate session: UIDropSession, withDestinationIndexPath destinationIndexPath: IndexPath?) -> UITableViewDropProposal {
    // The .move operation is available only for dragging within a single app.
    if tableView.hasActiveDrag {
        if session.items.count > 1 {
            return UITableViewDropProposal(operation: .cancel)
        } else {
            return UITableViewDropProposal(operation: .move, intent: .insertAtDestinationIndexPath)
        }
    } else {
        return UITableViewDropProposal(operation: .copy, intent: .insertAtDestinationIndexPath)
    }
}
```

Finally, after the user lifts their finger from the screen, indicating their intent to drop the drag items, your table view has one opportunity to request particular data representations of the drag items:

``` swift
/**
     This delegate method is the only opportunity for accessing and loading
     the data representations offered in the drag item. The drop coordinator
     supports accessing the dropped items, updating the table view, and specifying
     optional animations. Local drags with one item go through the existing
     `tableView(_:moveRowAt:to:)` method on the data source.
*/
func tableView(_ tableView: UITableView, performDropWith coordinator: UITableViewDropCoordinator) {
    let destinationIndexPath: IndexPath
    
    if let indexPath = coordinator.destinationIndexPath {
        destinationIndexPath = indexPath
    } else {
        // Get last index path of table view.
        let section = tableView.numberOfSections - 1
        let row = tableView.numberOfRows(inSection: section)
        destinationIndexPath = IndexPath(row: row, section: section)
    }
    
    coordinator.session.loadObjects(ofClass: NSString.self) { items in
        // Consume drag items.
        let stringItems = items as! [String]
        
        var indexPaths = [IndexPath]()
        for (index, item) in stringItems.enumerated() {
            let indexPath = IndexPath(row: destinationIndexPath.row + index, section: destinationIndexPath.section)
            self.model.addItem(item, at: indexPath.row)
            indexPaths.append(indexPath)
        }

        tableView.insertRows(at: indexPaths, with: .automatic)
    }
}
```

In addition to these three methods, drag and drop offers additional API hooks for customizing your adoption of this feature for table views. For more about providing and consuming data, see [Supporting Drag and Drop in Table Views](https://developer.apple.com/documentation/uikit/views_and_controls/table_views/supporting_drag_and_drop_in_table_views).
