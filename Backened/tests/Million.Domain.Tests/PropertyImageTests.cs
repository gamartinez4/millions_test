using Million.Domain.Models;
using NUnit.Framework;
using System;

namespace Million.Domain.Tests
{
    [TestFixture]
    public class PropertyImageTests
    {
        [Test]
        public void PropertyImage_Creation_Should_Succeed_With_Valid_Data()
        {
            // Arrange
            var propertyId = 1;
            var file = "image.jpg";
            var enabled = true;

            // Act
            var propertyImage = new PropertyImage(propertyId, file, enabled);

            // Assert
            Assert.AreEqual(propertyId, propertyImage.PropertyId);
            Assert.AreEqual(file, propertyImage.File);
            Assert.AreEqual(enabled, propertyImage.Enabled);
        }

        [Test]
        public void PropertyImage_Creation_Should_Fail_With_Invalid_PropertyId()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new PropertyImage(0, "image.jpg", true));
        }

        [Test]
        public void PropertyImage_Creation_Should_Fail_With_Empty_File()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new PropertyImage(1, "", true));
        }
    }
} 