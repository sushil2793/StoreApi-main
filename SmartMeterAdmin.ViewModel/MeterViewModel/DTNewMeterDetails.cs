using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SmartMeterAdmin.ViewModel.MeterViewModel
{
    public class DTNewMeterDetails
    {
        public string ConsumerId { get; set; }
        public string BookNo { get; set; }
        public string OldReadingKWH { get; set; }
        public string OldReadingKVAH { get; set; }
        public string OldReadingKVA { get; set; }
        public string OldMeterStatus { get; set; }
        public string LTCTRating { get; set; }
        public string NoOfLTCTInstalled { get; set; }
        public string LugsSize { get; set; }
        public string NoOfLugsInstalled { get; set; }
        public string CableSize { get; set; }
        public string LengthOfCable { get; set; }
        public string NewSmartMeterSrNo { get; set; }
        public string SMCBoxSealNo { get; set; }
        public string InitialReadingKWH { get; set; }
        public string InitialReadingKVAH { get; set; }
        public string InitialReadingKVA { get; set; }
        public string LTCTMake { get; set; }
        public string LTCTType { get; set; }
        public string NewMMF { get; set; }
        public string LengthOfCopperCable { get; set; }
        public IFormFile NewMeterPhoto { get; set; }
        public string NewMeterPhotourl { get; set; }
        public IFormFile SMCBoxPhoto { get; set; }
        public string SMCBoxPhotourl { get; set; }
        public string Action { get; set; } = string.Empty;
        public string LineCTRatio { get; set; }
        public string LinePTRatio { get; set; }
    }
}
