using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryManagement.Business.DTOs.UserDTOs;

namespace LibraryManagement.Business.Interfaces
{
    public interface IUserService
    {
        Task<UserProfileDto> GetProfileAsync(Guid id);
        Task<UserProfileDto> UpdateProfileAsync(Guid id, UpdateProfileDto model);
    }
}
