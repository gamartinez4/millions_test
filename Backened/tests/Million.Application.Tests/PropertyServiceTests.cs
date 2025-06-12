using Million.Application.Contracts;
using Million.Application.DTOs;
using Million.Application.Services;
using Million.Domain.Models;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Million.Application.Tests
{
    [TestFixture]
    public class PropertyServiceTests
    {
        private Mock<IPropertyRepository> _mockPropertyRepository;
        private IPropertyService _propertyService;

        [SetUp]
        public void Setup()
        {
            _mockPropertyRepository = new Mock<IPropertyRepository>();
            _propertyService = new PropertyService(_mockPropertyRepository.Object);
        }

        [Test]
        public async Task CreatePropertyAsync_Should_Create_And_Return_Property()
        {
            // Arrange
            var propertyRequest = new PropertyRequest { Name = "New House", Address = "789 Pine St", Price = 600000m, Year = 2022, OwnerId = 1 };

            // Act
            var result = await _propertyService.CreatePropertyAsync(propertyRequest);

            // Assert
            _mockPropertyRepository.Verify(repo => repo.AddAsync(It.IsAny<Property>()), Times.Once);
            Assert.IsNotNull(result);
            Assert.AreEqual(propertyRequest.Name, result.Name);
        }

        [Test]
        public async Task GetPropertyByIdAsync_Should_Return_Property_When_Exists()
        {
            // Arrange
            var property = new Property("Test House", "123 Test St", 50000m, 2021, 1) { Id = 1 };
            _mockPropertyRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(property);

            // Act
            var result = await _propertyService.GetPropertyByIdAsync(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(property.Name, result.Name);
        }

        [Test]
        public async Task GetPropertiesFilteredAsync_Should_Return_Filtered_Properties()
        {
            // Arrange
            var properties = new List<Property>
            {
                new Property("House 1", "Address 1", 100000m, 2020, 1),
                new Property("House 2", "Address 2", 200000m, 2021, 1),
                new Property("Mansion 1", "Address 3", 300000m, 2022, 1)
            };
            _mockPropertyRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(properties);

            // Act
            var result = await _propertyService.GetPropertiesFilteredAsync("House", null, null, null);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }
    }
} 