using Microsoft.AspNetCore.Http;

namespace SmartMeterAdmin.ViewModel.MeterViewModel
{
    public class HTCNewMeterDetails : ConsumerNewMeterDetails
    {
        public IFormFile MeterPhotoWithKVAH { get; set; }
        public string MeterPhotoWithKVAHUrl { get; set; }
        public IFormFile MeterPhotoWithKWH { get; set; }
        public string MeterPhotoWithKWHUrl { get; set; }
        public string LineCTRatio { get; set; }
        public string LinePTRatio { get; set; }

    }

}
