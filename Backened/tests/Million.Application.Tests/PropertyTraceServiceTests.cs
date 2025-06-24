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
    public class PropertyTraceServiceTests
    {
        private Mock<IPropertyTraceRepository> _mockTraceRepository;
        private IPropertyTraceService _traceService;

        [SetUp]
        public void Setup()
        {
            _mockTraceRepository = new Mock<IPropertyTraceRepository>();
            _traceService = new PropertyTraceService(_mockTraceRepository.Object);
        }

        [Test]
        public async Task GetAllTracesAsync_Should_Return_All_Traces()
        {
            // Arrange
            var traces = new List<PropertyTrace>
            {
                new PropertyTrace(1, new System.DateTime(2023, 1, 1), "Trace 1", 1000m, 100m, 1),
                new PropertyTrace(2, new System.DateTime(2023, 1, 2), "Trace 2", 2000m, 200m, 1)
            };
            _mockTraceRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(traces);

            // Act
            var result = await _traceService.GetAllTracesAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.First().Name, Is.EqualTo("Trace 1"));
        }
    }
} 