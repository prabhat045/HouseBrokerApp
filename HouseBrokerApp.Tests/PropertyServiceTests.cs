using Moq;
using HouseBrokerApp.Application.Services;
using HouseBrokerApp.Application.DTO;
using HouseBrokerApp.Application.Manager;
using HouseBrokerApp.Application.Service;
using HouseBrokerApp.Domain.Enum;
using HouseBrokerApp.Domain.Entities;
using MockQueryable;

namespace HouseBrokerApp.Tests
{
    public class PropertyServiceTests
    {
        private readonly Mock<IPropertyRepository> _propertyRepositoryMock;
        private readonly IPropertyService _propertyService;

        public PropertyServiceTests()
        {
            _propertyRepositoryMock = new Mock<IPropertyRepository>();
            _propertyService = new PropertyService(_propertyRepositoryMock.Object);
        }

        [Fact]
        public async Task GetAllPropertiesAsync_ShouldReturnListOfProperties()
        {
            // Arrange
            var properties = new List<Property>
            {
                new Property
                {
                    Id = "1",
                    Title = "Property 1",
                    Description = "Description 1",
                    Price = 100000,
                    Location = "Location 1",
                    PropertyType = PropertyType.Apartment,
                    BrokerId = "broker1",
                    Images = new List<PropertyImage>
                    {
                        new PropertyImage { Id = "img1", ImageUrl = "http://example.com/img1.jpg" }
                    },
                    Features = new List<PropertyFeature>
                    {
                        new PropertyFeature { Id = "feat1", FeatureName = "Feature1" }
                    },
                    Broker = new ApplicationUser
                    {
                        Id = "broker1",
                        FullName = "Broker One",
                        Email = "broker1@example.com",
                        PhoneNumber = "1111111111"
                    }
                },
                new Property
                {
                    Id = "2",
                    Title = "Property 2",
                    Description = "Description 2",
                    Price = 200000,
                    Location = "Location 2",
                    PropertyType = PropertyType.House,
                    BrokerId = "broker2",
                    Images = new List<PropertyImage>(),
                    Features = new List<PropertyFeature>(),
                    Broker = new ApplicationUser
                    {
                        Id = "broker2",
                        FullName = "Broker Two",
                        Email = "broker2@example.com",
                        PhoneNumber = "2222222222"
                    }
                }
            };

            var mockQueryable = properties.AsQueryable().BuildMock();
            _propertyRepositoryMock.Setup(repo => repo.GetAll())
                .Returns(mockQueryable);

            var result = await _propertyService.GetAllPropertiesAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal("Property 1", result.First().Title);
        }

        [Fact]
        public async Task GetPropertyByIdAsync_ShouldThrowException_WhenPropertyNotFound()
        {
            _propertyRepositoryMock.Setup(repo => repo.GetByIdAsync("nonexistent"))
                .ReturnsAsync((Property)null);

            await Assert.ThrowsAsync<Exception>(() => _propertyService.GetPropertyByIdAsync("nonexistent"));
        }

        [Fact]
        public async Task CreatePropertyAsync_ShouldReturnCreatedPropertyDto()
        {
            var propertyDto = new PropertyDto
            {
                Title = "New Property",
                Description = "New Description",
                Price = 300000,
                Location = "New Location",
                PropertyType = "House"
            };
            string brokerId = "broker1";
            var property = propertyDto.ToEntity(brokerId);
            property.Id = "newPropertyId";

            _propertyRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Property>()))
                .Returns(Task.CompletedTask)
                .Callback<Property>(p => p.Id = "newPropertyId");

            var result = await _propertyService.CreatePropertyAsync(propertyDto, brokerId);

            Assert.NotNull(result);
            Assert.Equal("newPropertyId", result.Id);
            Assert.Equal(brokerId, result.BrokerId);
        }

        [Fact]
        public async Task UpdatePropertyAsync_ShouldThrowException_WhenPropertyDoesNotExist()
        {
            var propertyDto = new PropertyDto
            {
                Id = "invalidId",
                Title = "Updated Property",
                Description = "Updated Description",
                Price = 350000,
                Location = "Updated Location",
                PropertyType = "House",
                Features = new List<string> { "Feature1", "Feature2" }
            };

            _propertyRepositoryMock.Setup(repo => repo.GetByIdAsync("invalidId"))
                .ReturnsAsync((Property)null);

            await Assert.ThrowsAsync<Exception>(() => _propertyService.UpdatePropertyAsync(propertyDto));
        }

        [Fact]
        public async Task UpdatePropertyAsync_ShouldUpdatePropertySuccessfully()
        {
            var property = new Property
            {
                Id = "prop1",
                Title = "Old Title",
                Description = "Old Description",
                Price = 200000,
                Location = "Old Location",
                PropertyType = PropertyType.House,
                BrokerId = "broker1",
                Features = new List<PropertyFeature>()
            };
            var propertyDto = new PropertyDto(property)
            {
                Title = "New Title",
                Description = "New Description",
                Price = 250000,
                Location = "New Location",
                PropertyType = "House",
                Features = new List<string> { "NewFeature" }
            };

            _propertyRepositoryMock.Setup(repo => repo.GetByIdAsync("prop1"))
                .ReturnsAsync(property);
            _propertyRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Property>()))
                .Returns(Task.CompletedTask);

            var updateResult = await _propertyService.UpdatePropertyAsync(propertyDto);

            Assert.True(updateResult);
            _propertyRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<Property>(p => p.Title == "New Title")), Times.Once);
        }

        [Fact]
        public async Task DeletePropertyAsync_ShouldThrowException_WhenPropertyDoesNotExist()
        {
            _propertyRepositoryMock.Setup(repo => repo.GetByIdAsync("invalidId"))
                .ReturnsAsync((Property)null);

            await Assert.ThrowsAsync<Exception>(() => _propertyService.DeletePropertyAsync("invalidId", "broker1"));
        }

        [Fact]
        public async Task DeletePropertyAsync_ShouldDeleteProperty_WhenBrokerIsValid()
        {
            var property = new Property
            {
                Id = "prop1",
                Title = "Property 1",
                BrokerId = "broker1"
            };

            _propertyRepositoryMock.Setup(repo => repo.GetByIdAsync("prop1"))
                .ReturnsAsync(property);
            _propertyRepositoryMock.Setup(repo => repo.DeleteAsync("prop1"))
                .Returns(Task.CompletedTask);

            var result = await _propertyService.DeletePropertyAsync("prop1", "broker1");

            Assert.True(result);
        }

        [Fact]
        public async Task AddImagesAsync_ShouldThrowException_WhenPropertyNotFound()
        {
            _propertyRepositoryMock.Setup(repo => repo.GetByIdAsync("invalidId"))
                .ReturnsAsync((Property)null);

            await Assert.ThrowsAsync<Exception>(() => _propertyService.AddImagesAsync("invalidId", new List<string> { "http://image.url" }, "broker1"));
        }

        [Fact]
        public async Task DeleteImageAsync_ShouldThrowException_WhenImageNotFound()
        {
            var property = new Property
            {
                Id = "prop1",
                BrokerId = "broker1",
                Images = new List<PropertyImage>()
            };

            _propertyRepositoryMock.Setup(repo => repo.GetByIdAsync("prop1"))
                .ReturnsAsync(property);

            await Assert.ThrowsAsync<Exception>(() => _propertyService.DeleteImageAsync("prop1", "nonexistentImage", "broker1"));
        }

        [Fact]
        public async Task DeleteImageAsync_ShouldDeleteImage_WhenFound()
        {
            var image = new PropertyImage { Id = "img1", ImageUrl = "http://example.com/img1.jpg", PropertyId = "prop1" };
            var property = new Property
            {
                Id = "prop1",
                BrokerId = "broker1",
                Images = new List<PropertyImage> { image }
            };

            _propertyRepositoryMock.Setup(repo => repo.GetByIdAsync("prop1"))
                .ReturnsAsync(property);
            _propertyRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Property>()))
                .Returns(Task.CompletedTask);

            var result = await _propertyService.DeleteImageAsync("prop1", "img1", "broker1");

            Assert.True(result);
            Assert.Empty(property.Images);
        }
    }
}
