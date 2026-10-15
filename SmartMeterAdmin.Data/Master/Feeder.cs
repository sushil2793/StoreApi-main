using System.ComponentModel.DataAnnotations;
using SmartMeterAdmin.Data.BaseClass;
namespace SmartMeterAdmin.Data.Master
{
    public class Feeder : BaseEntity
    {
        [Key]
        public int FeederID { get; set; }
        public int ZoneID { get; set; }
        public int CircleID { get; set; }
        public int DivisionID { get; set; }
        public int SubDivisionID { get; set; }
        public int SubStationID { get; set; }
        public string FeederName { get; set; }
    }
}
