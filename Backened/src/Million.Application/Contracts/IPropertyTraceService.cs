using Million.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Million.Application.Contracts
{
    public interface IPropertyTraceService
    {
        Task<IEnumerable<PropertyTraceResponse>> GetTracesForPropertyAsync(int propertyId);
        Task<PropertyTraceResponse> GetTraceByIdAsync(int id);
        Task<PropertyTraceResponse> AddTraceToPropertyAsync(PropertyTraceRequest traceRequest);
        Task UpdateTraceAsync(int id, PropertyTraceRequest traceRequest);
        Task DeleteTraceAsync(int id);
        Task<IEnumerable<PropertyTraceResponse>> GetAllTracesAsync();
    }
} 