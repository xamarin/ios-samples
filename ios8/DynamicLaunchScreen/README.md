Dynamic Launch Screens
======================

This sample application shows how to create a special type of storyboard to be used as a Dynamic Launch Screen in a Xamarin.iOS application. 

## Unified Storyboards

New to iOS 8, Unified Storyboards allow the developer to create one, unified storyboard file that can be displayed on both iPhone and iPad devices by targeting multiple Size Classes. By using Unified Storyboards, the developer writes less UI specific code and has only interface design to create and maintain.
The key benefits of Unified Storyboards are:* Use the same storyboard file for iPhone and iPad.* Able to deploy backwards to iOS 6 and iOS 7.* Preview the layout for different devices, orientations and OS versions all from within the Xamarin iOS Designer.Storyboards using this feature will require Xcode 6 and Auto Layout, so they will not be compatible with older versions of Xcode.### Enabling Size Classes
Before Size Classes and Adaptive Segues can be used inside a storyboard, it must first be converted to the Xcode 6 Unified Storyboard format from inside the iOS Designer.
To do this:
1. Open the Storyboard to be converted in the iOS Designer and check the Use **Size Classes** check box.
2. The iOS Designer will confirm that the developer wants to convert the format of the storyboard to use Size Classes.
3. Click the Enable Size Classes button to convert the format to continue and start the conversion process.

### Adaptive Segue Types

If the developer has used storyboards before then they will be familiar with the existing segue types of **Push**, **Modal** and **Popover**. When Size Classes are enabled on a Unified Storyboard file, the following Adaptive Segue Types are made available: Show and Show Detail.

When Size Classes are enabled, any existing segues will be converted to the new types. 

### Generic Device Types

Once the storyboard has been converted to use **Size Classes**, it will be redisplayed in the **Design Surface** and the **View As** device will be **Generic**. When the Generic device type is selected, all View Controllers will be resized to a 480 x 480 Square. This square represents sizes of **any width** and **any height**. When the iOS Designer is in this mode, any edits will apply to all of the Size Classes.

### Select a Size Class

The **Size Class Selector** button is at the upper left hand corner of the **Design Surface** (near the **View As** dropdown). It allows the developer to select which Size Classes are currently being edited.

The selector presents the Size Class selection as a 3 x 3 grid. Each of the squares in the grid represents a combination of a **Width Class** and a **Height Class**. The center square selects the **Any Width/Any Height Size class** (which is the default view for a Unified Storyboard). When this square is selected, the developer is editing the default layout, which is inherited by all the other configurations. 

### Excluding an Element from a Size Class

There are times when a given element (such as a View, Control or a Constraint) is not required or desired inside of a specific Size Class. To exclude an Item from a Size Class, select the desired class from the **Size Class Selector**, and uncheck the **Installed** checkbox in the **Properties Explorer**.

## Dynamic Launch Screens

A Dynamic Launch Screen is a special storyboard (or optionally a `.XIB` file) added to a Xamarin.iOS project that acts as a universal launch screen. The Dynamic Launch Screen can only be form from simple controls (such as Images Views and Labels), uses Size Classes and Auto Layout for element positioning and must _not_ do any form of calculation or have any backing code.