using System;

using Foundation;
using CoreGraphics;
using UIKit;

namespace ZoomingPdfViewer {
	public class ModelController : NSObject, IUIPageViewControllerDataSource {
		readonly nint numberOfPages;
		CGPDFDocument pdf;

		public ModelController ()
		{
			NSUrl pdfURL = NSBundle.MainBundle.GetUrlForResource ("Tamarin", "pdf");
			pdf = CGPDFDocument.FromUrl (pdfURL.AbsoluteString);
			numberOfPages = pdf.Pages;
			if (numberOfPages % 2 != 0)
				numberOfPages++;
		}

		public DataViewController GetViewController (nint index, UIStoryboard storyboard)
		{
			var dataViewController = (DataViewController)storyboard.InstantiateViewController ("DataViewController");
			dataViewController.PageNumber = index + 1;
			dataViewController.Pdf = pdf;
			return dataViewController;
		}

		public nint IndexOfViewController (DataViewController viewController)
		{
			return viewController.PageNumber - 1;
		}

		public UIViewController GetNextViewController (UIPageViewController pageViewController, UIViewController referenceViewController)
		{
			nint index = IndexOfViewController ((DataViewController)referenceViewController);

			if (index == 0)
				return null;

			index--;
			return GetViewController (index, referenceViewController.Storyboard);
		}

		public UIViewController GetPreviousViewController (UIPageViewController pageViewController, UIViewController referenceViewController)
		{
			nint index = IndexOfViewController ((DataViewController)referenceViewController);

			if (index == 0)
				return null;

			index++;
			if (index == numberOfPages)
				return null;

			return GetViewController (index, referenceViewController.Storyboard);
		}
	}
}

