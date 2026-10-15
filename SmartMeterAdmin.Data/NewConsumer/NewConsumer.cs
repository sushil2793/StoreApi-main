using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmartMeterAdmin.Data.BaseClass;
using SmartMeterAdmin.Data.User;
namespace SmartMeterAdmin.Data.NewConsumer
{
    public class NewConsumer : BaseEntity
    {
        [Key]
        public int ConsumerID { get; set; }
        public string ConsumerName { get; set; }
        public string MobileNo { get; set; }
        public bool IsMeterBefore { get; set; }
        public string TentativeLoad { get; set; }
        public string Address { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Latitude { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Longitude { get; set; }
        public string ImageUrl { get; set; }
        public string District { get; set; }

        [ForeignKey("CreatedBy")]
        public Users GetUsers { get; set; }
    }
}
