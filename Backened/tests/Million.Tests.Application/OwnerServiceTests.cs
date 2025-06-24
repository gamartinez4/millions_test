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
        }
    }
} 