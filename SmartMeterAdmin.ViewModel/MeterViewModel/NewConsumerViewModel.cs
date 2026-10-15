namespace SmartMeterAdmin.ViewModel.MeterViewModel
{
    public class NewConsumerViewModel
    {
        public int ConsumerID { get; set; }
        public string ConsumerName { get; set; }
        public string MobileNo { get; set; }
        public bool IsMeterBefore { get; set; }
        public string TentativeLoad { get; set; }
        public string Address { get; set; }
        public string ImageUrl { get; set; }
        public string District { get; set; }
        public string CreatedBy { get; set; }
        public string Created { get; set; }
    }
}
