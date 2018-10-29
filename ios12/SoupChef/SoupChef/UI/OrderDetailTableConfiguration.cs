
namespace SoupChef
{
    using System.Collections.Generic;

    /// <summary>
    /// This class encapsulates the configuration of the `UITableView` in `OrderDetailViewController`.
    /// </summary>
    public class OrderDetailTableConfiguration
    {
        public OrderDetailTableConfiguration(OrderTypeEnum orderType)
        {
            this.OrderType = orderType;
        }

        public OrderTypeEnum OrderType { get; private set; }

        private static readonly List<SectionModel> NewOrderSectionModel = new List<SectionModel>
        {
            new SectionModel(SectionType.Price, 1, BasicCellType.Basic),
            new SectionModel(SectionType.Quantity, 1, QuantityCell.CellIdentifier),
            //new SectionModel(SectionType.Options, MenuItemOption.All.Length, BasicCellType.Basic),
            new SectionModel(SectionType.Total, 1, BasicCellType.Basic),
        };

        private static readonly List<SectionModel> HistoricalOrderSectionModel = new List<SectionModel>
        {
            new SectionModel(SectionType.Quantity, 1, QuantityCell.CellIdentifier),
            //new SectionModel(SectionType.Options, MenuItemOption.All.Length, BasicCellType.Basic),
            new SectionModel(SectionType.Total, 1, BasicCellType.Basic),
        };

        public List<SectionModel> Sections
        {
            get
            {
                switch (OrderType)
                {
                    case OrderTypeEnum.New:
                        return OrderDetailTableConfiguration.NewOrderSectionModel;
                    default:
                        return OrderDetailTableConfiguration.HistoricalOrderSectionModel;
                }
            }
        }

        /* helpers */

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
        }

        public static class BasicCellType
        {
            public const string Basic = "Basic Cell";
            public const string Quantity = "Quantity Cell";
        }

        public class SectionModel
        {
            public SectionModel(string type, int rowCount, string cellReuseIdentifier)
            {
                this.Type = type;
                this.RowCount = rowCount;
                this.CellReuseIdentifier = cellReuseIdentifier;
            }

            public string Type { get; private set; }

            public int RowCount { get; private set; }

            public string CellReuseIdentifier { get; private set; }
        }
    }
}