using System;
using System.ComponentModel.DataAnnotations;

namespace SmartMeterAdmin.Data.SimData
{
    public class SimData
    {
        [Key]
        public int SimID { get; set; }
        public int SINumber { get; set; }
        public string MSISDN { get; set; }
        public string ICCID { get; set; }
        public string IMSINO { get; set; }
        public string IPV6 { get; set; }
        public string GCODE { get; set; }
        public string APN { get; set; }
        public string MeterSerialNo { get; set; }
        public string GPRSSealNo { get; set; }
        public string MeterSealNo { get; set; }
        public string BoxSealNo { get; set; }
        public int CreatedBy { get; set; }
        public DateTime Created { get; set; }
        public string CellID { get; set; }
        public string SimStatus { get; set; }
        public string MeterManufacture { get; set; }//sushil
        public string MeterType { get; set; }//sushil
        


    }
}
