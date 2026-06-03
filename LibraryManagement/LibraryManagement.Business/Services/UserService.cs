using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryManagement.Business.DTOs.UserDTOs;
using LibraryManagement.Business.Interfaces;
using LibraryManagement.Data.Interfaces;
using LibraryManagement.Data.UnitOfWorks;
using LibraryManagement.Models.Models;

namespace LibraryManagement.Business.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UserProfileDto> GetProfileAsync(Guid id)
        {
            var user = await _unitOfWork.UserRepository.GetUserByIdAsync(id);

            return new UserProfileDto
            {
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.Phone,
                Address = user.Address,
                DateOfBirth = user.DateOfBirth
            };
        }

        public Task<UserProfileDto> UpdateProfileAsync(Guid id, UpdateProfileDto model)
        {
            throw new NotImplementedException();
        }

        //public async Task<UserProfileDto> UpdateProfileAsync(Guid id, UpdateProfileDto dto)
        //{
        //    var user = await _unitOfWork.UserRepository.GetUserByIdAsync(id);

        //    user.FullName = dto.FullName;
        //    user.Phone = dto.Phone;
        //    user.Address = dto.Address;
        //    user.DateOfBirth = dto.DateOfBirth;

        //    await _unitOfWork.SaveChangesAsync();
        //}
    }
}
