
namespace SoupChef.Support
{
    using System;
    using UIKit;

    /// <summary>
    /// Utility extension on `UIImageView` for visual appearance.
    /// </summary>
    public static class UIImageViewExtensions
    {
        public static void ApplyRoundedCorners(this UIImageView self)
        {
            self.Layer.CornerRadius = 8f;
            self.ClipsToBounds = true;
        }
    }
}