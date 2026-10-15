using SmartMeterAdmin.ViewModel.CustomerData;
using SmartMeterAdmin.ViewModel.DataTable;
using SmartMeterAdmin.ViewModel.Hes;
using SmartMeterAdmin.ViewModel.MeterViewModel;
using SmartMeterAdmin.ViewModel.Production;
using SmartMeterAdmin.ViewModel.Survey;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartMeterAdmin.Services.Meter
{
    public interface IMeter
    {
        #region Consumer
        Task SyncConsumerNewMeter(int userId);
        Task<IEnumerable<ConsumerFullNewMeterRenderViewModel>> GetFullNewConsumerMeterRender(AgGridTable dataTable);
        Task<IEnumerable<ConsumerNewMeterRenderViewModel>> GetAllNewConsumerMeterRender(AgGridTable dataTable);
        Task<IEnumerable<ConsumerNewMeterRenderViewModel>> GetConsumerNewMeterByConsumerId(string consumerId);
        Task<ConsumerNewMeterRenderViewModel> GetConsumerMeterDetails(string consumerId);
        Task<string> SaveConsumerNewMeterDetails(ConsumerNewMeterDetails ConsumerNewMeterDetails, int userId, string roleType);
        Task<string> UpdateConsumerNewMeterDetails(ConsumerNewMeterDetails model, int userId);
        Task<IEnumerable<ConsumerNewMeterListViewModel>> GetConsumerNewMeterList(int userId, string roleType);
        Task<IEnumerable<CustomerDataForMobile>> GetCustomerDetailsByCustomerNumber(int recivedata);
        Task<int> CountCustomerDetails();
        #endregion Consumer End 


        #region HTCMeter
        Task SyncHTCNewMeter(int userId);
        Task<IEnumerable<HTCFullNewMeterRenderViewModel>> GetFullNewHTCMeterRender(AgGridTable dataTable);
        Task<IEnumerable<HTCNewMeterRenderViewModel>> GetHTCNewMeterByConsumerId(string consumerId);
        Task<IEnumerable<HTCNewMeterRenderViewModel>> GetAllNewHTCMeterRender(AgGridTable dataTable);
        Task<HTCNewMeterRenderViewModel> GetHTCMeterDetails(string consumerId);
        Task<string> SaveHTCNewMeterDetails(HTCNewMeterDetails HTCNewMeterDetails, int userId, string roleType);
        Task<string> UpdateHTCNewMeterDetails(HTCNewMeterDetails HTCNewMeterDetails, int userId);
        #endregion HTCMeter End 

        #region DTCMeter
        Task SyncDTCNewMeter(int userId);
        Task<IEnumerable<DTCFullNewMeterRenderViewModel>> GetFullNewDTCMeterRender(AgGridTable dataTable);
        Task<IEnumerable<DTCNewMeterRenderViewModel>> GetDTCNewMeterByConsumerId(string consumerId);
        Task<IEnumerable<DTCNewMeterRenderViewModel>> GetAllNewDTCMeterRender(AgGridTable dataTable);
        Task<DTCNewMeterRenderViewModel> GetDTCMeterDetails(string consumerId);
        Task<string> SaveDTCNewMeterDetails(DTCNewMeterDetails DTCNewMeterDetails, int userId, string roleType);
        Task<string> UpdateDTCNewMeterDetails(DTCNewMeterDetails DTCNewMeterDetails, int userId);

        #endregion DTCMeter End 

        #region DT
        Task SyncDTNewMeter(int userid);
        Task<IEnumerable<DTFullNewMeterRenderViewModel>> GetFullNewDTMeterRender(AgGridTable dataTable);
        Task<DTNewMeterRenderViewModel> GetDTNewMeterByConsumerId(string consumerId);
        Task<IEnumerable<DTNewMeterRenderViewModel>> GetAllNewDTMeterRender(AgGridTable dataTable);
        Task<DTNewMeterRenderViewModel> GetDTMeterDetails(string consumerId);
        Task<string> SaveDTNewMeterDetails(DTNewMeterDetails DTNewMeterDetails, int userId, string roleType);
        Task<string> UpdateDTNewMeterDetails(DTNewMeterDetails DTNewMeterDetails, int userId);
        Task<IEnumerable<DTNewMeterListViewModel>> GetDTNewMeterList(int userId, string roleType);
        #endregion DT End

        #region Feeder

        Task SyncFeederNewMeter(int userId);
        Task<IEnumerable<FeederFullNewMeterRenderViewModel>> GetFullNewFeederMeterRender(AgGridTable dataTable);
        Task<FeederNewMeterRenderViewModel> GetFeederNewMeterByConsumerId(string consumerId);
        Task<IEnumerable<FeederNewMeterRenderViewModel>> GetAllNewFeederMeterRender(AgGridTable dataTable);
        Task<string> SaveFeederNewMeterDetails(FeederNewMeterDetails FeederNewMeterDetails, int userId, string roleType);
        Task<string> UpdateFeederNewMeterDetails(FeederNewMeterDetails FeederNewMeterDetails, int userId);
        Task<string> BulkUpdateLineRatiosAsync(string meterNo, string lineCTRatio, string linePTRatio, int userId);
        Task<FeederNewMeterRenderViewModel> GetFeederMeterDetails(string consumerId);
        Task<IEnumerable<FeederNewMeterListViewModel>> GetFeederNewMeterList(int userId, string roleType);

        #endregion Feeder End

        Task<MeterCountViewModel> GetMeterCountByID(int id, string roleType);
        Task<NewMeterDashboardCount> GetNewMeterDashboardCount(string roleType, int userId);
        Task UpdateHesStatus(string areaType, string areaValue, string status, string pushedBy = null);

        Task<IEnumerable<HesSendDTO>> GetPendingHesMeter(string areaType);
        Task<ProductionMeter> GetNewMeterDetials(string meterNo);
        Task<List<MeterPushInfo>> GetMeterNosByIds(string areaType, string commaIds);
        Task<IEnumerable<MeterCountFunc>> GetMeterCountWithName(string roleName, int Id);

        Task<string> SwapMeter(string consumerId, string oldMeterNo, string newMeterNo, int userId);

        // ── MeterSwap: full snapshot save + history list ──
        Task<object> SaveSwapMeterDetail(string consumerId, string oldMeterNo, string newMeterNo, int userId, string swapRemark = null);
        Task<IEnumerable<SwapMeterDetailViewModel>> GetSwapMeterList(string search, int pageNumber, int pageSize);
    }
}
