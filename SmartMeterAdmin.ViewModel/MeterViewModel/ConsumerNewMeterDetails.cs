using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Http;

namespace SmartMeterAdmin.ViewModel.MeterViewModel
{
    public class ConsumerNewMeterDetails
    {
        public string ConsumerId { get; set; }
        public string BookNoOrSlipNo { get; set; }
        public string OldMeterReadingKwh { get; set; }
        public string OldMeterReadingKvah { get; set; }
        public string BalanceOfPrepaidMeter { get; set; }
        public string ExistingReturnMeter { get; set; } 
        public string NewSmartMeterSrNo { get; set; }
        public string InitialReadingKwh { get; set; }
        public string InitialReadingKvah { get; set; }
        public IFormFile NewMeterPhoto { get; set; }
        public string NewMeterPhotoUrl { get; set; }
        public IFormFile MeterPhotoWithConsumer { get; set; }
        public string MeterPhotoWithConsumerUrl { get; set; }
        public string OldMeterStatusAtInstallation { get; set; }
        public string CtRatio { get; set; }
        public IFormFile OldMeterPhotoWithCUMkvah { get; set; }
        public string OldMeterPhotoWithCUMkvahUrl { get; set; }
        public string InitialNetMeterImportReading { get; set; }
        public string InitialNetMeterExportReading { get; set; }
        public string Action { get; set; }=string.Empty;
    }
}
