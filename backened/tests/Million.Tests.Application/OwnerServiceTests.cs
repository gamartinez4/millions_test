using Moq;
using Shouldly;
using Million.Application.Services;
using Million.Application.Contracts;
using Million.Domain.Models;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Linq.Expressions;
using Million.Application.DTOs;

namespace Million.Tests.Application
{
    public class OwnerServiceTests
    {
        private readonly Mock<IOwnerRepository> _mockOwnerRepository;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly OwnerService _ownerService;

        public OwnerServiceTests()
        {
            _mockOwnerRepository = new Mock<IOwnerRepository>();
            _mockConfiguration = new Mock<IConfiguration>();

            // Mock IConfiguration
            var mockJwtSection = new Mock<IConfigurationSection>();
            mockJwtSection.Setup(s => s["Key"]).Returns("your-super-secret-key-that-is-long-enough");
            mockJwtSection.Setup(s => s["Issuer"]).Returns("your-issuer");
            mockJwtSection.Setup(s => s["Audience"]).Returns("your-audience");

            _mockConfiguration.Setup(c => c.GetSection("Jwt")).Returns(mockJwtSection.Object);

            _ownerService = new OwnerService(_mockOwnerRepository.Object, _mockConfiguration.Object);
        }

        [Fact]
        public async Task GetOwnerByIdAsync_ShouldReturnOwner_WhenOwnerExists()
        {
            // Arrange
            var owner = new Owner { Id = 1, Name = "Test Owner" };
            _mockOwnerRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(owner);

            // Act
            var result = await _ownerService.GetOwnerByIdAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.Name.ShouldBe("Test Owner");
        }

        [Fact]
        public async Task CreateOwnerAsync_ShouldCreateAndReturnOwner()
        {
            // Arrange
            var ownerRequest = new OwnerRequest { Name = "New Owner" };
            var owner = new Owner { Id = 2, Name = "New Owner" };

            _mockOwnerRepository.Setup(repo => repo.AddAsync(It.IsAny<Owner>())).Returns(Task.CompletedTask).Callback<Owner>(o => o.Id = 2);

            // Act
            var result = await _ownerService.CreateOwnerAsync(ownerRequest);

            // Assert
            result.ShouldNotBeNull();
            result.Name.ShouldBe("New Owner");
            result.Id.ShouldBe(2);
            _mockOwnerRepository.Verify(repo => repo.AddAsync(It.IsAny<Owner>()), Times.Once);
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldReturnLoginResponse_WhenCredentialsAreValid()
        {
            // Arrange
            var loginRequest = new OwnerLoginRequest { Username = "testuser", Password = "password" };
            var owner = new Owner { Id = 1, Username = "testuser", Password = "password" };
            _mockOwnerRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Owner, bool>>>()))
                                .ReturnsAsync(new List<Owner> { owner });

            // Act
            var result = await _ownerService.AuthenticateAsync(loginRequest);

            // Assert
            result.ShouldNotBeNull();
            result.Token.ShouldNotBeNullOrEmpty();
            result.Owner.Username.ShouldBe("testuser");
            result.Owner.Id.ShouldBe(1);
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldReturnNull_WhenUsernameNotFound()
        {
            // Arrange
            var loginRequest = new OwnerLoginRequest { Username = "nonexistent", Password = "password" };
            _mockOwnerRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Owner, bool>>>()))
                                .ReturnsAsync(new List<Owner>());

            // Act
            var result = await _ownerService.AuthenticateAsync(loginRequest);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldReturnNull_WhenPasswordIsIncorrect()
        {
            // Arrange
            var loginRequest = new OwnerLoginRequest { Username = "testuser", Password = "wrongpassword" };
            var owner = new Owner { Id = 1, Username = "testuser", Password = "password" };
            _mockOwnerRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Owner, bool>>>()))
                                .ReturnsAsync(new List<Owner> { owner });

            // Act
            var result = await _ownerService.AuthenticateAsync(loginRequest);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetAllOwnersAsync_ShouldReturnAllOwners()
        {
            // Arrange
            var owners = new List<Owner>
            {
                new Owner { Id = 1, Name = "Owner 1", Username = "user1" },
                new Owner { Id = 2, Name = "Owner 2", Username = "user2" }
            };
            _mockOwnerRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(owners);

            // Act
            var result = await _ownerService.GetAllOwnersAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(2);
            result.First().Id.ShouldBe(1);
            result.Last().Id.ShouldBe(2);
        }

        [Fact]
        public async Task GetOwnerByIdAsync_ShouldReturnNull_WhenOwnerNotExists()
        {
            // Arrange
            _mockOwnerRepository.Setup(repo => repo.GetByIdAsync(999)).ReturnsAsync((Owner?)null);

            // Act
            var result = await _ownerService.GetOwnerByIdAsync(999);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task UpdateOwnerAsync_ShouldUpdateOwner_WhenOwnerExists()
        {
            // Arrange
            var owner = new Owner { Id = 1, Name = "Old Name", Username = "olduser" };
            var updateRequest = new OwnerRequest 
            { 
                Name = "New Name", 
                Username = "newuser",
                Address = "New Address",
                Photo = "new-photo.jpg",
                Birthday = DateTime.UtcNow.AddYears(-25),
                Password = "newpassword"
            };
            _mockOwnerRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(owner);

            // Act
            await _ownerService.UpdateOwnerAsync(1, updateRequest);

            // Assert
            owner.Name.ShouldBe("New Name");
            owner.Username.ShouldBe("newuser");
            owner.Address.ShouldBe("New Address");
            owner.Photo.ShouldBe("new-photo.jpg");
            owner.Password.ShouldBe("newpassword");
            _mockOwnerRepository.Verify(repo => repo.Update(owner), Times.Once);
        }

        [Fact]
        public async Task UpdateOwnerAsync_ShouldNotUpdate_WhenOwnerNotExists()
        {
            // Arrange
            var updateRequest = new OwnerRequest { Name = "New Name" };
            _mockOwnerRepository.Setup(repo => repo.GetByIdAsync(999)).ReturnsAsync((Owner?)null);

            // Act
            await _ownerService.UpdateOwnerAsync(999, updateRequest);

            // Assert
            _mockOwnerRepository.Verify(repo => repo.Update(It.IsAny<Owner>()), Times.Never);
        }

        [Fact]
        public async Task DeleteOwnerAsync_ShouldDeleteOwner_WhenOwnerExists()
        {
            // Arrange
            var owner = new Owner { Id = 1, Name = "Test Owner" };
            _mockOwnerRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(owner);

            // Act
            await _ownerService.DeleteOwnerAsync(1);

            // Assert
            _mockOwnerRepository.Verify(repo => repo.Remove(owner), Times.Once);
        }

        [Fact]
        public async Task DeleteOwnerAsync_ShouldNotDelete_WhenOwnerNotExists()
        {
            // Arrange
            _mockOwnerRepository.Setup(repo => repo.GetByIdAsync(999)).ReturnsAsync((Owner?)null);

            // Act
            await _ownerService.DeleteOwnerAsync(999);

            // Assert
            _mockOwnerRepository.Verify(repo => repo.Remove(It.IsAny<Owner>()), Times.Never);
        }
        
        [Fact]
        public async Task CreateOwnerAsync_ShouldCreateOwnerWithAllFields()
        {
            // Arrange
            var ownerRequest = new OwnerRequest 
            { 
                Name = "Full Owner", 
                Address = "123 Main St",
                Photo = "photo.jpg",
                Birthday = DateTime.UtcNow.AddYears(-30),
                Username = "fulluser",
                Password = "fullpassword"
            };
            _mockOwnerRepository.Setup(repo => repo.AddAsync(It.IsAny<Owner>()))
                               .Returns(Task.CompletedTask)
                               .Callback<Owner>(o => o.Id = 3);

            // Act
            var result = await _ownerService.CreateOwnerAsync(ownerRequest);

            // Assert
            result.ShouldNotBeNull();
            result.Name.ShouldBe("Full Owner");
            result.Address.ShouldBe("123 Main St");
            result.Photo.ShouldBe("photo.jpg");
            result.Username.ShouldBe("fulluser");
            result.Id.ShouldBe(3);
            // Password should not be in response
            _mockOwnerRepository.Verify(repo => repo.AddAsync(It.IsAny<Owner>()), Times.Once);
        }
    }
} 