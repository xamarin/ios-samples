using System;
using Foundation;

namespace TableSearch
{
    /// <summary>
    /// The data model object describing the product displayed in both main and results tables.
    /// </summary>
    public class Product : NSObject, INSCoding
    {
        [Export("title")]
        public string Title { get; private set; }

        [Export("yearIntroduced")]
        public int YearIntroduced { get; private set; }

        [Export("introPrice")]
        public double IntroPrice { get; private set; }

        public Product(string title, int yearIntroduced, double price)
        {
            Title = title;
            YearIntroduced = yearIntroduced;
            IntroPrice = price;
        }

        [Export("initWithCoder:")]
        public Product(NSCoder coder) 
        {
            var decodedTitle = coder.DecodeObject(CoderKeys.nameKey) as NSString;
            if (decodedTitle == null)
            {
                throw new Exception("A title did not exist. In your app, handle this gracefully.");
            }
            Title = decodedTitle;
            YearIntroduced = coder.DecodeInt(CoderKeys.yearKey);
            IntroPrice = coder.DecodeDouble(CoderKeys.priceKey);
        }

        public void EncodeTo(NSCoder encoder)
        {
            encoder.Encode(new NSString(Title), CoderKeys.nameKey);
            encoder.Encode(YearIntroduced, CoderKeys.yearKey);
            encoder.Encode(IntroPrice, CoderKeys.priceKey);
        }
    }

    class CoderKeys
    {
        public const string nameKey = "nameKey";
        public const string yearKey = "yearKey";
        public const string priceKey = "priceKey";
    }
}