using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Million.Application.Contracts;
using Million.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Million.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PropertyTracesController : ControllerBase
    {
        private readonly IPropertyTraceService _traceService;

        public PropertyTracesController(IPropertyTraceService traceService)
        {
            _traceService = traceService;
        }

        [HttpGet("property/{propertyId}")]
        public async Task<ActionResult<IEnumerable<PropertyTraceResponse>>> GetTracesForProperty(int propertyId)
        {
            var traces = await _traceService.GetTracesForPropertyAsync(propertyId);
            return Ok(traces);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PropertyTraceResponse>> Get(int id)
        {
            var trace = await _traceService.GetTraceByIdAsync(id);
            if (trace == null)
            {
                return NotFound();
            }
            return Ok(trace);
        }

        [HttpPost]
        public async Task<ActionResult<PropertyTraceResponse>> Post([FromBody] PropertyTraceRequest traceRequest)
        {
            try
            {
                var createdTrace = await _traceService.AddTraceToPropertyAsync(traceRequest);
                return CreatedAtAction(nameof(Get), new { id = createdTrace.Id }, createdTrace);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] PropertyTraceRequest traceRequest)
        {
            await _traceService.UpdateTraceAsync(id, traceRequest);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _traceService.DeleteTraceAsync(id);
            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PropertyTraceResponse>>> GetAll()
        {
            var traces = await _traceService.GetAllTracesAsync();
            return Ok(traces);
        }
    }
} 