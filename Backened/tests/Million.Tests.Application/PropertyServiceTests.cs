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
    }
} 