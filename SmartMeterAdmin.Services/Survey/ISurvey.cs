using SmartMeterAdmin.ViewModel.DataTable;
using SmartMeterAdmin.ViewModel.Survey;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartMeterAdmin.Services.Survey
{
    public interface ISurvey
    {
        Task ImportConsumerSurvey(int userId);
        Task SyncSurvey(int userid);
        Task SaveSurveyDetails(SurveyDetails surveyDetails, int userId);
        Task<IEnumerable<SurveyList>> GetSurveyList(AgGridTable table);
        Task<SurveyDetails> GetSurveyDetails(string customerId);
        Task<IEnumerable<SurveyMeterListViewModel>> GetSurveyUtilityData(string role, int userid, AgGridTable dataTable);
        Task<IEnumerable<SurveyMeterListViewModel>> GetSurveyMetersList(AgGridTable dataTable);
        Task<SurveyDashboardCount> GetSurveyDashboardCount(string roleType, int userId);
    }
}
