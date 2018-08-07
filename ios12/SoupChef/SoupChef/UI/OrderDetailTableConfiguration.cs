/*
See LICENSE folder for this sample’s licensing information.

Abstract:
This struct encapsulates the configuration of the `UITableView` in `OrderDetailViewController`.
*/

using SoupKit.Data;

namespace SoupChef
{
    public struct OrderDetailTableConfiguration
    {
        public enum OrderTypeEnum
        {
            New,
            Historical
        }

        public static class SectionType
        {
            public const string Price = "Price";
            public const string Quantity = "Quantity";
            public const string Options = "Options";
            public const string Total = "Total";
            public const string VoiceShortcut = "Shortcut Phrase";
        }

        public static class BasicCellType
        {
            public const string Basic = "Basic Cell";
            public const string VoiceShortcut = "Voice Shortcut";
        }

        public OrderTypeEnum OrderType { get; private set; }

        public OrderDetailTableConfiguration(OrderTypeEnum orderType)
        {
            OrderType = orderType;
        }

        public struct SectionModel
        {
            public string Type { get; private set; }
            public int RowCount { get; private set; }
            public string CellReuseIdentifier { get; private set; }

            public SectionModel(string type, int rowCount, string cellReuseIdentifier)
            {
                this.Type = type;
                this.RowCount = rowCount;
                this.CellReuseIdentifier = cellReuseIdentifier;
            }
        }

        static SectionModel[] NewOrderSectionModel = {
            new SectionModel(SectionType.Price, 1, BasicCellType.Basic),
            new SectionModel(SectionType.Quantity, 1, QuantityCell.CellIdentifier),
            new SectionModel(SectionType.Options, MenuItemOption.All.Length, BasicCellType.Basic),
            new SectionModel(SectionType.Total, 1, BasicCellType.Basic),
        };

        static SectionModel[] HistoricalOrderSectionModel = {
            new SectionModel(SectionType.Quantity, 1, QuantityCell.CellIdentifier),
            new SectionModel(SectionType.Options, MenuItemOption.All.Length, BasicCellType.Basic),
            new SectionModel(SectionType.Total, 1, BasicCellType.Basic),
            new SectionModel(SectionType.VoiceShortcut, 1, BasicCellType.VoiceShortcut),
        };

        public SectionModel[] Sections
        {
            get
            {
                switch (OrderType)
                {
                    case OrderTypeEnum.New:
                        return NewOrderSectionModel;
                    default:
                        return HistoricalOrderSectionModel;
                }
            }
        }
    }
}
