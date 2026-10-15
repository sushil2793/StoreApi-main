using System;

namespace SmartMeterAdmin.Data.BaseClass
{
    public class BaseEntity
    {
        public DateTime Created { get; set; } = DateTime.Now;
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }

        public DateTime? Updated { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
        public bool IsDelete { get; set; } = false;
        public string DeviceType { get; set; } 
    }
}
