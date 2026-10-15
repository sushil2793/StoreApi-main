using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SmartMeterAdmin.ViewModel.HTCSurvey;
using SmartMeterAdmin.ViewModel.Survey;

namespace SmartMeterAdmin.ViewModel.MeterViewModel
{
    public class HTCFullNewMeterRenderViewModel: SurveyDetails
    {
        public string OldMeterType { get; set; }
        public string NewSmartMeterSrNo { get; set; }
        public new string MeterType { get; set; }
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
        public new string MeterBoxSealNo { get; set; }
        public string ICCID { get; set; }
        public string OldMeterStatusAtInstallation { get; set; }
        public string MeterPhotoWithKVAHUrl { get; set; }
        public string MeterPhotoWithKWHUrl { get; set; }
        public string Remark { get; set; }
        public string RemarkDescription { get; set; }
        public string HesSend { get; set; }
        public string LineCTRatio { get; set; }
        public string LinePTRatio { get; set; }
        public string PhotoWithCumKVAHUrl { get; set; }
        public string PhotoWithCumKWHUrl { get; set; }
        public string PhotoWithKVRHUrl { get; set; }
        public string ContractorName { get; set; }
    }
}
