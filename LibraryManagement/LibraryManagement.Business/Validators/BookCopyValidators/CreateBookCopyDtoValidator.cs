using FluentValidation;
using LibraryManagement.Business.DTOs.BookCopyDTOs;

namespace LibraryManagement.Business.Validators.BookCopyValidators
{
    public class CreateBookCopyDtoValidator : AbstractValidator<CreateBookCopyDto>
    {
        public CreateBookCopyDtoValidator()
        {
            RuleFor(x => x.BookId)
                .NotEmpty().WithMessage("BookId không được để trống.");

            RuleFor(x => x.Barcode)
                .NotEmpty().WithMessage("Barcode không được để trống.")
                .MaximumLength(100).WithMessage("Barcode không được vượt quá 100 ký tự.");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Trạng thái không được để trống.")
                .Must(s => s == "Available" || s == "Borrowed" || s == "Lost" || s == "Damaged" || s == "Hidden")
                .WithMessage("Trạng thái phải là Available, Borrowed, Lost, Damaged hoặc Hidden.");

            RuleFor(x => x.ReplacementPrice)
                .GreaterThan(0).WithMessage("Giá thay thế phải lớn hơn 0.");
        }
    }
}
