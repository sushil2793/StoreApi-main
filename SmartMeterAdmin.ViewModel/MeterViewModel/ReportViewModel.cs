using System;

namespace SmartMeterAdmin.ViewModel.MeterViewModel
{
    public class ReportViewModel
    {
        public string RoleType { get; set; }
        public int ElectricainID { get; set; }
        public int ContractorID { get; set; }
        public string MeterType { get; set; }
        public string District { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int Length { get; set; }
        public string SearchColumn { get; set; }
        public string SearchValue { get; set; }
    }
}
