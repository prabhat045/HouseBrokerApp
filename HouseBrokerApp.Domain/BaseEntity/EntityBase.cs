using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseBrokerApp.Domain.BaseEntity
{
    public abstract class EntityBase : IEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
    }
}
