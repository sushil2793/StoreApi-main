using SmartMeterAdmin.Data.BaseClass;
using System.ComponentModel.DataAnnotations;

namespace SmartMeterAdmin.Data.HelpEntity
{
    public class HelpDocument : BaseEntity
    {
        [Key]
        public int HelpDocumentID { get; set; }
        public string DocumentName { get; set; }
        public string AttachmentUrl { get; set; }
    }
}
