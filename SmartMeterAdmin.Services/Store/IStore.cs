using SmartMeterAdmin.ViewModel.DataTable;
using SmartMeterAdmin.ViewModel.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmartMeterAdmin.Services.Store
{
    public interface IStore
    {
        Task SyncIssueMeterByMeterNo(int userId);
        Task<IEnumerable<T>> GetListOfMeterBox<T>(MeterRequest request);
        Task<IEnumerable<T>> GetListOfIssueMeters<T>(AgGridTable request, int storeId);
        Task<IEnumerable<T>> GetRetunMeters<T>(AgGridTable request, int storeID);
        Task AcceptProductionMeter(ListOfMeterNumber request);
        Task IssueProductionMeter(ListOfMeterNumber request);
        Task UpdateReturnMeter(string areaType, string areaValue);
        Task DeleteDataWithBackupProcedure(string actionType, List<string> keyValues, string deletedBy);

        Task<IEnumerable<ContractorSummaryDto>> GetContractorData(int userId, string roleType);
        Task<JsonElement?> GetMeterReplacementDataDynamicAsync(string accountId);
        Task SaveMeterReplacementDataDynamicAsync(Dictionary<string, string> data);
        Task<string> UpdateConsumerRemark(
         string consumerId,
         string tableName,
         string remark,
         string remarkDescription,
         ClaimsPrincipal userContext
     );
        Task<string> UpdateReplacementMeter(
             string areaType,
             string areaValue,
             string newAreaValue,
             string newInitialNetMeterImportReadingKWH,
             string newInitialNetMeterExportReadingKWH,
             string newInitialNetMeterImportReadingKVAH,
             string newInitialNetMeterExportReadingKVAH,
             string oldInitialNetMeterImportReadingKWH,
             string oldInitialNetMeterExportReadingKWH,
             string oldInitialNetMeterImportReadingKVAH,
             string oldInitialNetMeterExportReadingKVAH,
             string oldMeterPhotoUrl,
             string newMeterPhotoUrl
            );


        Task<IEnumerable<T>> GetReplacementMeters<T>();

        // HES UI session se old meter ki latest readings laane ke liye (proxy) - raw JSON string
        Task<string?> GetOldMeterReadingsFromHesAsync(string meterNo);



    }
}
