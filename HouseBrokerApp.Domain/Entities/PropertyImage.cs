using HouseBrokerApp.Domain.BaseEntity;

namespace HouseBrokerApp.Domain.Entities
{
    public class PropertyImage : EntityBase
    {
        public string ImageUrl { get; set; }
        public string PropertyId { get; set; }
        public Property Property { get; set; }
    }
}