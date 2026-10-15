using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Http;

namespace SmartMeterAdmin.ViewModel.MeterViewModel
{
    public class DTCNewMeterDetails : ConsumerNewMeterDetails
    {
        public string OldMeterReadingCumKVAH { get; set; }
        public string OldMeterReadingCumKWH { get; set; }
        public string LineCTRatio { get; set; }
        public string LinePTRatio { get; set; }

    }
}
