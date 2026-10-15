using System.Collections.Generic;

namespace SmartMeterAdmin.ViewModel.Store
{
    public class DeleteRequest
    {
        public string ActionType { get; set; }
        public List<string> KeyValues { get; set; }
        public string DeletedBy { get; set; }
    }
}
