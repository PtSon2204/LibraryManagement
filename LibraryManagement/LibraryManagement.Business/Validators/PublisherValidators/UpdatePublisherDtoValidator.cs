using FluentValidation;
using LibraryManagement.Business.DTOs.PublisherDTOs;

namespace LibraryManagement.Business.Validators.PublisherValidators
{
    public class UpdatePublisherDtoValidator : AbstractValidator<UpdatePublisherDto>
    {
        public UpdatePublisherDtoValidator()
        {
            RuleFor(x => x.PublisherName)
                .NotEmpty().WithMessage("Tên nhà xuất bản không được để trống.")
                .MaximumLength(255).WithMessage("Tên nhà xuất bản không được vượt quá 255 ký tự.");

            RuleFor(x => x.Address)
                .MaximumLength(500).WithMessage("Địa chỉ không được vượt quá 500 ký tự.");

            RuleFor(x => x.Phone)
                .MaximumLength(20).WithMessage("Số điện thoại không được vượt quá 20 ký tự.")
                .Matches(@"^\+?[0-9\s-]*$").WithMessage("Số điện thoại không đúng định dạng.")
                .When(x => !string.IsNullOrEmpty(x.Phone));

            RuleFor(x => x.Email)
                .MaximumLength(255).WithMessage("Email không được vượt quá 255 ký tự.")
                .EmailAddress().WithMessage("Email không đúng định dạng.")
                .When(x => !string.IsNullOrEmpty(x.Email));
        }
    }
}
