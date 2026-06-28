using FluentValidation;
using LibraryManagement.Business.DTOs.AuthDTOs;

namespace LibraryManagement.Business.Validators.AuthValidators
{
    public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
    {
        public ChangePasswordDtoValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Mật khẩu hiện tại không được để trống.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("Mật khẩu mới không được để trống.")
                .MinimumLength(6).WithMessage("Mật khẩu mới phải có ít nhất 6 ký tự.");

            RuleFor(x => x.ConfirmNewPassword)
                .NotEmpty().WithMessage("Xác nhận mật khẩu mới không được để trống.")
                .Equal(x => x.NewPassword).WithMessage("Mật khẩu xác nhận không khớp với mật khẩu mới.");
        }
    }
}
