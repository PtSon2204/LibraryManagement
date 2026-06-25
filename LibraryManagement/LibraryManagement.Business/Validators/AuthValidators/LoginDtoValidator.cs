using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using LibraryManagement.Business.DTOs.AuthDTOs;

namespace LibraryManagement.Business.Validators.AuthValidators
{
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                    .WithMessage("Email không được để trống.")
                .EmailAddress()
                    .WithMessage("Email không đúng định dạng.")
                .MaximumLength(100)
                    .WithMessage("Email không được vượt quá 100 ký tự.");

            RuleFor(x => x.Password)
                .NotEmpty()
                    .WithMessage("Mật khẩu không được để trống.")
                .MaximumLength(100)
                    .WithMessage("Mật khẩu không được vượt quá 100 ký tự.");
        }
    }
}
