﻿using RMDataManager.Library.Internal.DataAccess;
using RMDataManager.Library.Internal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMDataManager.Library.DataAccess
{
    public class ProductData
    {
        public List<ProductModel> GetProducts()
        {
            SqlDataAccess sql = new SqlDataAccess();
         
            var output = sql.LoadData<ProductModel, dynamic>("dbo.spProductGetAll", new { }, "RMData");
            return output;
        }
    }
}