using Microsoft.AspNetCore.Http;

public class UpdateReplacementMeterRequest
{
    public string AreaType { get; set; }                 // e.g. REASON
    public string AreaValue { get; set; }                // OLD meter no (to replace)
    public string NewAreaValue { get; set; }             // NEW smart meter no

    // ✅ New meter initial readings (user enters these)
    public string NewinitialNetMeterImportReadingKWH { get; set; }
    public string NewinitialNetMeterExportReadingKWH { get; set; }
    public string NewinitialNetMeterImportReadingKVAH { get; set; }
    public string NewinitialNetMeterExportReadingKVAH { get; set; }

    // ✅ Old meter readings (auto-filled from HES daily load profile API when user enters old meter no)
    public string OldinitialNetMeterImportReadingKWH { get; set; }
    public string OldinitialNetMeterExportReadingKWH { get; set; }
    public string OldinitialNetMeterImportReadingKVAH { get; set; }
    public string OldinitialNetMeterExportReadingKVAH { get; set; }

    // ✅ Photos
    public IFormFile OldMeterPhoto { get; set; }
    public IFormFile NewMeterPhoto { get; set; }
}