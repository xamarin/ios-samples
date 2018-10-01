
namespace ExceptionalAccessibility
{
    using Foundation;
    using UIKit;

    /// <summary>
    /// The custom carousel accessibility element is a core part of this sample.
    /// It is illustrating a way in which we choose to tweak the accessibility experience in a unique and interesting way.
    /// If we leave the collection view as is, then the VoiceOver user has to swipe to the end of the carousel
    /// before they can reach either button or the data for the dogs, meaning that they will only ever be able to
    /// get to the data for the last dog in the carousel through swiping alone.We instead create this custom element,
    /// and make it an adjustable element that responds to `accessibilityIncrement` and `accessibilityDecrement`,
    /// so that when a user swipes from it, they swipe immediately to the favorite and gallery buttons, then on to the data,
    /// for the specific dog.In some ways, we've transformed the collection view into acting more like a picker.
    /// </summary>
    public class CarouselAccessibilityElement : UIAccessibilityElement
    {
        public CarouselAccessibilityElement(NSObject accessibilityContainer, Dog dog) : base(accessibilityContainer)
        {
            this.CurrentDog = dog;
        }

        public Dog CurrentDog { get; set; }

        /// <summary>
        /// This indicates to the user what exactly this element is supposed to be.
        /// </summary>
        public override string AccessibilityLabel
        {
            get => "Dog Picker";
            set => base.AccessibilityLabel = value;
        }

        public override string AccessibilityValue
        {
            get => this.CurrentDog != null ? this.CurrentDog.Name : base.AccessibilityValue;
            set => base.AccessibilityValue = value;
        }

        /// <summary>
        /// This tells VoiceOver that our element will support the increment and decrement callbacks.
        /// </summary>
        public override ulong AccessibilityTraits { get => (ulong)UIAccessibilityTrait.Adjustable; set => base.AccessibilityTraits = value; }

        /// <summary>
        /// A convenience for forward scrolling in both `accessibilityIncrement` and `accessibilityScroll`.
        /// It returns a `Bool` because `accessibilityScroll` needs to know if the scroll was successful.
        /// </summary>
        private bool AccessibilityScrollForward()
        {
            var result = false;

            // Initialize the container view which will house the collection view.
            if (this.AccessibilityContainer is DogCarouselContainerView containerView)
            {
                // Store the currently focused dog and the list of all dogs.
                if (this.CurrentDog != null && containerView.Dogs != null)
                {
                    // Get the index of the currently focused dog from the list of dogs (if it's a valid index).
                    var index = containerView.Dogs.IndexOf(this.CurrentDog);
                    if (index > -1 && index < containerView.Dogs.Count - 1)
                    {
                        // Scroll the collection view to the currently focused dog.
                        containerView.dogCollectionView.ScrollToItem(NSIndexPath.FromRowSection(index + 1, 0),
                                                                     UICollectionViewScrollPosition.CenteredHorizontally,
                                                                     true);

                        result = true;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// A convenience for backward scrolling in both `accessibilityIncrement` and `accessibilityScroll`.
        /// It returns a `Bool` because `accessibilityScroll` needs to know if the scroll was successful.
        /// </summary>
        private bool AccessibilityScrollBackward()
        {
            var result = false;
            if (this.AccessibilityContainer is DogCarouselContainerView containerView)
            {
                if (this.CurrentDog != null && containerView.Dogs != null)
                {
                    var index = containerView.Dogs.IndexOf(this.CurrentDog);
                    if (index != -1)
                    {
                        containerView.dogCollectionView.ScrollToItem(NSIndexPath.FromRowSection(index - 1, 0),
                                                                     UICollectionViewScrollPosition.CenteredHorizontally,
                                                                     true);

                        result = true;
                    }
                }
            }

            return result;
        }

        #region Accessibility

        /*
            Overriding the following two methods allows the user to perform increment and decrement actions
            (done by swiping up or down).
        */

        [Export("accessibilityIncrement")]
        public void AccessibilityIncrement()
        {
            // This causes the picker to move forward one if the user swipes up.
            _ = this.AccessibilityScrollForward();
        }

        [Export("accessibilityDecrement")]
        public void AccessibilityDecrement()
        {
            // This causes the picker to move back one if the user swipes down.
            _ = this.AccessibilityScrollBackward();
        }

        /// <summary>
        /// This will cause the picker to move forward or backwards on when the user does a 3-finger swipe,
        /// depending on the direction of the swipe.The return value indicates whether or not the scroll was successful,
        /// so that VoiceOver can alert the user if it was not.
        /// </summary>
        [Export("accessibilityScroll:")]
        public bool AccessibilityScroll(UIAccessibilityScrollDirection direction)
        {
            var result = false;
            if (direction == UIAccessibilityScrollDirection.Left)
            {
                result = this.AccessibilityScrollForward();
            }
            else if (direction == UIAccessibilityScrollDirection.Right)
            {
                result = this.AccessibilityScrollBackward();
            }

            return result;
        }

        #endregion
    }
}
