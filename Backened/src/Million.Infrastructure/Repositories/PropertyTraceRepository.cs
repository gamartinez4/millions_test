using Million.Application.Contracts;
using Million.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Million.Infrastructure.Repositories
{
    public class PropertyTraceRepository : IPropertyTraceRepository
    {
        private readonly List<PropertyTrace> _propertyTraces = new List<PropertyTrace>();
        private int _nextId = 1;

        public PropertyTraceRepository()
        {
            _propertyTraces.Add(new PropertyTrace(new DateTime(2023, 1, 10), "Initial Sale", 200000, 20000, 1) { Id = _nextId++ });
            _propertyTraces.Add(new PropertyTrace(new DateTime(2024, 2, 20), "Price Update", 210000, 21000, 1) { Id = _nextId++ });
        }

        public Task<PropertyTrace> GetByIdAsync(int id)
        {
            return Task.FromResult(_propertyTraces.FirstOrDefault(pt => pt.Id == id));
        }

        public Task<IEnumerable<PropertyTrace>> GetAllAsync()
        {
            return Task.FromResult(_propertyTraces.AsEnumerable());
        }

        public Task<IEnumerable<PropertyTrace>> FindAsync(Expression<Func<PropertyTrace, bool>> expression)
        {
            return Task.FromResult(_propertyTraces.AsQueryable().Where(expression).AsEnumerable());
        }

        public Task AddAsync(PropertyTrace entity)
        {
            entity.Id = _nextId++;
            _propertyTraces.Add(entity);
            return Task.CompletedTask;
        }

        public void Update(PropertyTrace entity)
        {
            var existingTrace = _propertyTraces.FirstOrDefault(pt => pt.Id == entity.Id);
            if (existingTrace != null)
            {
                existingTrace.DateSale = entity.DateSale;
                existingTrace.Name = entity.Name;
                existingTrace.Value = entity.Value;
                existingTrace.Tax = entity.Tax;
                existingTrace.PropertyId = entity.PropertyId;
            }
        }

        public void Remove(PropertyTrace entity)
        {
            _propertyTraces.Remove(entity);
        }
    }
} 