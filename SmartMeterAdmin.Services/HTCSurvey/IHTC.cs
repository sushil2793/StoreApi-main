using SmartMeterAdmin.ViewModel;
using SmartMeterAdmin.ViewModel.DataTable;
using SmartMeterAdmin.ViewModel.HTCSurvey;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartMeterAdmin.Services.HTCSurvey
{
    public interface IHTC
    {
        Task SyncHTC(int userid);
        Task SaveHTCDetails(HTCDetails htcDetails, int userId);
        Task<HTCDetails> GetHTCDetails(string customerId);
        Task<IEnumerable<HTCMeterListViewModel>> GetHTCMetersList(AgGridTable dataTable);
    }
}
