using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SmartMeterAdmin.ViewModel.MeterViewModel
{
    public class FeederNewMeterListViewModel
    {
        public string ConsumerId { get; set; }
        public string DateOfInstallation { get; set; }
        public string NewSmartMeterSrNo { get; set; }
    }
}
