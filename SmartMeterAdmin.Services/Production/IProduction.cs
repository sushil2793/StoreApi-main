using System.Collections.Generic;
using System.Threading.Tasks;
using SmartMeterAdmin.ViewModel.DataTable;
using SmartMeterAdmin.ViewModel.Production;

namespace SmartMeterAdmin.Services.Production
{
    public interface IProduction
    {

        #region Import Meters From Excel
        Task SyncProductionMeter(int userid, int storeId);
        Task<IEnumerable<ProductionMeter>> GetProductionMetersList(AgGridTable dataTable);
        Task<DashboardCount> GetDashboardCount(int userID, string userRole);
        Task<IEnumerable<DashboardListData>> GetDashboardList(int userID, string userRole);
        Task<IEnumerable<string>> GetMeterNoList(string meterNo, string roleType, int userID);
        #endregion
    }
}
