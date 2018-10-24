using CoreGraphics;
using Foundation;
using System;
using UIKit;

namespace ZoomingPdfViewer
{
    /// <summary>
    /// This view controller manages the display of a set of view controllers by way of implementing the UIPageViewControllerDataSource protocol.
    /// </summary>
    public class ModelController : NSObject, IUIPageViewControllerDataSource
    {
        private readonly nint numberOfPages;

        private CGPDFDocument pdf;

        public ModelController()
        {
            var pdfURL = NSBundle.MainBundle.GetUrlForResource("Tamarin", "pdf");
            pdf = CGPDFDocument.FromUrl(pdfURL.AbsoluteString);
            numberOfPages = pdf.Pages;
            if (numberOfPages % 2 != 0)
            {
                numberOfPages++;
            }
            else
            {
                // missing pdf file, cannot proceed.
                Console.WriteLine("Missing pdf file 'Tamarin.pdf'");
            }
        }

        public DataViewController GetViewController(nint index, UIStoryboard storyboard)
        {
            var dataViewController = storyboard.InstantiateViewController("DataViewController") as DataViewController;
            dataViewController.PageNumber = index + 1;
            dataViewController.Pdf = pdf;

            return dataViewController;
        }

        public nint IndexOfViewController(DataViewController viewController)
        {
            // Return the index of the given data view controller.
            // For simplicity, this implementation uses a static array of model objects and the view controller stores the model object; you can therefore use the model object to identify the index.
            return viewController.PageNumber - 1;
        }

        public UIViewController GetNextViewController(UIPageViewController pageViewController, UIViewController referenceViewController)
        {
            UIViewController result = null;

            var index = IndexOfViewController(referenceViewController as DataViewController);
            if (index != 0)
            {
                index--;
                result = GetViewController(index, referenceViewController.Storyboard);
            }

            return result;
        }

        public UIViewController GetPreviousViewController(UIPageViewController pageViewController, UIViewController referenceViewController)
        {
            UIViewController result = null;

            var index = IndexOfViewController(referenceViewController as DataViewController);
            if (index != 0)
            {
                index++;
                if (index != numberOfPages)
                {
                    result = GetViewController(index, referenceViewController.Storyboard);
                }
            }

            return result;
        }
    }
}