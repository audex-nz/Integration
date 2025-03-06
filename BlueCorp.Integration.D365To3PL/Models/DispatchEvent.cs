using System.Text.Json.Serialization;

namespace BlueCorp.Integration.D365To3PL.Models;

public class DispatchEvent
{
    [JsonPropertyName("controlNumber")]
    public int ControlNumber { get; set; }

    [JsonPropertyName("salesOrder")]
    public string SalesOrder { get; set; } = string.Empty;

    [JsonPropertyName("containers")]
    public List<Container> Containers { get; set; } = new List<Container>();

    [JsonPropertyName("deliveryAddress")]
    public DeliveryAddress DeliveryAddress { get; set; } = new DeliveryAddress();
}

public class Container
{
    [JsonPropertyName("loadId")]
    public string LoadId { get; set; } = string.Empty;

    [JsonPropertyName("containerType")]
    public string ContainerType { get; set; } = string.Empty;

    [JsonPropertyName("items")]
    public List<Item> Items { get; set; } = new List<Item>();
}

public class Item
{
    [JsonPropertyName("itemCode")]
    public string ItemCode { get; set; } = string.Empty;

    [JsonPropertyName("quantity")]
    public decimal Quantity { get; set; }

    [JsonPropertyName("cartonWeight")]
    public decimal CartonWeight { get; set; }
}

public class DeliveryAddress
{
    [JsonPropertyName("street")]
    public string Street { get; set; } = string.Empty;

    [JsonPropertyName("city")]
    public string City { get; set; } = string.Empty;

    [JsonPropertyName("state")]
    public string State { get; set; } = string.Empty;

    [JsonPropertyName("postalCode")]
    public string PostalCode { get; set; } = string.Empty;

    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;
}