namespace XamarinShot.Utils;


/// <summary>
/// Convenience extension for generating screeshots from ARSCNView.
/// </summary>
public static class ARSCNViewExtensions
{
        public static UIImage? CreateScreenshot (this ARSCNView view, UIDeviceOrientation interfaceOrientation)
        {
                if (view.Session.CurrentFrame is null)
                {
                        return null;
                }

                var ciImage = new CIImage (view.Session.CurrentFrame.CapturedImage);

                // TODO: check
                var keys = new NSString [] { CIFilterInputKey.Scale, CIFilterInputKey.AspectRatio };
                var objects = new NSNumber [] { new NSNumber (0.5f), new NSNumber (1f) };

                var dictionary = NSDictionary.FromObjectsAndKeys (objects, keys);
                var scaledImage = ciImage.CreateByFiltering ("CILanczosScaleTransform", dictionary);

                var context = new CIContext (new CIContextOptions { UseSoftwareRenderer = false });
                var cgimage = context.CreateCGImage (scaledImage, scaledImage.Extent);
                if (cgimage is not null)
                {
                        var orientation = UIImageOrientation.Right;
                        switch (interfaceOrientation)
                        {
                                case UIDeviceOrientation.Portrait:
                                        orientation = UIImageOrientation.Right;
                                        break;
                                case UIDeviceOrientation.PortraitUpsideDown:
                                        orientation = UIImageOrientation.Left;
                                        break;
                                case UIDeviceOrientation.LandscapeLeft:
                                        orientation = UIImageOrientation.Up;
                                        break;
                                case UIDeviceOrientation.LandscapeRight:
                                        orientation = UIImageOrientation.Down;
                                        break;
                        }

                        return new UIImage (cgimage, 1f, orientation);
                }

                return null;
        }
}
