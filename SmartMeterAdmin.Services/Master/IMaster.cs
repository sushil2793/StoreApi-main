using SmartMeterAdmin.Data.CustomerData;
using SmartMeterAdmin.Data.SimData;
using SmartMeterAdmin.Infrastructure.Enumeration;
using SmartMeterAdmin.ViewModel.DataTable;
using SmartMeterAdmin.ViewModel.DropDown;
using SmartMeterAdmin.ViewModel.Master;
using SmartMeterAdmin.ViewModel.SimDataView;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartMeterAdmin.Services.Master
{
    public interface IMaster
    {
        Task<IEnumerable<DropDown>> GetAreaDropDown(string areaType, string areaValue);
        Task<DTViewModel> GetAreaDetails(string areaType, string areaValue);
        Task DeleteArea(string areaType, string areaValue);
        Task<ExportExcelResponse> ManageExportDataExcel(string actionType, string dataType, string excelFileUrl);

        #region Zone
        Task<IEnumerable<ZoneViewModel>> GetAllZone(string roleType);
        Task ManageZone(ZoneViewModel data);
        #endregion

        #region Circle
        Task<IEnumerable<CircleViewModel>> GetAllCircles(string roleType);
        Task ManageCircle(CircleViewModel data);
        #endregion

        #region Division
        Task<IEnumerable<DivisionViewModel>> GetAllDivision(string roleType);
        Task ManageDivision(DivisionViewModel data);
        #endregion


        #region Subdivision
        Task<IEnumerable<SubdivisionViewModel>> GetAllSubdivision(string roleType);
        Task ManageSubdivision(SubdivisionViewModel data);
        #endregion

        #region Substation

        Task<IEnumerable<SubstationViewModel>> GetAllSubstation(string roleType);
        Task ManageSubstation(SubstationViewModel data);
        #endregion

        #region Feeder
        Task<IEnumerable<FeederViewModel>> GetAllFeeder(string roleType);
        Task ManageFeeder(FeederViewModel data);
        #endregion

        #region DT
        Task<IEnumerable<DTViewModel>> GetAllDT(string roleType);
        Task<DTViewModel> GetDTData(string dtCode);
        Task ManageDT(DTViewModel data);
        #endregion

        #region SimData
        Task<IList<SimData>> GetSimDatas();

        Task<IEnumerable<SimDataForMobile>> GetSimDataForMobile(DateTime date);
        Task<IList<SimDataForMobile>> GetSimDataForMobile();
        Task<IEnumerable<SimDataView>> GetSimDataRender(AgGridTable dataTable);

        Task SyncData(int userid);
        Task<IEnumerable<AddDeviceApiModel>> SyncSimStatusData(int userId);
        Task SyncCustomerData(int userId);
        Task UpdateDataByKeys(int userId);
        Task SyncCustomerNoAndNameData(int userId);
        Task<string> GetIPAddress(string meterno);



        #endregion

        #region Customer Data


        Task<IEnumerable<CustomerData>> GetCustomerData(string role, int userid, AgGridTable dataTable);
        Task DeleteSelectedCustomer(List<int> customerID);
        Task DeleteSelectedSim(List<int> customerID);

        Task<int> CountCustomerData(string role, int userid, string searchKey);
        Task<bool> CheckCustomerNoAvilable(string customerno);
        Task UpdateCustomerDataByCustomerNo(string customerno);
        Task<string> GetCustomerNoByNewConsumer(string newcustomerno);


        #endregion

        #region Dynamic Table Column

        Task<IEnumerable<TableColumnDto>> GetTableColumnByUserId(TableNameEnum table, int userId, int templateId);
        Task UpdateTableColumnByUserId(IEnumerable<TableColumnDto> columnDtos, int userId);
        Task<IEnumerable<TableTemplate>> GetTableTemplate(int userId);
        Task<TableTemplate> GetTableTemplate(int userId, int id);
        Task<IEnumerable<DropDown>> GetTableTemplateDropDown(RoleEnum role, int userId, string tableName);
        Task ManageTableTemplate(TableTemplate template, string action);
        #endregion
    }
}
