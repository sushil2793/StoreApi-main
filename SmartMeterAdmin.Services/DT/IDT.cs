using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMeterAdmin.ViewModel.Survey;
using SmartMeterAdmin.ViewModel.DT;
using SmartMeterAdmin.ViewModel.DataTable;

namespace SmartMeterAdmin.Services.DT
{
    public interface IDT
    {
        Task SyncDTSurvey(int userid);
        Task SaveDTDetails(DTDetails dtDetails, int userId);
        Task<IEnumerable<DTList>> GetDTList(AgGridTable table);
        Task<DTDetails> GetDTDetails(string customerId);
        Task<IEnumerable<DTMeterListViewModel>> GetDTMetersList(AgGridTable dataTable);
        Task<IEnumerable<DTMeterListViewModel>> GetDTDataList(string role, int userid, AgGridTable dataTable);
        Task DeleteDTRecord(string consumerId);
    }
}

