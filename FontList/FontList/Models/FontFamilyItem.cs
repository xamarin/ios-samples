using System.Collections.Generic;

namespace FontList.Models
{
    /// <summary>
    /// A group that contains table items
    /// </summary>
    public class FontFamilyItem
    {
        public string Name { get; set; }

        public List<FontItem> Items { get; set; }

        public FontFamilyItem()
        {
            Items = new List<FontItem>();
        }

        public FontFamilyItem(string name) : this()
        {
            Name = name;
        }
    }
}