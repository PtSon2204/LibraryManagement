using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using LibraryManagement.Business.DTOs.UserProfileDTOs;
using LibraryManagement.Business.Interfaces;
using LibraryManagement.Data.UnitOfWorks;
using LibraryManagement.Models.Models;
using Microsoft.Identity.Client;

namespace LibraryManagement.Business.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserProfileService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UserProfileDto?> GetUserProfileAsync(Guid userId, string role)
        {
            if (role == "Reader")
            {
                var reader = await _unitOfWork.ReaderRepository.GetReaderByIdAsync(userId);

                if (reader == null)
                {
                    return null;
                }

                return new UserProfileDto
                {
                    Email = reader.Email,
                    FullName = reader.Profile.FullName,
                    Phone = reader.Profile?.Phone,
                    Address = reader.Profile?.Address,
                    DateOfBirth = reader.Profile?.DateOfBirth
                };
            }

            var account = await _unitOfWork.AccountRepository.GetAccountByIdAsync(userId);

            if (account == null) return null;

            return new UserProfileDto
            {
                Email = account.Email,
                FullName = account.Profile.FullName,
                Phone = account.Profile?.Phone,
                Address = account.Profile?.Address,
                DateOfBirth = account.Profile?.DateOfBirth
            };
        }

        public async Task UpdateUserProfileAsync(Guid userId, string role, UpdateUserProfileDto model)
        {
            if (role == "Reader")
            {
                var reader = await _unitOfWork.ReaderRepository.GetReaderByIdAsync(userId);

                reader.Profile.FullName = model.FullName;
                reader.Profile.Phone = model.Phone;
                reader.Profile.Address = model.Address;
                reader.Profile.DateOfBirth = model.DateOfBirth;

                _unitOfWork.ReaderRepository.UpdateReader(reader);
            }
            else
            {
                var account = await _unitOfWork.AccountRepository.GetAccountByIdAsync(userId);

                account.Profile.FullName = model.FullName;
                account.Profile.Phone = model.Phone;
                account.Profile.Address = model.Address;
                account.Profile.DateOfBirth = model.DateOfBirth;

                _unitOfWork.AccountRepository.UpdateAccount(account);
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
