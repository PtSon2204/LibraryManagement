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
            .NotEmpty().WithMessage("Name không được để trống")
            .MaximumLength(100).WithMessage("Name tối đa 100 ký tự");

            RuleFor(x => x.Password)
                 .NotEmpty().WithMessage("Name không được để trống");
        }
    }
}
