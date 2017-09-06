# Drag and Drop with Collection and Table View

Demonstrates how to adopt the drag and drop APIs of [`UICollectionView`](https://developer.apple.com/documentation/uikit/uicollectionview) and [`UITableView`](https://developer.apple.com/documentation/uikit/uitableview).

## Overview

This sample application, **PhotoAlbum**, displays a table view of photo albums and a collection view of photos in the selected album. Both the table view and collection view support drag and drop.

`UICollectionView` and `UITableView` each expose a `dragDelegate` and `dropDelegate`. You can set one or both of these delegates and implement methods in the corresponding protocols to support drag and drop, including reordering.

### Collection View

Photos can be dragged out of the collection view and moved to other albums within the app, or copied to other apps.

Photos can also be reordered within the same collection view.

Photos from another album can be dropped into the collection view to move them, and photos from other apps can be dropped to the collection view to copy them into the album.

### Table View

Dragging a row in the table view will begin a drag with the photos inside the corresponding album, which can be moved to another album within the app, or copied to other apps.

By tapping the Edit button, the albums in the table view can be reordered.

Photos from another album can be dropped into a row in the table view to move them into the album, and photos from other apps can be dropped into a row in the table view to copy them into the album.
