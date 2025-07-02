using Million.Application.Contracts;
using Million.Domain.Models;
using Million.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Million.Infrastructure.Repositories
{
    public class OwnerRepository : MongoRepository<Owner>, IOwnerRepository
    {
        public OwnerRepository(IMongoDbContext context) : base(context, "owners")
        {
        }

        // All CRUD operations are handled by the generic base class (MongoRepository<T>)
    }
} 