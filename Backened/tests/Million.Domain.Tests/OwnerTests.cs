using Million.Domain.Models;
using NUnit.Framework;
using System;

namespace Million.Domain.Tests
{
    [TestFixture]
    public class OwnerTests
    {
        [Test]
        public void Owner_Creation_Should_Succeed_With_Valid_Data()
        {
            // Arrange
            var name = "John Doe";
            var address = "123 Main St";
            var photo = "photo.jpg";
            var birthday = new DateTime(1990, 1, 1);

            // Act
            var owner = new Owner(name, address, photo, birthday);

            // Assert
            Assert.AreEqual(name, owner.Name);
            Assert.AreEqual(address, owner.Address);
            Assert.AreEqual(photo, owner.Photo);
            Assert.AreEqual(birthday, owner.Birthday);
        }

        [Test]
        public void Owner_Creation_Should_Fail_With_Empty_Name()
        {
            // Arrange
            var address = "123 Main St";
            var photo = "photo.jpg";
            var birthday = new DateTime(1990, 1, 1);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Owner("", address, photo, birthday));
        }

        [Test]
        public void Owner_Creation_Should_Fail_With_Empty_Address()
        {
            // Arrange
            var name = "John Doe";
            var photo = "photo.jpg";
            var birthday = new DateTime(1990, 1, 1);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Owner(name, "", photo, birthday));
        }
    }
} 