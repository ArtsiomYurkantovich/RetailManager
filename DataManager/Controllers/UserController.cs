﻿using Microsoft.AspNet.Identity;
using RMDataManager.Library.DataAccess;
using RMDataManager.Library.Internal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;


namespace DataManager.Controllers
{
    public class UserController : ApiController
    {
        [Authorize]
        public UserModel GetById()
        {
            string userId = RequestContext.Principal.Identity.GetUserId();
            UserData data = new UserData();
            return data.GetUserById(userId).First();
        }
    }
}