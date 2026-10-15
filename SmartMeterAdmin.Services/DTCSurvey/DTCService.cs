
using Dapper;
using SmartMeterAdmin.Data;
using SmartMeterAdmin.ViewModel;
using SmartMeterAdmin.ViewModel.Feeder;
using SmartMeterAdmin.ViewModel.Survey;
using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartMeterAdmin.Services.Dapper_ORM;
using SmartMeterAdmin.ViewModel.DataTable;
using SmartMeterAdmin.ViewModel.DTCSurvey;
using SmartMeterAdmin.ViewModel.MeterViewModel;

namespace SmartMeterAdmin.Services.DTCSurvey
{
    public class DTCService : IDTC
    {

        private readonly IDapper dapper;
        public DTCService(IDapper dapper)
        {
            this.dapper = dapper;
        }
        public async Task SaveDTCSurvey(DTCDetails dtcDetails, int userId)
        {
            var parameters = new DynamicParameters();


            parameters.Add("@ConsumerId", dtcDetails.ConsumerId);
            parameters.Add("@NewConsumerId", dtcDetails.NewConsumerId);
            parameters.Add("@Zone", dtcDetails.Zone);
            parameters.Add("@CircleWise", dtcDetails.CircleWise);
            parameters.Add("@Division", dtcDetails.Division);
            parameters.Add("@Subdivision", dtcDetails.Subdivision);
            parameters.Add("@Substation", dtcDetails.Substation);
            parameters.Add("@SubstationCode", dtcDetails.SubstationCode);
            parameters.Add("@DTFeeder", dtcDetails.DTFeeder);
            parameters.Add("@DTFeederCode", dtcDetails.DTFeederCode);
            parameters.Add("@DTName", dtcDetails.DTName);
            parameters.Add("@DTCode", dtcDetails.DTCode);
            parameters.Add("@ConsumerName", dtcDetails.ConsumerName);
            parameters.Add("@ConsumerResponse", dtcDetails.ConsumerResponse);
            parameters.Add("@ConsumerRelativeName", dtcDetails.ConsumerRelativeName);
            parameters.Add("@IdentityType", dtcDetails.IdentityType);
            parameters.Add("@AadharNo", dtcDetails.AadharNo);
            parameters.Add("@ConsumerMobileNo", dtcDetails.ConsumerMobileNo);
            parameters.Add("@ConsumerAddress", dtcDetails.ConsumerAddress);
            parameters.Add("@CorrectConsumerAddress", dtcDetails.CorrectConsumerAddress);
            parameters.Add("@District", dtcDetails.District);
            parameters.Add("@TarrifName", dtcDetails.TarrifName);
            parameters.Add("@SanctionLoad", dtcDetails.SanctionLoad);
            parameters.Add("@MeterdConsumer", dtcDetails.MeterdConsumer);
            parameters.Add("@ConnectionStatus", dtcDetails.ConnectionStatus);
            parameters.Add("@MeterMake", dtcDetails.MeterMake);
            parameters.Add("@MeterNo", dtcDetails.MeterNo);
            parameters.Add("@CorrectMeterNo", dtcDetails.CorrectMeterNo);
            parameters.Add("@MeterBoxStatus", dtcDetails.MeterBoxStatus);
            parameters.Add("@MeterStatus", dtcDetails.MeterStatus);
            parameters.Add("@MeterSealStatus", dtcDetails.MeterSealStatus);
            parameters.Add("@MeterType", dtcDetails.MeterType);
            parameters.Add("@MeterLocation", dtcDetails.MeterLocation);
            parameters.Add("@MeterPhotoUrl", dtcDetails.MeterPhotoUrl);
            parameters.Add("@MeterReading", dtcDetails.MeterReading);
            parameters.Add("@GPRSSignalstrengthAirtel", dtcDetails.GPRSSignalstrengthAirtel);
            parameters.Add("@GPRSSignalStrengthJio", dtcDetails.GPRSSignalStrengthJio);
            parameters.Add("@Latitude", dtcDetails.Latitude);
            parameters.Add("@Longitude", dtcDetails.Longitude);
            parameters.Add("@MeterBoxSealNo", dtcDetails.MeterBoxSealNo);
            parameters.Add("@MeterTerminalSealNo", dtcDetails.MeterTerminalSealNo);
            parameters.Add("@SurveyAgency", dtcDetails.SurveyAgency);
            parameters.Add("@MeterMode", dtcDetails.MeterMode);
            parameters.Add("@DtRatingKVA", dtcDetails.DtRatingKVA);
            parameters.Add("@CtRatio", dtcDetails.CtRatio);
            parameters.Add("@OldMeterReadingCumKVAH", dtcDetails.OldMeterReadingCumKVAH);
            parameters.Add("@OldMeterReadingCumKWH", dtcDetails.OldMeterReadingCumKWH);
            parameters.Add("@PhotoOfMeterOfCumKVAHUrl", dtcDetails.PhotoOfMeterOfCumKVAHUrl);
            parameters.Add("@PhotoWithMeterCumReadingKWHUrl", dtcDetails.PhotoWithMeterCumReadingKWHUrl);
            parameters.Add("@UserId", userId);
            parameters.Add("@Action", dtcDetails.Action);
            await dapper.ExecuteProcedure("SaveDTCDetails", parameters);
        }

        public async Task<DTCDetails> GetDTCSurveyByConsumerId(string customerId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@consumerId", customerId);
            return await dapper.ExecuteProcedureFirstOrDefault<DTCDetails>("GetDTCDetails", parameters);
        }

        public async Task SyncDTCSurvey(int userid)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@USERID", userid);
            await dapper.ExecuteProcedure("SYNCDTCSURVEY", parameters);
        }
        public async Task<IEnumerable<DTCMeterListViewModel>> GetDTCMetersList(AgGridTable model)
        {
            var parameters = GetAgGridDataFilterParameter(model);
            return await dapper.ExecuteProcedure<DTCMeterListViewModel>("GETALLDTCMETER", parameters);
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