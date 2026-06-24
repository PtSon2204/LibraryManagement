using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using LibraryManagement.Business.DTOs.AuthDTOs;

namespace LibraryManagement.Business.Validators.AuthValidators
{
    public class ForgotPasswordDtoValidator
    : AbstractValidator<ForgotPasswordDto>
    {
        public ForgotPasswordDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();
        }
    }
}
