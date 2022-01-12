using RMDataManager.Library.Internal.DataAccess;
using RMDataManager.Library.Internal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMDataManager.Library.DataAccess
{
    public class SaleData
    {
        public void SaveSale(SaleModel saleInfo, string cashierId)
        {
            List<SaleDetailDBModel> details = new List<SaleDetailDBModel>();
            ProductData products = new ProductData();
            var taxRate = ConfigHelper.GetTaxRate();

            foreach (var item in saleInfo.SaleDetails)
            {
                var detail = new SaleDetailDBModel
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                };

                // Get the information  about this product

                var productInfo = products.GetProductById(detail.ProductId);
                if (productInfo == null)
                {
                    throw new Exception($"The product Id of {detail.ProductId} could not be found in database.");
                }

                detail.PurchasePrice = (productInfo.RetailPrice * detail.Quantity);

                if (productInfo.IsTaxable)
                {
                    detail.Tax = (detail.PurchasePrice * taxRate) / 100;
                }

                details.Add(detail);
            }

            // Create the Sale model

            SaleDBModel sale = new SaleDBModel
            {
                SubTotal = details.Sum(x => x.PurchasePrice),
                Tax = details.Sum(x => x.Tax),
                CashierId = cashierId
            };

            sale.Total = sale.SubTotal + sale.Tax;

            // Save the sale model

            SqlDataAccess sql = new SqlDataAccess();
            sql.SaveData("dbo.spSaleInsert", sale, "RMData");

            // Get the Id from the sale model

            sale.Id = sql.LoadData<int, dynamic>("spSaleLookUp", new {sale.CashierId, sale.SaleDate }, "RMData").FirstOrDefault();
            // Finish filling in the sale detail model

            foreach (var item in details)
            {
                item.SaleId = sale.Id;

                // Save the same detail models
                sql.SaveData("dbo.spSaleDetailInsert", item, "RMData");
            }
        }
    }
}
