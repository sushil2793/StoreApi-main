using System.ComponentModel.DataAnnotations;

namespace SmartMeterAdmin.Data.VillageData
{
    public class VillageData
    {
        [Key]
        public int DataID { get; set; }
        public string CensusCode { get; set; }
        public string Name { get; set; }
    }
}
