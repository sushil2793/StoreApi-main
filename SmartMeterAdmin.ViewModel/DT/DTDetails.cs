using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SmartMeterAdmin.ViewModel.DT
{
    public class DTDetails
    {
        public string ConsumerId { get; set; }
        public string DateOfSurvey { get; set; }
        public string ZoneName { get; set; }
        public string CircleName { get; set; }
        public string DivisionName { get; set; }
        public string SubDivisionName { get; set; }
        public string FeedingSubStationName { get; set; }
        public string FeedingSubStationCode { get; set; }
        public string FeederName { get; set; }
        public string FeederCode { get; set; }
        public string DTLocationName { get; set; }
        public string DTRatingKVA { get; set; }
        public string NamePlateVisibility { get; set; }
        public string DTMountingAt { get; set; }
        public string DTAccessibility { get; set; }
        public string DTCode { get; set; }
        public string DTType { get; set; }
        public string VillageName { get; set; }
        public string VillageCensusCode { get; set; }
        public string StreetColonyLocality { get; set; }
        public string Block { get; set; }
        public string District { get; set; }
        public string MeterAvailable { get; set; }
        public string MeterSerialNo { get; set; }
        public string MeterManufacturer { get; set; }
        public string MeterRating { get; set; }
        public string MeterType { get; set; }
        public string MeterWorking { get; set; }
        public string MeterBoxAvailable { get; set; }
        public string TTBAvailable { get; set; }
        public string LTCTAvailable { get; set; }
        public string NoOfLTCTAvailable { get; set; }
        public string LTCTMake { get; set; }
        public string LTCTRating { get; set; }
        public string LTCTType { get; set; }
        public string LTCTMeterFittedInSMC { get; set; }
        public string LTPowerCableAvailable { get; set; }
        public string CableSize { get; set; }
        public string LTPowerCableType { get; set; }
        public string LTPowerCableRating { get; set; }
        public string LTPowerCableTerminatingAt { get; set; }
        public string ExistingLTCableSuitable { get; set; }
        public string AluminiumCableLugsRequired { get; set; }
        public string LugSize { get; set; }
        public string NoOfLugs { get; set; }
        public string Remark { get; set; }
        public string MeterSitePhoto { get; set; }
        public string CTSitePhoto { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string GPRSSignalStrengthAirtel { get; set; }
        public string GPRSSignalStrengthJio { get; set; }
        public string SurveyAgencyName { get; set; }
        public string SurveyorName { get; set; }
        public string AdditionalRemark { get; set; }
        public IFormFile MeterPhoto { get; set; }
        public string Action { get; set; } = string.Empty;

    }
}
