using Dapper;
using Microsoft.Extensions.Configuration;
using NPOI.SS.UserModel;
using SmartMeterAdmin.Services.Dapper_ORM;
using SmartMeterAdmin.Services.DT;
using SmartMeterAdmin.ViewModel.DataTable;
using SmartMeterAdmin.ViewModel.DT;
using SmartMeterAdmin.ViewModel.MeterViewModel;
using SmartMeterAdmin.ViewModel.Survey;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMeterAdmin.Services.DT
{
    public class DTService : IDT
    {
        private readonly IDapper dapper;
        private readonly IConfiguration configuration;

        public DTService(IDapper dapper, IConfiguration configuration)
        {
            this.dapper = dapper;
            this.configuration = configuration;
        }

        public async Task<IEnumerable<DTList>> GetDTList(AgGridTable table)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@userId", table.UserId);
            parameters.Add("@fromDate", table.FromDate);
            parameters.Add("@toDate", table.ToDate);
            parameters.Add("@ROLETYPE", table.RoleType);

            return await dapper.ExecuteProcedure<DTList>("GetDTList", parameters);
        }

        public async Task<DTDetails> GetDTDetails(string customerId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@customerId", customerId);
            return await dapper.ExecuteProcedureFirstOrDefault<DTDetails>("GetDTDetails", parameters);
        }

        public async Task SaveDTDetails(DTDetails dtDetails, int userId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ConsumerId", dtDetails.ConsumerId);
            parameters.Add("@ZoneName", dtDetails.ZoneName);
            parameters.Add("@CircleName", dtDetails.CircleName);
            parameters.Add("@DivisionName", dtDetails.DivisionName);
            parameters.Add("@SubDivisionName", dtDetails.SubDivisionName);
            parameters.Add("@FeedingSubStationName", dtDetails.FeedingSubStationName);
            parameters.Add("@FeedingSubStationCode", dtDetails.FeedingSubStationCode);
            parameters.Add("@FeederName", dtDetails.FeederName);
            parameters.Add("@FeederCode", dtDetails.FeederCode);
            parameters.Add("@DTLocationName", dtDetails.DTLocationName);
            parameters.Add("@DTRatingKVA", dtDetails.DTRatingKVA);
            parameters.Add("@NamePlateVisibility", dtDetails.NamePlateVisibility);
            parameters.Add("@DTMountingAt", dtDetails.DTMountingAt);
            parameters.Add("@DTAccessibility", dtDetails.DTAccessibility);
            parameters.Add("@DTCode", dtDetails.DTCode);
            parameters.Add("@DTType", dtDetails.DTType);
            parameters.Add("@VillageName", dtDetails.VillageName);
            parameters.Add("@VillageCensusCode", dtDetails.VillageCensusCode);
            parameters.Add("@StreetColonyLocality", dtDetails.StreetColonyLocality);
            parameters.Add("@Block", dtDetails.Block);
            parameters.Add("@District", dtDetails.District);
            parameters.Add("@MeterAvailable", dtDetails.MeterAvailable);
            parameters.Add("@MeterSerialNo", dtDetails.MeterSerialNo);
            parameters.Add("@MeterManufacturer", dtDetails.MeterManufacturer);
            parameters.Add("@MeterRating", dtDetails.MeterRating);
            parameters.Add("@MeterType", dtDetails.MeterType);
            parameters.Add("@MeterWorking", dtDetails.MeterWorking);
            parameters.Add("@MeterBoxAvailable", dtDetails.MeterBoxAvailable);
            parameters.Add("@TTBAvailable", dtDetails.TTBAvailable);
            parameters.Add("@LTCTAvailable", dtDetails.LTCTAvailable);
            parameters.Add("@NoOfLTCTAvailable", dtDetails.NoOfLTCTAvailable);
            parameters.Add("@LTCTMake", dtDetails.LTCTMake);
            parameters.Add("@LTCTRating", dtDetails.LTCTRating);
            parameters.Add("@LTCTType", dtDetails.LTCTType);
            parameters.Add("@LTCTMeterFittedInSMC", dtDetails.LTCTMeterFittedInSMC);
            parameters.Add("@LTPowerCableAvailable", dtDetails.LTPowerCableAvailable);
            parameters.Add("@CableSize", dtDetails.CableSize);
            parameters.Add("@LTPowerCableType", dtDetails.LTPowerCableType);
            parameters.Add("@LTPowerCableRating", dtDetails.LTPowerCableRating);
            parameters.Add("@LTPowerCableTerminatingAt", dtDetails.LTPowerCableTerminatingAt);
            parameters.Add("@ExistingLTCableSuitable", dtDetails.ExistingLTCableSuitable);
            parameters.Add("@AluminiumCableLugsRequired", dtDetails.AluminiumCableLugsRequired);
            parameters.Add("@LugSize", dtDetails.LugSize);
            parameters.Add("@NoOfLugs", dtDetails.NoOfLugs);
            parameters.Add("@Remark", dtDetails.Remark);
            parameters.Add("@MeterSitePhoto", dtDetails.MeterSitePhoto);
            parameters.Add("@CTSitePhoto", dtDetails.CTSitePhoto);
            parameters.Add("@Latitude", dtDetails.Latitude);
            parameters.Add("@Longitude", dtDetails.Longitude);
            parameters.Add("@GPRSSignalStrengthAirtel", dtDetails.GPRSSignalStrengthAirtel);
            parameters.Add("@GPRSSignalStrengthJio", dtDetails.GPRSSignalStrengthJio);
            parameters.Add("@SurveyAgencyName", dtDetails.SurveyAgencyName);
            parameters.Add("@AdditionalRemark", dtDetails.AdditionalRemark);
            parameters.Add("@UserId", userId);
            parameters.Add("@Action", dtDetails.Action);
            await dapper.ExecuteProcedure("SaveDTSurveyDetails", parameters);
        }

        public async Task<IEnumerable<DTMeterListViewModel>> GetDTMetersList(AgGridTable model)
        {
            var parameters = GetAgGridDataFilterParameter(model);
            return await dapper.ExecuteProcedure<DTMeterListViewModel>("GETALLDTMETER", parameters);
        }

        public async Task<IEnumerable<DTMeterListViewModel>> GetDTDataList(string role, int userid, AgGridTable model)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@RoleType", role);
            parameters.Add("@SearchValue", model.SearchValue);
            parameters.Add("@CreatedBy", userid);
            parameters.Add("@RecordLength", model.Length);
            parameters.Add("@SearchColumn", model.SearchColumn);
            return await dapper.ExecuteProcedure<DTMeterListViewModel>("GETALLDTDATA", parameters);
        }

        public async Task SyncDTSurvey(int userid)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@USERID", userid);
            await dapper.ExecuteProcedure("SYNCDTSURVEY", parameters);
        }

        public async Task DeleteDTRecord(string consumerId) // Update parameter name
        {
            var connectionString = configuration.GetConnectionString("DevConnection");
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();
            try
            {
                var queries = new[]
                {
                            "DELETE FROM DTConsumer WHERE ConsumerId = @ConsumerId",
                            "DELETE FROM DTMeter WHERE ConsumerId = @ConsumerId",
                            "DELETE FROM DTSurvey WHERE ConsumerId = @ConsumerId"
                        };

                foreach (var query in queries)
                {
                    using var command = new SqlCommand(query, connection, transaction);
                    command.Parameters.AddWithValue("@ConsumerId", consumerId); // Update parameter name
                    await command.ExecuteNonQueryAsync();
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
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
