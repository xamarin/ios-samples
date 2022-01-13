
namespace ExceptionalAccessibility;

/// <summary>
/// Dog model housing all of the data for each different dog up for adoption.
/// </summary>
public class Dog : IEquatable<Dog>
{
        readonly static UIImage DefaultImage = UIImage.FromBundle ("husky")!;

        public Dog (string name, List<UIImage> images, string breed, float age, float weight, string shelterName)
        {
                this.Name = name;
                this.Images = images;
                this.Breed = breed;
                this.Age = age;
                this.Weight = weight;
                this.ShelterName = shelterName;
        }

        public UIImage? FeaturedImage => Images?.FirstOrDefault ();

        public string Name { get; private set; }

        public List<UIImage>? Images { get; set; }

        public string Breed { get; private set; }

        public float Age { get; private set; }

        public float Weight { get; private set; }

        public string ShelterName { get; private set; }

        /// <summary>
        /// Convenience initializer for faked data
        /// </summary>
        public static List<Dog> All { get; } = new List<Dog>
        {
                new Dog("Lilly", new List<UIImage> { DefaultImage, DefaultImage, DefaultImage }, "Corgi", 5, 26, "Cupertino Animal Shelter"),
                new Dog("Mr. Hammond", new List<UIImage> { DefaultImage },  "Pug", 2, 23,  "Cupertino Animal Shelter"),
                new Dog("Bubbles", new List<UIImage> { DefaultImage, DefaultImage, DefaultImage }, "Golden Retriever", 8, 65,  "Cupertino Animal Shelter"),
                new Dog("Pinky", new List<UIImage> { DefaultImage },  "Maltese", 4,  28, "Cupertino Animal Shelter")
        };

        public bool Equals (Dog? other)
        {
                if (other is null)
                        return false;
                if (ReferenceEquals (this, other)) return true;

                return Name == other.Name &&
                       Images == other.Images &&
                       Breed == other.Breed &&
                       Age == other.Age &&
                       Weight == other.Weight &&
                       ShelterName == other.ShelterName;
        }
}
