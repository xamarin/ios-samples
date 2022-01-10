using Foundation;
using System;
using System.Collections.Generic;
using UIKit;

namespace PeekPopNavigation.Models
{
    /// <summary>
    /// A class that provides a sample set of color data.
    /// </summary>
    public class ColorData : NSObject
    {
        public List<ColorItem> Colors { get; } = new List<ColorItem>
        {
            new ColorItem("Red", UIColor.FromRGBA(1f, 0.231372549f, 0.1882352941f, 1f), false),
            new ColorItem("Orange", UIColor.FromRGBA(1f, 0.5843137255f, 0f, 1f), false),
            new ColorItem("Yellow", UIColor.FromRGBA(1f, 0.8f, 0f, 1f),  false),
            new ColorItem("Green", UIColor.FromRGBA(0.2980392157f, 0.8509803922f, 0.3921568627f, 1f),  false),
            new ColorItem("Teal Blue", UIColor.FromRGBA(0.3529411765f,  0.7843137255f, 0.9803921569f, 1f), false),
            new ColorItem("Blue", UIColor.FromRGBA(0f,  0.4784313725f, 1f, 1f), false),
            new ColorItem("Purple", UIColor.FromRGBA(0.3450980392f, 0.337254902f, 0.8392156863f, 1f), false),
            new ColorItem("Pink", UIColor.FromRGBA(1f, 0.1764705882f,  0.3333333333f, 1f), false)
        };

        /// <summary>
        /// Delete a ColorItem object from the set of colors.
        /// </summary>
        public void Delete(ColorItem colorItem)
        {
            var arrayIndex = Colors.IndexOf(colorItem);
            if (arrayIndex == -1)
            {
                throw new ArgumentOutOfRangeException("Expected colorItem to exist in colors");
            }

            this.Colors.RemoveAt(arrayIndex);

            // Send a notifications so that UI can be updated and pass the index where the colorItem was removed from.
            NSNotificationCenter.DefaultCenter.PostNotificationName(ColorItem.ColorItemDeleted, this, NSDictionary.FromObjectAndKey(new NSNumber(arrayIndex), new NSString("index")));
        }
    }
}