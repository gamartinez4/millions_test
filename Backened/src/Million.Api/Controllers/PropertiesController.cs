using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Million.Application.Contracts;
using Million.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Million.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PropertiesController : ControllerBase
    {
        private readonly IPropertyService _propertyService;

        public PropertiesController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PropertyResponse>>> GetProperties(
            [FromQuery] string? name,
            [FromQuery] string? address,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice)
        {
            var properties = await _propertyService.GetPropertiesFilteredAsync(name, address, minPrice, maxPrice);
            return Ok(properties);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PropertyResponse>> GetProperty(int id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            if (property == null)
            {
                return NotFound();
            }
            return Ok(property);
        }

        [HttpPost]
        public async Task<ActionResult<PropertyResponse>> CreateProperty(PropertyRequest propertyRequest)
        {
            var newProperty = await _propertyService.CreatePropertyAsync(propertyRequest);
            return CreatedAtAction(nameof(GetProperty), new { id = newProperty.Id }, newProperty);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProperty(int id, PropertyRequest propertyRequest)
        {
            await _propertyService.UpdatePropertyAsync(id, propertyRequest);
            return NoContent();
        }


    }

} 