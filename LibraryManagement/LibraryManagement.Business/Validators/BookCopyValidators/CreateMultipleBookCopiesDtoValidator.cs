using FluentValidation;
using LibraryManagement.Business.DTOs.BookCopyDTOs;

namespace LibraryManagement.Business.Validators.BookCopyValidators
{
    public class BookCopyItemDtoValidator : AbstractValidator<BookCopyItemDto>
    {
        public BookCopyItemDtoValidator()
        {
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

    public class CreateMultipleBookCopiesDtoValidator : AbstractValidator<CreateMultipleBookCopiesDto>
    {
        public CreateMultipleBookCopiesDtoValidator()
        {
            RuleFor(x => x.BookId)
                .NotEmpty().WithMessage("BookId không được để trống.");

            RuleFor(x => x.Copies)
                .NotEmpty().WithMessage("Danh sách bản sao không được để trống.")
                .Must(c => c != null && c.Count > 0).WithMessage("Phải có ít nhất 1 bản sao.")
                .Must(c => c == null || c.Count <= 100).WithMessage("Chỉ có thể tạo tối đa 100 bản sao mỗi lần.");

            RuleForEach(x => x.Copies).SetValidator(new BookCopyItemDtoValidator());
        }
    }
}
