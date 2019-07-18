---
name: 'Xamarin.iOS - ConvertedFrogScroller '
description: 'Description: Converted FrogScroller is an example of enhancing scrollview experience with the use of UIPageViewController, UIScrollView and...'
page_type: sample
languages:
- csharp
products:
- xamarin
technologies:
- xamarin-ios
urlFragment: frogscroller
---
# ConvertedFrogScroller 

Description:

Converted FrogScroller is an example of enhancing scrollview experience with the use of
UIPageViewController, UIScrollView and CATiledLayer.


Description:
 
FrogScroller opens with images that can be scrolled, panned and zoomed using ImageScrollView, 
a subclass of UIScrollView and  PageViewController  used for indexing and creating new pages.
  
Packaging List: 

AppDelegate:
-A UIApplication delegate to set up application and UIPageViewController to set up datasource
to index the page

ImageScrollView:
-Configure scrollview to display new image

ImageViewController: 
- Page Indexing and ImageScrollview configurtaion

TilingView:
-CATiledLayer to handle tile drawing

ImageDetails.xml:
-Contains Full resolution image data

From the WWDC 2012 sample

Ported By: GouriKumari
