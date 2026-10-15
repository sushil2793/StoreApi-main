namespace SmartMeterAdmin.ViewModel.CustomerData
{
    public class CustomerDataForMobile
    {
        public string SubDivision { get; set; }
        public string SubStation { get; set; }
        public string Feeder { get; set; }
        public string DT { get; set; }
        public string CustomerName { get; set; }
        public string CustomerNo { get; set; }
        public bool IsReplaceMeter { get; set; }
        public string Address { get; set; }
    }
    public class CustomerDataShortKeyForMobile
    {
        public string SD { get; set; }
        public string SS { get; set; }
        public string FR { get; set; }
        public string DT { get; set; }
        public string CN { get; set; }
        public string CNO { get; set; }
        public bool RM { get; set; }
        public string AD { get; set; }
    }
    public class GetNewAndOldConsumerNo
    {
        public string NewConsumerNo { get; set; }
        public string OldConsumerNo { get; set; }
    }

}
