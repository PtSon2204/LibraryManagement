using FluentValidation;
using LibraryManagement.Business.DTOs.AuthDTOs;

namespace LibraryManagement.Business.Validators.AuthValidators
{
    public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email không được để trống")
                .EmailAddress().WithMessage("Email không đúng định dạng")
                .MaximumLength(100).WithMessage("Email tối đa 100 ký tự");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Mật khẩu không được để trống")
                .MinimumLength(6).WithMessage("Mật khẩu phải từ 6 ký tự trở lên");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Họ và tên không được để trống")
                .MaximumLength(255).WithMessage("Họ và tên tối đa 255 ký tự");
        }
    }
}
