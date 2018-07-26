# Mastering Drag and Drop

This sample code contains two projects that use the more advanced UIDragInteraction and UIDropInteraction APIs to customize drag and drop previews, animations, and other aspects of drag and drop.

## Overview

There are two sample apps in the sample code: DragSource and DropDestination. Both projects are included in a common workspace under the Source directory. DragSource uses drag interaction APIs to customize drag previews, drag items, and animations alongside lift and cancel animations. There are four self-contained scenarios in this sample app, each involving a different drag interaction. The first three scenarios exercise advanced APIs on UIDragInteraction, while the fourth exists for the purpose of testing delayed drop handling in DropDestination.

DropDestination uses drop destination APIs to customize the behavior of drops. There are two different use cases of UIDropInteraction, one of which handles long-running data loading on drop with custom progress UI, and the other of which is more limited in scope, and only handles drops local to the app.

In both sample apps, all logic relevant to drag and drop can be found in folder groups named "Drag and Drop" -- additional supporting views containing additional layout or interaction support can be found in folder groups labeled "Custom Views".

## Getting Started

Ensure that an iOS 11 or later SDK is available. Open Source/MasteringDragAndDrop.xcworkspace. Build and run DragSource.app and DropDestination.app using the DragSource and DropDestination schemes, respectively. To exercise the full flow of drag and drop from DragSource to DropDestination, installing and testing on an iPad is recommended, with both apps open simultaneously in split-screen mode.

## DragSource
DragSource consists of three main scenarios. Each scenarios corresponds to a view which is implemented in a separate Swift file. These scenarios are outlined below:

### DraggableStackedPhotosView

In our first scenario, observe that there are four `DraggableStackedPhotosViews`, each containing a number of image views. Beginning a drag on one of these views or tapping on one of these views after a drag session has started will generate new drag items per image view in the stack and add them to the drag session. This logic is run during both `dragInteraction(_:itemsForBeginning:)` and `dragInteraction(_:itemsForAddingTo:withTouchAt:)`.  Additionally, `dragInteraction(_:previewForLifting:session:)` is implemented to return a new `UITargetedDragPreview` using the `UIImageView` used to generate the corresponding `UIDragItem`. Observe that we determine this `UIImageView` by setting it on the `UIDragItem`'s `localObject` property when creating the item.

### DraggableQRCodeImageView

In the second scenario, we have an image view that detects QR codes in its image, computing a rect in image coordinates and a cropped image for each QR code that is detected. We vend an item for each QR code in our image using this cropped image, and in both `dragInteraction(_:previewForLifting:session:)` and `dragInteraction(_:previewForCancelling:withDefault:)`, we generate new `UITargetedDragPreview`s hosted in the image view's window. We also use `dragInteraction(_:willAnimateLiftWith:session:)`, `dragInteraction(_:willAnimateCancelWith:session:)` to fade the image view out during the lift and in during a cancel by using alongside animation blocks on the given `UIDragAnimating`. Lastly, we use `dragInteraction(_:session:didEndWith:)` as an opporunity to restore the view's alpha to 1, in the event that the drag ends without cancelling.

### DraggableLocationImageView

In the third scenario, we have an image view that is represented as a location, in the form of an `MKMapItem`. We register both the image and map item to a single item provider, which we use to generate and vend a new drag item when beginning the drag. The purpose of this scenario is to illustrate changing the drag item's `previewProvider` on the fly. When a user begins to move the drag image, we call into our implementation of `dragInteraction(_:sessionWillBegin:)`, which we use as a hook to set the `previewProvider` to a new block that generates a new `UIDragPreview` using a custom `LocationPlatterView`. Here, in our `UIDragPreviewParameters`, we set the `visiblePath` to an a rounded rect outsetted from the bounds of the `LocationPlatterView`, which gives the updated drag preview rounded margins.

### SlowDraggableImageView

This final scenario only implements a basic form of drag interactions already covered by previous scenarios. However, its item provider load is artificially delayed, simulating assets that take a significant amount of time to load, such as those arriving from remote sources like iCloud. This view is convenient for testing the behavior of our DropDestination app later; to tweak this artificial delay, change the `delay` argument when initialzing the `SlowDraggableImageView` in `ViewController.swift` in the DragSource project.

## DropDestination
DropDestination consists of views that implement both drag and drop interactions. Together, these interactions allow a user to (1) drop images into the main scroll view and have them populate a grid of image views, and (2) drag images and drop them into the area below the red line to remove them from the grid.

### DraggableImageView

A `DraggableImageView` is a `UIImageView` subclass that implements basic dragging functionality and vends exactly 1 `UIDragItem` if its image is non-nil. Its drag item's `localObject` is also `self`, allowing local drop targets (i.e. the `DroppableDeleteView`) to determine which view in the image gallery to remove on drop. This view exists to support drop-to-delete functionality in `DroppableDeleteView`. In addition, the `DraggableImageView` contains helper methods to show or hide a progress UI view over the image, given a `Progress`. This is used later on in `DroppableImageGridViewController` when handling animations for a drop.

### DroppableDeleteView

A `DroppableDeleteView` only supports drops from local drag sessions, returning a `UIDropProposal` with `UIDropOperation.forbidden` from `dropInteraction(_:sessionDidUpdate:)` if the drop session's `localDragSession` is nil. Otherwise, it returns `UIDropOperation.move`. On drop, we implement `dropInteraction(_:performDrop:)` to notify the `DeleteViewDelegate` that all of the local image views captured in the drag session's items (see `DraggableImageView` above) should be deleted. Additionally, `dropInteraction(_:previewForDropping:)` is overridden to retarget the default drag preview to the delete icon on the left side of the `DroppableDeleteView`, and also apply a transform to shrink its width and height dimensions down to 10% or the original preview dimensions.

### DroppableImageGridViewController

The `DroppableImageGridViewController` is an `ImageGridViewController` that inserts dropped images in its grid container view as `DraggableImageView`s, handling drops via custom progress UI rather than the default system modal dialog. In `dropInteraction(_:performDrop:)`, we begin loading an image from each item provider that is capable of loading an image. Simultaneously, we insert a new image view into the grid (note that `ImageGridViewController.nextView` already positions this inserted image in its final destination). We also update an `itemStates` dictionary mapping dropped `UIDragItem`s to their corresponding target `DraggableImageView`s and `Progress` objects returned by the item provider when beginning to load. Next, `dropInteraction(_:previewForDropping:withDefault:)` is invoked, which creates a new `UITargetedDragPreview` using a new `ProgressSpinnerView`, a custom view that observes changes in `Progress.fractionCompleted` and renders those changes using a spinner progress UI. This preview is targeted to rest at the same location as the final destination `DraggableImageView` for the drag item.

Next, `dropInteraction(_:item:willAnimateDropWith:)` completely hides the contents of the destination view until the drop is done animating, by setting the `alpha` of the destination `DraggableImageView` to be 0 as the drop animation is about to begin and instantly making it 1 when the animation is complete. We also begin showing progress UI (using the same `Progress` object) at the destination at this time. Even though the view is hidden when we begin showing progress, observe that when the drop animation ends and the `ProgressSpinnerView` in the targeted drop preview is removed, we will fade in the `DraggableImageView` at the destination to reveal either the final image if the item provider has already finished loading, or an identical-looking `ProgressSpinnerView` if the item provider is still loading. In the latter case, we will continue presenting the progress UI at the destination view until the item provider is done, at which point we remove the progress UI by calling `stopShowingProgress` and present the loaded `UIImage` by settig the image view's `image` property.
