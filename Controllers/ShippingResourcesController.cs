using Microsoft.AspNetCore.Mvc;
using TestTaskAPI.Interface.InterfaceServices;
using TestTaskAPI.Model;
using TestTaskAPI.Services;

namespace TestTaskAPI.Controllers
{
    [ApiController]
    [Route ("api/shippingresource")]
    public class ShippingResourcesController : ControllerBase
    {
        private readonly IShippingResourcesServices _shippingResourcesServices;

        public ShippingResourcesController(IShippingResourcesServices shippingResourcesServices)
        {
            _shippingResourcesServices = shippingResourcesServices;
        }

        [HttpGet]
        public async  Task<ActionResult<ShippingResource>> GetShippingResource()
        {
            try
            {
                var shippingresource = await _shippingResourcesServices.GetShippingResourceTransfer();
                return Ok(shippingresource);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ошибка при запросе");
            }
        }

        [HttpPost]
        public async Task<ActionResult<ShippingResource>> CreateReceiptResources([FromBody] ShippingResource newResource)
        {
            if (newResource == null)
            {
                return BadRequest("Данные не введены.");

            }
            try
            {
                var recourceCreate = await _shippingResourcesServices.CreateShippingResource(newResource);
                return CreatedAtAction(nameof(GetShippingResource), new { id = recourceCreate.Id }, recourceCreate);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ошибка при запросе");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReceiptResources(int id, [FromBody] ShippingResource updResorce)
        {
            try
            {
                if (id != updResorce.Id)
                    return BadRequest("ID не найден");

                await _shippingResourcesServices.UpdateShippingResource(updResorce);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ошибка при запросе");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReceiptResources(int id)
        {
            try
            {
                await _shippingResourcesServices.DeleteShippingResource(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ошибка при запросе");
            }
        }
    }
}

