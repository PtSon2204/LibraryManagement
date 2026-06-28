using FluentValidation;
using LibraryManagement.Business.DTOs.BookDTOs;

namespace LibraryManagement.Business.Validators.BookValidators
{
    public class CreateBookValidator : AbstractValidator<CreateBookDto>
    {
        public CreateBookValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(255).WithMessage("Title cannot exceed 255 characters.");

            RuleFor(x => x.ISBN)
                .MaximumLength(50).WithMessage("ISBN cannot exceed 50 characters.");

            RuleFor(x => x.Language)
                .MaximumLength(50).WithMessage("Language cannot exceed 50 characters.");
                
            RuleFor(x => x.Edition)
                .MaximumLength(50).WithMessage("Edition cannot exceed 50 characters.");
        }
    }
}
