using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryManagement.Business.DTOs.UserProfileDTOs;

namespace LibraryManagement.Business.Interfaces
{
    public interface IUserProfileService
    {
        Task<UserProfileDto?> GetUserProfileAsync(Guid userId, string role);
        Task UpdateUserProfileAsync(Guid userId, string role, UpdateUserProfileDto model);
    }
}
