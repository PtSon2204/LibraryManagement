using LibraryManagement.MVC.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LibraryManagement.MVC.Controllers
{
    public class SlotTemplatesController : Controller
    {
        private readonly ISlotTemplateService _slotTemplateService;

        public SlotTemplatesController(ISlotTemplateService slotTemplateService)
        {
            _slotTemplateService = slotTemplateService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("SlotTemplates/GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var resultJson = await _slotTemplateService.GetAllTemplatesAsync();
                return Content(resultJson, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("SlotTemplates/Create")]
        public async Task<IActionResult> Create([FromBody] object payload)
        {
            var success = await _slotTemplateService.CreateTemplateAsync(payload);
            if (success) return Ok();
            return BadRequest();
        }

        [HttpPut("SlotTemplates/Toggle/{id}")]
        public async Task<IActionResult> Toggle(int id)
        {
            var success = await _slotTemplateService.ToggleTemplateStatusAsync(id);
            if (success) return Ok();
            return BadRequest();
        }

        [HttpDelete("SlotTemplates/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _slotTemplateService.DeleteTemplateAsync(id);
            if (success) return Ok();
            return BadRequest();
        }
    }
}
