using FluentValidation;
using LibraryManagement.Business.DTOs.RoomDTOs;

namespace LibraryManagement.Business.Validators.RoomValidators
{
    public class CreateRoomDtoValidator : AbstractValidator<CreateRoomDto>
    {
        public CreateRoomDtoValidator()
        {
            RuleFor(x => x.RoomName)
                .NotEmpty().WithMessage("Tên phòng không được để trống.")
                .MaximumLength(100).WithMessage("Tên phòng không được vượt quá 100 ký tự.");

            RuleFor(x => x.Capacity)
                .GreaterThan(0).WithMessage("Số chỗ ngồi tối đa phải lớn hơn 0.");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Trạng thái phòng không được để trống.")
                .Must(x => x == "Available" || x == "Occupied" || x == "Maintenance")
                .WithMessage("Trạng thái phòng không hợp lệ (Chỉ chấp nhận: Available, Occupied, Maintenance).");
        }
    }
}
