using System;
using UIKit;

namespace AdaptiveElements
{
    /// <summary>
    /// The ExampleContainerViewController can look and act differently depending on its size.
    /// We call each way of acting differently a "design".
    /// Design is a struct that encapsulates everything that distinguishes one design from another.
    /// Its definition is specific to this particular sample app, but you may want to use the same concept in your own apps.
    /// </summary>
    public class Design
    {
        /// <summary>
        /// Whether to be horizontal or vertical
        /// </summary>
        public UILayoutConstraintAxis Axis { get; set; }

        /// <summary>
        /// Whether the elements inside are small or large
        /// </summary>
        public ElementKind Kind { get; set; }

        /// <summary>
        /// We also implement a computed read-only property, which returns the identifier
        /// of the view controller in the storyboard that this design should use.
        /// </summary>
        public string ElementIdentifier
        {
            get
            {
                switch (this.Kind)
                {
                    case ElementKind.Small: return "smallElement";
                    case ElementKind.Large: return "largeElement";
                    default: throw new NotImplementedException();
                }
            }
        }

        public enum ElementKind
        {
            Small,
            Large
        }
    }
}