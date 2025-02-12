using HouseBrokerApp.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseBrokerApp.Application.Service
{
    public interface IPropertyService
    {
        Task<List<PropertyDto>> GetAllPropertiesAsync();
        Task<PropertyDto> GetPropertyByIdAsync(string id);
        Task<PropertyDto> CreatePropertyAsync(PropertyDto propertyDto,string BrokerId);
        Task<bool> UpdatePropertyAsync(PropertyDto propertyDto);
        Task<bool> DeletePropertyAsync(string id,string brokerId);
        Task<IEnumerable<PropertyDto>> SearchPropertiesAsync(SearchDto searchDto);
        Task<bool> AddImagesAsync(string propertyId, IEnumerable<string> imageUrls,string brokerId);
        Task<bool> DeleteImageAsync(string propertyId, string imageId,string brokerId);
    }
}
