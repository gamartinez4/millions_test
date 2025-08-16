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
using System.Linq;

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
            result.Value.ShouldBe(200000m);
            result.Tax.ShouldBe(20000m);
            result.PropertyId.ShouldBe(1);
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
            var exception = await Should.ThrowAsync<InvalidOperationException>(async () =>
            {
                await _traceService.AddTraceToPropertyAsync(traceRequest);
            });
            exception.Message.ShouldBe("La propiedad ya fue comprada y no puede volver a comprarse.");
        }

        [Fact]
        public async Task GetTraceByIdAsync_ShouldReturnTrace_WhenTraceExists()
        {
            // Arrange
            var trace = new PropertyTrace(1, DateTime.UtcNow, "Sale", 200000m, 20000m, 1) { Id = 1 };
            _mockTraceRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(trace);

            // Act
            var result = await _traceService.GetTraceByIdAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.Name.ShouldBe("Sale");
            result.Value.ShouldBe(200000m);
            result.Tax.ShouldBe(20000m);
            result.PropertyId.ShouldBe(1);
        }

        [Fact]
        public async Task GetTraceByIdAsync_ShouldReturnNull_WhenTraceNotExists()
        {
            // Arrange
            _mockTraceRepository.Setup(repo => repo.GetByIdAsync(999)).ReturnsAsync((PropertyTrace)null);

            // Act
            var result = await _traceService.GetTraceByIdAsync(999);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetTracesForPropertyAsync_ShouldReturnTracesForSpecificProperty()
        {
            // Arrange
            var traces = new List<PropertyTrace>
            {
                new PropertyTrace(1, DateTime.UtcNow, "Sale", 200000m, 20000m, 1) { Id = 1 },
                new PropertyTrace(2, DateTime.UtcNow, "Transfer", 180000m, 18000m, 1) { Id = 2 }
            };
            _mockTraceRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<PropertyTrace, bool>>>()))
                               .ReturnsAsync(traces);

            // Act
            var result = await _traceService.GetTracesForPropertyAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(2);
            result.All(t => t.PropertyId == 1).ShouldBeTrue();
        }

        [Fact]
        public async Task GetAllTracesAsync_ShouldReturnAllTraces()
        {
            // Arrange
            var traces = new List<PropertyTrace>
            {
                new PropertyTrace(1, DateTime.UtcNow, "Sale", 200000m, 20000m, 1) { Id = 1 },
                new PropertyTrace(2, DateTime.UtcNow, "Transfer", 180000m, 18000m, 2) { Id = 2 }
            };
            _mockTraceRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(traces);

            // Act
            var result = await _traceService.GetAllTracesAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(2);
            result.First().Id.ShouldBe(1);
            result.Last().Id.ShouldBe(2);
        }

        [Fact]
        public async Task UpdateTraceAsync_ShouldUpdateTrace_WhenTraceExists()
        {
            // Arrange
            var trace = new PropertyTrace(1, DateTime.UtcNow, "Old Sale", 150000m, 15000m, 1) { Id = 1 };
            var updateRequest = new PropertyTraceRequest 
            { 
                Name = "New Sale", 
                Value = 250000m, 
                Tax = 25000m,
                PropertyId = 2,
                DateSale = DateTime.UtcNow.AddDays(1)
            };
            _mockTraceRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(trace);

            // Act
            await _traceService.UpdateTraceAsync(1, updateRequest);

            // Assert
            trace.Name.ShouldBe("New Sale");
            trace.Value.ShouldBe(250000m);
            trace.Tax.ShouldBe(25000m);
            trace.PropertyId.ShouldBe(2);
            _mockTraceRepository.Verify(repo => repo.Update(trace), Times.Once);
        }

        [Fact]
        public async Task UpdateTraceAsync_ShouldNotUpdate_WhenTraceNotExists()
        {
            // Arrange
            var updateRequest = new PropertyTraceRequest 
            { 
                Name = "New Sale", 
                Value = 250000m, 
                Tax = 25000m 
            };
            _mockTraceRepository.Setup(repo => repo.GetByIdAsync(999)).ReturnsAsync((PropertyTrace)null);

            // Act
            await _traceService.UpdateTraceAsync(999, updateRequest);

            // Assert
            _mockTraceRepository.Verify(repo => repo.Update(It.IsAny<PropertyTrace>()), Times.Never);
        }

        [Fact]
        public async Task DeleteTraceAsync_ShouldDeleteTrace_WhenTraceExists()
        {
            // Arrange
            var trace = new PropertyTrace(1, DateTime.UtcNow, "Sale", 200000m, 20000m, 1) { Id = 1 };
            _mockTraceRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(trace);

            // Act
            await _traceService.DeleteTraceAsync(1);

            // Assert
            _mockTraceRepository.Verify(repo => repo.Remove(trace), Times.Once);
        }

        [Fact]
        public async Task DeleteTraceAsync_ShouldNotDelete_WhenTraceNotExists()
        {
            // Arrange
            _mockTraceRepository.Setup(repo => repo.GetByIdAsync(999)).ReturnsAsync((PropertyTrace)null);

            // Act
            await _traceService.DeleteTraceAsync(999);

            // Assert
            _mockTraceRepository.Verify(repo => repo.Remove(It.IsAny<PropertyTrace>()), Times.Never);
        }
    }
} 