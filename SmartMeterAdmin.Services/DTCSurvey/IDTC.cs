using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMeterAdmin.ViewModel.DTCSurvey;
using SmartMeterAdmin.ViewModel.DataTable;
using SmartMeterAdmin.ViewModel;

namespace SmartMeterAdmin.Services.DTCSurvey
{
    public interface IDTC
    {
        Task SaveDTCSurvey(DTCDetails dtcDetails, int userId);
        Task<DTCDetails> GetDTCSurveyByConsumerId(string customerId);
        Task<IEnumerable<DTCMeterListViewModel>> GetDTCMetersList(AgGridTable dataTable);
        Task SyncDTCSurvey(int userid);
    }
}

