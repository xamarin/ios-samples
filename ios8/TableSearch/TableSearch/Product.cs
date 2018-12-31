using System;
using Foundation;

namespace TableSearch
{
    /// <summary>
    /// The data model object describing the product displayed in both main and results tables.
    /// </summary>
    public class Product : NSObject
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
    }
}