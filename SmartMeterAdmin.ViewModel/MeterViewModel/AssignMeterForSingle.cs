namespace SmartMeterAdmin.ViewModel.MeterViewModel
{
    public class AssignMeterForSingle
    {
        public string CreatedDate { get; set; }
        public int MeterNo { get; set; }
        public string MeterType { get; set; }
        public string IssueBy { get; set; }
        public string IssueTo { get; set; }
        public bool InstalledStatus { get; set; }
       
    }
    public class IncomingMeterPrint
    {
        public string CreatedDate { get; set; }
        public string SinglePhase { get; set; }
        public string ThreePhase { get; set; }
        public string CtMeter { get; set; }
        public string HtMeter { get; set; }
        
    }
}
