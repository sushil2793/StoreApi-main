using Dapper;
using SmartMeterAdmin.Data;
using SmartMeterAdmin.Services.Dapper_ORM;
using SmartMeterAdmin.ViewModel.DataTable;
using SmartMeterAdmin.ViewModel.Production;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartMeterAdmin.Services.Production
{
    public class ProductionService : IProduction
    {
        private readonly IDapper dapper;

        public ProductionService(IDapper dapper)
        {
            this.dapper = dapper;
        }

        public async Task<DashboardCount> GetDashboardCount(int userID, string userRole)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@UserID", userID);
            parameters.Add("@UserRole", userRole);

            return await dapper.ExecuteProcedureFirstOrDefault<DashboardCount>("GetProductionDashboard", parameters);
        }

        public async Task<IEnumerable<DashboardListData>> GetDashboardList(int userID, string userRole)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@UserID", userID);
            parameters.Add("@UserRole", userRole);

            return await dapper.ExecuteProcedure<DashboardListData>("GetProductionDashboardList", parameters);
        }

        public async Task<IEnumerable<ProductionMeter>> GetProductionMetersList(AgGridTable model)
        {
            var parameters = GetAgGridDataFilterParameter(model);
            return await dapper.ExecuteProcedure<ProductionMeter>("GetProductionMeters", parameters);
        }


        public async Task SyncProductionMeter(int userid, int storeId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@USERID", userid);
            parameters.Add("@STOREID", storeId);

            await dapper.ExecuteProcedure("SYNCPRODUCTIONMETER", parameters);
        }

        public async Task<IEnumerable<string>> GetMeterNoList(string meterNo,string roleType,int userID)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@METERNO", meterNo);
            parameters.Add("@USER_ID", userID);
            parameters.Add("@ROLE_TYPE", roleType);
            return await dapper.ExecuteProcedure<string>("GET_METER_NO", parameters);
        }

        private static DynamicParameters GetAgGridDataFilterParameter(AgGridTable model)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@FromDate", model.FromDate);
            parameters.Add("@ToDate", model.ToDate);
            parameters.Add("@SearchColumn", model.SearchColumn);
            parameters.Add("@SearchValue", model.SearchValue);
            parameters.Add("@PageNumber", model.PageNumber);
            parameters.Add("@RecordLength", model.Length);
            parameters.Add("@USERID", model.UserId);
            parameters.Add("@ROLETYPE", model.RoleType);
            return parameters;
        }

    }
}
