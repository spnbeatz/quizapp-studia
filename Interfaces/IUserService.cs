using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using projekt.Models;

namespace projekt.Interfaces
{
    public interface IUserService : IDisposable
    {
        User? AuthenticateUser(string username, string password);
        string? GetUserPasswordHash(string username);
        bool RegisterUser(string username, string password);
        User? GetUserById(int userId);
    }
}
