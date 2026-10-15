namespace SmartMeterAdmin.ViewModel.MeterViewModel
{
    public class DTCNewMeterRenderViewModel : ConsumerNewMeterRenderViewModel
    {
        public string DtRatingKVA { get; set; }
        public string CtRatio { get; set; }
        public string OldMeterReadingCumKVAH { get; set; }
        public string OldMeterReadingCumKWH { get; set; }
        public string PhotoOfMeterOfCumKVAHUrl { get; set; }
        public string PhotoWithMeterCumReadingKWHUrl { get; set; }
        public string LineCTRatio { get; set; }
        public string LinePTRatio { get; set; }
    }
}
