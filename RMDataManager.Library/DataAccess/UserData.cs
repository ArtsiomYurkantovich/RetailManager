using Microsoft.Extensions.Configuration;
using RMDataManager.Library.Internal.DataAccess;
using RMDataManager.Library.Internal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMDataManager.Library.DataAccess
{
    public class UserData : IUserData
    {
        private readonly ISqlDataAccess _sql;

        public UserData(ISqlDataAccess sql)
        {
           _sql = sql;
        }

        public List<UserModel> GetUserById(string id)
        {
            var output = _sql.LoadData<UserModel, dynamic>("dbo.spUserLookUp", new { id}, "RMData");
            return output;
        }
    }
}
