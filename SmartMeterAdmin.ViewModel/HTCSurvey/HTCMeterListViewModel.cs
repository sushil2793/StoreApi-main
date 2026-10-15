using System;
using Microsoft.AspNetCore.Http;
using SmartMeterAdmin.ViewModel.Survey;

namespace SmartMeterAdmin.ViewModel.HTCSurvey
{
    public class HTCMeterListViewModel :SurveyMeterListViewModel
    {
        public string FeederCategory { get; set; }
        public string FeederLengthKM { get; set; }
        public string FeederVoltageLevelKV { get; set; }
        public string FeederConductor { get; set; }
        public string FeederMonitored { get; set; }
        public string MeterAvailableIfYes { get; set; }
        public string OldMeterManufacturer { get; set; }
        public string MeterRating { get; set; }
        public string MeterReadingCumKVAH { get; set; }
        public string MeterReadingCumKWH { get; set; }
        public string MeterBoxAvailable { get; set; }
        public string MeterMultiplicationFactor { get; set; }
        public string CapturingOfCBStatus { get; set; }
        public string LinePTArrangement { get; set; }
        public string CTPTAvailableIfYes { get; set; }
        public string CTMake { get; set; }
        public string CTRating { get; set; }
        public string CTType { get; set; }
        public string CTConnectedRatio { get; set; }
        public string PTMake { get; set; }
        public string PTRating { get; set; }
        public string PTType { get; set; }
        public string PTConnectedRatio { get; set; }
        public string NosOfDTsConnected { get; set; }
        public string PhotoWithCumKVAHUrl { get; set; }
        public string PhotoWithCumKWHUrl { get; set; }
        public string PhotoWithKVRHUrl { get; set; }
        public string TotalTransformationCapacityOfDTsKVA { get; set; }

    }
}
