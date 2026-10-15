using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmartMeterAdmin.Data.BaseClass;
using SmartMeterAdmin.Data.User;

namespace SmartMeterAdmin.Data.AssignMeter
{
    public class AssignMeter : BaseEntity
    {
        [Key]
        public int AssignID { get; set; }
        public int ContractorID { get; set; }
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
        public int IssuedMeter { get; set; }
        public int RemainingMeter { get; set; }
        public bool IsReturn { get; set; }

        [ForeignKey("CreatedBy")]
        public Users GetUsers { get; set; }

        [ForeignKey("ContractorID")]
        public Users Getcontractor { get; set; }
    }
}
