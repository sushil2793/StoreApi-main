using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmartMeterAdmin.Data.BaseClass;
using SmartMeterAdmin.Data.User;
namespace SmartMeterAdmin.Data.IncomingMeter
{
    public class IncomingMeter : BaseEntity
    {
        [Key]
        public int IncomingID { get; set; }
        public int StoreID { get; set; }
        public int OnePFromMeterNo { get; set; }
        public int OnePToMeterNo { get; set; }
        public int OnePMerterQty { get; set; }
        public int ThreePFromMeterNo { get; set; }
        public int ThreePMerterQty { get; set; }
        public int ThreePToMeterNo { get; set; }
        public int CTPFromMeterNo { get; set; }
        public int CTPToMeterNo { get; set; }
        public int CTPMerterQty { get; set; }
        public int HTPFromMeterNo { get; set; }
        public int HTPMerterQty { get; set; }
        public int HTPToMeterNo { get; set; }

        [ForeignKey("CreatedBy")]
        public Users GetUsers { get; set; }

        [ForeignKey("StoreID")]
        public Users GetStore { get; set; }
    }
}
