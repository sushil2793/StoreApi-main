using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMeterAdmin.ViewModel.Master
{
    public class ListOfId
    {
        public List<int> ID { get; set; }
        public string Password { get; set; }
    }
    public class ListOfConsumerId
    {
        public List<string> ID { get; set; }
        public string AreaType { get; set; }
    }
}
