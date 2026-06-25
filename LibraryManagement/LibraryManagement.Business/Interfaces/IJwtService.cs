using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Business.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(Guid id, string email, string role);
    }
}
