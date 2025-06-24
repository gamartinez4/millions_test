using Moq;
using Shouldly;
using Million.Application.Services;
using Million.Application.Contracts;
using Million.Domain.Models;
using System.Threading.Tasks;
using Xunit;
using Million.Application.DTOs;

namespace Million.Tests.Application
{
    public class PropertyImageServiceTests
    {
        private readonly Mock<IPropertyImageRepository> _mockImageRepository;
        private readonly PropertyImageService _imageService;

        public PropertyImageServiceTests()
        {
            _mockImageRepository = new Mock<IPropertyImageRepository>();
            _imageService = new PropertyImageService(_mockImageRepository.Object);
        }

        [Fact]
        public async Task AddImageToPropertyAsync_ShouldAddAndReturnImage()
        {
            // Arrange
            var imageRequest = new PropertyImageRequest { PropertyId = 1, FileUrl = "http://example.com/img.png", Enabled = true };
            _mockImageRepository.Setup(repo => repo.AddAsync(It.IsAny<PropertyImage>()))
                                .Returns(Task.CompletedTask)
                                .Callback<PropertyImage>(pi => pi.Id = 1);

            // Act
            var result = await _imageService.AddImageToPropertyAsync(imageRequest);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.FileUrl.ShouldBe("http://example.com/img.png");
            _mockImageRepository.Verify(repo => repo.AddAsync(It.IsAny<PropertyImage>()), Times.Once);
        }
    }
} 