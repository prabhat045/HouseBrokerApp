using HouseBrokerApp.Application.DTO;
using HouseBrokerApp.Application.Manager;
using HouseBrokerApp.Application.Service;
using HouseBrokerApp.Domain.Entities;
using HouseBrokerApp.Domain.Enum;
using Microsoft.EntityFrameworkCore;
namespace HouseBrokerApp.Application.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _propertyRepository;

        public PropertyService(IPropertyRepository propertyRepository)
        {
            _propertyRepository = propertyRepository;
        }

        public async Task<List<PropertyDto>> GetAllPropertiesAsync()
        {
            var properties = await _propertyRepository.GetAll().Include(p=>p.Images).Include(p=>p.Features).ToListAsync().ConfigureAwait(false);
            return properties.Select(p => new PropertyDto(p)).ToList();
        }

        public async Task<PropertyDto> GetPropertyByIdAsync(string id)
        {
            var property = await _propertyRepository.GetByIdAsync(id).ConfigureAwait(false);
            if (property == null)
            {
                throw new Exception("Property Not found");
            }
            return new PropertyDto(property);
        }

        public async Task<PropertyDto> CreatePropertyAsync(PropertyDto propertyDto, string brokerId)
        {
            var property = propertyDto.ToEntity(brokerId);
            await _propertyRepository.AddAsync(property).ConfigureAwait(false);
            return new PropertyDto(property);
        }

        public async Task<bool> UpdatePropertyAsync(PropertyDto propertyDto)
        {
            var property = await _propertyRepository.GetByIdAsync(propertyDto.Id).ConfigureAwait(false);
            if (property == null)
            {
                throw new Exception("Property Does not exist");
            }

            property.Title = propertyDto.Title;
            property.Description = propertyDto.Description;
            property.Price = propertyDto.Price;
            property.Location = propertyDto.Location;
            property.PropertyType = (PropertyType)Enum.Parse(typeof(PropertyType), propertyDto.PropertyType, true);
            property.Features = propertyDto.Features.Select(f => new PropertyFeature { FeatureName = f, PropertyId = property.Id }).ToList();
            await _propertyRepository.UpdateAsync(property).ConfigureAwait(false);
            return true;
        }

        public async Task<bool> DeletePropertyAsync(string id,string brokerId)
        {
            var property = await _propertyRepository.GetByIdAsync(id).ConfigureAwait(false);
            if (property == null)
            {
                throw new Exception("Property Does not exist");
            }
            ValidateBroker(brokerId, property.BrokerId);
            await _propertyRepository.DeleteAsync(id).ConfigureAwait(false);
            return true;
        }

        public async Task<IEnumerable<PropertyDto>> SearchPropertiesAsync(
            SearchDto searchDto)
        {
            var properties = await _propertyRepository.FindAsync(p =>
                (string.IsNullOrEmpty(searchDto.Location) || p.Location.Contains(searchDto.Location)) &&
                (!searchDto.MinPrice.HasValue || p.Price >= searchDto.MinPrice.Value) &&
                (!searchDto.MaxPrice.HasValue || p.Price <= searchDto.MaxPrice.Value) &&
                (string.IsNullOrEmpty(searchDto.PropertyType) || p.PropertyType.ToString().Equals(searchDto.PropertyType, StringComparison.OrdinalIgnoreCase))
            ).ConfigureAwait(false);
            return properties.Select(p => new PropertyDto(p));
        }

        public async Task<bool> AddImagesAsync(string propertyId, IEnumerable<string> imageUrls,string brokerId)
        {
            var property = await _propertyRepository.GetByIdAsync(propertyId).ConfigureAwait(false);
            if (property == null)
            {
                throw new Exception("Property Does not exist");
            }
            ValidateBroker(brokerId, property.BrokerId);
            foreach (var url in imageUrls)
            {
                property.Images.Add(new PropertyImage { ImageUrl = url, PropertyId = property.Id });
            }
            await _propertyRepository.UpdateAsync(property).ConfigureAwait(false);
            return true;
        }

        
        public async Task<bool> DeleteImageAsync(string propertyId, string imageId, string brokerId)
        {
            var property = await _propertyRepository.GetByIdAsync(propertyId).ConfigureAwait(false);
            if (property == null)
            {
                throw new Exception("Property Does not exist");
            }
            ValidateBroker(brokerId, property.BrokerId);
            var image = property.Images.FirstOrDefault(i => i.Id == imageId);
            if (image == null)
            {
                throw new Exception("Image Does not exist");
            }

            property.Images.Remove(image);
            await _propertyRepository.UpdateAsync(property).ConfigureAwait(false);
            return true;
        }

        private static void ValidateBroker(string brokerId, string propertyBrokerId)
        {
            if(brokerId!=propertyBrokerId)
            {
                throw new Exception("User Does not have required permission");
            }
        }
    }
}
