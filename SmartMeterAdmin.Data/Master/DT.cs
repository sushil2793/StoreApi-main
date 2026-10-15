using System.ComponentModel.DataAnnotations;
using SmartMeterAdmin.Data.BaseClass;
namespace SmartMeterAdmin.Data.Master
{
    public class DT : BaseEntity
    {

        [Key]
        public int DTID { get; set; }
        public int ZoneID { get; set; }
        public int CircleID { get; set; }
        public int DivisionID { get; set; }
        public int SubDivisionID { get; set; }
        public int SubStationID { get; set; }
        public int FeederID { get; set; }
        public string DTName { get; set; }


    }
}
