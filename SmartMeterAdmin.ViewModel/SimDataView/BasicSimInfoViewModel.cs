namespace SmartMeterAdmin.ViewModel.SimDataView
{
    public  class BasicSimInfoViewModel
    {
        public BasicSimInfoViewModel()
        {
            MSISDN = "";
            IPV6 = "";
            GCODE = "";
            CellID = "";
            SimStatus = "";
           

        }
        public string MSISDN { get; set; }
        public string IPV6 { get; set; }
        public string GCODE { get; set; }
        public string CellID { get; set; }
        public string SimStatus { get; set; }
      
    }
}
