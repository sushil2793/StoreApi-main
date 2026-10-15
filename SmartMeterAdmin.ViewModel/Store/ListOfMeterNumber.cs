using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMeterAdmin.ViewModel.Store
{
    public class ListOfMeterNumber
    {
        public List<string> Number { get; set; }
        public bool IsBox { get; set; }
        public int? Id { get; set; }
    }
}
