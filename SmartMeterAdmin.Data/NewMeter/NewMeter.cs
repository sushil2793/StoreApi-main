using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using SmartMeterAdmin.ViewModel.MeterViewModel;
using SmartMeterAdmin.Data.BaseClass;
using SmartMeterAdmin.Data.User;
namespace SmartMeterAdmin.Data.NewMeter
{
    public class NewMeter : BaseEntity
    {
        [Key]
        
        public int NewMeterID { get; set; }
        public string CustomerName { get; set; } = "";
        public string CustomerNo { get; set; } = "";
        public string Address { get; set; } = "";
        public string Pincode { get; set; } = "";
        public string WhatsappNo { get; set; } = "";
        
        public int SubstationID { get; set; }
        public int SubdivisionID { get; set; }
        public int FeederID { get; set; }
        public int DTID { get; set; }
        public string MeterSerialNumber { get; set; } = "";
        public string MobileSerialNumber { get; set; } = "";
        public string SimImei { get; set; } = "";
        public string IPv6Address { get; set; } = "";
        public string MeterSealNo { get; set; } = "";
        public string BoxSealNo { get; set; } = "";
        public string GprsSealNo { get; set; } = "";
        public string MeterManufacture { get; set; } = "";
        public string SectionLoad { get; set; } = "";

        
        public string InstalledKWH { get; set; } = "";
        public string MeterImage { get; set; } = "";
        public string MobileNo { get; set; } = "";
        public string MeterType { get; set; } = "";
        public string Latitude { get; set; } = "";
        public string Longitude { get; set; } = "";

        
        public string DOB { get; set; } = "";
        public string Gents { get; set; } = "";
        public string Ladies { get; set; } = "";
        public string Kids { get; set; } = "";
        public string Income { get; set; } = "";
        public string AddSource { get; set; } = "";
        public bool IsReplace { get; set; }
        public string AadharNo { get; set; } = "";
        public string BookNo { get; set; } = "";
        public string Houseno { get; set; } = "";
        public string VillageName { get; set; } = "";
        public string SimType { get; set; } = "";
        public string SimStatus { get; set; } = "";
        public string CensusCode { get; set; } = "";

        public bool IsSupplyCable { get; set; }
        public string CableLength { get; set; } = "";
        public string NewConsumerNo { get; set; } = "";
        public string BillingCategory { get; set; } = "";
        public string CompanyName { get; set; } = "";
        public decimal? BillingRate { get; set; }

        [ForeignKey("CreatedBy")]
        public Users GetUsers { get; set; }
    }
}
