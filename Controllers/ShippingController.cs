using Microsoft.AspNetCore.Mvc;
using TestTaskAPI.Interface.InterfaceServices;
using TestTaskAPI.Model;
using TestTaskAPI.Services;

namespace TestTaskAPI.Controllers
{
    [ApiController]
    [Route ("api/shipping")]
    public class ShippingController : ControllerBase
    {
        private readonly IShippingServices _shippingServices;
        public ShippingController(IShippingServices shippingServices)
        {
            _shippingServices = shippingServices;
        }
        [HttpGet]
        public async Task<ActionResult<ShippingDocument>> GetShippingDocument()
        {
            try
            {
                var shipping = await _shippingServices.GetShippingDocumentTransfer();
                return Ok(shipping);
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

        [HttpGet("{id}")]
        public async Task<ActionResult<ShippingDocument>> GetShippingByID(int id)
        {
            try
            {
                var receipt = await _shippingServices.GetShippingDocumentByIdTransferAsync(id);
                return Ok(receipt);
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
        public async Task<ActionResult<ReceiptDocumet>> CreateShipping([FromBody] ShippingDocument newDocument)
        {
            if (newDocument == null)
            {
                return BadRequest("Данные клиента не введены.");

            }
            try
            {
                var receiptCreate = await _shippingServices.CreateShippingDocument(newDocument);
                return CreatedAtAction(nameof(GetShippingDocument), new { id = receiptCreate.Id }, receiptCreate);
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
        public async Task<IActionResult> UpdateShipping(int id, [FromBody] ShippingDocument updShipping)
        {
            try
            {
                if (id != updShipping.Id)
                    return BadRequest("ID не найден");

                await _shippingServices.UpdateShippingDocument(updShipping);
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
        public async Task<IActionResult> DeleteShipping(int id)
        {
            try
            {
                await _shippingServices.DeleteShippingDocument(id);
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

