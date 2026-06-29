using System;
using FluentValidation;
using LibraryManagement.Business.DTOs.AuthorDTOs;

namespace LibraryManagement.Business.Validators.AuthorValidators
{
    public class UpdateAuthorDtoValidator : AbstractValidator<UpdateAuthorDto>
    {
        public UpdateAuthorDtoValidator()
        {
            RuleFor(x => x.AuthorId)
                .GreaterThan(0)
                    .WithMessage("ID tác giả không hợp lệ.");

            RuleFor(x => x.FullName)
                .NotEmpty()
                    .WithMessage("Tên tác giả không được để trống.")
                .MaximumLength(255)
                    .WithMessage("Tên tác giả không được vượt quá 255 ký tự.");
        }
    }
}
