using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMeterAdmin.ViewModel.MeterViewModel
{
    public class FeederProductionMeterDataViewModel
    {
        public string MeterNo { get; set; }
        public string MeterMaker { get; set; }
        public string MeterType { get; set; }
        public string MeterBoxSealNo { get; set; }
        public string ICCID { get; set; }
        public string SIMNW { get; set; }
    }
}