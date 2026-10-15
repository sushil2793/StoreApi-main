namespace SmartMeterAdmin.ViewModel.Store
{
    public class MeterRequest
    {
        public string Status { get; set; }
        public int StoreId { get; set; }
        public string BoxId { get; set; }
        public string RoleType { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Length { get; set; }
    }
}
