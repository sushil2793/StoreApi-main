using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;

using SmartMeterAdmin.Services.Feeder;
using SmartMeterAdmin.ViewModel.DT;
using SmartMeterAdmin.ViewModel.Feeder;
using SmartMeterAdmin.ViewModel.Survey;
using System.Data.SqlClient;
using SmartMeterAdmin.Services.Dapper_ORM;
using SmartMeterAdmin.ViewModel.DataTable; // Add this line

namespace SmartMeterAdmin.Services.Feeder
{
    public class FeederService : IFeeder
    {
        private readonly IDapper dapper;
        private readonly IConfiguration configuration;
        public FeederService(IDapper dapper, IConfiguration configuration) // Update constructor
        {
            this.dapper = dapper;
            this.configuration = configuration; // Initialize configuration
        }
        public async Task<IEnumerable<FeederList>> GetFeederList(AgGridTable table)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@userId", table.UserId);
            parameters.Add("@fromDate", table.FromDate);
            parameters.Add("@toDate", table.ToDate);
            parameters.Add("@ROLETYPE", table.RoleType);

            return await dapper.ExecuteProcedure<FeederList>("GetFeederList", parameters);
        }
        public async Task<FeederDetails> GetFeederDetails(string consumerId, string meterNo)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@consumerId", consumerId);
            parameters.Add("@meterNo", meterNo);

            return await dapper.ExecuteProcedureFirstOrDefault<FeederDetails>("GetFeederDetails", parameters);
        }
        public async Task SaveFeederDetails(FeederDetails feederDetails, int UserId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ConsumerId", feederDetails.ConsumerId);
            parameters.Add("@DISCOMName", feederDetails.DISCOMName);
            parameters.Add("@DistrictName", feederDetails.DistrictName);
            parameters.Add("@NameOfBlock", feederDetails.NameOfBlock);
            parameters.Add("@Latitude", feederDetails.Latitude);
            parameters.Add("@Longitude", feederDetails.Longitude);
            parameters.Add("@CircleName", feederDetails.CircleName);
            parameters.Add("@DivisionName", feederDetails.DivisionName);
            parameters.Add("@DivisionId", feederDetails.DivisionId);
            parameters.Add("@SubDivisionName", feederDetails.SubDivisionName);
            parameters.Add("@SubDivisionId", feederDetails.SubDivisionId);
            parameters.Add("@FeedingSubStationName", feederDetails.FeedingSubStationName);
            parameters.Add("@FeedingSubStationId", feederDetails.FeedingSubStationId);
            parameters.Add("@VoltageRatio", feederDetails.VoltageRatio);
            parameters.Add("@CapacityMVA", feederDetails.CapacityMVA);
            parameters.Add("@Configuration", feederDetails.Configuration);
            parameters.Add("@Name", feederDetails.Name);
            parameters.Add("@Code", feederDetails.Code);
            parameters.Add("@Category", feederDetails.Category);
            parameters.Add("@Length", feederDetails.Length);
            parameters.Add("@VoltageLevel", feederDetails.VoltageLevel);
            parameters.Add("@ConductorType", feederDetails.ConductorType);
            parameters.Add("@OriginalCommissioningDate", feederDetails.OriginalCommissioningDate);
            parameters.Add("@IncomingFeederName", feederDetails.IncomingFeederName);
            parameters.Add("@MonitoredRTDAS", feederDetails.MonitoredRTDAS);
            parameters.Add("@MeterSerialNo", feederDetails.MeterSerialNo);
            parameters.Add("@MeterManufacturersName", feederDetails.MeterManufacturersName);
            parameters.Add("@MeterRating", feederDetails.MeterRating);
            parameters.Add("@MeterType", feederDetails.MeterType);
            parameters.Add("@MeterWorking", feederDetails.MeterWorking);
            parameters.Add("@MeterBoxAvailable", feederDetails.MeterBoxAvailable);
            parameters.Add("@MeterMultiplication", feederDetails.MeterMultiplication);
            parameters.Add("@CBStatus", feederDetails.CBStatus);
            parameters.Add("@LinePTArrangement", feederDetails.LinePTArrangement);
            parameters.Add("@CTPTAvailable", feederDetails.CTPTAvailable);
            parameters.Add("@CTMake", feederDetails.CTMake);
            parameters.Add("@CTRating", feederDetails.CTRating);
            parameters.Add("@CTType", feederDetails.CTType);
            parameters.Add("@CTConnectedRatio", feederDetails.CTConnectedRatio);
            parameters.Add("@PTMake", feederDetails.PTMake);
            parameters.Add("@PTRating", feederDetails.PTRating);
            parameters.Add("@PTType", feederDetails.PTType);
            parameters.Add("@PTCorrectedRatio", feederDetails.PTCorrectedRatio);
            parameters.Add("@CTPTMeterPhoto", feederDetails.CTPTMeterPhoto);
            parameters.Add("@CTMeterAvailable", feederDetails.CTMeterAvailable);
            parameters.Add("@PTMeterAvailable", feederDetails.PTMeterAvailable);
            parameters.Add("@DTConnected", feederDetails.DTConnected);
            parameters.Add("@TotalTransformationCapacityDT", feederDetails.TotalTransformationCapacityDT);
            parameters.Add("@UrbanConsumers", feederDetails.UrbanConsumers);
            parameters.Add("@RuralConsumers", feederDetails.RuralConsumers);
            parameters.Add("@IndustrialConsumers", feederDetails.IndustrialConsumers);
            parameters.Add("@AgriculturalConsumers", feederDetails.AgriculturalConsumers);
            parameters.Add("@PriorityConsumers", feederDetails.PriorityConsumers);
            parameters.Add("@BlockName", feederDetails.BlockName);
            parameters.Add("@MobileNoJE", feederDetails.MobileNoJE);
            parameters.Add("@MobileNoEE", feederDetails.MobileNoEE);
            parameters.Add("@GPRSSignalStrength", feederDetails.GPRSSignalStrength);
            parameters.Add("@SIMType", feederDetails.SIMType);
            parameters.Add("@AgencyName", feederDetails.AgencyName);
            parameters.Add("@Remarks", feederDetails.Remarks);
            parameters.Add("@MeterAvailable", feederDetails.MeterAvailable);
            parameters.Add("@UserId", UserId);
            parameters.Add("@MeterInstallationDate", feederDetails.MeterInstallationDate);
            parameters.Add("@Action", feederDetails.Action);
            await dapper.ExecuteProcedure("SaveFeederDetails", parameters);
        }
        public async Task<IEnumerable<FeederMeterListViewModel>> GetFeederMetersList(AgGridTable model)
        {
            var parameters = GetAgGridDataFilterParameter(model);
            return await dapper.ExecuteProcedure<FeederMeterListViewModel>("GETALLFEEDERMETER", parameters);
        }
        public async Task<IEnumerable<FeederMeterListViewModel>> GetFeederDataList(string role, int userid, AgGridTable model)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@RoleType", role);
            parameters.Add("@SearchValue", model.SearchValue);
            parameters.Add("@CreatedBy", userid);
            parameters.Add("@RecordLength", model.Length);
            parameters.Add("@SearchColumn", model.SearchColumn);

            return await dapper.ExecuteProcedure<FeederMeterListViewModel>("GETALLFEEDERDATA", parameters);
        }
        public async Task SyncFeederData(int userid)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@USERID", userid);
            await dapper.ExecuteProcedure("SYNCFEEDERSURVEY", parameters);
        }


        public async Task DeleteFeederRecord(string consumerId) // Update parameter name
        {
            var connectionString = configuration.GetConnectionString("DevConnection");
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();
            try
            {
                var queries = new[]
                {
                            "DELETE FROM FeederConsumer WHERE ConsumerId = @ConsumerId",
                            "DELETE FROM FeederMeter WHERE ConsumerId = @ConsumerId",
                            "DELETE FROM FeederSurvey WHERE ConsumerId = @ConsumerId"
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
