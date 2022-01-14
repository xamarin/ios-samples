
namespace ExceptionalAccessibility;
/// <summary>
/// The carousel container is a container view for the carousel collection view and the favorite and gallery button.
/// We subclass it so that we can override its `accessibilityElements` to exclude the collection view
/// and use our custom element instead, and so that we can add and remove the gallery buton as
/// an accessibility element as it appears and disappears.
/// </summary>
public partial class DogCarouselContainerView : UIView, IUIAccessibilityContainer
{
        CarouselAccessibilityElement? carouselAccessibilityElement;

        Dog? currentDog;

        protected DogCarouselContainerView (IntPtr handle) : base (handle) { }

        public List<Dog> Dogs { get; set; } = new List<Dog> ();

        public Dog? CurrentDog
        {
                get => currentDog;
                set
                {
                        currentDog = value;
                        this.SetAccessibilityElements (NSArray.FromNSObjects (GetAccessibilityElements ()?.ToArray ()));

                        if (currentDog is not null && carouselAccessibilityElement is not null)
                        {
                                carouselAccessibilityElement.CurrentDog = currentDog;
                        }
                }
        }

        #region Accessibility

        List<NSObject>? GetAccessibilityElements ()
        {
                List<NSObject>? result = null;
                if (currentDog is not null)
                {
                        CarouselAccessibilityElement accessibilityElement;
                        if (carouselAccessibilityElement is not null)
                        {
                                accessibilityElement = carouselAccessibilityElement;
                        }
                        else
                        {
                                accessibilityElement = new CarouselAccessibilityElement (this, currentDog);
                                accessibilityElement.AccessibilityFrameInContainerSpace = dogCollectionView.Frame;
                                carouselAccessibilityElement = accessibilityElement;
                        }

                        // Only show the gallery button if we have multiple images.
                        if (currentDog.Images.Count > 1)
                        {
                                result = new List<NSObject> { carouselAccessibilityElement, galleryButton };
                        }
                        else
                        {
                                result = new List<NSObject> { carouselAccessibilityElement };
                        }
                }

                return result;
        }

        #endregion
}
