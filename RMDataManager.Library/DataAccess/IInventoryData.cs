using RMDataManager.Library.Internal.Models;
using System.Collections.Generic;

namespace RMDataManager.Library.DataAccess
{
    public interface IInventoryData
    {
        List<InventoryModel> GetInventory();
        void SaveInventoryRecord(InventoryModel item);
    }
}