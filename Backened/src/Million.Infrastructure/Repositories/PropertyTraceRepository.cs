using Million.Application.Contracts;
using Million.Domain.Models;
using Million.Infrastructure.Data;

namespace Million.Infrastructure.Repositories
{
    public class PropertyTraceRepository : MongoRepository<PropertyTrace>, IPropertyTraceRepository
    {
        public PropertyTraceRepository(IMongoDbContext context) : base(context, "propertyTraces")
        {
        }
    }
} 