
namespace ARMultiuser
{
    using ARKit;

    public static class ARWorldMappingStatusExtensions
    {
        public static string GetDescription(this ARWorldMappingStatus status)
        {
            string result = null;

            switch (status)
            {
                case ARWorldMappingStatus.NotAvailable:
                    result = "Not Available";
                    break;

                case ARWorldMappingStatus.Limited:
                    result = "Limited";
                    break;

                case ARWorldMappingStatus.Extending:
                    result = "Extending";
                    break;

                case ARWorldMappingStatus.Mapped:
                    result = "Mapped";
                    break;
            }

            return result;
        }
    }
}