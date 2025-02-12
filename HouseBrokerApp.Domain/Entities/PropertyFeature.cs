using HouseBrokerApp.Domain.BaseEntity;

namespace HouseBrokerApp.Domain.Entities
{
    public class PropertyFeature : EntityBase
    {
        public string FeatureName { get; set; }
        public string PropertyId { get; set; }
        public Property Property { get; set; }
    }
}