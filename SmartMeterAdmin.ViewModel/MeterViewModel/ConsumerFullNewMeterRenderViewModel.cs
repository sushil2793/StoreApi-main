using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SmartMeterAdmin.ViewModel.MeterViewModel
{
    public class ConsumerFullNewMeterRenderViewModel
    {
        public string ConsumerId { get; set; }
        public string NewConsumerId { get; set; }
        public string OldMeterNo { get; set; }
        public string OldMeterType { get; set; }
        public string OldMeterMode { get; set; }
        public string NewSmartMeterSrNo { get; set; }
        public string MeterType { get; set; }
        public string MeterMode { get; set; }
        public string BookNoOrSlipNo { get; set; }
        public string DateOfInstallation { get; set; }
        public string OldMeterReadingKwh { get; set; }
        public string OldMeterReadingKvah { get; set; }
        public string BalanceOfPrepaidMeter { get; set; }
        public string ExistingReturnMeter { get; set; }
        public string InitialReadingKwh { get; set; }
        public string InitialReadingKvah { get; set; }
        public string NewMeterPhoto { get; set; }
        public string MeterPhotoWithConsumer { get; set; }
        public string InstallerName { get; set; }
        public string InstallerMobileNo { get; set; }
        public string MeterMaker { get; set; }
        public string MeterBoxSealNo { get; set; }
        public string ICCID { get; set; }
        public string SIMNW { get; set; }
        public string DateOfSurvey { get; set; }
        public string Zone { get; set; }
        public string Circle { get; set; }
        public string Division { get; set; }
        public string Subdivision { get; set; }
        public string Substation3311KVName { get; set; }
        public string Substation3311KVCode { get; set; }
        public string Feeder11KVName { get; set; }
        public string Feeder11KVCode { get; set; }
        public string Substation110440KVName { get; set; }
        public string Substation110440KVCode { get; set; }
        public string ConsumerName { get; set; }
        public string FatherHusbandProprietorName { get; set; }
        public string SanctionLoadKW { get; set; }
        public string Address { get; set; }
        public string DistrictName { get; set; }
        public string ConsumerMobileNo { get; set; }
        public string ConsumerIDType { get; set; }
        public string ConsumerIDCardNo { get; set; }
        public string TariffName { get; set; }
        public string MeterMake { get; set; }
        public string OldMeterSerialNo { get; set; }
        public string OldMeterStatus { get; set; }
        public string OldMeterTerminalSealNo { get; set; }
        public string OldMeterBoxSealNo { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string OldMeterStatusAtInstallation { get; set; }
        public string DTCode { get; set; }
        public string DTName { get; set; }
        public string OldMeterMake { get; set; }
        public string NetMeterImportReading { get; set; }
        public string NetMeterExportReading { get; set; }

        //new properties
        public string OldMeterPhotoWithCUMkvah { get; set; }
        public string InitialNetMeterImportReading { get; set; }
        public string InitialNetMeterExportReading { get; set; }
        public string Remark { get; set; }
        public string RemarkDescription { get; set; }
        public string HESSend { get; set; }
        public string MeterPhotoUrl { get; set; }
        public string MeterPhotoUrlKVAH { get; set; }
        public string SanctionLoad { get; set; }
        public string NewSmartmeterKWH { get; set; }
        public string NewSmartmeterKVAH { get; set; }
        public string ContractorName { get; set; }

    }
}
