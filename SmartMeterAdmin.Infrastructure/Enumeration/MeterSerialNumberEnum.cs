using System.ComponentModel;

namespace SmartMeterAdmin.Infrastructure.Enumeration
{
    public enum MeterSerialNumberEnum
    {
        [Description("IN")]
        CT,
        [Description("11")]
        SinglePhase,
        [Description("22")]
        ThreePhase
    }

    
}
