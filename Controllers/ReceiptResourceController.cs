using Microsoft.AspNetCore.Mvc;
using System.Net;
using TestTaskAPI.Interface.InterfaceServices;
using TestTaskAPI.Model;
using TestTaskAPI.Services;

namespace TestTaskAPI.Controllers
{
    [ApiController]
    [Route ("api/receiptresource")]
    public class ReceiptResourceController : ControllerBase
    {
        private readonly IReceiptResourcesServices _receiptResourcesServices;

        public ReceiptResourceController(IReceiptResourcesServices receiptResourcesServices)
        {
            _receiptResourcesServices = receiptResourcesServices;
        }

        [HttpGet] 
        public async Task<ActionResult<ReceiptResource>> GetReceiptResource()
        {

            try
            {
                var receiptResource = await _receiptResourcesServices.GetReceiptResourcesTransfer();
                return Ok(receiptResource);
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
        public async Task<ActionResult<ReceiptResource>> CreateReceiptResources([FromBody] ReceiptResource newResource)
        {
            if (newResource == null)
            {
                return BadRequest("Данные не введены.");

            }
            try
            {
                var recourceCreate = await _receiptResourcesServices.CreateReceiptResources(newResource);
                return CreatedAtAction(nameof(GetReceiptResource), new { id = recourceCreate.Id }, recourceCreate);
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
        public async Task<IActionResult> UpdateReceiptResources(int id, [FromBody] ReceiptResource updResorce)
        {
            try
            {
                if (id != updResorce.Id)
                    return BadRequest("ID не найден");

                await _receiptResourcesServices.UpdateReceiptResources(updResorce);
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
                await _receiptResourcesServices.DeleteReceiptResources(id);
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
