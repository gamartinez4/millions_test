using Million.Application.Contracts;
using Million.Domain.Models;
using Million.Infrastructure.Data;

namespace Million.Infrastructure.Repositories
{
    public class PropertyImageRepository : MongoRepository<PropertyImage>, IPropertyImageRepository
    {
        public PropertyImageRepository(IMongoDbContext context) : base(context, "propertyImages")
        {
        }
    }
} 