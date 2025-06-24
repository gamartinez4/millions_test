using Moq;
using Shouldly;
using Million.Application.Services;
using Million.Application.Contracts;
using Million.Domain.Models;
using System.Threading.Tasks;
using Xunit;
using Million.Application.DTOs;
using System;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Million.Tests.Application
{
    public class PropertyTraceServiceTests
    {
        private readonly Mock<IPropertyTraceRepository> _mockTraceRepository;
        private readonly PropertyTraceService _traceService;

        public PropertyTraceServiceTests()
        {
            _mockTraceRepository = new Mock<IPropertyTraceRepository>();
            _traceService = new PropertyTraceService(_mockTraceRepository.Object);
        }

        [Fact]
        public async Task AddTraceToPropertyAsync_ShouldAddAndReturnTrace()
        {
            // Arrange
            var traceRequest = new PropertyTraceRequest { PropertyId = 1, Name = "Sale", Value = 200000m, Tax = 20000m };
            _mockTraceRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<PropertyTrace, bool>>>()))
                                .ReturnsAsync(new List<PropertyTrace>());
            _mockTraceRepository.Setup(repo => repo.AddAsync(It.IsAny<PropertyTrace>()))
                                .Returns(Task.CompletedTask)
                                .Callback<PropertyTrace>(pt => pt.Id = 1);

            // Act
            var result = await _traceService.AddTraceToPropertyAsync(traceRequest);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.Name.ShouldBe("Sale");
            _mockTraceRepository.Verify(repo => repo.AddAsync(It.IsAny<PropertyTrace>()), Times.Once);
        }

        [Fact]
        public async Task AddTraceToPropertyAsync_ShouldThrowException_WhenTraceExists()
        {
            // Arrange
            var traceRequest = new PropertyTraceRequest { PropertyId = 1, Name = "Existing Sale" };
            var existingTrace = new PropertyTrace(1, DateTime.UtcNow, "Existing Sale", 150000m, 15000m, 1);
            _mockTraceRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<PropertyTrace, bool>>>()))
                                .ReturnsAsync(new List<PropertyTrace> { existingTrace });

            // Act & Assert
            await Should.ThrowAsync<InvalidOperationException>(async () =>
            {
                await _traceService.AddTraceToPropertyAsync(traceRequest);
            });
        }
    }
} 