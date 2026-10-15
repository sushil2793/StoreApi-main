using Dapper;
using SmartMeterAdmin.Services.Dapper_ORM;
using SmartMeterAdmin.ViewModel;
using SmartMeterAdmin.ViewModel.CustomerData;
using SmartMeterAdmin.ViewModel.DataTable;
using SmartMeterAdmin.ViewModel.Hes;
using SmartMeterAdmin.ViewModel.MeterViewModel;
using SmartMeterAdmin.ViewModel.Production;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Threading.Tasks;

namespace SmartMeterAdmin.Services.Meter
{
    public class MeterServices : IMeter
    {
        private readonly IDapper _dapper;
        public MeterServices(IDapper dapper)
        {
            _dapper = dapper;
        }

        #region Consumer 

        public async Task SyncConsumerNewMeter(int userid)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@USERID", userid);
            await _dapper.ExecuteProcedure("SYNCCONSUMERNEWMETER", parameters);
        }

        public async Task<IEnumerable<ConsumerFullNewMeterRenderViewModel>> GetFullNewConsumerMeterRender(AgGridTable model)
        {
            var parameters = GetAgGridDataFilterParameter(model);
            return await _dapper.ExecuteProcedure<ConsumerFullNewMeterRenderViewModel>("GetFullConsumerNewMeterRender", parameters);
        }
        public async Task<string> SaveConsumerNewMeterDetails(ConsumerNewMeterDetails model, int userId,string roleType)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@ConsumerId", model.ConsumerId);
            parameters.Add("@BookNoOrSlipNo", model.BookNoOrSlipNo);
            parameters.Add("@OldMeterReadingKwh", model.OldMeterReadingKwh);
            parameters.Add("@OldMeterReadingKvah", model.OldMeterReadingKvah);
            parameters.Add("@BalanceOfPrepaidMeter", model.BalanceOfPrepaidMeter);
            parameters.Add("@ExistingReturnMeter", model.ExistingReturnMeter);
            parameters.Add("@NewSmartMeterSrNo", model.NewSmartMeterSrNo);
            parameters.Add("@InitialReadingKwh", model.InitialReadingKwh);
            parameters.Add("@InitialReadingKvah", model.InitialReadingKvah);
            parameters.Add("@NewMeterPhoto", model.NewMeterPhotoUrl);
            parameters.Add("@MeterPhotoWithConsumer", model.MeterPhotoWithConsumerUrl);
            parameters.Add("@UserId", userId);
            parameters.Add("@OldMeterStatusAtInstallation", model.OldMeterStatusAtInstallation);
            parameters.Add("@CtRatio", model.CtRatio);
            parameters.Add("@OldMeterPhotoWithCUMkvah", model.OldMeterPhotoWithCUMkvah);
            parameters.Add("@InitialNetMeterImportReading", model.InitialNetMeterImportReading);
            parameters.Add("@InitialNetMeterExportReading", model.InitialNetMeterExportReading);
            parameters.Add("@Action", model.Action);
            parameters.Add("@ROLE_TYPE", roleType);

            return await _dapper.ExecuteProcedureFirstOrDefault<string>("SaveConsumerNewMeterDetails", parameters);
        }
        public async Task<string> UpdateConsumerNewMeterDetails(ConsumerNewMeterDetails model, int userId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@ConsumerId", model.ConsumerId);
            parameters.Add("@BookNoOrSlipNo", model.BookNoOrSlipNo);
            parameters.Add("@OldMeterReadingKwh", model.OldMeterReadingKwh);
            parameters.Add("@OldMeterReadingKvah", model.OldMeterReadingKvah);
            parameters.Add("@BalanceOfPrepaidMeter", model.BalanceOfPrepaidMeter);
            parameters.Add("@ExistingReturnMeter", model.ExistingReturnMeter);
            parameters.Add("@InitialReadingKwh", model.InitialReadingKwh);
            parameters.Add("@InitialReadingKvah", model.InitialReadingKvah);
            parameters.Add("@NewMeterPhoto", model.NewMeterPhotoUrl);
            parameters.Add("@MeterPhotoWithConsumer", model.MeterPhotoWithConsumerUrl);
            parameters.Add("@UserId", userId);
            parameters.Add("@OldMeterStatusAtInstallation", model.OldMeterStatusAtInstallation);
            parameters.Add("@CtRatio", model.CtRatio);
            parameters.Add("@OldMeterPhotoWithCUMkvah", model.OldMeterPhotoWithCUMkvah);
            parameters.Add("@InitialNetMeterImportReading", model.InitialNetMeterImportReading);
            parameters.Add("@InitialNetMeterExportReading", model.InitialNetMeterExportReading);

            return await _dapper.ExecuteProcedureFirstOrDefault<string>("UpdateConsumerNewMeterDetails", parameters);
        }

        public async Task<IEnumerable<ConsumerNewMeterRenderViewModel>> GetConsumerNewMeterByConsumerId(string consumerId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ConsumerId", consumerId);

            return await _dapper.ExecuteProcedure<ConsumerNewMeterRenderViewModel>("GetConsumerNewMeterByConsumerId",parameters);
        }

        public async Task<IEnumerable<ConsumerNewMeterRenderViewModel>> GetAllNewConsumerMeterRender(AgGridTable model)
        {
            var parameters = GetAgGridDataFilterParameter(model);
            return await _dapper.ExecuteProcedure<ConsumerNewMeterRenderViewModel>("GetAllConsumerNewMeterRender", parameters);
        }
        public async Task<ConsumerNewMeterRenderViewModel> GetConsumerMeterDetails(string consumerId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@ConsumerId", consumerId);
            return await _dapper.ExecuteProcedureFirstOrDefault<ConsumerNewMeterRenderViewModel>("GetConsumerMeterDetails", parameters);
        }

        public async Task<IEnumerable<CustomerDataForMobile>> GetCustomerDetailsByCustomerNumber(int recivedata)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@TOP_COUNT", recivedata);

            return await _dapper.ExecuteProcedure<CustomerDataForMobile>("GET_CUSTOMER_DATA_MOBILE", parameters);
        }
        public async Task<IEnumerable<ConsumerNewMeterListViewModel>> GetConsumerNewMeterList(int userId, string roleType)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);
            parameters.Add("@ROLETYPE", roleType);

            return await _dapper.ExecuteProcedure<ConsumerNewMeterListViewModel>("GetConsumerNewMeterList", parameters);
        }
        public async Task<int> CountCustomerDetails()
        {
            var parameters = new DynamicParameters();
            string query = "SELECT COUNT(1) FROM CUSTOMERDATA WITH(NOLOCK)";
            return await _dapper.GetFirstOrDefaultAsync<int>(query, parameters);
        }


        #endregion Consumer End

        #region HTCMeter 

        public async Task SyncHTCNewMeter(int userid)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@USERID", userid);
            await _dapper.ExecuteProcedure("SYNCHTCNEWMETER", parameters);
        }

        public async Task<IEnumerable<HTCFullNewMeterRenderViewModel>> GetFullNewHTCMeterRender(AgGridTable model)
        {
            var parameters = GetAgGridDataFilterParameter(model);
            return await _dapper.ExecuteProcedure<HTCFullNewMeterRenderViewModel>("GetFullHTCNewMeterRender", parameters);
        }
        public async Task<string> SaveHTCNewMeterDetails(HTCNewMeterDetails model, int userId,string roleType)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@ConsumerId", model.ConsumerId);
            parameters.Add("@BookNoOrSlipNo", model.BookNoOrSlipNo);
            parameters.Add("@OldMeterReadingKwh", model.OldMeterReadingKwh);
            parameters.Add("@OldMeterReadingKvah", model.OldMeterReadingKvah);
            parameters.Add("@BalanceOfPrepaidMeter", model.BalanceOfPrepaidMeter);
            parameters.Add("@ExistingReturnMeter", model.ExistingReturnMeter);
            parameters.Add("@NewSmartMeterSrNo", model.NewSmartMeterSrNo);
            parameters.Add("@InitialReadingKwh", model.InitialReadingKwh);
            parameters.Add("@InitialReadingKvah", model.InitialReadingKvah);
            parameters.Add("@NewMeterPhoto", model.NewMeterPhotoUrl);
            parameters.Add("@MeterPhotoWithConsumer", model.MeterPhotoWithConsumerUrl);
            parameters.Add("@UserId", userId);
            parameters.Add("@OldMeterStatusAtInstallation", model.OldMeterStatusAtInstallation);
            parameters.Add("@CtRatio", model.CtRatio);
            parameters.Add("@MeterPhotoWithKVAH", model.MeterPhotoWithKVAHUrl);
            parameters.Add("@MeterPhotoWithKWH", model.MeterPhotoWithKWHUrl);
            parameters.Add("@Action", model.Action);
            parameters.Add("@ROLE_TYPE", roleType);
            parameters.Add("@LineCTRatio", model.LineCTRatio);
            parameters.Add("@LinePTRatio", model.LinePTRatio);
            return await _dapper.ExecuteProcedureFirstOrDefault<string>("SaveHTCNewMeterDetails", parameters);
        }
        public async Task<string> UpdateHTCNewMeterDetails(HTCNewMeterDetails model, int userId)
        {
            var parameters = new DynamicParameters();

            // Required
            parameters.Add("@ConsumerId", model.ConsumerId);
            parameters.Add("@BookNoOrSlipNo", model.BookNoOrSlipNo);
            parameters.Add("@OldMeterReadingKwh", string.IsNullOrEmpty(model.OldMeterReadingKwh) ? null : model.OldMeterReadingKwh);
            parameters.Add("@OldMeterReadingKvah", string.IsNullOrEmpty(model.OldMeterReadingKvah) ? null : model.OldMeterReadingKvah);
            parameters.Add("@BalanceOfPrepaidMeter", string.IsNullOrEmpty(model.BalanceOfPrepaidMeter) ? null : model.BalanceOfPrepaidMeter);
            parameters.Add("@ExistingReturnMeter", string.IsNullOrEmpty(model.ExistingReturnMeter) ? null : model.ExistingReturnMeter);
            parameters.Add("@InitialReadingKwh", string.IsNullOrEmpty(model.InitialReadingKwh) ? null : model.InitialReadingKwh);
            parameters.Add("@InitialReadingKvah", string.IsNullOrEmpty(model.InitialReadingKvah) ? null : model.InitialReadingKvah);
            parameters.Add("@NewMeterPhoto", string.IsNullOrEmpty(model.NewMeterPhotoUrl) ? null : model.NewMeterPhotoUrl);
            parameters.Add("@MeterPhotoWithConsumer", string.IsNullOrEmpty(model.MeterPhotoWithConsumerUrl) ? null : model.MeterPhotoWithConsumerUrl);
            parameters.Add("@MeterPhotoWithKVAH", string.IsNullOrEmpty(model.MeterPhotoWithKVAHUrl) ? null : model.MeterPhotoWithKVAHUrl);
            parameters.Add("@MeterPhotoWithKWH", string.IsNullOrEmpty(model.MeterPhotoWithKWHUrl) ? null : model.MeterPhotoWithKWHUrl);
            parameters.Add("@OldMeterStatusAtInstallation", string.IsNullOrEmpty(model.OldMeterStatusAtInstallation) ? null : model.OldMeterStatusAtInstallation);
            parameters.Add("@CtRatio", string.IsNullOrEmpty(model.CtRatio) ? null : model.CtRatio);
            parameters.Add("@UserId", userId);

            return await _dapper.ExecuteProcedureFirstOrDefault<string>("UpdateHTCNewMeterDetails", parameters);
        }

     
        public async Task<IEnumerable<HTCNewMeterRenderViewModel>> GetHTCNewMeterByConsumerId(string consumerId)
        {
            if (string.IsNullOrEmpty(consumerId))
                return Enumerable.Empty<HTCNewMeterRenderViewModel>();

            var parameters = new DynamicParameters();
            parameters.Add("@ConsumerId", consumerId);

            return await _dapper.ExecuteProcedure<HTCNewMeterRenderViewModel>("GetHTCNewMeterByConsumerId", parameters);
        }

        public async Task<IEnumerable<HTCNewMeterRenderViewModel>> GetAllNewHTCMeterRender(AgGridTable model)
        {
            var parameters = GetAgGridDataFilterParameter(model);
            return await _dapper.ExecuteProcedure<HTCNewMeterRenderViewModel>("GetAllHTCNewMeterRender", parameters);
        }
        public async Task<HTCNewMeterRenderViewModel> GetHTCMeterDetails(string consumerId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@ConsumerId", consumerId);
            return await _dapper.ExecuteProcedureFirstOrDefault<HTCNewMeterRenderViewModel>("GetHTCMeterDetails", parameters);
        }
        #endregion HTCMeter End

        #region DTCMeter 

        public async Task SyncDTCNewMeter(int userid)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@USERID", userid);
            await _dapper.ExecuteProcedure("SYNCDTCNEWMETER", parameters);
        }

        public async Task<IEnumerable<DTCFullNewMeterRenderViewModel>> GetFullNewDTCMeterRender(AgGridTable model)
        {
            var parameters = GetAgGridDataFilterParameter(model);
            return await _dapper.ExecuteProcedure<DTCFullNewMeterRenderViewModel>("GetFullDTCNewMeterRender", parameters);
        }
        public async Task<string> SaveDTCNewMeterDetails(DTCNewMeterDetails model, int userId, string roleType)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@ConsumerId", model.ConsumerId);
            parameters.Add("@BookNoOrSlipNo", model.BookNoOrSlipNo);
            parameters.Add("@OldMeterReadingKwh", model.OldMeterReadingKwh);
            parameters.Add("@OldMeterReadingKvah", model.OldMeterReadingKvah);
            parameters.Add("@BalanceOfPrepaidMeter", model.BalanceOfPrepaidMeter);
            parameters.Add("@ExistingReturnMeter", model.ExistingReturnMeter);
            parameters.Add("@NewSmartMeterSrNo", model.NewSmartMeterSrNo);
            parameters.Add("@InitialReadingKwh", model.InitialReadingKwh);
            parameters.Add("@InitialReadingKvah", model.InitialReadingKvah);
            parameters.Add("@NewMeterPhoto", model.NewMeterPhotoUrl);
            parameters.Add("@MeterPhotoWithConsumer", model.MeterPhotoWithConsumerUrl);
            parameters.Add("@UserId", userId);
            parameters.Add("@OldMeterStatusAtInstallation", model.OldMeterStatusAtInstallation);
            parameters.Add("@CtRatio", model.CtRatio);
            parameters.Add("@OldMeterReadingCumKVAH", model.OldMeterReadingCumKVAH);
            parameters.Add("@OldMeterReadingCumKWH", model.OldMeterReadingCumKWH);
            parameters.Add("@Action", model.Action);
            parameters.Add("@ROLE_TYPE", roleType);
            parameters.Add("@LineCTRatio", model.LineCTRatio);
            parameters.Add("@LinePTRatio", model.LinePTRatio);
            return await _dapper.ExecuteProcedureFirstOrDefault<string>("SaveDTCNewMeterDetails", parameters);
        }

        public async Task<string> UpdateDTCNewMeterDetails(DTCNewMeterDetails model, int userId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@ConsumerId", model.ConsumerId);
            parameters.Add("@BookNoOrSlipNo", model.BookNoOrSlipNo);
            parameters.Add("@OldMeterReadingKwh", model.OldMeterReadingKwh);
            parameters.Add("@OldMeterReadingKvah", model.OldMeterReadingKvah);
            parameters.Add("@BalanceOfPrepaidMeter", model.BalanceOfPrepaidMeter);
            parameters.Add("@ExistingReturnMeter", model.ExistingReturnMeter);
            parameters.Add("@NewSmartMeterSrNo", model.NewSmartMeterSrNo);
            parameters.Add("@InitialReadingKwh", model.InitialReadingKwh);
            parameters.Add("@InitialReadingKvah", model.InitialReadingKvah);
            parameters.Add("@OldMeterStatusAtInstallation", model.OldMeterStatusAtInstallation);
            parameters.Add("@CtRatio", model.CtRatio);
            parameters.Add("@OldMeterReadingCumKVAH", model.OldMeterReadingCumKVAH);
            parameters.Add("@OldMeterReadingCumKWH", model.OldMeterReadingCumKWH);
            parameters.Add("@LineCTRatio", model.LineCTRatio);
            parameters.Add("@LinePTRatio", model.LinePTRatio);
            
            return await _dapper.ExecuteProcedureFirstOrDefault<string>("UpdateDTCNewMeterDetails", parameters);
        }

        public async Task<IEnumerable<DTCNewMeterRenderViewModel>> GetDTCNewMeterByConsumerId(string consumerId)
        {
            if (string.IsNullOrEmpty(consumerId))
                return Enumerable.Empty<DTCNewMeterRenderViewModel>();

            var parameters = new DynamicParameters();
            parameters.Add("@ConsumerId", consumerId);

            return await _dapper.ExecuteProcedure<DTCNewMeterRenderViewModel>("GetDTCNewMeterByConsumerId", parameters);
        }

        public async Task<IEnumerable<DTCNewMeterRenderViewModel>> GetAllNewDTCMeterRender(AgGridTable model)
        {
            var parameters = GetAgGridDataFilterParameter(model);
            return await _dapper.ExecuteProcedure<DTCNewMeterRenderViewModel>("GetAllDTCNewMeterRender", parameters);
        }
        public async Task<DTCNewMeterRenderViewModel> GetDTCMeterDetails(string consumerId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@ConsumerId", consumerId);
            return await _dapper.ExecuteProcedureFirstOrDefault<DTCNewMeterRenderViewModel>("GetDTCMeterDetails", parameters);
        }
        #endregion DTCMeter End

        #region DT 
        public async Task SyncDTNewMeter(int userid)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@USERID", userid);
            await _dapper.ExecuteProcedure("SYNCDTNEWMETER", parameters);
        }
        public async Task<IEnumerable<DTFullNewMeterRenderViewModel>> GetFullNewDTMeterRender(AgGridTable model)
        {
            var parameters = GetAgGridDataFilterParameter(model);

            return await _dapper.ExecuteProcedure<DTFullNewMeterRenderViewModel>("GetFullDTNewMeterRender", parameters);
        }

        public async Task<DTNewMeterRenderViewModel> GetDTNewMeterByConsumerId(string consumerId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ConsumerId", consumerId);
            return await _dapper.ExecuteProcedureFirstOrDefault<DTNewMeterRenderViewModel>("GetDTNewMeterByConsumerId", parameters);
        }

        public async Task<IEnumerable<DTNewMeterRenderViewModel>> GetAllNewDTMeterRender(AgGridTable model)
        {
            var parameters = GetAgGridDataFilterParameter(model);
            return await _dapper.ExecuteProcedure<DTNewMeterRenderViewModel>("GetAllDTNewMeterRender", parameters);
        }
        public async Task<DTNewMeterRenderViewModel> GetDTMeterDetails(string consumerId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ConsumerId", consumerId);
            return await _dapper.ExecuteProcedureFirstOrDefault<DTNewMeterRenderViewModel>("GetDtMeterDetails", parameters);
        }
        public async Task<string> SaveDTNewMeterDetails(DTNewMeterDetails DTNewMeterDetails, int userId, string roleType)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@ConsumerId", DTNewMeterDetails.ConsumerId);
            parameters.Add("@BookNo", DTNewMeterDetails.BookNo);
            parameters.Add("@OldReadingKWH", DTNewMeterDetails.OldReadingKWH);
            parameters.Add("@OldReadingKVAH", DTNewMeterDetails.OldReadingKVAH);
            parameters.Add("@OldReadingKVA", DTNewMeterDetails.OldReadingKVA);
            parameters.Add("@OldMeterStatus", DTNewMeterDetails.OldMeterStatus);
            parameters.Add("@LTCTRating", DTNewMeterDetails.LTCTRating);
            parameters.Add("@NoOfLTCTInstalled", DTNewMeterDetails.NoOfLTCTInstalled);
            parameters.Add("@LugsSize", DTNewMeterDetails.LugsSize);
            parameters.Add("@NoOfLugsInstalled", DTNewMeterDetails.NoOfLugsInstalled);
            parameters.Add("@CableSize", DTNewMeterDetails.CableSize);
            parameters.Add("@LengthOfCable", DTNewMeterDetails.LengthOfCable);
            parameters.Add("@NewSmartMeterSrNo", DTNewMeterDetails.NewSmartMeterSrNo);
            parameters.Add("@SMCBoxSealNo", DTNewMeterDetails.SMCBoxSealNo);
            parameters.Add("@InitialReadingKWH", DTNewMeterDetails.InitialReadingKWH);
            parameters.Add("@InitialReadingKVAH", DTNewMeterDetails.InitialReadingKVAH);
            parameters.Add("@InitialReadingKVA", DTNewMeterDetails.InitialReadingKVA);
            parameters.Add("@LTCTMake", DTNewMeterDetails.LTCTMake);
            parameters.Add("@LTCTType", DTNewMeterDetails.LTCTType);
            parameters.Add("@NewMMF", DTNewMeterDetails.NewMMF);
            parameters.Add("@LengthOfCopperCable", DTNewMeterDetails.LengthOfCopperCable);
            parameters.Add("@NewMeterPhotourl", DTNewMeterDetails.NewMeterPhotourl);
            parameters.Add("@SMCBoxPhotourl", DTNewMeterDetails.SMCBoxPhotourl);
            parameters.Add("@UserId", userId);
            parameters.Add("@Action", DTNewMeterDetails.Action);
            parameters.Add("@ROLE_TYPE", roleType);
            parameters.Add("@LineCTRatio", DTNewMeterDetails.LineCTRatio);
            parameters.Add("@LinePTRatio", DTNewMeterDetails.LinePTRatio);
            return await _dapper.ExecuteProcedureFirstOrDefault<string>("SaveDTNewMeterDetails", parameters);
        }
        public async Task<string> UpdateDTNewMeterDetails(DTNewMeterDetails DTNewMeterDetails, int userId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@ConsumerId", DTNewMeterDetails.ConsumerId);
            parameters.Add("@BookNo", DTNewMeterDetails.BookNo);
            parameters.Add("@OldReadingKWH", DTNewMeterDetails.OldReadingKWH);
            parameters.Add("@OldReadingKVAH", DTNewMeterDetails.OldReadingKVAH);
            parameters.Add("@OldReadingKVA", DTNewMeterDetails.OldReadingKVA);
            parameters.Add("@OldMeterStatus", DTNewMeterDetails.OldMeterStatus);
            parameters.Add("@LTCTRating", DTNewMeterDetails.LTCTRating);
            parameters.Add("@NoOfLTCTInstalled", DTNewMeterDetails.NoOfLTCTInstalled);
            parameters.Add("@LugsSize", DTNewMeterDetails.LugsSize);
            parameters.Add("@NoOfLugsInstalled", DTNewMeterDetails.NoOfLugsInstalled);
            parameters.Add("@CableSize", DTNewMeterDetails.CableSize);
            parameters.Add("@LengthOfCable", DTNewMeterDetails.LengthOfCable);
            parameters.Add("@NewSmartMeterSrNo", DTNewMeterDetails.NewSmartMeterSrNo);
            parameters.Add("@SMCBoxSealNo", DTNewMeterDetails.SMCBoxSealNo);
            parameters.Add("@InitialReadingKWH", DTNewMeterDetails.InitialReadingKWH);
            parameters.Add("@InitialReadingKVAH", DTNewMeterDetails.InitialReadingKVAH);
            parameters.Add("@InitialReadingKVA", DTNewMeterDetails.InitialReadingKVA);
            parameters.Add("@LTCTMake", DTNewMeterDetails.LTCTMake);
            parameters.Add("@LTCTType", DTNewMeterDetails.LTCTType);
            parameters.Add("@NewMMF", DTNewMeterDetails.NewMMF);
            parameters.Add("@LengthOfCopperCable", DTNewMeterDetails.LengthOfCopperCable);
            parameters.Add("@NewMeterPhotourl", DTNewMeterDetails.NewMeterPhotourl);
            parameters.Add("@SMCBoxPhotourl", DTNewMeterDetails.SMCBoxPhotourl);
            parameters.Add("@UserId", userId);
            parameters.Add("@LineCTRatio", DTNewMeterDetails.LineCTRatio);
            parameters.Add("@LinePTRatio", DTNewMeterDetails.LinePTRatio);
            return await _dapper.ExecuteProcedureFirstOrDefault<string>("UpdateDTNewMeterDetails", parameters);
        }
        public async Task<IEnumerable<DTNewMeterListViewModel>> GetDTNewMeterList(int userId, string roleType)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);
            parameters.Add("@ROLETYPE", roleType);

            return await _dapper.ExecuteProcedure<DTNewMeterListViewModel>("GetDTNewMeterList", parameters);
        }

        #endregion DT End

        #region Feeder 
        public async Task SyncFeederNewMeter(int userid)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@USERID", userid);
            await _dapper.ExecuteProcedure("SYNCFEEDERNEWMETER", parameters);
        }
        public async Task<IEnumerable<FeederFullNewMeterRenderViewModel>> GetFullNewFeederMeterRender(AgGridTable model)
        {
            var parameters = GetAgGridDataFilterParameter(model);

            return await _dapper.ExecuteProcedure<FeederFullNewMeterRenderViewModel>("GetFullFeederNewMeterRender", parameters);
        }
        public async Task<FeederNewMeterRenderViewModel> GetFeederNewMeterByConsumerId(string consumerId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ConsumerId", consumerId);

            return await _dapper.ExecuteProcedureFirstOrDefault<FeederNewMeterRenderViewModel>("GetFeederNewMeterByConsumerId", parameters);
        }
        public async Task<IEnumerable<FeederNewMeterRenderViewModel>> GetAllNewFeederMeterRender(AgGridTable model)
        {
            var parameters = GetAgGridDataFilterParameter(model);
            return await _dapper.ExecuteProcedure<FeederNewMeterRenderViewModel>("GetAllFeederNewMeterRender", parameters);
        }
        public async Task<FeederNewMeterRenderViewModel> GetFeederMeterDetails(string consumerId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ConsumerId", consumerId);
            return await _dapper.ExecuteProcedureFirstOrDefault<FeederNewMeterRenderViewModel>("GetFeederMeterDetails", parameters);
        }
        public async Task<string> SaveFeederNewMeterDetails(FeederNewMeterDetails FeederNewMeterDetails, int userId, string roleType)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@ConsumerId", FeederNewMeterDetails.ConsumerId);
            parameters.Add("@BookNo", FeederNewMeterDetails.BookNo);
            parameters.Add("@OldReadingKWH", FeederNewMeterDetails.OldReadingKWH);
            parameters.Add("@OldReadingKVAH", FeederNewMeterDetails.OldReadingKVAH);
            parameters.Add("@OldReadingKVA", FeederNewMeterDetails.OldReadingKVA);
            parameters.Add("@NewSmartMeterSrNo", FeederNewMeterDetails.NewSmartMeterSrNo);
            parameters.Add("@InitialReadingKWH", FeederNewMeterDetails.InitialReadingKWH);
            parameters.Add("@InitialReadingKVAH", FeederNewMeterDetails.InitialReadingKVAH);
            parameters.Add("@InitialReadingKVA", FeederNewMeterDetails.InitialReadingKVA);
            parameters.Add("@NewMeterPhoto", FeederNewMeterDetails.NewMeterPhotoUrl);
            parameters.Add("@FeederControlPanelPhoto", FeederNewMeterDetails.FeederControlPanelPhotoUrl);
            parameters.Add("@UserId", userId);
            parameters.Add("@Action", FeederNewMeterDetails.Action);
            parameters.Add("@ROLE_TYPE", roleType);
            parameters.Add("@LineCTRatio", FeederNewMeterDetails.LineCTRatio);
            parameters.Add("@LinePTRatio", FeederNewMeterDetails.LinePTRatio);
            return await _dapper.ExecuteProcedureFirstOrDefault<string>("SaveFeederNewMeterDetails", parameters);
        }
        public async Task<string> UpdateFeederNewMeterDetails(FeederNewMeterDetails FeederNewMeterDetails, int userId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@ConsumerId", FeederNewMeterDetails.ConsumerId);
            parameters.Add("@BookNo", FeederNewMeterDetails.BookNo);
            parameters.Add("@OldReadingKWH", FeederNewMeterDetails.OldReadingKWH);
            parameters.Add("@OldReadingKVAH", FeederNewMeterDetails.OldReadingKVAH);
            parameters.Add("@OldReadingKVA", FeederNewMeterDetails.OldReadingKVA);
            parameters.Add("@InitialReadingKWH", FeederNewMeterDetails.InitialReadingKWH);
            parameters.Add("@InitialReadingKVAH", FeederNewMeterDetails.InitialReadingKVAH);
            parameters.Add("@InitialReadingKVA", FeederNewMeterDetails.InitialReadingKVA);
            parameters.Add("@UserId", userId);
            parameters.Add("@LineCTRatio", FeederNewMeterDetails.LineCTRatio);
            parameters.Add("@LinePTRatio", FeederNewMeterDetails.LinePTRatio);

            return await _dapper.ExecuteProcedureFirstOrDefault<string>("UpdateFeederNewMeterDetails", parameters);
        }

        public async Task<string> BulkUpdateLineRatiosAsync(string meterNo, string lineCTRatio, string linePTRatio, int userId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@MeterNo", meterNo);
            parameters.Add("@LineCTRatio", lineCTRatio);
            parameters.Add("@LinePTRatio", linePTRatio);
            parameters.Add("@UserId", userId);
            var result = await _dapper.ExecuteProcedureFirstOrDefault<string>("BULK_UPDATE_LINE_RATIOS", parameters);
            return string.IsNullOrEmpty(result) ? "NOT_FOUND" : result;
        }

        public async Task<FeederProductionMeterDataViewModel> GetFeederProductionMeterData(string meterNo)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@meterNo", meterNo);
            parameters.Add("@RoleType", "Production");


            return await _dapper.ExecuteProcedureFirstOrDefault<FeederProductionMeterDataViewModel>("GetFeederProcuctionNewMeter", parameters);
        }
        public async Task<IEnumerable<FeederNewMeterListViewModel>> GetFeederNewMeterList(int userId, string roleType)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);
            parameters.Add("@ROLETYPE", roleType);

            return await _dapper.ExecuteProcedure<FeederNewMeterListViewModel>("GetFeederNewMeterList", parameters);
        }

        #endregion Feeder End

        public async Task<MeterCountViewModel> GetMeterCountByID(int id, string roleType)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@USERID", id);
            parameters.Add("@ROLETYPE", roleType);

            return await _dapper.ExecuteProcedureFirstOrDefault<MeterCountViewModel>("MeterCount", parameters);
        }
        public async Task<NewMeterDashboardCount> GetNewMeterDashboardCount(string roleType, int userId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@USERID", userId);
            parameters.Add("@ROLETYPE", roleType);

            return await _dapper.ExecuteProcedureFirstOrDefault<NewMeterDashboardCount>("GET_NEWMETER_COUNT_WEB", parameters);
        }

        public async Task UpdateHesStatus(string areaType, string areaValue, string status)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@AREA_TYPE", areaType);
            parameters.Add("@AREA_VALUE", areaValue);
            parameters.Add("@STATUS", status);

            await _dapper.ExecuteProcedure("UPDATE_HES_SEND", parameters);
        }

        public async Task UpdateHesStatus(string areaType, string areaValue, string status, string pushedBy)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@AREA_TYPE", areaType);
            parameters.Add("@AREA_VALUE", areaValue);
            parameters.Add("@STATUS", status);
            parameters.Add("@HesPushedBy", pushedBy);  

            await _dapper.ExecuteProcedure("UPDATE_HES_SEND", parameters);
        }
        public async Task<IEnumerable<HesSendDTO>> GetPendingHesMeter(string areaType)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@AREA_TYPE", areaType);

            return await _dapper.ExecuteProcedure<HesSendDTO>("GET_PENDING_HES_METER_SYNC", parameters);
        }

        /// <summary>
        /// Audit log ke liye: pushed ids (consumer ids) ke new meter numbers + new consumer ids
        /// nikaalta hai (Consumer/DT/DTC/Feeder/HTC - areaType ke hisaab se). Fail-safe: empty list.
        /// </summary>
        public async Task<List<MeterPushInfo>> GetMeterNosByIds(string areaType, string commaIds)
        {
            var result = new List<MeterPushInfo>();
            if (string.IsNullOrWhiteSpace(commaIds)) return result;

            try
            {
                string table = areaType?.Trim().ToUpperInvariant() switch
                {
                    "CONSUMER" => "ConsumerNewMeterDetails",
                    "DT" => "DTNewMeterDetails",
                    "DTC" => "DTCNewMeterDetails",
                    "FEEDER" => "FeederNewMeterDetails",
                    "HT" => "HTCNewMeterDetails",
                    "HTC" => "HTCNewMeterDetails",
                    _ => null
                };
                if (table == null) return result;

                // FeederNewMeterDetails me NewConsumerId column nahi hai - ConsumerId fallback
                string newConsumerIdExpr = table == "FeederNewMeterDetails"
                    ? "ConsumerId"
                    : "ISNULL(NULLIF(NewConsumerId,''), ConsumerId)";

                // table name whitelist se aata hai (injection safe), ids dapper param me jaate hain
                var parameters = new DynamicParameters();
                parameters.Add("@IDS", commaIds);
                var rows = await _dapper.GetAllAsync<MeterPushInfo>(
                    $"SELECT NewSmartMeterSrNo AS MeterNo, {newConsumerIdExpr} AS NewConsumerId " +
                    $"FROM {table} WITH (NOLOCK) WHERE ConsumerId IN (SELECT value FROM STRING_SPLIT(@IDS, ','))",
                    parameters);

                result = rows
                    .Where(x => !string.IsNullOrWhiteSpace(x?.MeterNo) || !string.IsNullOrWhiteSpace(x?.NewConsumerId))
                    .GroupBy(x => x.MeterNo)
                    .Select(g => g.First())
                    .ToList();
            }
            catch
            {
                // audit enrichment fail-safe - push flow pe koi asar nahi
            }
            return result;
        }
        public async Task<ProductionMeter> GetNewMeterDetials(string meterNo)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@METERNO", meterNo);

            return await _dapper.ExecuteProcedureFirstOrDefault<ProductionMeter>("GET_NEW_METER_DETAILS", parameters);
        }
        public async Task<IEnumerable<MeterCountFunc>> GetMeterCountWithName(string roleName, int Id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@USERID", Id);
            parameters.Add("@ROLE", roleName);
            return await _dapper.ExecuteProcedure<MeterCountFunc>("METERCOUNTFUNC", parameters);
        }

        // ── MeterSwap: full snapshot save (SAVE_SWAP_METER_DETAIL) ──
        public async Task<object> SaveSwapMeterDetail(string consumerId, string oldMeterNo, string newMeterNo, int userId, string swapRemark = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ConsumerId", consumerId?.Trim());
            parameters.Add("@PreviousMeterNo", oldMeterNo?.Trim());
            parameters.Add("@SwapMeterNo", newMeterNo?.Trim());
            parameters.Add("@UserId", userId);
            parameters.Add("@SwapRemark", string.IsNullOrWhiteSpace(swapRemark) ? null : swapRemark.Trim());

            var result = await _dapper.ExecuteProcedureFirstOrDefault<SwapMeterSaveResult>("SAVE_SWAP_METER_DETAIL", parameters);
            return new ViewModel.MeterViewModel.SwapMeterSaveResultDto { SwapId = result?.SwapId };
        }

        private class SwapMeterSaveResult
        {
            public string SwapId { get; set; }
        }

        // ── MeterSwap: history list (GET_SWAP_METER_LIST) ──
        public async Task<IEnumerable<SwapMeterDetailViewModel>> GetSwapMeterList(string search, int pageNumber, int pageSize)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Search", string.IsNullOrWhiteSpace(search) ? null : search.Trim());
            parameters.Add("@PageNumber", pageNumber < 0 ? 0 : pageNumber);
            parameters.Add("@PageSize", pageSize <= 0 ? 50 : pageSize);

            return await _dapper.ExecuteProcedure<SwapMeterDetailViewModel>("GET_SWAP_METER_LIST", parameters);
        }

        public async Task<string> SwapMeter(string consumerId, string oldMeterNo, string newMeterNo, int userId)
        {
            if (string.IsNullOrWhiteSpace(consumerId) || string.IsNullOrWhiteSpace(oldMeterNo) || string.IsNullOrWhiteSpace(newMeterNo))
                return "Invalid input.";

            oldMeterNo = oldMeterNo.Trim();
            newMeterNo = newMeterNo.Trim();
            consumerId = consumerId.Trim();

            try
            {
                var pProd = new DynamicParameters();
                pProd.Add("@MeterNo", newMeterNo);
                var prodCount = await _dapper.GetFirstOrDefaultAsync<int>("SELECT COUNT(1) FROM ProductionMeter WITH(NOLOCK) WHERE MeterNo = @MeterNo", pProd);
                if (prodCount == 0)
                {
                    return "New meter not available in production.";
                }
            }
            catch (Exception ex)
            {
                return $"Error checking production meter availability: {ex.Message}";
            }

            string existingConsumerForOld = null;
            string existingConsumerForNew = null;

            try
            {
                string q = "SELECT NewConsumerId FROM ConsumerNewMeterDetails WITH(NOLOCK) WHERE NewSmartMeterSrNo = @MeterNo";
                var p = new DynamicParameters();
                p.Add("@MeterNo", oldMeterNo);
                existingConsumerForOld = await _dapper.GetFirstOrDefaultAsync<string>(q, p);
            }
            catch
            {
            }

            if (string.IsNullOrEmpty(existingConsumerForOld))
                return "Old meter number not found.";

            if (!existingConsumerForOld.Equals(consumerId, StringComparison.OrdinalIgnoreCase))
            {
                return $"Old meter number is assigned to consumer {existingConsumerForOld}.";
            }

            try
            {
                string qn = "SELECT NewConsumerId FROM ConsumerNewMeterDetails WITH(NOLOCK) WHERE NewSmartMeterSrNo = @MeterNo";
                var pn = new DynamicParameters();
                pn.Add("@MeterNo", newMeterNo);
                existingConsumerForNew = await _dapper.GetFirstOrDefaultAsync<string>(qn, pn);
            }
            catch
            {
            }

            if (!string.IsNullOrEmpty(existingConsumerForNew))
            {
                return $"New meter is already assigned to consumer {existingConsumerForNew}.";
            }

            var parameters = new DynamicParameters();
            parameters.Add("@NewConsumerId", consumerId);
            parameters.Add("@OldMeterNo", oldMeterNo);
            parameters.Add("@NewMeterNo", newMeterNo);
            parameters.Add("@UserId", userId);

            try
            {
                var result = await _dapper.ExecuteProcedureFirstOrDefault<string>("SWAP_METER", parameters);
                return string.IsNullOrEmpty(result) ? "Meter swapped successfully." : result;
            }
            catch (Exception ex)
            {
                return ex.Message;
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
