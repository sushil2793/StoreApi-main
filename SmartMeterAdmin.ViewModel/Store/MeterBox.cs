using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMeterAdmin.ViewModel.Store
{
    public class MeterBox
    {
        public string BOXID { get; set; }
        public int MeterCount { get; set; }
        public string IncomingDate { get; set; }
        public string ApprovedDate { get; set; }
    }
}
