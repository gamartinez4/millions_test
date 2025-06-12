using Million.Application.Contracts;
using Million.Application.DTOs;
using Million.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Million.Application.Services
{
    public class PropertyTraceService : IPropertyTraceService
    {
        private readonly IPropertyTraceRepository _traceRepository;

        public PropertyTraceService(IPropertyTraceRepository traceRepository)
        {
            _traceRepository = traceRepository;
        }

        public async Task<PropertyTraceResponse> AddTraceToPropertyAsync(PropertyTraceRequest traceRequest)
        {
            var propertyTrace = new PropertyTrace(
                traceRequest.DateSale,
                traceRequest.Name,
                traceRequest.Value,
                traceRequest.Tax,
                traceRequest.PropertyId
            );

            await _traceRepository.AddAsync(propertyTrace);

            return MapToTraceResponse(propertyTrace);
        }

        public async Task DeleteTraceAsync(int id)
        {
            var trace = await _traceRepository.GetByIdAsync(id);
            if (trace != null)
            {
                _traceRepository.Remove(trace);
            }
        }

        public async Task<PropertyTraceResponse> GetTraceByIdAsync(int id)
        {
            var trace = await _traceRepository.GetByIdAsync(id);
            return trace == null ? null : MapToTraceResponse(trace);
        }

        public async Task<IEnumerable<PropertyTraceResponse>> GetTracesForPropertyAsync(int propertyId)
        {
            var traces = await _traceRepository.FindAsync(pt => pt.PropertyId == propertyId);
            return traces.Select(MapToTraceResponse);
        }

        public async Task UpdateTraceAsync(int id, PropertyTraceRequest traceRequest)
        {
            var trace = await _traceRepository.GetByIdAsync(id);
            if (trace != null)
            {
                trace.DateSale = traceRequest.DateSale;
                trace.Name = traceRequest.Name;
                trace.Value = traceRequest.Value;
                trace.Tax = traceRequest.Tax;
                trace.PropertyId = traceRequest.PropertyId;
                _traceRepository.Update(trace);
            }
        }

        private PropertyTraceResponse MapToTraceResponse(PropertyTrace trace)
        {
            return new PropertyTraceResponse
            {
                Id = trace.Id,
                DateSale = trace.DateSale,
                Name = trace.Name,
                Value = trace.Value,
                Tax = trace.Tax,
                PropertyId = trace.PropertyId
            };
        }
    }
} 