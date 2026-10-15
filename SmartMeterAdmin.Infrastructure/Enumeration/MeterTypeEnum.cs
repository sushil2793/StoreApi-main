using System.ComponentModel;

namespace SmartMeterAdmin.Infrastructure.Enumeration
{
    public enum MeterTypeEnum
    {
        [Description("Single Phase")]
        SinglePhase,
        [Description("Three Phase")]
        ThreePhase,
        CT
    }
}
