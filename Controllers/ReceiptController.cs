using Microsoft.AspNetCore.Mvc;
using System.Net;
using TestTaskAPI.Interface.InterfaceServices;
using TestTaskAPI.Model;
using TestTaskAPI.Services;

namespace TestTaskAPI.Controllers
{
    [ApiController]
    [Route("api/receipt")]
    public class ReceiptController : ControllerBase
    {
        private readonly IReceiptServices _receiptServices;

        public ReceiptController(IReceiptServices receiptServices)
        {
            _receiptServices = receiptServices;
        }

        [HttpGet]
        public async Task<ActionResult<ReceiptDocumet>> GetReceipt()
        {
            try
            {
                var receipt = await _receiptServices.GetReceiptDocumetTransfer();
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

        [HttpGet("{id}")]
        public async Task<ActionResult<ReceiptDocumet>> GetReceiptByID(int id)
        {
            try
            {
                var receipt = await _receiptServices.GetReceiptDocumetByIdTransferAsync(id);
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
        public async Task<ActionResult<ReceiptDocumet>> CreateReceipt([FromBody] ReceiptDocumet newDocument)
        {
            if (newDocument == null)
            {
                return BadRequest("Данные клиента не введены.");

            }
            try
            {

                var receiptCreate = await _receiptServices.CreateReceiptDocumet(newDocument);
                return CreatedAtAction(nameof(GetReceipt), new { id = receiptCreate.Id }, receiptCreate);
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
        public async Task<IActionResult> UpdateReceipt(int id, [FromBody] ReceiptDocumet updDocument)
        {
            try
            {
                if(id != updDocument.Id)
                    return BadRequest("ID не найден");

                await _receiptServices.UpdateReceiptDocumet(updDocument);
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
        public async Task<IActionResult> DeleteReceipt(int id)
        {
            try
            {
                await _receiptServices.DeleteReceiptDocumet(id);
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
