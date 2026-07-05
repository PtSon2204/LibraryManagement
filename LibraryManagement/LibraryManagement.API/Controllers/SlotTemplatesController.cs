using LibraryManagement.Business.DTOs.ReservationDTOs;
using LibraryManagement.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LibraryManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SlotTemplatesController : ControllerBase
    {
        private readonly ISlotTemplateService _slotTemplateService;

        public SlotTemplatesController(ISlotTemplateService slotTemplateService)
        {
            _slotTemplateService = slotTemplateService;
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveTemplates()
        {
            var templates = await _slotTemplateService.GetAllActiveTemplatesAsync();
            return Ok(templates);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTemplates()
        {
            var templates = await _slotTemplateService.GetAllTemplatesAsync();
            return Ok(templates);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTemplate([FromBody] CreateSlotTemplateDto dto)
        {
            var template = await _slotTemplateService.CreateTemplateAsync(dto);
            return Ok(template);
        }

        [HttpPut("{id}/toggle")]
        public async Task<IActionResult> ToggleTemplateStatus(int id)
        {
            var success = await _slotTemplateService.ToggleTemplateStatusAsync(id);
            if (!success) return NotFound();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTemplate(int id)
        {
            var success = await _slotTemplateService.DeleteTemplateAsync(id);
            if (!success) return NotFound();
            return Ok();
        }
    }
}
