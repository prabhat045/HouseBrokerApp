using HouseBrokerApp.Domain.Entities;
using HouseBrokerApp.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseBrokerApp.Application.DTO
{
    public class PropertyDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Location { get; set; }
        public string PropertyType { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
        public List<string> Features { get; set; } = new List<string>();
        public string BrokerId { get; set; }
        public string BrokerFullName { get; set; }
        public string BrokerEmail { get; set; }
        public string BrokerPhoneNumber { get; set; }

        public PropertyDto() { }

        public PropertyDto(Property property)
        {
            Id = property.Id;
            Title = property.Title;
            Description = property.Description;
            Price = property.Price;
            Location = property.Location;
            PropertyType = property.PropertyType.ToString();
            BrokerId = property.BrokerId;
            ImageUrls = property.Images?.Select(i => i.ImageUrl).ToList() ?? [];
            Features = property.Features?.Select(f => f.FeatureName).ToList() ?? [];
            if (property.Broker != null)
            {
                BrokerFullName = property.Broker.FullName;
                BrokerEmail = property.Broker.Email;
                BrokerPhoneNumber = property.Broker.PhoneNumber;
            }

        }

       
        public Property ToEntity(string brokerId)
        {
            var property = new Property
            {
                Title = Title,
                Description = Description,
                Price = Price,
                Location = Location,
                PropertyType = Enum.Parse<PropertyType>(PropertyType, true),
                BrokerId = brokerId
            };

            if (ImageUrls != null)
                property.Images = ImageUrls.Select(url => new PropertyImage { ImageUrl = url, PropertyId = property.Id }).ToList();
            if (Features != null)
                property.Features = Features.Select(f => new PropertyFeature { FeatureName = f, PropertyId = property.Id }).ToList();

            return property;
        }
    }
}
