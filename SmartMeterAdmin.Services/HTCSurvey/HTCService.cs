
using Dapper;
using SmartMeterAdmin.Data;
using SmartMeterAdmin.ViewModel;
using SmartMeterAdmin.ViewModel.Feeder;
using SmartMeterAdmin.ViewModel.HTCSurvey;
using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartMeterAdmin.Services.Dapper_ORM;
using SmartMeterAdmin.ViewModel.DataTable;
using SmartMeterAdmin.ViewModel.DTCSurvey;
using SmartMeterAdmin.ViewModel.MeterViewModel;

namespace SmartMeterAdmin.Services.HTCSurvey
{
    public class HTCService : IHTC
    {

        private readonly IDapper dapper;
        public HTCService(IDapper dapper)
        {
            this.dapper = dapper;
        }

        public async Task<HTCDetails> GetHTCDetails(string customerId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@consumerId", customerId);
            return await dapper.ExecuteProcedureFirstOrDefault<HTCDetails>("GetHTCDetails", parameters);
        }

        public async Task SaveHTCDetails(HTCDetails htcDetails, int userId)
        {
            var parameters = new DynamicParameters();


            parameters.Add("@ConsumerId", htcDetails.ConsumerId);
            parameters.Add("@NewConsumerId", htcDetails.NewConsumerId);
            parameters.Add("@Zone", htcDetails.Zone);
            parameters.Add("@CircleWise", htcDetails.CircleWise);
            parameters.Add("@Division", htcDetails.Division);
            parameters.Add("@Subdivision", htcDetails.Subdivision);
            parameters.Add("@Substation", htcDetails.Substation);
            parameters.Add("@SubstationCode", htcDetails.SubstationCode);
            parameters.Add("@DTFeeder", htcDetails.DTFeeder);
            parameters.Add("@DTFeederCode", htcDetails.DTFeederCode);
            parameters.Add("@DTName", htcDetails.DTName);
            parameters.Add("@DTCode", htcDetails.DTCode);
            parameters.Add("@ConsumerName", htcDetails.ConsumerName);
            parameters.Add("@ConsumerResponse", htcDetails.ConsumerResponse);
            parameters.Add("@IdentityType", htcDetails.IdentityType);
            parameters.Add("@AadharNo", htcDetails.AadharNo);
            parameters.Add("@ConsumerMobileNo", htcDetails.ConsumerMobileNo);
            parameters.Add("@ConsumerAddress", htcDetails.ConsumerAddress);
            parameters.Add("@CorrectConsumerAddress", htcDetails.CorrectConsumerAddress);
            parameters.Add("@District", htcDetails.District);
            parameters.Add("@TarrifName", htcDetails.TarrifName);
            parameters.Add("@SanctionLoad", htcDetails.SanctionLoad);
            parameters.Add("@MeterdConsumer", htcDetails.MeterdConsumer);
            parameters.Add("@MeterNo", htcDetails.MeterNo);
            parameters.Add("@CorrectMeterNo", htcDetails.CorrectMeterNo);
            parameters.Add("@MeterStatus", htcDetails.MeterStatus);
            parameters.Add("@MeterType", htcDetails.MeterType);
            parameters.Add("@GPRSSignalstrengthAirtel", htcDetails.GPRSSignalstrengthAirtel);
            parameters.Add("@GPRSSignalStrengthJio", htcDetails.GPRSSignalStrengthJio);
            parameters.Add("@Latitude", htcDetails.Latitude);
            parameters.Add("@Longitude", htcDetails.Longitude);
            parameters.Add("@SurveyAgency", htcDetails.SurveyAgency);
            parameters.Add("@MeterMode", htcDetails.MeterMode);
            parameters.Add("@FeederCategory", htcDetails.FeederCategory);
            parameters.Add("@FeederLengthKM", htcDetails.FeederLengthKM);
            parameters.Add("@FeederVoltageLevelKV", htcDetails.FeederVoltageLevelKV);
            parameters.Add("@FeederConductor", htcDetails.FeederConductor);
            parameters.Add("@FeederMonitored", htcDetails.FeederMonitored);
            parameters.Add("@MeterAvailableIfYes", htcDetails.MeterAvailableIfYes);
            parameters.Add("@OldMeterManufacturer", htcDetails.OldMeterManufacturer);
            parameters.Add("@MeterRating", htcDetails.MeterRating);
            parameters.Add("@MeterReadingCumKVAH", htcDetails.MeterReadingCumKVAH);
            parameters.Add("@MeterReadingCumKWH", htcDetails.MeterReadingCumKWH);
            parameters.Add("@MeterBoxAvailable", htcDetails.MeterBoxAvailable);
            parameters.Add("@MeterMultiplicationFactor", htcDetails.MeterMultiplicationFactor);
            parameters.Add("@CapturingOfCBStatus", htcDetails.CapturingOfCBStatus);
            parameters.Add("@LinePTArrangement", htcDetails.LinePTArrangement);
            parameters.Add("@CTPTAvailableIfYes", htcDetails.CTPTAvailableIfYes);
            parameters.Add("@CTMake", htcDetails.CTMake);
            parameters.Add("@CTRating", htcDetails.CTRating);
            parameters.Add("@CTType", htcDetails.CTType);
            parameters.Add("@CTConnectedRatio", htcDetails.CTConnectedRatio);
            parameters.Add("@PTMake", htcDetails.PTMake);
            parameters.Add("@PTRating", htcDetails.PTRating);
            parameters.Add("@PTType", htcDetails.PTType);
            parameters.Add("@PTConnectedRatio", htcDetails.PTConnectedRatio);
            parameters.Add("@NosOfDTsConnected", htcDetails.NosOfDTsConnected);
            parameters.Add("@PhotoWithCumKVAHUrl", htcDetails.PhotoWithCumKVAHUrl);
            parameters.Add("@PhotoWithCumKWHUrl", htcDetails.PhotoWithCumKWHUrl);
            parameters.Add("@PhotoWithKVRHUrl", htcDetails.PhotoWithKVRHUrl);
            parameters.Add("@TotalTransformationCapacityOfDTsKVA", htcDetails.TotalTransformationCapacityOfDTsKVA);
            parameters.Add("@Action", htcDetails.Action);
            parameters.Add("@UserId", userId);

            await dapper.ExecuteProcedure("SaveHTCDetails", parameters);
        }
        public async Task<IEnumerable<HTCMeterListViewModel>> GetHTCMetersList(AgGridTable model)
        {
            var parameters = GetAgGridDataFilterParameter(model);
            return await dapper.ExecuteProcedure<HTCMeterListViewModel>("GETALLHTCMETER", parameters);
        }
        
        public async Task SyncHTC(int userid)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@USERID", userid);
            await dapper.ExecuteProcedure("SYNCHTCSURVEY", parameters);
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