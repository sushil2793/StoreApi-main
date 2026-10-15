using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SmartMeterAdmin.ViewModel.MeterViewModel
{
    public class NewMeterViewModel
    {
        public string CustomerNo { get; set; }
        public string MeterSerialNumber { get; set; }
        public string MeterManufacture { get; set; }
        public string TypeOfMeter { get; set; }
        public string SectionLoad { get; set; }
        public string FinalReadingOnLCDKWH { get; set; }
        public string FinalReadingKVAH { get; set; }
        public string MeterImage { get; set; }
        public string MeterType { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public bool IsSupplyCable { get; set; }
        public string CableLength { get; set; }
        public string BookNo { get; set; }
        public string CTRatio { get; set; }
        public string PTRatio { get; set; }
        public string MeterStatus { get; set; }
        public string NeturalAvability { get; set; }
        public string GPRSSignalStrength { get; set; }
        public string ConsumerName { get; set; }
        public string HouseOwnername { get; set; }
        public string ConsumerRelativeName { get; set; }
        public string ConsumerType { get; set; }
        public string AadharNo { get; set; }
        public string ConsumerMobileNo { get; set; }
        public string CustomerAddress { get; set; }
        public string VillageName { get; set; }
        public string Block { get; set; }
        public string TehsilName { get; set; }
        public string District { get; set; }
        public string ZoneName { get; set; }
        public string CircleName { get; set; }
        public string Division { get; set; }
        public string SubDivision { get; set; }
        public string SubStation { get; set; }
        public string SubstationCode { get; set; }
        public string Feeder { get; set; }
        public string DTFeederCode { get; set; }
        public string DT { get; set; }
        public string LTPoleCode { get; set; }
        public string DtCode { get; set; }
        public string NoOfConnectionInPremise { get; set; }
        public string CategoryOfUse { get; set; }
        public string BillingCategory { get; set; }
        public string MeterSealNo { get; set; }
        public string GPRSSealNo { get; set; }
        public string BoxSealNo { get; set; }
        public string TerminalSealNo { get; set; }
        public string PreInstalledSim { get; set; }
        public string SimType { get; set; }
        public string ContractorName { get; set; }

        public string SimStatus { get; set; }
        public string InstallationDate { get; set; }
    }

    public class PostDeviceMeterViewModel
    {


        public string CustomerName { get; set; }
        public string CustomerNo { get; set; }
        public string Address { get; set; }
        public string SubstationID { get; set; }
        public string SubdivisionID { get; set; }
        public string FeederID { get; set; }
        public string DTID { get; set; }
        public string MeterSerialNumber { get; set; }
        public string SimImei { get; set; }
        public string IPv6Address { get; set; }

        public string MeterManufacture { get; set; }
        public string MeterType { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string ISCUSTOMERNO { get; set; }
        public string Created { get; set; }

    }
}
