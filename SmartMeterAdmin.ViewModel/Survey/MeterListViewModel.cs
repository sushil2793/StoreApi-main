using System;
using Microsoft.AspNetCore.Http;

namespace SmartMeterAdmin.ViewModel.Survey
{
    public class MeterListViewModel
        {
            public string ConsumerId { get; set; }      
            public string OldMeterNo { get; set; }
            public string NewSmartMeterSrNo { get; set; }
            public string ICCID { get; set; }
            public string SIMNW { get; set; }
           
        }
    }
