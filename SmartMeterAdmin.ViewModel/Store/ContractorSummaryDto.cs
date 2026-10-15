namespace SmartMeterAdmin.ViewModel.Store
{
    public class ContractorSummaryDto
    {
        public int UserId { get; set; }
        public int ParentUserId { get; set; }
        public string FullName { get; set; }
        public string ParentName { get; set; }
        public string RoleType { get; set; }
        public string ParentRole { get; set; }
        public int FeederNewMeter { get; set; }
        public int ConsumerNewMeter { get; set; }
        public int DTNewMeter { get; set; }
        public int ConsumerSurvey { get; set; }
        public int DTSurvey { get; set; }
        public int FeederSurvey { get; set; }
    }
}
