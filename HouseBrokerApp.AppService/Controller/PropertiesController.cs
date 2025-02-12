using HouseBrokerApp.Application.DTO;
using HouseBrokerApp.Application.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HouseBrokerApp.AppService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertiesController : ControllerBase
    {
        private readonly IPropertyService _propertyService;

        public PropertiesController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        [HttpGet("GetAllProperties")]
        public async Task<List<PropertyDto>> GetAll()
        {
            return await _propertyService.GetAllPropertiesAsync();
        }

        [HttpGet("GetPropertyById/{id}")]
        [AllowAnonymous]
        public async Task<PropertyDto> Get(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                throw new Exception("Invalid Input Parameter");
            }
            var property = await _propertyService.GetPropertyByIdAsync(id);
            return property;
        }

        [HttpPost("CreateProperty")]
        [Authorize(Roles = "Broker")]
        public async Task<PropertyDto> Create([FromBody] PropertyDto propertyDto)
        {
            if (propertyDto == null)
            {
                throw new Exception("Invalid Input Parameters");
            }
            var brokerId = User.FindFirst("uid")?.Value;
            ValidateBroker(brokerId);
            return await _propertyService.CreatePropertyAsync(propertyDto, brokerId);

        }

        private static void ValidateBroker(string? brokerId)
        {
            if (brokerId==null)
            {
                throw new Exception("Invalid User");
            }
        }

        [HttpPut("UpdateProperty/{id}")]
        [Authorize(Roles = "Broker")]
        public async Task<PropertyDto> Update([FromBody] PropertyDto propertyDto)
        {
            var brokerId = User.FindFirst("uid")?.Value;
            if(brokerId!= propertyDto.BrokerId)
            {
                throw new Exception("User Cannot Update the property");
            }
            await _propertyService.UpdatePropertyAsync(propertyDto);
            return await _propertyService.GetPropertyByIdAsync(propertyDto.Id);
        }

        [HttpDelete("DeleteProperty/{id}")]
        [Authorize(Roles = "Broker")]
        public async Task<bool> Delete(string id)
        {
            var brokerId = User.FindFirst("uid")?.Value;
            ValidateBroker(brokerId);
            return await _propertyService.DeletePropertyAsync(id,brokerId);
        }

        [HttpPost("UploadPropertyImages/{id}")]
        [Authorize(Roles = "Broker")]
        public async Task<IEnumerable<string>> UploadImages(string id, [FromForm] List<IFormFile> files)
        {
            var brokerId = User.FindFirst("uid")?.Value;
            ValidateBroker(brokerId);
            var imageUrls = new List<string>();
            var uploadsFolder = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!System.IO.Directory.Exists(uploadsFolder))
                System.IO.Directory.CreateDirectory(uploadsFolder);

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var uniqueFileName = $"{System.Guid.NewGuid()}_{System.IO.Path.GetFileName(file.FileName)}";
                    var filePath = System.IO.Path.Combine(uploadsFolder, uniqueFileName);
                    using (var stream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    var url = $"{Request.Scheme}://{Request.Host}/images/{uniqueFileName}";
                    imageUrls.Add(url);
                }
            }

            await _propertyService.AddImagesAsync(id, imageUrls,brokerId);
            return imageUrls;
        }

        [HttpGet("SearchProperties")]
        [AllowAnonymous]
        public async Task<IEnumerable<PropertyDto>> Search([FromBody]SearchDto searchDto)
        {
            return await _propertyService
                .SearchPropertiesAsync(searchDto);
        }

      
        [HttpDelete("RemovePropertyImage/{propertyId}/{imageId}")]
        [Authorize(Roles = "Broker")]
        public async Task<bool> RemovePropertyImage(string propertyId, string imageId)
        {
            var brokerId = User.FindFirst("uid")?.Value;
            ValidateBroker(brokerId);
            return await _propertyService.DeleteImageAsync(propertyId, imageId,brokerId).ConfigureAwait(false);
        }

        [HttpGet("GetMyProperties")]
        [Authorize(Roles = "Broker")]
        public async Task<IEnumerable<PropertyDto>> GetMyProperties()
        {
            var brokerId = User.FindFirst("uid")?.Value;
            ValidateBroker(brokerId);
            var allProperties = await _propertyService.GetAllPropertiesAsync().ConfigureAwait(false);
            return allProperties.Where(p => p.BrokerId == brokerId);
        }
    }
}
