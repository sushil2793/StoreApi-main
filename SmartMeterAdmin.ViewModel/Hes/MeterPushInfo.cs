namespace SmartMeterAdmin.ViewModel.Hes
{
    /// <summary>
    /// Audit log enrichment ke liye - pushed id ka new meter no + new consumer id.
    /// </summary>
    public class MeterPushInfo
    {
        public string MeterNo { get; set; }
        public string NewConsumerId { get; set; }
    }
}
