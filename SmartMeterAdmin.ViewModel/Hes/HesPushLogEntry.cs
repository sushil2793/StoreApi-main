using System.Collections.Generic;

namespace SmartMeterAdmin.ViewModel.Hes
{
    /// <summary>
    /// CONSUMER push-to-HES (sync) ka per-meter log entry.
    /// UpdateHesStatus response ke data.logs me wapas jata hai
    /// (wahi logs jo pehle sirf log file me jaate the).
    /// </summary>
    public class HesPushLogEntry
    {
        public string ConsumerId { get; set; }
        public string MeterNo { get; set; }
        public string Status { get; set; }         // SUCCESS / FAILED
        public string HesHttpStatus { get; set; }  // HES HTTP status code (200/500...)
        public string HesResponse { get; set; }    // HES ka response body ya exception message
        public string Message { get; set; }        // DB status update / extra detail
        public string Payload { get; set; }        // HES ko bheja gaya poora JSON payload
    }

    /// <summary>Poore sync push ka summary + per-meter logs</summary>
    public class HesPushResult
    {
        public string AreaType { get; set; }
        public int Total { get; set; }
        public int Success { get; set; }
        public int Failed { get; set; }
        public string Error { get; set; }          // HES login fail / koi bada exception
        public List<HesPushLogEntry> Logs { get; set; } = new List<HesPushLogEntry>();
    }
}
