# PDFAnnotationWidgetsAdvanced iOS Application

This sample demonstrates how to use [PDFKit](https://developer.apple.com/documentation/pdfkit) to add widgets – interactive form elements – to your own PDF document.

## Overview

The PDF file format allows for annotations to be associated with a page and a location on that page.
One powerful annotation type is the widget annotation which represents interactive form elements. There 
are three widget types that PDFKit supports: text, button, and choice widgets. Text widgets are basic
text fields. Button widgets include three subtypes: radio buttons, checkboxes, and push buttons. Choice 
widgets include two subtypes: list (i.e. a table view), and combo box (i.e. a drop down). This example 
will dive into two of the three widget types - text and buttons.

## Widget Annotation Creation

Before we dive into the example, it's important to understand how widget annotations are created in
PDFKit.

All widget annotations are instantiated using the 
[`PDFAnnotation`](https://developer.apple.com/documentation/pdfkit/pdfannotation) constructor with subtype 
[`.widget`](https://developer.apple.com/documentation/pdfkit/pdfannotationsubtype/2876103-widget). In
order for PDFKit to know which type of interactive element to add to your document, you must explicitly
set the `widgetFieldType` property:

```swift
// Create a text widget
widgetAnnotation.widgetFieldType = PDFAnnotationWidgetSubtype.text.rawValue
```

```swift
// Create a button widget
widgetAnnotation.widgetFieldType = PDFAnnotationWidgetSubtype.button.rawValue
```

```swift
// Create a choice widget
widgetAnnotation.widgetFieldType = PDFAnnotationWidgetSubtype.choice.rawValue
```

Similarly, when creating a button widget annotation, you must explicitly set the `widgetControlType` property 
in order for PDFKit to know which type of button to add:

```swift
// Create a radio button
widgetAnnotation.widgetControlType = .radioButtonControl
```

```swift
// Create a checkbox
widgetAnnotation.widgetControlType = .checkBoxControl
```

```swift
// Create a push button
widgetAnnotation.widgetControlType = .pushButtonControl
```

When creating a choice widget annotation, you can toggle between the two flavors of choice widgets using
the `isListChoice` property (default is list choice):

```swift
// Create a list choice widget (default)
widgetAnnotation.isListChoice = true
```

```swift
// Create a combo box choice widget
widgetAnnotation.isListChoice = false
```

## ViewController.swift

`ViewController` first loads a path to our MyForm.pdf file through the application's main bundle. This URL
is then used to instantiate a [`PDFDocument`](https://developer.apple.com/documentation/pdfkit/pdfdocument).
On success, the document is assigned to our 
[`PDFView`](https://developer.apple.com/documentation/pdfkit/pdfview), which
was setup in InterfaceBuilder. Once the document has been successfully loaded, we can extract the first
page in order to begin adding our widget annotations.

`ViewController` adds the following widget types to the extracted 
[`PDFPage`](https://developer.apple.com/documentation/pdfkit/pdfpage): three text fields, two radio
buttons, three checkboxes, and one push button. To create all of these widgets is simple. From above, we
know how to set the `widgetFieldType` to create text and button widgets, and we know how to set the
`widgetControlType` to create radio buttons, checkboxes, and push buttons.

In addition to basic widget creation, `ViewController` includes a few extra widget-specific properties
which are worth mentioning:

### `maximumLength` and `hasComb`
The `textFieldDate` text widget annotation sets two extra properties: `maximumLength` and `hasComb`.
The maximum length property sets the maximum amount of characters a user can enter into the text
field. The `hasComb` property, if set, will divide the text field into as many equally spaced
positions, or combs, as the value of the maximum length of the field. The `hasComb` property only works
in conjunction with the `maximumLength` property.

### `fieldName` and `buttonWidgetStateString`
The `radioButtonYes` and `radioButtonNo` button widget annotations set two extra properties: `fieldName` and
`buttonWidgetStateString`.
All widgets have their own unique identifier, which is determined by the `fieldName` property. In
order to group widgets together, for this case our radio buttons, the buttons must have the same
field name, which we set in `ViewController`. In order to give radio buttons of the same field name
their own unique sub-indentifier, we must set the `buttonWidgetStateString` property on each, which
we also do in `ViewController`. By grouping together the radio buttons by field name, and giving
each button its own unique `buttonWidgetStateString`, the buttons will now behave like normal radio
buttons (where selecting one will deselect others in the group).
Note: the strings used for both `fieldName` and `buttonWidgetStateString` are arbitrary; what matters is
that the `fieldName` for the buttons are the same, and the `buttonWidgetStateStrings` for each button
are unique to that grouping.

### `isMultiline`
The `textFieldMultiline` text widget annotation sets one extra property: `isMultiline`.
By default, all text widget annotations are single line. This means they do not include word
wrapping. If you would like to create a large, multiline text widget with word wrapping, set this
property to `true`.

### `action` and [`PDFActionResetForm`](https://developer.apple.com/documentation/pdfkit/pdfactionresetform)
The `resetButton` button widget annotation sets one extra property: action using `PDFActionResetForm`.
By setting this property, the given action will performed when the user clicks (or taps) on this
button. The `PDFActionResetForm` action is special in that it will clear whatever widgets are
associated to it. `PDFActionResetForm` takes an array of strings which represent widget field names.
The default behavior is to clear all fields included in that list. This behavior can be changed to
clear all widgets not included in that list. This is done via setting the `fieldsIncludedAreCleared`
property to false.
In `ViewController`, we do two things: 

1. We don't explicitly set any field names
2. We set `fieldsIncludedAreCleared` to `false`. Because we set the behavior to clear all widgets not included 
in our list, and we did not set any field names, the result is that this action will clear all widgets in the document.
