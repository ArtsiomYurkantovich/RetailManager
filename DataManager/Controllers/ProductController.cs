using RMDataManager.Library.DataAccess;
using RMDataManager.Library.Internal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DataManager.Controllers
{
    public class ProductController : ApiController
    {
        //[Authorize]
        public List<ProductModel> Get()
        {

            ProductData data = new ProductData();
            return data.GetProducts();
        }
    }
}
