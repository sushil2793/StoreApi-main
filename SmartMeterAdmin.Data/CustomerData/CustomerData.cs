using System;
using System.ComponentModel.DataAnnotations;

namespace SmartMeterAdmin.Data.CustomerData
{
    public class CustomerData
    {
        [Key]
        public int CustomerID { get; set; }
        public string Dist_Division { get; set; }
        public string SubDivision { get; set; }
        public string SubStation { get; set; }
        public string Feeder { get; set; }
        public string DT { get; set; }
        public string District { get; set; }
        public string ConsumerName { get; set; }
        public string Division { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerAddress { get; set; }
        public string TariffName { get; set; }
        public string ProductName { get; set; }
        public string ConnectionName { get; set; }
        public string NewConsumerNo { get; set; }
        /*---Add 3 colomn */
        public string BillingCategory { get; set; }
        public string BillingRate { get; set; }
        public string Remark { get; set; }
        /*---add 3 colomn */
        public DateTime CreatedDate { get; set; }
        public bool IsReplaceMeter { get; set; }


    }

}
