# DocumentWatermark iOS Application

DocumentWatermark is an example of custom graphics drawing of a PDFPage, which affects both
the containing PDFView and the saved file.

## Overview

This iOS application gives an example of custom PDFPage drawing, which can be useful for
watermarking a document or adding custom layered effects. This is accomplished by three
steps: we first register a delegate to the loaded PDFDocument. This delegate, of type
PDFDocumentDelegate, has a method we must implement: classForPage() -> AnyClass. This method
allows us to declare which PDFPage-subclass type should be instantiated when building pages
for drawing (or saving). Finally, the last step is to override the drawing function of PDFPage
in your custom PDFPage-subclass.

In the source of the project, you will see these three steps. The first two are in ViewController.swift
with the last step in WatermarkPage.swift:

``` swift
// 1. Set delegate
document.delegate = self
pdfView?.document = document
```
[View in Source](x-source-tag://SetDelegate)

``` swift
// 2. Return your custom PDFPage class
/// - Tag: ClassForPage
func classForPage() -> AnyClass {
    return WatermarkPage.self
}
```
[View in Source](x-source-tag://ClassForPage)

``` swift
// 3. Override PDFPage custom draw
/// - Tag: OverrideDraw
override func draw(with box: PDFDisplayBox, to context: CGContext) {

    // Draw original content
    super.draw(with: box, to: context)

    // Draw rotated overlay string
    UIGraphicsPushContext(context)
    context.saveGState()

    let pageBounds = self.bounds(for: box)
    context.translateBy(x: 0.0, y: pageBounds.size.height)
    context.scaleBy(x: 1.0, y: -1.0)
    context.rotate(by: CGFloat.pi / 4.0)

    let string: NSString = "U s e r   3 1 4 1 5 9"
    let attributes = [
        NSAttributedStringKey.foregroundColor: UIColor(colorLiteralRed: 0.5, green: 0.5, blue: 0.5, alpha: 0.5),
        NSAttributedStringKey.font: UIFont.boldSystemFont(ofSize: 64)
    ]

    string.draw(at: CGPoint(x:250, y:40), withAttributes: attributes)

    context.restoreGState()
    UIGraphicsPopContext()

}
```
[View in Source](x-source-tag://OverrideDraw)

## ViewController.swift

The ViewController class initializes a PDFDocument, sets its delegate to self, and implements
the classForPage() delegate method. This declares that all instantiated PDFPages for
the presented document (through PDFView) should instantiate the subclass WatermarkPage instead.
This subclass, found in WatermarkPage.swift, implements custom drawing.

ViewController first loads a path to our Sample.pdf file through the application's
main bundle. This URL is then used to instantiate a PDFDocument. On success, the document
is assigned to our PDFView, which was setup in InterfaceBuilder.

Before document assignment, it is critical to assign our delegate, which is the ViewController
itself, so that classForPage() (a PDFDocumentDelgetate method) implements classForPage().
This method returns the PDFPage subclass used for custom drawing.

## WatermarkPages.swift

WatermarkPage subclasses PDFPage so that it can override the draw(with box: to context:) method.
This method is called by PDFDocument to draw the page into a PDFView. All custom drawing for a PDF
page should be done through this mechanism.

Custom drawing methods should always be thread-safe and call the super-class method. This is needed
to draw the original PDFPage content. Custom drawing code can execute before or after this super-class
call, though order matters! If your graphics run before the super-class call, they are drawn below the
PDFPage content. Conversely, if your graphics run after the super-class call, they are drawn above the
PDFPage.
