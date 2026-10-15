namespace SmartMeterAdmin.ViewModel.MeterViewModel
{
    public class NewMeterInput
    {

     

        public int NewMeterID { get; set; }
        public string CustomerName { get; set; } = "";
        public string CustomerNo { get; set; } = "";
        public string MobileNo { get; set; } = "";
        public string Address { get; set; } = "";


        public int SubstationID { get; set; }
        public int SubdivisionID { get; set; }
        public int FeederID { get; set; }
        

        public string MeterSerialNumber { get; set; } = "";
        public string MeterManufacture { get; set; } = "";
        public string MeterType { get; set; } = "";

        public string SimImei { get; set; } = "";
        public string IPv6Address { get; set; } = "";
        public string SimType { get; set; } = "";
        public string SimStatus { get; set; } = "";



        public string Latitude { get; set; } = "";
        public string Longitude { get; set; } = "";
        public string VillageName { get; set; } = "";
        public string BillingCategory { get; set; }

        public decimal? BillingRate { get; set; }

    }
}
