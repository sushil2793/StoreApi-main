using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMeterAdmin.ViewModel.Survey;
using SmartMeterAdmin.ViewModel.Feeder;
using SmartMeterAdmin.ViewModel.DT;
using SmartMeterAdmin.ViewModel.DataTable;

namespace SmartMeterAdmin.Services.Feeder
{
    public interface IFeeder
    {
        Task SyncFeederData(int userid);
        Task SaveFeederDetails(FeederDetails feederDetails, int UserId);
        Task<IEnumerable<FeederList>> GetFeederList(AgGridTable table);
        Task<FeederDetails> GetFeederDetails(string consumerId, string meterNo);
        Task<IEnumerable<FeederMeterListViewModel>> GetFeederMetersList( AgGridTable dataTable);
        Task<IEnumerable<FeederMeterListViewModel>> GetFeederDataList(string role, int userid, AgGridTable dataTable);
        Task DeleteFeederRecord(string consumerId);
    }
}
