using Million.Application.Contracts;
using Million.Domain.Models;
using Million.Infrastructure.Data;

namespace Million.Infrastructure.Repositories
{
    public class PropertyRepository : MongoRepository<Property>, IPropertyRepository
    {
        public PropertyRepository(IMongoDbContext context) : base(context, "properties")
        {
        }
    }
} 