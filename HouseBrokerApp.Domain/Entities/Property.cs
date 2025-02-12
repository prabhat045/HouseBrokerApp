using HouseBrokerApp.Domain.BaseEntity;
using HouseBrokerApp.Domain.Enum;
using System.ComponentModel.DataAnnotations;


namespace HouseBrokerApp.Domain.Entities
{
    public class Property  : EntityBase
    {
       
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public PropertyType PropertyType { get; set; }

        // Lists for photos and features
        public List<PropertyImage> Images { get; set; } = [];

        public List<PropertyFeature> Features { get; set; } = [];

        // The broker who created the listing
        [Required]
        public string BrokerId { get; set; }
        public ApplicationUser Broker { get; set; }
    }
}
