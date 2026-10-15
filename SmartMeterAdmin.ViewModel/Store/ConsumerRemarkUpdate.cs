
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SmartMeterAdmin.ViewModel.Store
{
    public class ConsumerRemarkUpdate
    {
        public string ConsumerId { get; set; }
        public string TableName { get; set; }
        public string Remark { get; set; }             // ACCEPT, REJECT, REACCEPT
        public string RemarkDescription { get; set; }  // Reason for REJECT
    }
}
