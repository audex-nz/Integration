using BlueCorp.Integration.D365To3PL.Models;

namespace BlueCorp.Integration.D365To3PL.Mappings
{
    public static class DispatchEventToThirdPartyCsvRecord
    {
        public static List<ThirdPartyCsvRecord> ConvertToCsvFormat(DispatchEvent dispatchEvent)
        {
            var csvRecords = new List<ThirdPartyCsvRecord>();

            foreach (var container in dispatchEvent.Containers)
            {
                // Convert the container type to the format expected by the 3PL system
                var mappedContainerType = Convert(container.ContainerType);

                foreach (var item in container.Items)
                {
                    var csvRecord = new ThirdPartyCsvRecord
                    {
                        CustomerReference = dispatchEvent.SalesOrder,
                        LoadId = container.LoadId,
                        ContainerType = mappedContainerType,
                        ItemCode = item.ItemCode,
                        ItemQuantity = item.Quantity,
                        ItemWeight = item.CartonWeight,
                        Street = dispatchEvent.DeliveryAddress.Street,
                        City = dispatchEvent.DeliveryAddress.City,
                        State = dispatchEvent.DeliveryAddress.State,
                        PostalCode = dispatchEvent.DeliveryAddress.PostalCode,
                        Country = dispatchEvent.DeliveryAddress.Country
                    };

                    csvRecords.Add(csvRecord);
                }
            }

            return csvRecords;
        }

        private static readonly Dictionary<string, string> ContainerMapping = new()
        {
            { "20RF", "REF20" },
            { "40RF", "REF40" },
            { "20HC", "HC20" },
            { "40HC", "HC40" }
        };

        private static string Convert(string d365ContainerType)
        {
            if (ContainerMapping.TryGetValue(d365ContainerType, out var mappedType))
            {
                return mappedType;
            }

            throw new ArgumentException($"Unknown container type: {d365ContainerType}");
        }
    }
}
