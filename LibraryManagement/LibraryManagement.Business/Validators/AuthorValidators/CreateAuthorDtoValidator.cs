using System;
using FluentValidation;
using LibraryManagement.Business.DTOs.AuthorDTOs;

namespace LibraryManagement.Business.Validators.AuthorValidators
{
    public class CreateAuthorDtoValidator : AbstractValidator<CreateAuthorDto>
    {
        public CreateAuthorDtoValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty()
                    .WithMessage("Tên tác giả không được để trống.")
                .MaximumLength(255)
                    .WithMessage("Tên tác giả không được vượt quá 255 ký tự.");
        }
    }
}
