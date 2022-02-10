using RMDataManager.Library.Internal.Models;
using System.Collections.Generic;

namespace RMDataManager.Library.DataAccess
{
    public interface IUserData
    {
        List<UserModel> GetUserById(string id);
    }
}