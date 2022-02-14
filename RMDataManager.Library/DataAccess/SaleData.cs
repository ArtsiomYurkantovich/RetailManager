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
    public class SaleData : ISaleData
    {

        private readonly decimal _texRate = 8.75m;

        private readonly ISqlDataAccess _sql;
        private readonly IProductData _productData;
        private readonly IConfiguration _configuration;

        public SaleData(ISqlDataAccess sql, IProductData productData, IConfiguration configuration)
        {
            _sql = sql;
            _productData = productData;
           _configuration = configuration;
        }

        public void SaveSale(SaleModel saleInfo, string cashierId)
        {
            List<SaleDetailDBModel> details = new List<SaleDetailDBModel>();
            //var taxRate = /*ConfigHelper.GetTaxRate()*/;

            foreach (var item in saleInfo.SaleDetails)
            {
                var detail = new SaleDetailDBModel
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                };

                // Get the information  about this product

                var productInfo = _productData.GetProductById(detail.ProductId);
                if (productInfo == null)
                {
                    throw new Exception($"The product Id of {detail.ProductId} could not be found in database.");
                }

                detail.PurchasePrice = (productInfo.RetailPrice * detail.Quantity);

                if (productInfo.IsTaxable)
                {
                    detail.Tax = detail.PurchasePrice * _texRate / 100;
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

            try
            {
                _sql.StartTransaction("RMData");

                // Save the sale model
                _sql.SaveDataInTransaction("dbo.spSaleInsert", sale);

                // Get the Id from the sale model
                sale.Id = _sql.LoadDataInTransaction<int, dynamic>("spSaleLookUp", new { sale.CashierId, sale.SaleDate }).FirstOrDefault();

                // Finish filling in the sale detail model
                foreach (var item in details)
                {
                    item.SaleId = sale.Id;

                    // Save the same detail models
                    _sql.SaveDataInTransaction("dbo.spSaleDetailInsert", item);
                }

                _sql.CommitTransaction();
            }
            catch
            {
                _sql.RollbackTransaction();
                throw;
            }

        }

        public List<SaleReportModel> GetSaleReport()
        {
            var output = _sql.LoadData<SaleReportModel, dynamic>("dpo.spSaleReport", new { }, "RMData");
            return output;
        }
    }
}
