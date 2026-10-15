using System;
using System.Collections.Generic;

namespace SmartMeterAdmin.ViewModel.DataTable
{
    public class DataTableViewModel
    {
        public int Length { get; set; }
        public int Start { get; set; }
        public List<Order> Order { get; set; }
        public SearchModel Search { get; set; }
    }

    public class AgGridTable
    {
        public int Length { get; set; }
        public string SearchColumn { get; set; }
        public string SearchValue { get; set; }
        public string RoleType { get; set; }
        public string FromMeter { get; set; }
        public string ToMeter { get; set; }
        public int PageNumber { get; set; }
        public int UserId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class SearchModel
    {
        public string Value { get; set; }
    }
    public class Order
    {
        public string Column { get; set; }
        public string Dir { get; set; }
    }
}
