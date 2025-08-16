using Moq;
using Shouldly;
using Million.Application.Services;
using Million.Application.Contracts;
using Million.Domain.Models;
using System.Threading.Tasks;
using Xunit;
using Million.Application.DTOs;
using System.Collections.Generic;
using System.Linq.Expressions;
using System;

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
            result.Enabled.ShouldBe(true);
            result.PropertyId.ShouldBe(1);
            _mockImageRepository.Verify(repo => repo.AddAsync(It.IsAny<PropertyImage>()), Times.Once);
        }

        [Fact]
        public async Task GetImageByIdAsync_ShouldReturnImage_WhenImageExists()
        {
            // Arrange
            var image = new PropertyImage(1, "http://example.com/img.png", true) { Id = 1 };
            _mockImageRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(image);

            // Act
            var result = await _imageService.GetImageByIdAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.FileUrl.ShouldBe("http://example.com/img.png");
            result.Enabled.ShouldBe(true);
            result.PropertyId.ShouldBe(1);
        }

        [Fact]
        public async Task GetImageByIdAsync_ShouldReturnNull_WhenImageNotExists()
        {
            // Arrange
            _mockImageRepository.Setup(repo => repo.GetByIdAsync(999)).ReturnsAsync((PropertyImage?)null);

            // Act
            var result = await _imageService.GetImageByIdAsync(999);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetAllImagesAsync_ShouldReturnAllImages()
        {
            // Arrange
            var images = new List<PropertyImage>
            {
                new PropertyImage(1, "http://example.com/img1.png", true) { Id = 1 },
                new PropertyImage(2, "http://example.com/img2.png", false) { Id = 2 }
            };
            _mockImageRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(images);

            // Act
            var result = await _imageService.GetAllImagesAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(2);
            result.First().Id.ShouldBe(1);
            result.Last().Id.ShouldBe(2);
        }

        [Fact]
        public async Task GetImagesForPropertyAsync_ShouldReturnImagesForSpecificProperty()
        {
            // Arrange
            var images = new List<PropertyImage>
            {
                new PropertyImage(1, "http://example.com/img1.png", true) { Id = 1 },
                new PropertyImage(1, "http://example.com/img2.png", true) { Id = 2 }
            };
            _mockImageRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<PropertyImage, bool>>>()))
                               .ReturnsAsync(images);

            // Act
            var result = await _imageService.GetImagesForPropertyAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(2);
            result.All(img => img.PropertyId == 1).ShouldBeTrue();
        }

        [Fact]
        public async Task UpdateImageAsync_ShouldUpdateImage_WhenImageExists()
        {
            // Arrange
            var image = new PropertyImage(1, "http://example.com/old.png", false) { Id = 1 };
            var updateRequest = new PropertyImageRequest 
            { 
                PropertyId = 2, 
                FileUrl = "http://example.com/new.png", 
                Enabled = true 
            };
            _mockImageRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(image);

            // Act
            await _imageService.UpdateImageAsync(1, updateRequest);

            // Assert
            image.PropertyId.ShouldBe(2);
            image.File.ShouldBe("http://example.com/new.png");
            image.Enabled.ShouldBe(true);
            _mockImageRepository.Verify(repo => repo.Update(image), Times.Once);
        }

        [Fact]
        public async Task UpdateImageAsync_ShouldNotUpdate_WhenImageNotExists()
        {
            // Arrange
            var updateRequest = new PropertyImageRequest 
            { 
                PropertyId = 2, 
                FileUrl = "http://example.com/new.png", 
                Enabled = true 
            };
            _mockImageRepository.Setup(repo => repo.GetByIdAsync(999)).ReturnsAsync((PropertyImage?)null);

            // Act
            await _imageService.UpdateImageAsync(999, updateRequest);

            // Assert
            _mockImageRepository.Verify(repo => repo.Update(It.IsAny<PropertyImage>()), Times.Never);
        }

        [Fact]
        public async Task DeleteImageAsync_ShouldDeleteImage_WhenImageExists()
        {
            // Arrange
            var image = new PropertyImage(1, "http://example.com/img.png", true) { Id = 1 };
            _mockImageRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(image);

            // Act
            await _imageService.DeleteImageAsync(1);

            // Assert
            _mockImageRepository.Verify(repo => repo.Remove(image), Times.Once);
        }

        [Fact]
        public async Task DeleteImageAsync_ShouldNotDelete_WhenImageNotExists()
        {
            // Arrange
            _mockImageRepository.Setup(repo => repo.GetByIdAsync(999)).ReturnsAsync((PropertyImage?)null);

            // Act
            await _imageService.DeleteImageAsync(999);

            // Assert
            _mockImageRepository.Verify(repo => repo.Remove(It.IsAny<PropertyImage>()), Times.Never);
        }
    }
} 