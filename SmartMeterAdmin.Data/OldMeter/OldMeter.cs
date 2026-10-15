using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmartMeterAdmin.Data.BaseClass;
using SmartMeterAdmin.Data.User;
namespace SmartMeterAdmin.Data.OldMeter
{
    public class OldMeter : BaseEntity
    {
        [Key]
        public int OldMeterID { get; set; }
        public int NewMeterID { get; set; }
       
        public string CustomerNumber { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string Pincode { get; set; }
        public string OldMeterSerialNumber { get; set; }
        
        public string LastBillingReading { get; set; }
        public string SectionLoad { get; set; }
        public string MeterCondition { get; set; }
        public string MonthlyBilled { get; set; }
        public string MobileNo { get; set; }
        public string MeterManufacture { get; set; }
        public string MeterStatus { get; set; }
        public string OldMeterImage { get; set; }
        public string OldBillImage { get; set; }
        public string MeterType { get; set; }
        public string AddSource { get; set; }

        public string BillingCategory { get; set; }



        [ForeignKey("CreatedBy")]
        public Users GetUsers { get; set; }

    }
}
