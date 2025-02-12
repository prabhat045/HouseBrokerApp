using HouseBrokerApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HouseBrokerApp.Application.Manager
{
    public interface IPropertyRepository
    {
        Task<Property?> GetByIdAsync(string id);
        IQueryable<Property> GetAll();
        Task AddAsync(Property property);
        Task UpdateAsync(Property property);
        Task DeleteAsync(string id);
        Task<IEnumerable<Property>> FindAsync(Expression<Func<Property, bool>> predicate);
        Task<IEnumerable<Property>> SearchPropertiesAsync(string location, decimal? minPrice, decimal? maxPrice, string propertyType);
    }
}
