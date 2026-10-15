
using Microsoft.AspNetCore.Http;
using SmartMeterAdmin.ViewModel.Survey;

namespace SmartMeterAdmin.ViewModel.DTCSurvey
{
    public class DTCDetails : SurveyDetails
    {

        public string NewConsumerId { get; set; }
        public string DtRatingKVA { get; set; }
        public string CtRatio { get; set; }
        public string OldMeterReadingCumKVAH { get; set; }
        public string OldMeterReadingCumKWH { get; set; }
        public string PhotoOfMeterOfCumKVAHUrl { get; set; }
        public string PhotoWithMeterCumReadingKWHUrl { get; set; }
        public IFormFile PhotoOfMeterOfCumKVAH { get; set; }
        public IFormFile PhotoWithMeterCumReadingKWH { get; set; }

    }
   
}