using HouseBrokerApp.Application.Manager;
using HouseBrokerApp.Domain.Entities;
using HouseBrokerApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HouseBrokerApp.Infrastructure.Repositories
{
    public class PropertyRepository : IPropertyRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<Property> _dbSet;

        public PropertyRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<Property>();
        }

        public async Task<Property?> GetByIdAsync(string id)
        {
            return await _dbSet
                .Include(p => p.Images)
                .Include(p => p.Features)
                .FirstOrDefaultAsync(p => p.Id == id)
                .ConfigureAwait(false);
        }

        public IQueryable<Property> GetAll()
        {
            return _dbSet.AsQueryable();
        }

        public async Task AddAsync(Property property)
        {
            await _dbSet.AddAsync(property).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task UpdateAsync(Property property)
        {
            _dbSet.Update(property);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task DeleteAsync(string id)
        {
            var property = await GetByIdAsync(id).ConfigureAwait(false);
            if (property != null)
            {
                _dbSet.Remove(property);
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<Property>> FindAsync(Expression<Func<Property, bool>> predicate)
        {
            return await _dbSet
                .Include(p => p.Images)
                .Include(p => p.Features)
                .Where(predicate)
                .ToListAsync()
                .ConfigureAwait(false);
        }


        public async Task<IEnumerable<Property>> SearchPropertiesAsync(string location, decimal? minPrice, decimal? maxPrice, string propertyType)
        {
            var query = _dbSet
                .Include(p => p.Images)
                .Include(p => p.Features)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(location))
                query = query.Where(p => p.Location.Contains(location));
            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);
            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);
            if (!string.IsNullOrWhiteSpace(propertyType))
                query = query.Where(p => p.PropertyType.ToString().Equals(propertyType, StringComparison.OrdinalIgnoreCase));

            return await query.ToListAsync().ConfigureAwait(false);
        }
    }
}
