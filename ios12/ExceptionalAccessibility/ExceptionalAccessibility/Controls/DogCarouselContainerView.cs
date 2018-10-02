
namespace ExceptionalAccessibility
{
    using Foundation;
    using System;
    using System.Collections.Generic;
    using UIKit;

    /// <summary>
    /// The carousel container is a container view for the carousel collection view and the favorite and gallery button.
    /// We subclass it so that we can override its `accessibilityElements` to exclude the collection view
    /// and use our custom element instead, and so that we can add and remove the gallery buton as
    /// an accessibility element as it appears and disappears.
    /// </summary>
    public partial class DogCarouselContainerView : UIView, IUIAccessibilityContainer
    {
        private CarouselAccessibilityElement carouselAccessibilityElement;

        private Dog currentDog;

        public DogCarouselContainerView(IntPtr handle) : base(handle) { }

        public List<Dog> Dogs { get; set; }

        public Dog CurrentDog
        {
            get => this.currentDog;
            set
            {
                this.currentDog = value;
                this.SetAccessibilityElements(NSArray.FromNSObjects(this.GetAccessibilityElements()?.ToArray()));

                if (this.currentDog != null && this.carouselAccessibilityElement != null)
                {
                    this.carouselAccessibilityElement.CurrentDog = this.currentDog;
                }
            }
        }

        #region Accessibility

        private List<NSObject> GetAccessibilityElements()
        {
            List<NSObject> result = null;
            if (this.currentDog != null)
            {
                CarouselAccessibilityElement accessibilityElement;
                if (this.carouselAccessibilityElement != null)
                {
                    accessibilityElement = this.carouselAccessibilityElement;
                }
                else
                {
                    accessibilityElement = new CarouselAccessibilityElement(this, this.currentDog);
                    accessibilityElement.AccessibilityFrameInContainerSpace = dogCollectionView.Frame;
                    this.carouselAccessibilityElement = accessibilityElement;
                }

                // Only show the gallery button if we have multiple images.
                if (this.currentDog.Images.Count > 1)
                {
                    result = new List<NSObject> { this.carouselAccessibilityElement, this.galleryButton };
                }
                else
                {
                    result = new List<NSObject> { this.carouselAccessibilityElement };
                }
            }

            return result;
        }

        #endregion
    }
}