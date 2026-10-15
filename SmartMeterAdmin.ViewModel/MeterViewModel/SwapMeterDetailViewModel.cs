using System;

namespace SmartMeterAdmin.ViewModel.MeterViewModel
{
    /// <summary>
    /// SwapMeter table ka full snapshot row - UI meter-swap history list + expand detail
    /// isi se bharta hai. (GET_SWAP_METER_LIST proc ke column aliases se match karta hai.)
    /// </summary>
    public class SwapMeterDetailViewModel
    {
        // ── List row ──
        public int SwapId { get; set; }
        public string ConsumerId { get; set; }
        public string NewConsumerId { get; set; }
        public string OldMeterNo { get; set; }          // PreviousMeterNo
        public string SwapMeterNo { get; set; }
        public string ConsumerName { get; set; }
        public string Subdivision { get; set; }
        public string SwappedByName { get; set; }        // Users.FullName (kisne kiya)
        public int? SwappedBy { get; set; }              // UserId (kisne kiya)
        public string SwappedOn { get; set; }            // formatted string
        public string SwapDateTime { get; set; }
        public string SwapDate { get; set; }

        // ── Consumer detail (expand) ──
        public string FatherHusbandProprietorName { get; set; }
        public string ConsumerMobileNo { get; set; }
        public string Address { get; set; }
        public string SanctionLoadKW { get; set; }
        public string TariffName { get; set; }

        // ── Old meter detail (expand) ──
        public string OldMeterType { get; set; }
        public string OldMeterMode { get; set; }
        public string OldMeterMake { get; set; }
        public string OldMeterStatusAtInstallation { get; set; }
        public string OldMeterReadingKwh { get; set; }
        public string OldMeterReadingKvah { get; set; }

        // ── New meter detail (expand) ──
        public string MeterType { get; set; }
        public string MeterMode { get; set; }
        public string MeterMaker { get; set; }
        public string MeterBoxSealNo { get; set; }
        public string ICCID { get; set; }
        public string SIMNW { get; set; }
        public string BookNoOrSlipNo { get; set; }
        public string DateOfInstallation { get; set; }
        public string InitialReadingKwh { get; set; }
        public string InitialReadingKvah { get; set; }

        // ── Installer (expand) ──
        public string InstallerName { get; set; }
        public string InstallerMobileNo { get; set; }

        // ── Location / Hierarchy (expand) ──
        public string Zone { get; set; }
        public string Circle { get; set; }
        public string Division { get; set; }
        public string Feeder11KVName { get; set; }
        public string Feeder11KVCode { get; set; }
        public string DTCode { get; set; }
        public string DTName { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }

        // ── Status + photos (expand) ──
        public string Remark { get; set; }               // ab SwapRemark value isme aati hai (GET proc alias)
        public string SwapRemark { get; set; }           // raw column (future use)
        public string HesSend { get; set; }
        public string ContractorName { get; set; }
        public string NewMeterPhoto { get; set; }
        public string MeterPhotoWithConsumer { get; set; }
    }
}
