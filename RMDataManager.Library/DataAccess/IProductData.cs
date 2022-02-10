using RMDataManager.Library.Internal.Models;
using System.Collections.Generic;

namespace RMDataManager.Library.DataAccess
{
    public interface IProductData
    {
        ProductModel GetProductById(int productId);
        List<ProductModel> GetProducts();
    }
}