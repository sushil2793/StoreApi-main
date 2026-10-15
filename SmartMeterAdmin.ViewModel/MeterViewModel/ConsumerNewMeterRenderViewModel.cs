using Microsoft.AspNetCore.Http;

namespace SmartMeterAdmin.ViewModel.MeterViewModel
{
    public class ConsumerNewMeterRenderViewModel
    {
        public string ConsumerId { get; set; }
        public string NewConsumerId { get; set; }
        public string OldMeterNo { get; set; }
        public string OldMeterType { get; set; }
        public string NewSmartMeterSrNo { get; set; }
        public string MeterType { get; set; }
        public string Subdivision { get; set; }// already in procedure but not in viewmodel
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
        public string MeterStatus { get; set; }
        public string OldMeterStatusAtInstallation { get; set; }
        public string HesSend { get; set; }
        public string OldMeterPhotoWithCUMkvahUrl { get; set; }
        public string InitialNetMeterImportReading { get; set; }
        public string InitialNetMeterExportReading { get; set; }
        public string InitialNetMeterImportReadingKVAH { get; set; }
        public string InitialNetMeterExportReadingKVAH { get; set; }
        public string NewSmartmeterKWH { get; set; }
        public string NewSmartmeterKVAH { get; set; }
        public string ConnectionStatus { get; set; }
        public string District { get; set; }
        public string ContractorName { get; set; }

    }
}
