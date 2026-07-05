using LibraryManagement.Business.DTOs.ReservationDTOs;
using LibraryManagement.Business.Interfaces;
using LibraryManagement.Data.UnitOfWorks;
using LibraryManagement.Models.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagement.Business.Services
{
    public class SlotTemplateService : ISlotTemplateService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SlotTemplateService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<SlotTemplateDto>> GetAllActiveTemplatesAsync()
        {
            var templates = await _unitOfWork.SlotTemplates.Query()
                .Where(t => t.IsActive)
                .OrderBy(t => t.StartTime)
                .ToListAsync();

            return templates.Select(t => new SlotTemplateDto
            {
                SlotTemplateId = t.SlotTemplateId,
                StartTime = t.StartTime,
                EndTime = t.EndTime,
                IsActive = t.IsActive
            }).ToList();
        }

        public async Task<List<SlotTemplateDto>> GetAllTemplatesAsync()
        {
            var templates = await _unitOfWork.SlotTemplates.Query()
                .OrderBy(t => t.StartTime)
                .ToListAsync();

            return templates.Select(t => new SlotTemplateDto
            {
                SlotTemplateId = t.SlotTemplateId,
                StartTime = t.StartTime,
                EndTime = t.EndTime,
                IsActive = t.IsActive
            }).ToList();
        }

        public async Task<SlotTemplateDto> CreateTemplateAsync(CreateSlotTemplateDto dto)
        {
            var template = new SlotTemplate
            {
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                IsActive = true
            };

            await _unitOfWork.SlotTemplates.AddAsync(template);
            await _unitOfWork.SaveChangesAsync();

            return new SlotTemplateDto
            {
                SlotTemplateId = template.SlotTemplateId,
                StartTime = template.StartTime,
                EndTime = template.EndTime,
                IsActive = template.IsActive
            };
        }

        public async Task<bool> ToggleTemplateStatusAsync(int id)
        {
            var template = await _unitOfWork.SlotTemplates.GetByIdAsync(id);
            if (template == null) return false;

            template.IsActive = !template.IsActive;
            _unitOfWork.SlotTemplates.Update(template);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTemplateAsync(int id)
        {
            var template = await _unitOfWork.SlotTemplates.GetByIdAsync(id);
            if (template == null) return false;

            _unitOfWork.SlotTemplates.Delete(template);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
