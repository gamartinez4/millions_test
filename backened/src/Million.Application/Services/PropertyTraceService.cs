using Million.Application.Contracts;
using Million.Application.DTOs;
using Million.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;

namespace Million.Application.Services
{
    public class PropertyTraceService : IPropertyTraceService
    {
        private readonly IPropertyTraceRepository _traceRepository;
        private readonly ILogger<PropertyTraceService> _logger;

        public PropertyTraceService(IPropertyTraceRepository traceRepository, ILogger<PropertyTraceService> logger)
        {
            _traceRepository = traceRepository;
            _logger = logger;
        }

        public async Task<PropertyTraceResponse> AddTraceToPropertyAsync(PropertyTraceRequest traceRequest)
        {
            // Validate if property already has a trace (sold)
            var existingTraces = await _traceRepository.FindAsync(pt => pt.PropertyId == traceRequest.PropertyId);
            if (existingTraces.Any())
            {
                throw new InvalidOperationException("La propiedad ya fue comprada y no puede volver a comprarse.");
            }

            var propertyTrace = new PropertyTrace(
                traceRequest.Id,
                DateTime.UtcNow,
                traceRequest.Name,
                traceRequest.Value,
                traceRequest.Tax,
                traceRequest.PropertyId
            );

            await _traceRepository.AddAsync(propertyTrace);

            // Log the property trace creation for Docker logs
            _logger.LogInformation("=== PROPERTY TRACE CREATED ===");
            _logger.LogInformation($"Property ID: {propertyTrace.PropertyId}");
            _logger.LogInformation($"Buyer: {propertyTrace.Name}");
            _logger.LogInformation($"Sale Date: {propertyTrace.DateSale:yyyy-MM-dd HH:mm:ss} UTC");
            _logger.LogInformation($"Sale Value: ${propertyTrace.Value:N2}");
            _logger.LogInformation($"Tax: ${propertyTrace.Tax:N2}");
            _logger.LogInformation($"Trace ID: {propertyTrace.Id}");
            _logger.LogInformation("================================");

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

        public async Task<IEnumerable<PropertyTraceResponse>> GetAllTracesAsync()
        {
            var traces = await _traceRepository.GetAllAsync();
            var traceResponses = traces.Select(MapToTraceResponse).ToList();
            
            // Log all property traces for Docker logs
            _logger.LogInformation("=== ALL PROPERTY TRACES RETRIEVED ===");
            _logger.LogInformation($"Total traces found: {traceResponses.Count}");
            
            foreach (var trace in traceResponses)
            {
                _logger.LogInformation($"Trace ID: {trace.Id} | Property ID: {trace.PropertyId} | Buyer: {trace.Name} | Value: ${trace.Value:N2} | Date: {trace.DateSale:yyyy-MM-dd HH:mm:ss}");
            }
            
            _logger.LogInformation("=====================================");
            
            return traceResponses;
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