using Foundation;
using System;

namespace TableSearch
{
    /// <summary>
    /// The data model object describing the product displayed in both main and results tables.
    /// </summary>
    public class Product : NSObject, INSCoding
    {
        public Product(string title, int year, double price)
        {
            this.Title = title;
            this.YearIntroduced = year;
            this.IntroPrice = price;
        }

        [Export("initWithCoder:")]
        public Product(NSCoder coder)
        {
            if(coder.DecodeObject(nameof(Title)) is NSString decodedTitle)
            {
                this.Title = decodedTitle;
                this.YearIntroduced = coder.DecodeInt(nameof(this.YearIntroduced));
                this.IntroPrice = coder.DecodeDouble(nameof(this.IntroPrice));
            }
            else
            {
                throw new Exception("A title did not exist. In your app, handle this gracefully.");
            }
        }

        [Export("title")]
        public string Title { get; private set; }

        [Export("yearIntroduced")]
        public int YearIntroduced { get; private set; }

        [Export("introPrice")]
        public double IntroPrice { get; private set; }

        public void EncodeTo(NSCoder encoder)
        {
            encoder.Encode(new NSString(this.Title), nameof(this.Title));
            encoder.Encode(this.YearIntroduced, nameof(this.YearIntroduced));
            encoder.Encode(this.IntroPrice, nameof(this.IntroPrice));
        }
    }
}