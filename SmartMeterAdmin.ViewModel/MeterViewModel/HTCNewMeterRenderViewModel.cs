using Microsoft.AspNetCore.Http;

namespace SmartMeterAdmin.ViewModel.MeterViewModel
{
    public class HTCNewMeterRenderViewModel: ConsumerNewMeterRenderViewModel
    {
        public new string MeterPhotoWithConsumer { get; set; }
        public string MeterPhotoWithKVAH { get; set; }
        public string MeterPhotoWithKWH { get; set; }
        public string LineCTRatio { get; set; }
        public string LinePTRatio { get; set; }


    }
}
