using FluentValidation;
using LibraryManagement.Business.DTOs.CategoryDTOs;

namespace LibraryManagement.Business.Validators.CategoryValidators
{
    public class UpdateCategoryDtoValidator : AbstractValidator<UpdateCategoryDto>
    {
        public UpdateCategoryDtoValidator()
        {
            RuleFor(x => x.CategoryName)
                .NotEmpty().WithMessage("Tên thể loại không được để trống.")
                .MaximumLength(100).WithMessage("Tên thể loại không được vượt quá 100 ký tự.");
        }
    }
}
