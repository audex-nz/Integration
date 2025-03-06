using BlueCorp.Integration.D365To3PL.Mappings;
using BlueCorp.Integration.D365To3PL.Models;

namespace BlueCorp.Integration.D365To3PL.Tests.Services
{
    public class DispatchEventToThirdPartyCsvRecordTests
    {
        [Fact]
        public void ConvertToCsvFormat_ValidDispatchEvent_ReturnsCorrectCsvRecords()
        {
            // Arrange
            var dispatchEvent = new DispatchEvent
            {
                ControlNumber = 101,
                SalesOrder = "SO123456",
                DeliveryAddress = new DeliveryAddress
                {
                    Street = "35 Fake Street",
                    City = "Fake City",
                    State = "Fake State",
                    PostalCode = "12345",
                    Country = "Fake Country"
                },
                Containers =
                [
                    new Container
                    {
                        LoadId = "LOAD001",
                        ContainerType = "20RF",
                        Items =
                        [
                            new Item
                            {
                                ItemCode = "ITEM001",
                                Quantity = 10,
                                CartonWeight = (decimal)2.5
                            },

                            new Item
                            {
                                ItemCode = "ITEM002",
                                Quantity = 5,
                                CartonWeight = (decimal)1.8
                            }
                        ]
                    },

                    new Container
                    {
                        LoadId = "LOAD002",
                        ContainerType = "40HC",
                        Items =
                        [
                            new Item
                            {
                                ItemCode = "ITEM003",
                                Quantity = 20,
                                CartonWeight = 3
                            }
                        ]
                    }
                ]
            };

            // Act
            var result = DispatchEventToThirdPartyCsvRecord.ConvertToCsvFormat(dispatchEvent);
            // Assert
            Assert.Equal(3, result.Count);

            Assert.Equal("SO123456", result[0].CustomerReference);
            Assert.Equal("LOAD001", result[0].LoadId);
            Assert.Equal("REF20", result[0].ContainerType);
            Assert.Equal("ITEM001", result[0].ItemCode);
            Assert.Equal(10, result[0].ItemQuantity);
            Assert.Equal((decimal)2.5, result[0].ItemWeight);
            Assert.Equal("35 Fake Street", result[0].Street);
            Assert.Equal("Fake City", result[0].City);
            Assert.Equal("Fake State", result[0].State);
            Assert.Equal("12345", result[0].PostalCode);
            Assert.Equal("Fake Country", result[0].Country);

            Assert.Equal("SO123456", result[1].CustomerReference);
            Assert.Equal("LOAD001", result[1].LoadId);
            Assert.Equal("REF20", result[1].ContainerType);
            Assert.Equal("ITEM002", result[1].ItemCode);
            Assert.Equal(5, result[1].ItemQuantity);
            Assert.Equal((decimal)1.8, result[1].ItemWeight);
            Assert.Equal("35 Fake Street", result[1].Street);
            Assert.Equal("Fake City", result[1].City);
            Assert.Equal("Fake State", result[1].State);
            Assert.Equal("12345", result[1].PostalCode);
            Assert.Equal("Fake Country", result[1].Country);

            Assert.Equal("SO123456", result[2].CustomerReference);
            Assert.Equal("LOAD002", result[2].LoadId);
            Assert.Equal("HC40", result[2].ContainerType);
            Assert.Equal("ITEM003", result[2].ItemCode);
            Assert.Equal(20, result[2].ItemQuantity);
            Assert.Equal(3, result[2].ItemWeight);
            Assert.Equal("35 Fake Street", result[2].Street);
            Assert.Equal("Fake City", result[2].City);
            Assert.Equal("Fake State", result[2].State);
            Assert.Equal("12345", result[2].PostalCode);
            Assert.Equal("Fake Country", result[2].Country);
            }

        [Fact]
        public void ConvertToCsvFormat_UnknownContainerType_ThrowsArgumentException()
        {
            // Arrange
            var dispatchEvent = new DispatchEvent
            {
                SalesOrder = "SO12345",
                DeliveryAddress = new DeliveryAddress()
                {
                    Street = "35 Fake Street",
                    City = "Fake City",
                    State = "Fake State",
                    PostalCode = "12345",
                    Country = "Fake Country"
                },
                Containers = new List<Container>
            {
                new Container
                {
                    LoadId = "LOAD1",
                    ContainerType = "UNKNOWN",
                    Items = new List<Item>
                    {
                        new Item { ItemCode = "ITEM1", Quantity = 10, CartonWeight = (decimal)5.5 }
                    }
                }
            }
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => DispatchEventToThirdPartyCsvRecord.ConvertToCsvFormat(dispatchEvent));
        }
    }
}
