using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using System.Net;
using TestTaskAPI.Interface.InterfaceServices;
using TestTaskAPI.Model;
using TestTaskAPI.Services;

namespace TestTaskAPI.Controllers
{
    [ApiController]
    [Route ("api/measure")]
    public class MeasureController : ControllerBase
    {
        private readonly IMeasureServices _measureServices;

        public MeasureController(IMeasureServices measureServices)
        {
            _measureServices = measureServices;
        }
        [HttpGet]
        public async Task<ActionResult<Measurе>> GetMeasure()
        {
            try
            {
                var measures = await _measureServices.GetMeasureTransfer();
                return Ok(measures);
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
        public async Task<ActionResult<Measurе>> GetMeasureById(int id)
        {
            try
            {
                var resources = await _measureServices.GetMeasureByIdTransferAsync(id);
                return Ok(resources);
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
        public async Task<ActionResult<Measurе>> CreateClient([FromBody] Measurе newMeasure)
        {
            if (newMeasure == null)
            {
                return BadRequest("Данные единицы измерения не введены.");

            }
            try
            {

                var measureCreate = await _measureServices.CreateMeasure(newMeasure);
                return CreatedAtAction(nameof(GetMeasure), new { id = measureCreate.Id }, measureCreate);
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
        public async Task<IActionResult> UpdateMeasure(int id, [FromBody] Measurе updMeasure)
        {
            try
            {
                if (id != updMeasure.Id)
                    return BadRequest("ID не найден");

                await _measureServices.UpdateMeasure(updMeasure);
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
        public async Task<IActionResult> DeleteMeasure(int id)
        {
            try
            {
                await _measureServices.DeleteMeasure(id);
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
