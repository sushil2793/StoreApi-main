using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SmartMeterAdmin.ViewModel.MeterViewModel
{
    public class ConsumerNewMeterListViewModel
    {
        public string ConsumerId { get; set; }
        public string DateOfInstallation { get; set; }
        public string NewSmartMeterSrNo { get; set; }
    }
}
