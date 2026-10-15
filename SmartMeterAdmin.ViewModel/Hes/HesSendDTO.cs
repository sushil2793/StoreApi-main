using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMeterAdmin.ViewModel.Hes
{
    public class HesSendDTO
    {
        public string Description { get; set; }
        public string SubDivisionName { get; set; }
        public string SubStationName { get; set; }
        public string FeederName { get; set; }
        public string Address { get; set; }
        public string CommissioningStatus { get; set; }
        public string ConsumerName { get; set; }
        public string ConsumerNo { get; set; }
        public string DeviceSerialNo { get; set; }
        public string DeviceType { get; set; }
        public string DtName { get; set; }
        public string IpAddressMain { get; set; }
        public string IpPortMain { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Manufacturer { get; set; }
        public string OwnerName { get; set; }
        public string PhoneNumber { get; set; }
        public string SimNumber { get; set; }
        public string DivisionName { get; set; }
        public string ZoneName { get; set; }
        public string CircleName { get; set; }
        public string InstallationDateTime { get; set; }
        public string Network { get; set; }
        public string NewNetMeteringFlag { get; set; }
        public string PrepaidFlag { get; set; }
        public string NewMeterExportKwh { get; set; }
        public string NewMeterExportKvah { get; set; }
        public string OldMeterExportKwh { get; set; }
        public string OldMeterExportKvah { get; set; }
        public string NewMeterImportKwh { get; set; }
        public string NewMeterImportKvah { get; set; }
        public string OldMeterImportKWH { get; set; }
        public string OldMeterImportKvah { get; set; }
        public string TypeOfReplacementCode { get; set; }
        public string MeterCTRatio { get; set; }
        public string MeterPTRatio { get; set; }
        public string LineCtRatio { get; set; }
        public string LinePtRatio { get; set; }
        public string NewMeterMf { get; set; }
        public string OldMeterSerialNumber { get; set; }
        public string OldMeterMake { get; set; }
        public string MeterVoltageRating { get; set; }
        public string PaymentMode { get; set; }

    }
}
