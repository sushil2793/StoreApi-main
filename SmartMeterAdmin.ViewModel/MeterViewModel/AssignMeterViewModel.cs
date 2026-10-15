namespace SmartMeterAdmin.ViewModel.MeterViewModel
{
    public  class AssignMeterViewModel
    {
        public int AssignID { get; set; }
        public string ContractorName { get; set; }
        public string OnePMeterNo { get; set; }
        public int OnePMerterQty { get; set; }
        public string ThreePMeterNo { get; set; }
        public int ThreePMerterQty { get; set; }
        public string CTPMeterNo { get; set; }
        public int CTPMerterQty { get; set; }
        public string HTPMeterNo { get; set; }
        public int HTPMeterQty { get; set; }
        public string IssuedBy { get; set; }
        public string IssuedDate { get; set; }
        public bool IsReturn { get; set; }
    }
}
