using CsvHelper.Configuration.Attributes;

namespace BlueCorp.Integration.D365To3PL.Models;

public class ThirdPartyCsvRecord
{
    [Name("CustomerReference")]
    public string CustomerReference { get; set; } = string.Empty;

    [Name("LoadId")]
    public string LoadId { get; set; } = string.Empty;

    [Name("ContainerType")]
    public string ContainerType { get; set; } = string.Empty;

    [Name("ItemCode")]
    public string ItemCode { get; set; } = string.Empty;

    [Name("ItemQuantity")]
    public decimal ItemQuantity { get; set; }

    [Name("ItemWeight")]
    public decimal ItemWeight { get; set; }

    [Name("Street")]
    public string Street { get; set; } = string.Empty;

    [Name("City")]
    public string City { get; set; } = string.Empty;

    [Name("State")]
    public string State { get; set; } = string.Empty;

    [Name("PostalCode")]
    public string PostalCode { get; set; } = string.Empty;

    [Name("Country")]
    public string Country { get; set; } = string.Empty;
}