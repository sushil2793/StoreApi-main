
using Dapper;
using SmartMeterAdmin.Data;
using SmartMeterAdmin.ViewModel.Feeder;
using SmartMeterAdmin.ViewModel.Survey;
using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartMeterAdmin.Services.Dapper_ORM;
using SmartMeterAdmin.ViewModel.DataTable;
using SmartMeterAdmin.ViewModel;
using SmartMeterAdmin.ViewModel.MeterViewModel;

namespace SmartMeterAdmin.Services.Survey
{
    public class SurveyService : ISurvey
    {

        private readonly IDapper dapper;
        public SurveyService(IDapper dapper)
        {
            this.dapper = dapper;
        }

        public async Task ImportConsumerSurvey(int userId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@USERID", userId);

            await dapper.ExecuteProcedure("ImportConsumerSurveyMerged", parameters);
        }

        public async Task SyncSurvey(int userid)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@USERID", userid);
            await dapper.ExecuteProcedure("SYNCCONSUMERSURVEY", parameters);
        }


        public async Task<IEnumerable<SurveyList>> GetSurveyList(AgGridTable table)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@userId", table.UserId);
            parameters.Add("@fromDate", table.FromDate);
            parameters.Add("@toDate", table.ToDate);
            parameters.Add("@ROLETYPE", table.RoleType);
            return await dapper.ExecuteProcedure<SurveyList>("GetSurveyList", parameters);
        }

        public async Task<SurveyDetails> GetSurveyDetails(string customerId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@consumerId", customerId);
            return await dapper.ExecuteProcedureFirstOrDefault<SurveyDetails>("GetSurveyDetails", parameters);
        }

        public async Task SaveSurveyDetails(SurveyDetails surveyDetails, int userId)
        {
            var parameters = new DynamicParameters();


            parameters.Add("@ConsumerId", surveyDetails.ConsumerId);
            parameters.Add("@Zone", surveyDetails.Zone);
            parameters.Add("@CircleWise", surveyDetails.CircleWise);
            parameters.Add("@Division", surveyDetails.Division);
            parameters.Add("@Subdivision", surveyDetails.Subdivision);
            parameters.Add("@Substation", surveyDetails.Substation);
            parameters.Add("@SubstationCode", surveyDetails.SubstationCode);
            parameters.Add("@DTFeeder", surveyDetails.DTFeeder);
            parameters.Add("@DTFeederCode", surveyDetails.DTFeederCode);
            parameters.Add("@DTName", surveyDetails.DTName);
            parameters.Add("@DTCode", surveyDetails.DTCode);
            parameters.Add("@ConsumerName", surveyDetails.ConsumerName);
            parameters.Add("@ConsumerResponse", surveyDetails.ConsumerResponse);
            parameters.Add("@ConsumerRelativeName", surveyDetails.ConsumerRelativeName);
            parameters.Add("@IdentityType", surveyDetails.IdentityType);
            parameters.Add("@AadharNo", surveyDetails.AadharNo);
            parameters.Add("@ConsumerMobileNo", surveyDetails.ConsumerMobileNo);
            parameters.Add("@ConsumerAddress", surveyDetails.ConsumerAddress);
            parameters.Add("@CorrectConsumerAddress", surveyDetails.CorrectConsumerAddress);
            parameters.Add("@District", surveyDetails.District);
            parameters.Add("@TarrifName", surveyDetails.TarrifName);
            parameters.Add("@SanctionLoad", surveyDetails.SanctionLoad);
            parameters.Add("@MeterdConsumer", surveyDetails.MeterdConsumer);
            parameters.Add("@ConnectionStatus", surveyDetails.ConnectionStatus);
            parameters.Add("@MeterMake", surveyDetails.MeterMake);
            parameters.Add("@MeterNo", surveyDetails.MeterNo);
            parameters.Add("@CorrectMeterNo", surveyDetails.CorrectMeterNo);
            parameters.Add("@MeterBoxStatus", surveyDetails.MeterBoxStatus);
            parameters.Add("@MeterStatus", surveyDetails.MeterStatus);
            parameters.Add("@MeterSealStatus", surveyDetails.MeterSealStatus);
            parameters.Add("@MeterType", surveyDetails.MeterType);
            parameters.Add("@MeterLocation", surveyDetails.MeterLocation);
            parameters.Add("@MeterPhotoUrl", surveyDetails.MeterPhotoUrl);
            parameters.Add("@MeterPhotoUrlKVAH", surveyDetails.MeterPhotoUrlKVAH);
            parameters.Add("@MeterReading", surveyDetails.MeterReading);
            parameters.Add("@GPRSSignalstrengthAirtel", surveyDetails.GPRSSignalstrengthAirtel);
            parameters.Add("@GPRSSignalStrengthJio", surveyDetails.GPRSSignalStrengthJio);
            parameters.Add("@Latitude", surveyDetails.Latitude);
            parameters.Add("@Longitude", surveyDetails.Longitude);
            parameters.Add("@MeterBoxSealNo", surveyDetails.MeterBoxSealNo);
            parameters.Add("@MeterTerminalSealNo", surveyDetails.MeterTerminalSealNo);
            parameters.Add("@SurveyAgency", surveyDetails.SurveyAgency);
            parameters.Add("@MeterMode", surveyDetails.MeterMode);
            parameters.Add("@NetMeterImportReading", surveyDetails.NetMeterImportReading);
            parameters.Add("@NetMeterExportReading", surveyDetails.NetMeterExportReading);
            parameters.Add("@UserId", userId);
            parameters.Add("@Action", surveyDetails.Action);
            await dapper.ExecuteProcedure("SaveSurveyDetails", parameters);
        }
        public async Task<IEnumerable<SurveyMeterListViewModel>> GetSurveyMetersList(AgGridTable model)
        {
            var parameters = GetAgGridDataFilterParameter(model);
            return await dapper.ExecuteProcedure<SurveyMeterListViewModel>("GETALLSURVEYMETER", parameters);
        }
        public async Task<IEnumerable<SurveyMeterListViewModel>> GetSurveyUtilityData(string role, int userid, AgGridTable model)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@RoleType", role);
            parameters.Add("@SearchValue", model.SearchValue);
            parameters.Add("@CreatedBy", userid);
            parameters.Add("@RecordLength", model.Length);
            parameters.Add("@SearchColumn", model.SearchColumn);

            return await dapper.ExecuteProcedure<SurveyMeterListViewModel>("GETALLSURVEYUTILITYDATA", parameters);
        }
      
        public async Task<SurveyDashboardCount> GetSurveyDashboardCount(string roleType, int userId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@USERID", userId);
            parameters.Add("@ROLETYPE", roleType);
            return await dapper.ExecuteProcedureFirstOrDefault<SurveyDashboardCount>("GET_SURVEY_COUNT_WEB", parameters);
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