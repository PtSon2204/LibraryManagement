using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using LibraryManagement.Business.DTOs.AuthDTOs;

namespace LibraryManagement.Business.Validators.AuthValidators
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                    .WithMessage("Email không được để trống.")
                .EmailAddress()
                    .WithMessage("Email không đúng định dạng.")
                .MaximumLength(100)
                    .WithMessage("Email không được vượt quá 100 ký tự.");

            RuleFor(x => x.FullName)
               .NotEmpty()
                   .WithMessage("FullName không được để trống.")
               .MaximumLength(50)
                   .WithMessage("FullName không được vượt quá 50 ký tự.");

            RuleFor(x => x.Password)
                .NotEmpty()
                    .WithMessage("Mật khẩu không được để trống.")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$")
                    .WithMessage("Mật khẩu phải có ít nhất 8 ký tự, bao gồm chữ hoa, chữ thường, số và ký tự đặc biệt.")
                .MaximumLength(100)
                    .WithMessage("Mật khẩu không được vượt quá 100 ký tự.");
        }
    }
}
