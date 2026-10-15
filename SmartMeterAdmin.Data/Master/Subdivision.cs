using System.ComponentModel.DataAnnotations;
using SmartMeterAdmin.Data.BaseClass;
namespace SmartMeterAdmin.Data.Master
{
    public class Subdivision : BaseEntity
    {
        [Key]
        public int SubDivisionID { get; set; }
        public int ZoneID { get; set; }
        public int CircleID { get; set; }
        public int DivisionID { get; set; }
        public string SubDivisionName { get; set; }
    }
}
