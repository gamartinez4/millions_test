using Microsoft.Extensions.Configuration;
using Moq;
using Million.Application.Contracts;
using Million.Application.DTOs;
using Million.Application.Services;
using Million.Domain.Models;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Million.Tests.Application
{
    public class PropertyServiceTests
    {
        private readonly Mock<IPropertyRepository> _mockPropertyRepository;
        private readonly PropertyService _propertyService;

        public PropertyServiceTests()
        {
            _mockPropertyRepository = new Mock<IPropertyRepository>();
            _propertyService = new PropertyService(_mockPropertyRepository.Object);
        }

        [Fact]
        public async Task GetPropertyByIdAsync_ShouldReturnProperty_WhenPropertyExists()
        {
            // Arrange
            var property = new Property("Test Property", "123 Main St", 100000m, 2020, 1) { Id = 1 };
            _mockPropertyRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(property);

            // Act
            var result = await _propertyService.GetPropertyByIdAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.Name.ShouldBe("Test Property");
        }

        [Fact]
        public async Task CreatePropertyAsync_ShouldCreateAndReturnProperty()
        {
            // Arrange
            var propertyRequest = new PropertyRequest { Name = "New Property", Address = "456 Oak Ave", Price = 150000m, Year = 2021, OwnerId = 1 };
            var property = new Property("New Property", "456 Oak Ave", 150000m, 2021, 1) { Id = 2 };

            _mockPropertyRepository.Setup(repo => repo.AddAsync(It.IsAny<Property>())).Returns(Task.CompletedTask).Callback<Property>(p => p.Id = 2);

            // Act
            var result = await _propertyService.CreatePropertyAsync(propertyRequest);

            // Assert
            result.ShouldNotBeNull();
            result.Name.ShouldBe("New Property");
            result.Id.ShouldBe(2);
            _mockPropertyRepository.Verify(repo => repo.AddAsync(It.IsAny<Property>()), Times.Once);
        }

        [Fact]
        public async Task UpdatePropertyForSaleAsync_ShouldUpdateForSaleStatus_WhenPropertyExists()
        {
            // Arrange
            var property = new Property("Test Property", "123 Main St", 100000m, 2020, 1) { Id = 1 };
            var updateRequest = new UpdatePropertyForSaleRequest { ForSale = true };
            
            _mockPropertyRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(property);

            // Act
            await _propertyService.UpdatePropertyForSaleAsync(1, updateRequest);

            // Assert
            property.ForSale.ShouldBe(true);
            _mockPropertyRepository.Verify(repo => repo.GetByIdAsync(1), Times.Once);
            _mockPropertyRepository.Verify(repo => repo.Update(property), Times.Once);
        }

        [Fact]
        public async Task UpdatePropertyForSaleAsync_ShouldThrowArgumentException_WhenPropertyNotFound()
        {
            // Arrange
            var updateRequest = new UpdatePropertyForSaleRequest { ForSale = true };
            _mockPropertyRepository.Setup(repo => repo.GetByIdAsync(999)).ReturnsAsync((Property?)null);

            // Act & Assert
            var exception = await Should.ThrowAsync<ArgumentException>(
                () => _propertyService.UpdatePropertyForSaleAsync(999, updateRequest)
            );
            exception.Message.ShouldContain("Property with ID 999 not found.");
        }

        [Fact]
        public async Task UpdatePropertyForSaleAsync_ShouldSetForSaleToFalse_WhenRequestedToTakeOffMarket()
        {
            // Arrange
            var property = new Property("Test Property", "123 Main St", 100000m, 2020, null, true) { Id = 1 };
            var updateRequest = new UpdatePropertyForSaleRequest { ForSale = false };
            
            _mockPropertyRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(property);

            // Act
            await _propertyService.UpdatePropertyForSaleAsync(1, updateRequest);

            // Assert
            property.ForSale.ShouldBe(false);
            _mockPropertyRepository.Verify(repo => repo.Update(property), Times.Once);
        }

        [Fact]
        public async Task UpdatePropertyOwnerAsync_ShouldUpdateOwnerId_WhenPropertyExists()
        {
            // Arrange
            var property = new Property("Test Property", "123 Main St", 100000m, 2020, 1) { Id = 1 };
            var updateRequest = new UpdatePropertyOwnerRequest { OwnerId = 5 };
            
            _mockPropertyRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(property);

            // Act
            await _propertyService.UpdatePropertyOwnerAsync(1, updateRequest);

            // Assert
            property.OwnerId.ShouldBe(5);
            _mockPropertyRepository.Verify(repo => repo.GetByIdAsync(1), Times.Once);
            _mockPropertyRepository.Verify(repo => repo.Update(property), Times.Once);
        }

        [Fact]
        public async Task UpdatePropertyOwnerAsync_ShouldThrowArgumentException_WhenPropertyNotFound()
        {
            // Arrange
            var updateRequest = new UpdatePropertyOwnerRequest { OwnerId = 5 };
            _mockPropertyRepository.Setup(repo => repo.GetByIdAsync(999)).ReturnsAsync((Property?)null);

            // Act & Assert
            var exception = await Should.ThrowAsync<ArgumentException>(
                () => _propertyService.UpdatePropertyOwnerAsync(999, updateRequest)
            );
            exception.Message.ShouldContain("Property with ID 999 not found.");
        }

        [Fact]
        public async Task UpdatePropertyOwnerAsync_ShouldSetOwnerIdToNull_WhenRequestedToRemoveOwner()
        {
            // Arrange
            var property = new Property("Test Property", "123 Main St", 100000m, 2020, 1) { Id = 1 };
            var updateRequest = new UpdatePropertyOwnerRequest { OwnerId = null };
            
            _mockPropertyRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(property);

            // Act
            await _propertyService.UpdatePropertyOwnerAsync(1, updateRequest);

            // Assert
            property.OwnerId.ShouldBe(null);
            _mockPropertyRepository.Verify(repo => repo.Update(property), Times.Once);
        }

        [Fact]
        public async Task UpdatePropertyOwnerAsync_ShouldChangeOwnerFromOneToAnother()
        {
            // Arrange
            var property = new Property("Test Property", "123 Main St", 100000m, 2020, 1) { Id = 1 };
            var updateRequest = new UpdatePropertyOwnerRequest { OwnerId = 3 };
            
            _mockPropertyRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(property);

            // Act
            await _propertyService.UpdatePropertyOwnerAsync(1, updateRequest);

            // Assert
            property.OwnerId.ShouldBe(3);
            _mockPropertyRepository.Verify(repo => repo.Update(property), Times.Once);
        }

        [Fact]
        public async Task UpdatePropertyPriceAsync_ShouldUpdatePrice_WhenPropertyExists()
        {
            // Arrange
            var property = new Property("Test Property", "123 Main St", 100000m, 2020, 1) { Id = 1 };
            var updateRequest = new UpdatePropertyPriceRequest { Price = 150000m };

            _mockPropertyRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(property);

            // Act
            await _propertyService.UpdatePropertyPriceAsync(1, updateRequest);

            // Assert
            property.Price.ShouldBe(150000m);
            _mockPropertyRepository.Verify(repo => repo.GetByIdAsync(1), Times.Once);
            _mockPropertyRepository.Verify(repo => repo.Update(property), Times.Once);
        }

        [Fact]
        public async Task UpdatePropertyPriceAsync_ShouldThrowArgumentException_WhenPropertyNotFound()
        {
            // Arrange
            var updateRequest = new UpdatePropertyPriceRequest { Price = 150000m };
            _mockPropertyRepository.Setup(repo => repo.GetByIdAsync(999)).ReturnsAsync((Property?)null);

            // Act & Assert
            var exception = await Should.ThrowAsync<ArgumentException>(
                () => _propertyService.UpdatePropertyPriceAsync(999, updateRequest)
            );
            exception.Message.ShouldBe("Property with ID 999 not found.");
        }

        [Fact]
        public async Task UpdatePropertyPriceAsync_ShouldUpdateToZero_WhenRequestedPriceIsZero()
        {
            // Arrange
            var property = new Property("Test Property", "123 Main St", 100000m, 2020, 1) { Id = 1 };
            var updateRequest = new UpdatePropertyPriceRequest { Price = 0m };

            _mockPropertyRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(property);

            // Act
            await _propertyService.UpdatePropertyPriceAsync(1, updateRequest);

            // Assert
            property.Price.ShouldBe(0m);
            _mockPropertyRepository.Verify(repo => repo.GetByIdAsync(1), Times.Once);
            _mockPropertyRepository.Verify(repo => repo.Update(property), Times.Once);
        }

        [Fact]
        public async Task UpdatePropertyPriceAsync_ShouldUpdateToHigherPrice_WhenIncreasingValue()
        {
            // Arrange
            var property = new Property("Test Property", "123 Main St", 100000m, 2020, 1) { Id = 1 };
            var updateRequest = new UpdatePropertyPriceRequest { Price = 250000m };

            _mockPropertyRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(property);

            // Act
            await _propertyService.UpdatePropertyPriceAsync(1, updateRequest);

            // Assert
            property.Price.ShouldBe(250000m);
            _mockPropertyRepository.Verify(repo => repo.GetByIdAsync(1), Times.Once);
            _mockPropertyRepository.Verify(repo => repo.Update(property), Times.Once);
        }

        [Fact]
        public async Task GetAllPropertiesAsync_ShouldReturnAllProperties()
        {
            // Arrange
            var properties = new List<Property>
            {
                new Property("Property 1", "123 Main St", 100000m, 2020, 1) { Id = 1 },
                new Property("Property 2", "456 Oak Ave", 150000m, 2021, 2) { Id = 2 }
            };
            _mockPropertyRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(properties);

            // Act
            var result = await _propertyService.GetAllPropertiesAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(2);
            result.First().Id.ShouldBe(1);
            result.Last().Id.ShouldBe(2);
        }

        [Fact]
        public async Task GetPropertyByIdAsync_ShouldReturnNull_WhenPropertyNotExists()
        {
            // Arrange
            _mockPropertyRepository.Setup(repo => repo.GetByIdAsync(999)).ReturnsAsync((Property?)null);

            // Act
            var result = await _propertyService.GetPropertyByIdAsync(999);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task UpdatePropertyAsync_ShouldUpdateProperty_WhenPropertyExists()
        {
            // Arrange
            var property = new Property("Old Property", "123 Main St", 100000m, 2020, 1) { Id = 1 };
            var updateRequest = new PropertyRequest
            {
                Name = "Updated Property",
                Address = "456 Updated St",
                Price = 200000m,
                Year = 2022,
                OwnerId = 2,
                ForSale = true
            };
            _mockPropertyRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(property);

            // Act
            await _propertyService.UpdatePropertyAsync(1, updateRequest);

            // Assert
            property.Name.ShouldBe("Updated Property");
            property.Address.ShouldBe("456 Updated St");
            property.Price.ShouldBe(200000m);
            property.Year.ShouldBe(2022);
            property.OwnerId.ShouldBe(2);
            property.ForSale.ShouldBe(true);
            _mockPropertyRepository.Verify(repo => repo.Update(property), Times.Once);
        }

        [Fact]
        public async Task UpdatePropertyAsync_ShouldNotUpdate_WhenPropertyNotExists()
        {
            // Arrange
            var updateRequest = new PropertyRequest { Name = "Updated Property" };
            _mockPropertyRepository.Setup(repo => repo.GetByIdAsync(999)).ReturnsAsync((Property?)null);

            // Act
            await _propertyService.UpdatePropertyAsync(999, updateRequest);

            // Assert
            _mockPropertyRepository.Verify(repo => repo.Update(It.IsAny<Property>()), Times.Never);
        }

        [Fact]
        public async Task CreatePropertyAsync_ShouldSetForSaleToTrue_WhenOwnerIdIsNull()
        {
            // Arrange
            var propertyRequest = new PropertyRequest 
            { 
                Name = "No Owner Property", 
                Address = "789 No Owner St", 
                Price = 120000m, 
                Year = 2021, 
                OwnerId = null 
            };
            _mockPropertyRepository.Setup(repo => repo.AddAsync(It.IsAny<Property>()))
                                  .Returns(Task.CompletedTask)
                                  .Callback<Property>(p => p.Id = 3);

            // Act
            var result = await _propertyService.CreatePropertyAsync(propertyRequest);

            // Assert
            result.ShouldNotBeNull();
            result.ForSale.ShouldBe(true); // Should be true when no owner
            result.OwnerId.ShouldBe(string.Empty);
        }

        [Fact]
        public async Task CreatePropertyAsync_ShouldSetForSaleFromRequest_WhenSpecified()
        {
            // Arrange
            var propertyRequest = new PropertyRequest 
            { 
                Name = "Explicit ForSale Property", 
                Address = "321 Explicit St", 
                Price = 180000m, 
                Year = 2023, 
                OwnerId = 1,
                ForSale = false
            };
            _mockPropertyRepository.Setup(repo => repo.AddAsync(It.IsAny<Property>()))
                                  .Returns(Task.CompletedTask)
                                  .Callback<Property>(p => p.Id = 4);

            // Act
            var result = await _propertyService.CreatePropertyAsync(propertyRequest);

            // Assert
            result.ShouldNotBeNull();
            result.ForSale.ShouldBe(false); // Should respect explicit value
            result.OwnerId.ShouldBe("1");
        }

        [Fact]
        public async Task UpdatePropertyAsync_ShouldSetForSaleBasedOnOwnerId_WhenForSaleNotSpecified()
        {
            // Arrange
            var property = new Property("Test Property", "123 Main St", 100000m, 2020, 1) { Id = 1 };
            var updateRequest = new PropertyRequest
            {
                Name = "Updated Property",
                Address = "456 Updated St",
                Price = 200000m,
                Year = 2022,
                OwnerId = null, // No owner
                ForSale = null  // Not specified
            };
            _mockPropertyRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(property);

            // Act
            await _propertyService.UpdatePropertyAsync(1, updateRequest);

            // Assert
            property.ForSale.ShouldBe(true); // Should be true when no owner and not specified
            _mockPropertyRepository.Verify(repo => repo.Update(property), Times.Once);
        }
    }
} 