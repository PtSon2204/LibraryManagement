using LibraryManagement.Business.DTOs.CategoryDTOs;
using LibraryManagement.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.API.Controllers
{
    [ApiController]
    [Route("api/categories")]
    [Authorize(Roles = "Admin,Librarian")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _categoryService.GetCategoriesAsync(search, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound(new { message = "Không tìm thấy thể loại." });
            return Ok(category);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var result = await _categoryService.CreateCategoryAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.CategoryId }, result);
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("duplicate key") == true ||
                                                ex.InnerException?.Message.Contains("Violation of UNIQUE KEY") == true)
            {
                return Conflict(new { message = "Tên thể loại đã tồn tại trong hệ thống. Vui lòng kiểm tra lại." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi tạo thể loại. Vui lòng thử lại." });
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != dto.CategoryId) return BadRequest(new { message = "ID không khớp." });

            try
            {
                var result = await _categoryService.UpdateCategoryAsync(id, dto);
                if (result == null) return NotFound(new { message = "Không tìm thấy thể loại cần cập nhật." });
                return NoContent();
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("duplicate key") == true ||
                                                ex.InnerException?.Message.Contains("Violation of UNIQUE KEY") == true)
            {
                return Conflict(new { message = "Tên thể loại đã tồn tại trong hệ thống. Vui lòng kiểm tra lại." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi cập nhật thể loại. Vui lòng thử lại." });
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _categoryService.DeleteCategoryAsync(id);
                if (!success) return NotFound(new { message = "Không tìm thấy thể loại." });
                return NoContent();
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("REFERENCE constraint") == true ||
                                                ex.InnerException?.Message.Contains("foreign key") == true)
            {
                return BadRequest(new { message = "Không thể xóa thể loại này vì có sách thuộc thể loại này đang tồn tại trong hệ thống." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi xóa thể loại. Vui lòng thử lại." });
            }
        }
    }
}
