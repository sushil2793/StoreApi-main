
using Dapper;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.UserModel;
using SmartMeterAdmin.Data.CustomerData;
using SmartMeterAdmin.Data.SimData;
using SmartMeterAdmin.Infrastructure.Data;
using SmartMeterAdmin.Infrastructure.Enumeration;
using SmartMeterAdmin.Services.Dapper_ORM;
using SmartMeterAdmin.ViewModel.DataTable;
using SmartMeterAdmin.ViewModel.DropDown;
using SmartMeterAdmin.ViewModel.Master;
using SmartMeterAdmin.ViewModel.SimDataView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace SmartMeterAdmin.Services.Master
{
    public class MasterService : IMaster
    {
        private readonly SmartMeterContext _context;
        private readonly IDapper _dapper;

        public MasterService(SmartMeterContext context, IDapper dapper)
        {
            _context = context;
            _dapper = dapper;
        }

        public async Task<IEnumerable<ZoneViewModel>> GetAllZone(string roleType)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@ROLE_TYPE", roleType);

            return await _dapper.ExecuteProcedure<ZoneViewModel>("GET_ZONE_LIST", parameters);
        }
        public async Task ManageZone(ZoneViewModel data)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@ZONE_NAME", data.ZoneName);
            parameters.Add("@USER_ID", data.CreatedBy);
            parameters.Add("@ID", data.Id);
            await _dapper.ExecuteProcedure("MANAGE_ZONE", parameters);
        }
        public async Task<IEnumerable<CircleViewModel>> GetAllCircles(string roleType)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@ROLE_TYPE", roleType);

            return await _dapper.ExecuteProcedure<CircleViewModel>("GET_CIRCLE_LIST", parameters);
        }
        public async Task ManageCircle(CircleViewModel data)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@CIRCLE_NAME", data.CircleName);
            parameters.Add("@ZONE_NAME", data.ZoneName);
            parameters.Add("@USER_ID", data.CreatedBy);
            parameters.Add("@ID", data.Id);

            await _dapper.ExecuteProcedure("MANAGE_CIRCLE", parameters);
        }
        public async Task ManageDivision(DivisionViewModel data)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@DIVISION_NAME", data.DivisionName);
            parameters.Add("@CIRCLE_NAME", data.CircleName);
            parameters.Add("@ZONE_NAME", data.ZoneName);
            parameters.Add("@USER_ID", data.CreatedBy);
            parameters.Add("@ID", data.Id);

            await _dapper.ExecuteProcedure("MANAGE_DIVISION", parameters);
        }
        public async Task ManageSubdivision(SubdivisionViewModel data)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@SUBDIVISION_NAME", data.SubDivisionName);
            parameters.Add("@DIVISION_NAME", data.DivisionName);
            parameters.Add("@CIRCLE_NAME", data.CircleName);
            parameters.Add("@ZONE_NAME", data.ZoneName);
            parameters.Add("@USER_ID", data.CreatedBy);
            parameters.Add("@ID", data.Id);

            await _dapper.ExecuteProcedure("MANAGE_SUBDIVISION", parameters);
        }
        public async Task ManageSubstation(SubstationViewModel data)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@SUBSTATION_NAME", data.SubStationName);
            parameters.Add("@SUBDIVISION_NAME", data.SubDivisionName);
            parameters.Add("@DIVISION_NAME", data.DivisionName);
            parameters.Add("@CIRCLE_NAME", data.CircleName);
            parameters.Add("@ZONE_NAME", data.ZoneName);
            parameters.Add("@USER_ID", data.CreatedBy);
            parameters.Add("@ID", data.Id);

            await _dapper.ExecuteProcedure("MANAGE_SUBSTATION", parameters);
        }
        public async Task ManageFeeder(FeederViewModel data)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@FEEDER_NAME", data.FeederName);
            parameters.Add("@SUBSTATION_NAME", data.SubStationName);
            parameters.Add("@SUBDIVISION_NAME", data.SubDivisionName);
            parameters.Add("@DIVISION_NAME", data.DivisionName);
            parameters.Add("@CIRCLE_NAME", data.CircleName);
            parameters.Add("@ZONE_NAME", data.ZoneName);
            parameters.Add("@USER_ID", data.CreatedBy);
            parameters.Add("@ID", data.Id);

            await _dapper.ExecuteProcedure("MANAGE_FEEDER", parameters);
        }
        public async Task ManageDT(DTViewModel data)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@DT_NAME", data.DTName);
            parameters.Add("@FEEDER_NAME", data.FeederName);
            parameters.Add("@SUBSTATION_NAME", data.SubStationName);
            parameters.Add("@SUBDIVISION_NAME", data.SubDivisionName);
            parameters.Add("@DIVISION_NAME", data.DivisionName);
            parameters.Add("@CIRCLE_NAME", data.CircleName);
            parameters.Add("@ZONE_NAME", data.ZoneName);
            parameters.Add("@USER_ID", data.CreatedBy);
            parameters.Add("@ID", data.Id);

            await _dapper.ExecuteProcedure("MANAGE_DT", parameters);
        }
        public async Task<IEnumerable<SubdivisionViewModel>> GetAllSubdivision(string roleType)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@ROLE_TYPE", roleType);

            return await _dapper.ExecuteProcedure<SubdivisionViewModel>("GET_SUBDIVISION_LIST", parameters);
        }
        public async Task<IEnumerable<DTViewModel>> GetAllDT(string roleType)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@ROLE_TYPE", roleType);

            return await _dapper.ExecuteProcedure<DTViewModel>("GET_DT_LIST", parameters);
        }
        public async Task<DTViewModel> GetDTData(string dtCode)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@DT_CODE", dtCode);

            return await _dapper.ExecuteProcedureFirstOrDefault<DTViewModel>("GET_DT_DETAIL", parameters);
        }
        public async Task<IEnumerable<FeederViewModel>> GetAllFeeder(string roleType)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@ROLE_TYPE", roleType);

            return await _dapper.ExecuteProcedure<FeederViewModel>("GET_FEEDER_LIST", parameters);
        }
        public async Task<IEnumerable<SubstationViewModel>> GetAllSubstation(string roleType)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@ROLE_TYPE", roleType);

            return await _dapper.ExecuteProcedure<SubstationViewModel>("GET_SUBSTATION_LIST", parameters);
        }
        public async Task<IEnumerable<DivisionViewModel>> GetAllDivision(string roleType)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@ROLE_TYPE", roleType);

            return await _dapper.ExecuteProcedure<DivisionViewModel>("GET_DIVISION_LIST", parameters);
        }
        public async Task<IEnumerable<DropDown>> GetAreaDropDown(string areaType, string areaValue)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@AREA_TYPE", areaType);
            parameters.Add("@AREA_VALUE", areaValue);

            return await _dapper.ExecuteProcedure<DropDown>("GET_AREA_DROPDOWN", parameters);
        }
        public async Task<DTViewModel> GetAreaDetails(string areaType, string areaValue)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@AREA_TYPE", areaType);
            parameters.Add("@AREA_VALUE", areaValue);

            return await _dapper.ExecuteProcedureFirstOrDefault<DTViewModel>("GET_AREA_DETAILS", parameters);
        }
        public async Task DeleteArea(string areaType, string areaValue)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@AREA_TYPE", areaType);
            parameters.Add("@AREA_VALUE", areaValue);

            await _dapper.ExecuteProcedure("DELETE_AREA", parameters);
        }
        public async Task<IList<SimData>> GetSimDatas()
        {

            return await _context.SimDatas.ToListAsync();

        }
        public async Task<string> GetIPAddress(string meterno)
        {
            return await _context.SimDatas.Where(a => a.MeterSerialNo == meterno).Select(a => a.IPV6).FirstOrDefaultAsync();


        }

        public async Task<IEnumerable<SimDataView>> GetSimDataRender(AgGridTable model)//sushil kam 
        {
            var parameters = new DynamicParameters();

            parameters.Add("@SearchValue", model.SearchValue);
            parameters.Add("@RecordLength", model.Length);
            parameters.Add("@SearchColumn", model.SearchColumn);
            try
            {
                return await _dapper.ExecuteProcedure<SimDataView>("Get_SIM_DATA_LIST", parameters);
            }
            catch (Exception )
            {
                throw;
            }
        }
        public async Task<IEnumerable<SimDataForMobile>> GetSimDataForMobile(DateTime dated)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@DATE", dated);
            return await _dapper.ExecuteProcedure<SimDataForMobile>("GET_SIM_DATA", parameters);
        }
        public async Task<IList<SimDataForMobile>> GetSimDataForMobile()
        {

            return await _context.SimDataViews.Select(a => new SimDataForMobile { MeterSerialNo = a.MeterSerialNo, MSISDN = a.MSISDN }).ToListAsync();


        }
        public async Task SyncData(int userid)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@USERID", userid);

            await _dapper.ExecuteProcedure("Pro_SyncSimData", parameters);
        }
        public async Task<IEnumerable<AddDeviceApiModel>> SyncSimStatusData(int userId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@USERID", userId);

            return await _dapper.ExecuteProcedure<AddDeviceApiModel>("SyncSimStatus", parameters);
        }
        public async Task SyncCustomerData(int userId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@USERID", userId);
            await _dapper.ExecuteProcedure("SyncCustomerData", parameters);
        }
        public async Task UpdateDataByKeys(int userId)
        {
            var parameters = new DynamicParameters();
            await _dapper.ExecuteProcedure("UPDATE_DATA_BY_KEYS", parameters);
        }
        public async Task SyncCustomerNoAndNameData(int userId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@USERID", userId);

            await _dapper.ExecuteProcedure<AddDeviceApiModel>("SyncCustomerNoAndName", parameters);
        }
        public async Task<IEnumerable<CustomerData>> GetCustomerData(string role, int userid, AgGridTable dataTable)
        {

            var parameters = new DynamicParameters();
            string WhereQuery = string.Empty;

            parameters.Add("@SearchValue", dataTable.SearchValue);

            string TopQuery;
            if (dataTable.Length == -1 || dataTable.Length == 0)
                TopQuery = string.Empty;
            else
                TopQuery = "top " + dataTable.Length + "";


            if (!string.IsNullOrEmpty(dataTable.SearchColumn) && !string.IsNullOrEmpty(dataTable.SearchValue))
            {
                if (string.IsNullOrEmpty(WhereQuery))
                    WhereQuery += " where  " + dataTable.SearchColumn + " like '%" + dataTable.SearchValue + "%'";
                else
                    WhereQuery += " and  " + dataTable.SearchColumn + " like '%" + dataTable.SearchValue + "%'";
            }

            string query = $"select " + TopQuery + " * from CustomerData " + WhereQuery + "";
            return await _dapper.GetAllAsync<CustomerData>(query, parameters);

        }
        public async Task DeleteSelectedCustomer(List<int> customerID)
        {

            var parameters = new DynamicParameters();
            parameters.Add("@id", customerID[0]);
            string parameterQuery = "CustomerID = @id";

            for (int i = 1; i < customerID.Count; i++)
            {
                parameters.Add("@id" + i + "", customerID[i]);
                parameterQuery += " or CustomerID = @id" + i + "";
            }

            string query = "delete from CustomerData WHERE " + parameterQuery + "";
            await _dapper.Delete(query, parameters);

        }
        public async Task DeleteSelectedSim(List<int> simId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@id", simId[0]);
            string parameterQuery = "SimId = @id";

            for (int i = 1; i < simId.Count; i++)
            {
                parameters.Add("@id" + i + "", simId[i]);
                parameterQuery += " or SimId = @id" + i + "";
            }

            string query = "delete from SimData WHERE " + parameterQuery + "";
            await _dapper.Delete(query, parameters);

        }
        public async Task<int> CountCustomerData(string role, int userid, string searchKey)
        {

            if (role == "Superadmin" || role == "Admin")
            {
                return await _context.CustomerData.Where(a =>
                a.CustomerNo.ToLower().Contains(searchKey.ToLower()) ||
                a.NewConsumerNo.ToLower().Contains(searchKey.ToLower()) ||
                a.ConsumerName.ToLower().Contains(searchKey.ToLower()) ||
                a.CustomerAddress.ToLower().Contains(searchKey.ToLower()) ||
                a.Dist_Division.ToLower().Contains(searchKey.ToLower()) ||
                a.SubDivision.ToLower().Contains(searchKey.ToLower()) ||
                a.SubStation.ToLower().Contains(searchKey.ToLower()) ||
                a.Feeder.ToLower().Contains(searchKey.ToLower()) ||
                a.DT.ToLower().Contains(searchKey.ToLower()) ||
                a.District.ToLower().Contains(searchKey.ToLower()) ||
                a.Division.ToLower().Contains(searchKey.ToLower()) ||
                a.IsReplaceMeter.ToString().ToLower().Contains(searchKey.ToLower())
                ).CountAsync();

            }
            else
            {
                return await _context.CustomerData.Where(a =>
                a.CustomerNo.ToLower().Contains(searchKey.ToLower()) ||
                a.NewConsumerNo.ToLower().Contains(searchKey.ToLower()) ||
                a.ConsumerName.ToLower().Contains(searchKey.ToLower()) ||
                a.CustomerAddress.ToLower().Contains(searchKey.ToLower()) ||
                a.Dist_Division.ToLower().Contains(searchKey.ToLower()) ||
                a.SubDivision.ToLower().Contains(searchKey.ToLower()) ||
                a.SubStation.ToLower().Contains(searchKey.ToLower()) ||
                a.Feeder.ToLower().Contains(searchKey.ToLower()) ||
                a.DT.ToLower().Contains(searchKey.ToLower()) ||
                a.District.ToLower().Contains(searchKey.ToLower()) ||
                a.Division.ToLower().Contains(searchKey.ToLower()) ||
                a.IsReplaceMeter.ToString().ToLower().Contains(searchKey.ToLower())
                ).CountAsync();
            }
        }
        public async Task<bool> CheckCustomerNoAvilable(string customerno)
        {
            return await _context.CustomerData.Where(a => a.CustomerNo == customerno).AnyAsync();
        }
        public async Task UpdateCustomerDataByCustomerNo(string customerno)
        {
            CustomerData customerData = await _context.CustomerData.Where(a => a.CustomerNo == customerno).FirstOrDefaultAsync();
            if (customerData != null)
            {
                customerData.IsReplaceMeter = false;
                _context.CustomerData.Update(customerData);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<string> GetCustomerNoByNewConsumer(string newcustomerno)
        {
            return await _context.CustomerData.Where(a => a.NewConsumerNo == newcustomerno).Select(a => a.CustomerNo).FirstOrDefaultAsync();
        }
        public async Task<ExportExcelResponse> ManageExportDataExcel(string actionType, string dataType, string excelFileUrl)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@DATATYPE", dataType);
            parameters.Add("@EXCEL_URL", excelFileUrl);
            parameters.Add("@ACTION_TYPE", actionType);

            return await _dapper.ExecuteProcedureFirstOrDefault<ExportExcelResponse>("MANAGE_EXPORT_DATA_EXCEL", parameters);
        }

        #region Dynamic Table Column
        public async Task<IEnumerable<TableColumnDto>> GetTableColumnByUserId(TableNameEnum table, int userId, int templateId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@TABLE_NAME", table.ToString());
            parameters.Add("@USER_ID", userId);
            parameters.Add("@TEMPLATE_ID", templateId);

            return await _dapper.ExecuteProcedure<TableColumnDto>("GET_TABLE_COLUMN", parameters);
        }
        public async Task UpdateTableColumnByUserId(IEnumerable<TableColumnDto> columnDtos, int userId)
        {
            foreach (var item in columnDtos)
            {
                var parameters = new DynamicParameters();
                parameters.Add("@TableId", item.TableId);
                parameters.Add("@UserColumnId", item.UserColumnId);
                parameters.Add("@IsSelected", item.IsSelected);
                parameters.Add("@UserId", userId);
                parameters.Add("@TemplateId", item.TemplateId);
                await _dapper.ExecuteProcedure<TableColumnDto>("MANAGE_USER_TABLE_COLUMN", parameters);
            }
        }

        public async Task<IEnumerable<TableTemplate>> GetTableTemplate(int userId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@USERID", userId);
            parameters.Add("@ID", 0);

            return await _dapper.ExecuteProcedure<TableTemplate>("GET_TABLE_TEMPLATE", parameters);
        }

        public async Task<TableTemplate> GetTableTemplate(int userId, int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@USERID", userId);
            parameters.Add("@ID", id);

            return await _dapper.ExecuteProcedureFirstOrDefault<TableTemplate>("GET_TABLE_TEMPLATE", parameters);
        }
        public async Task ManageTableTemplate(TableTemplate template, string action)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@NAME", template.Name);
            parameters.Add("@ID", template.Id);
            parameters.Add("@USREID", template.UserId);
            parameters.Add("@ACTION", action);

            await _dapper.ExecuteProcedure("MANAGE_TABLE_TEMPLATE", parameters);
        }

        public async Task<IEnumerable<DropDown>> GetTableTemplateDropDown(RoleEnum role, int userId, string tableName)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ROLETYPE", role.ToString());
            parameters.Add("@USERID", userId);
            parameters.Add("@TABLENAME", tableName);
            return await _dapper.ExecuteProcedure<DropDown>("GET_DROPDOWN", parameters);
        }

        #endregion
    }
}
