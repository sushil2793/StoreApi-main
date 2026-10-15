using System.ComponentModel.DataAnnotations;
using SmartMeterAdmin.Data.BaseClass;
namespace SmartMeterAdmin.Data.Master
{
    public class Circle : BaseEntity
    {
        [Key]
        public int CircleID { get; set; }
        public int ZoneID { get; set; }

        public string CircleName { get; set; }
    }
}
