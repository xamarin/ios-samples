# Placing Virtual Objects in Augmented Reality

Learn best practices for visual feedback, gesture interactions, and realistic rendering in AR experiences, as well as tips for building SceneKit-based AR apps.

## Overview

Augmented reality offers new ways for users to interact with real and virtual 3D content in your app. However, many of the fundamental principles of human interface design are still valid. Convincing AR illusions also require careful attention to 3D asset design and rendering. By following this article's guidelines for AR human interface principles and experimenting with this example code, you can create immersive, intuitive augmented reality experiences.

## Getting Started

ARKit is available on any iOS 11 device, but the world tracking features that enable high-quality AR experiences require a device with the A9 or later processor.

## Feedback

**Help users recognize when your app is ready for real-world interactions.**
Tracking the real-world environment involves complex algorithms whose timeliness and accuracy are affected by real-world conditions.

The `FocusSquare` class in this example project draws a square outline in the AR view, giving the user hints about the status of ARKit world tracking. The square changes size to reflect estimated scene depth, and switches between open and closed states with a "lock" animation to indicate whether ARKit has detected a plane suitable for placing an object.

Use the [`session(_:cameraDidChangeTrackingState:)`](https://developer.apple.com/documentation/arkit/arsessionobserver/2887450-session) delegate method to detect changes in tracking quality, and present feedback to the user when low-quality conditions are correctable (for example, by telling the user to move to an environment with better lighting).

Use specific terms a user is likely to recognize. For example, if you give textual feedback for plane detection, a user not familiar with technical definitions might mistake the word "plane" as referring to aircraft.

Fall back gracefully if tracking fails, and allow the user to reset tracking if their experience isn't working as expected. See the `restartExperience` button and method in this example's `ViewController` class. The `use3DOFTrackingFallback` variable controls whether to switch to a lower-fidelity session configuration when tracking quality is poor.

**Help users understand the relationship of your app's virtual content to the real world.** Use visual cues in your UI that react to changes in camera position relative to virtual content.

The focus square disappears after the user places an object in the scene, and reappears when the user points the camera away from the object.

The `Plane` class in this example handles visualization of real-world planes detected by ARKit. Its `createOcclusionNode` and `updateOcclusionNode` methods create invisible geometry that realistically obscures virtual content.

## Direct Manipulation

**Provide common gestures, familiar to users of other iOS apps, for interacting with real-world objects.** See the `Gesture` class in this example for implementations of the gestures available in this example app, such as one-finger dragging to move a virtual object and two-finger rotation to spin the object.

Map touch gestures into a restricted space so the user can more easily control results. Touch gestures are inherently two-dimensional, but an AR experience involves the three dimensions of the real world. For example:

- Limit object dragging to the two-dimensional plane the object rests on. (Especially if a plane represents the ground or floor, it often makes sense to ignore the plane's extent while dragging.)

- Limit object rotation to a single axis at a time. (In this example, each object rests on a plane, so the object can rotate around a vertical axis.)

- Don't allow the user to resize virtual objects, or offer this ability only sparingly. A virtual object inhabits the real world more convincingly when it has an intuitive intrinsic size. Additionally, a user may become confused as to whether they're resizing an object or changing its depth relative to the camera. (If you do provide object resizing, use pinch gestures.)

While the user is dragging a virtual object, smooth the changes in its position so that it doesn't appear to jump while moving. See the `updateVirtualObjectPosition` method in this example's `ViewController` class for an example of smoothing based on perceived distance from the camera.

Set thresholds for gestures so that the user doesn't trigger a gesture accidentally, but moderate your thresholds so that gestures aren't too hard to discover or intentionally trigger. See the `TwoFingerGesture` class for examples of using thresholds to dynamically choose gesture effects.

Provide a large enough area where the user can tap (or begin a drag) on a virtual object, so that they can still see the object while moving it. See how the `firstTouchWasOnObject` boolean is computed in the `TwoFingerGesture` class for examples.

**Design interactions for situations where AR illusions can be most convincing.** For example, place virtual content near the centers of detected planes, where it's safer to assume that the detected plane is a good match to the real-world surface. It may be tempting to design experiences that use the full surface of a table top, where virtual scene elements can react to or fall off the table's edges. However, world tracking and plane detection may not precisely estimate the edges of the table.


## User Control

**Strive for a balance between accurately placing virtual content and respecting the user's input.** For example, consider a situation where the user attempts to place content that should appear on top of a flat surface.

- First, try to place content by using the [`hitTest(_:types:)`](https://developer.apple.com/documentation/arkit/arframe/2875718-hittest) method to search for an intersection with a plane anchor. If you don't find a plane anchor, there might still be a plane at the target location that has not yet been identified by plane detection.
- Lacking a plane anchor, you can hit-test against scene features to get a rough estimate for where to place content right away, and refine that estimate over time as ARKit detects planes.
- When plane detection provides a better estimate for where to place content, use animation to subtly move that content to its new position. Having user-placed content suddenly jump to a new position can break the AR illusion and confuse the user.
- Filter out hit test results which are too close or too far away. In most scenarios there exists a reasonable limit for how far away virtual content can be placed. To prevent users from accidentally placing virtual content too far away you can make use of the `distance` property of `ARHitTestResult` to filter out hit tests which exeed the limit.

**Avoid interrupting the AR experience.** If the user transitions to another fullscreen UI in your app, the AR view might not be an expected state when coming back.

Use the popover presentation (even on iPhone) for auxiliary view controllers to keep the user in the AR experience while adjusting settings or making a modal selection. In this example, the `SettingsViewController` and `VirtualObjectSelectionViewController` classes use popover presentation.

## Testing

For testing and debugging AR experiences, it helps to have a live visualization of the scene processing that ARKit performs. See the `showDebugVisuals` method in this project's `ViewController` class for world tracking visualization, and the `HitTestVisualization` class for a demonstration of ARKit's feature detection methods.
