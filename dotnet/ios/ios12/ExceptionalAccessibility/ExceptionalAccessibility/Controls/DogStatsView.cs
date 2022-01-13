
namespace ExceptionalAccessibility;
/// <summary>
/// This view is a collection of labels that house the data for each dog.
/// There are 4 properties that each dog has, so 8 labels in total: a title label for what the data is
/// and then a content label for the value.
/// </summary>
public partial class DogStatsView : UIView, IUIAccessibilityContainer
{
        Dog? dog;

        public DogStatsView (IntPtr handle) : base (handle) { }

        public Dog? Dog
        {
                get => dog;
                set
                {
                        dog = value;
                        if (dog is not null)
                        {
                                nameLabel.Text = dog.Name;
                                breedLabel.Text = dog.Breed;
                                ageLabel.Text = $"{dog.Age} years";
                                weightLabel.Text = $"{dog.Weight} lbs";

                                this.SetAccessibilityElements (NSArray.FromNSObjects (GetAccessibilityElements ().ToArray ()));
                        }
                }
        }

        #region Accessibility Logic

        /*
            VoiceOver relies on `accessibilityElements` returning an array of consistent objects that persist
            as the user swipes through an app. We therefore have to cache our array of computed `accessibilityElements`
            so that we don't get into an infinite loop of swiping. We reset this cached array whenever a new dog object is set
            so that `accessibilityElements` can be recomputed.
        */

        List<UIAccessibilityElement> GetAccessibilityElements ()
        {
                /*
                    We want to create a custom accessibility element that represents a grouping of each
                    title and content label pair so that the VoiceOver user can interact with them as a unified element.
                    This is important because it reduces the amount of times the user has to swipe through the display
                    to find the information they're looking for, and because without grouping the labels,
                    the content labels lose the context of what they represent.
                */

                var elements = new List<UIAccessibilityElement> ();
                var nameElement = new UIAccessibilityElement (this);
                nameElement.AccessibilityLabel = $"{nameTitleLabel.Text}, {nameLabel.Text}";

                /*
                  This tells VoiceOver where the object should be onscreen. As the user
                  touches around with their finger, we can determine if an element is below
                  their finger.
                */

                nameElement.AccessibilityFrameInContainerSpace = CGRect.Union (nameTitleLabel.Frame, nameLabel.Frame);
                elements.Add (nameElement);

                var ageElement = new UIAccessibilityElement (this);
                ageElement.AccessibilityLabel = $"{ageTitleLabel.Text}, {ageLabel.Text}";
                ageElement.AccessibilityFrameInContainerSpace = CGRect.Union (ageTitleLabel.Frame, ageLabel.Frame);
                elements.Add (ageElement);

                var breedElement = new UIAccessibilityElement (this);
                breedElement.AccessibilityLabel = $"{breedTitleLabel.Text}, {breedLabel.Text}";
                breedElement.AccessibilityFrameInContainerSpace = CGRect.Union (breedTitleLabel.Frame, breedLabel.Frame);
                elements.Add (breedElement);

                var weightElement = new UIAccessibilityElement (this);
                weightElement.AccessibilityLabel = $"{weightTitleLabel.Text}, {weightLabel.Text}";
                weightElement.AccessibilityFrameInContainerSpace = CGRect.Union (this.weightTitleLabel.Frame, this.weightLabel.Frame);
                elements.Add (weightElement);

                return elements;
        }

        #endregion
}
