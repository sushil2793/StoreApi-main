using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SmartMeterAdmin.ViewModel.MeterViewModel
{
    public class FeederNewMeterDetails
    {
        public string ConsumerId { get; set; }
        public string BookNo { get; set; }
        public string OldReadingKWH { get; set; }
        public string OldReadingKVAH { get; set; }
        public string OldReadingKVA { get; set; }
        public string NewSmartMeterSrNo { get; set; }
        public string InitialReadingKWH { get; set; }
        public string InitialReadingKVAH { get; set; }
        public string InitialReadingKVA { get; set; }
        public string NewMeterPhotoUrl { get; set; }
        public IFormFile NewMeterPhoto { get; set; }
        public string FeederControlPanelPhotoUrl { get; set; }
        public IFormFile FeederControlPanelPhoto { get; set; }
        public string Action { get; set; } = string.Empty;
        public string LineCTRatio { get; set; }
        public string LinePTRatio { get; set; }
    }
}
