using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMeterAdmin.ViewModel.MeterViewModel
{
    public class DTNewMeterRenderViewModel
    {
        public string ConsumerId { get; set; }
        public string MeterNo { get; set; }
        public string OldMeterNo { get; set; }
        public string BookNo { get; set; }
        public string InstallationDateTime { get; set; }
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
        public string InstallerName { get; set; }
        public string InstallerMobileNo { get; set; }
        public string MeterType { get; set; }
        public string MeterMaker { get; set; }
        public string MeterBoxSealNo { get; set; }
        public string ICCID { get; set; }
        public string SIMNW { get; set; }
        public string LTCTMake { get; set; }
        public string LTCTType { get; set; }
        public string NewMMF { get; set; }
        public string LengthOfCopperCable { get; set; }
        public string NewMeterPhoto { get; set; }
        public string SMCBoxPhoto { get; set; }
        public string HesSend { get; set; }
        public string LineCTRatio { get; set; }
        public string LinePTRatio { get; set; }
        public string ContractorName { get; set; }
    }
}
