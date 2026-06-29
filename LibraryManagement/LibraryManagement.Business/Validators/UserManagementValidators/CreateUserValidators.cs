using System;
using FluentValidation;
using LibraryManagement.Business.DTOs.UserManagementDTOs;

namespace LibraryManagement.Business.Validators.UserManagementValidators
{
    public class CreateLibrarianDtoValidator : AbstractValidator<CreateLibrarianDto>
    {
        public CreateLibrarianDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                    .WithMessage("Email không được để trống.")
                .EmailAddress()
                    .WithMessage("Email không đúng định dạng.")
                .MaximumLength(255)
                    .WithMessage("Email không được vượt quá 255 ký tự.");

            RuleFor(x => x.FullName)
                .NotEmpty()
                    .WithMessage("Họ và tên không được để trống.")
                .MaximumLength(255)
                    .WithMessage("Họ và tên không được vượt quá 255 ký tự.");

            RuleFor(x => x.Phone)
                .MaximumLength(20)
                    .WithMessage("Số điện thoại không được vượt quá 20 ký tự.")
                .Matches(@"^[0-9+\s-]*$")
                    .WithMessage("Số điện thoại không hợp lệ.")
                .When(x => !string.IsNullOrEmpty(x.Phone));

            RuleFor(x => x.Address)
                .MaximumLength(500)
                    .WithMessage("Địa chỉ không được vượt quá 500 ký tự.");
        }
    }

    public class CreateReaderDtoValidator : AbstractValidator<CreateReaderDto>
    {
        public CreateReaderDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                    .WithMessage("Email không được để trống.")
                .EmailAddress()
                    .WithMessage("Email không đúng định dạng.")
                .MaximumLength(255)
                    .WithMessage("Email không được vượt quá 255 ký tự.");

            RuleFor(x => x.FullName)
                .NotEmpty()
                    .WithMessage("Họ và tên không được để trống.")
                .MaximumLength(255)
                    .WithMessage("Họ và tên không được vượt quá 255 ký tự.");

            RuleFor(x => x.Phone)
                .MaximumLength(20)
                    .WithMessage("Số điện thoại không được vượt quá 20 ký tự.")
                .Matches(@"^[0-9+\s-]*$")
                    .WithMessage("Số điện thoại không hợp lệ.")
                .When(x => !string.IsNullOrEmpty(x.Phone));

            RuleFor(x => x.Address)
                .MaximumLength(500)
                    .WithMessage("Địa chỉ không được vượt quá 500 ký tự.");
        }
    }
}
