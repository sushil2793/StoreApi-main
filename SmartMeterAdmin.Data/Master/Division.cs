using System.ComponentModel.DataAnnotations;
using SmartMeterAdmin.Data.BaseClass;
namespace SmartMeterAdmin.Data.Master
{
    public class Division : BaseEntity
    {
        [Key]
        public int DivisionID { get; set; }
        public int ZoneID { get; set; }
        public int CircleID { get; set; }

        public string DivisionName { get; set; }

    }
}
