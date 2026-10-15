using SmartMeterAdmin.Infrastructure.Enumeration;
using System.Collections.Generic;

namespace SmartMeterAdmin.Infrastructure.Helpers
{
    public static class ColumnNameByTable
    {
        //Note : Left Side Column name is the column name in the excel sheet and right side column name is the column name in the database table.
        private static Dictionary<string, string> columns;
        public static IDictionary<string, string> GetColumns(TableNameEnum tableName)
        {
            switch (tableName)
            {
                case TableNameEnum.TEMP_UPDATED_DATA:
                    return TEMP_UPDATED_DATA_TABLE();
                case TableNameEnum.TempProductionMeter:
                    return TempProductionMeterTable();
                case TableNameEnum.TempIssueMeter:
                    return TempIssueMeterTable();
                case TableNameEnum.TempOldMeter:
                    return TempOldMeterTable();
                case TableNameEnum.TempNewMeter:
                    return TempNewMeterTable();
                case TableNameEnum.TempSimData:
                    return TempDataTable();
                case TableNameEnum.TempDataSimStatus:
                    return TempDataSimStatusTable();
                case TableNameEnum.TempFullCustomerData:
                    return TempFullCustomerDataTable();
                case TableNameEnum.TempFeederSurvey:
                    return TempFeederSurveyTable();
                case TableNameEnum.TempDTSurvey:
                    return TempDTSurveyTable();
                case TableNameEnum.TempConsumerSurvey:
                    return TempConsumerSurveyTable();
                case TableNameEnum.TempDTNewMeter:
                    return TempDTNewMeterTable();
                case TableNameEnum.TempConsumerNewMeter:
                    return TempConsumerNewMeterTable();
                case TableNameEnum.TempFeederNewMeter:
                    return TempFeederNewMeterTable();
                case TableNameEnum.TempDTCNewMeter:
                    return TempDTCNewMeterTable();
                case TableNameEnum.TempDTCSurvey:
                    return TempDTCSurveyTable();
                case TableNameEnum.TempHTCNewMeter:
                    return TempHTCNewMeterTable();
                case TableNameEnum.TempHTCSurvey:
                    return TempHTCSurveyTable();
                case TableNameEnum.TempSurveyAllTable:
                    return TempSurveyAllTable();

            }
            return columns;
        }


        private static IDictionary<string, string> TempSurveyAllTable()
        {
            return new Dictionary<string, string>()
    {
        { "ConsumerId", "ConsumerId" },
        { "Latitude", "Latitude" },
        { "Longitude", "Longitude" },
        { "DateOfSurvey", "DateOfSurvey" },
        { "Zone", "Zone" },
        { "CircleWise", "CircleWise" },
        { "Division", "Division" },
        { "Subdivision", "Subdivision" },
        { "Substation", "Substation" },
        { "SubstationCode", "SubstationCode" },
        { "DTFeeder", "DTFeeder" },
        { "DTFeederCode", "DTFeederCode" },
        { "DTName", "DTName" },
        { "DTCode", "DTCode" },
        { "ConsumerName", "ConsumerName" },
        { "ConsumerResponse", "ConsumerResponse" },
        { "ConsumerRelativeName", "ConsumerRelativeName" },
        { "IdentityType", "IdentityType" },
        { "AadharNo", "AadharNo" },
        { "ConsumerMobileNo", "ConsumerMobileNo" },
        { "ConsumerAddress", "ConsumerAddress" },
        { "CorrectConsumerAddress", "CorrectConsumerAddress" },
        { "District", "District" },
        { "SurveyAgency", "SurveyAgency" },
        { "NewConsumerId", "NewConsumerId" },

        // Meter Fields
        { "TarrifName", "TarrifName" },
        { "SanctionLoad", "SanctionLoad" },
        { "MeterdConsumer", "MeterdConsumer" },
        { "ConnectionStatus", "ConnectionStatus" },
        { "MeterMake", "MeterMake" },
        { "MeterNo", "MeterNo" },
        { "CorrectMeterNo", "CorrectMeterNo" },
        { "MeterBoxStatus", "MeterBoxStatus" },
        { "MeterStatus", "MeterStatus" },
        { "MeterSealStatus", "MeterSealStatus" },
        { "MeterType", "MeterType" },
        { "MeterLocation", "MeterLocation" },
        { "MeterReading", "MeterReading" },
        { "GPRSSignalStrengthAirtel", "GPRSSignalStrengthAirtel" },
        { "GPRSSignalStrengthJio", "GPRSSignalStrengthJio" },
        { "MeterBoxSealNo", "MeterBoxSealNo" },
        { "MeterTerminalSealNo", "MeterTerminalSealNo" },
        { "MeterMode", "MeterMode" },
        { "NetMeterImportReading", "NetMeterImportReading" },
        { "NetMeterExportReading", "NetMeterExportReading" },
        { "MeterPhotoUrlKVAH", "MeterPhotoUrlKVAH" },

    };
        }
        private static IDictionary<string, string> TempIssueMeterTable()
        {
            return new Dictionary<string, string>()
    {
        // Excel Column Name → DB Column Name
        { "MeterNo", "MeterNo" },
        { "ContractorId", "ContractorId" },
        { "CreatedBy", "CreatedBy" }
    };
        }


        private static IDictionary<string, string> TempHTCNewMeterTable()
        {
            return new Dictionary<string, string>()
            {
               { "ConsumerId", "ConsumerId" },
{ "NewSmartMeterSrNo", "NewSmartMeterSrNo" },
{ "BookNoOrSlipNo", "BookNoOrSlipNo" },
{ "DateOfInstallation", "DateOfInstallation" },
{ "OldMeterReadingKwh", "OldMeterReadingKwh" },
{ "OldMeterReadingKvah", "OldMeterReadingKvah" },
{ "BalanceOfPrepaidMeter", "BalanceOfPrepaidMeter" },
{ "ExistingReturnMeter", "ExistingReturnMeter" },
{ "InitialReadingKwh", "InitialReadingKwh" },
{ "InitialReadingKvah", "InitialReadingKvah" },
{ "NewMeterPhoto", "NewMeterPhoto" },
{ "MeterPhotoWithConsumer", "MeterPhotoWithConsumer" },
{ "MeterPhotoWithKVAH", "MeterPhotoWithKVAH" },
{ "MeterPhotoWithKWH", "MeterPhotoWithKWH" },
{ "UserID", "UserID" },
{ "HesSend", "HesSend" },
{ "NewConsumerId", "NewConsumerId" },
{ "OldMeterStatusAtInstallation", "OldMeterStatusAtInstallation" },
{ "CtRatio", "CtRatio" }


    };
        }

        private static IDictionary<string, string> TempHTCSurveyTable()
        {
            return new Dictionary<string, string>()
            {
      { "ConsumerId", "ConsumerId" },
        { "NewSmartMeterSrNo", "NewSmartMeterSrNo" },
        { "BookNoOrSlipNo", "BookNoOrSlipNo" },
        { "DateOfInstallation", "DateOfInstallation" },
        { "OldMeterReadingKwh", "OldMeterReadingKwh" },
        { "OldMeterReadingKvah", "OldMeterReadingKvah" },
        { "BalanceOfPrepaidMeter", "BalanceOfPrepaidMeter" },
        { "ExistingReturnMeter", "ExistingReturnMeter" },
        { "InitialReadingKwh", "InitialReadingKwh" },
        { "InitialReadingKvah", "InitialReadingKvah" },
        { "NewMeterPhoto", "NewMeterPhoto" },
        { "MeterPhotoWithConsumer", "MeterPhotoWithConsumer" },
        { "MeterPhotoWithKVAH", "MeterPhotoWithKVAH" },
        { "MeterPhotoWithKWH", "MeterPhotoWithKWH" },
        { "UserID", "UserID" },
        { "HesSend", "HesSend" },
        { "NewConsumerId", "NewConsumerId" },
        { "OldMeterStatusAtInstallation", "OldMeterStatusAtInstallation" },
        { "CtRatio", "CtRatio" },


    };
        }

        private static IDictionary<string, string> TempDTCSurveyTable()
        {
            return new Dictionary<string, string>()
            {
                 { "ConsumerId", "ConsumerId" },
                { "Latitude", "Latitude" },
                { "Longitude", "Longitude" },
                { "DateOfSurvey", "DateOfSurvey" },
                { "Zone", "Zone" },
                { "CircleWise", "CircleWise" },
                { "Division", "Division" },
                { "Subdivision", "Subdivision" },
                { "Substation", "Substation" },
                { "SubstationCode", "SubstationCode" },
                { "DTFeeder", "DTFeeder" },
                { "DTFeederCode", "DTFeederCode" },
                { "DTName", "DTName" },
                { "DTCode", "DTCode" },
                { "ConsumerName", "ConsumerName" },
                { "ConsumerResponse", "ConsumerResponse" },
                { "ConsumerRelativeName", "ConsumerRelativeName" },
                { "IdentityType", "IdentityType" },
                { "AadharNo", "AadharNo" },
                { "ConsumerMobileNo", "ConsumerMobileNo" },
                { "ConsumerAddress", "ConsumerAddress" },
                { "CorrectConsumerAddress", "CorrectConsumerAddress" },
                { "District", "District" },
                { "SurveyAgency", "SurveyAgency" },
                { "UserId", "UserId" },
                { "CreatedDate", "CreatedDate" },
                { "NewConsumerId", "NewConsumerId" },
                { "TarrifName", "TarrifName" },
                { "SanctionLoad", "SanctionLoad" },
                { "MeterdConsumer", "MeterdConsumer" },
                { "ConnectionStatus", "ConnectionStatus" },
                { "MeterMake", "MeterMake" },
                { "MeterNo", "MeterNo" },
                { "CorrectMeterNo", "CorrectMeterNo" },
                { "MeterBoxStatus", "MeterBoxStatus" },
                { "MeterStatus", "MeterStatus" },
                { "MeterSealStatus", "MeterSealStatus" },
                { "MeterType", "MeterType" },
                { "MeterLocation", "MeterLocation" },
                { "MeterPhotoUrl", "MeterPhotoUrl" },
                { "MeterReading", "MeterReading" },
                { "GPRSSignalStrengthAirtel", "GPRSSignalStrengthAirtel" },
                { "GPRSSignalStrengthJio", "GPRSSignalStrengthJio" },
                { "MeterBoxSealNo", "MeterBoxSealNo" },
                { "MeterTerminalSealNo", "MeterTerminalSealNo" },
                { "MeterMode", "MeterMode" },
                { "DtRatingKVA", "DtRatingKVA" },
                { "CtRatio", "CtRatio" },
                { "PhotoOfMeterOfCumKVAHUrl", "PhotoOfMeterOfCumKVAHUrl" },
                { "PhotoWithMeterCumReadingKWHUrl", "PhotoWithMeterCumReadingKWHUrl" },
                { "OldMeterReadingCumKVAH", "OldMeterReadingCumKVAH" },
                { "OldMeterReadingCumKWH", "OldMeterReadingCumKWH" },




    };
        }


        private static IDictionary<string, string> TempDTCNewMeterTable()
        {
            return new Dictionary<string, string>()
    {
        { "ConsumerId", "ConsumerId" },
        { "NewSmartMeterSrNo", "NewSmartMeterSrNo" },
        { "BookNoOrSlipNo", "BookNoOrSlipNo" },
        { "DateOfInstallation", "DateOfInstallation" },
        { "OldMeterReadingKwh", "OldMeterReadingKwh" },
        { "OldMeterReadingKvah", "OldMeterReadingKvah" },
        { "BalanceOfPrepaidMeter", "BalanceOfPrepaidMeter" },
        { "ExistingReturnMeter", "ExistingReturnMeter" },
        { "InitialReadingKwh", "InitialReadingKwh" },
        { "InitialReadingKvah", "InitialReadingKvah" },
        { "NewMeterPhoto", "NewMeterPhoto" },
        { "MeterPhotoWithConsumer", "MeterPhotoWithConsumer" },
        { "UserID", "UserID" },
        { "HesSend", "HesSend" },
        { "NewConsumerId", "NewConsumerId" },
        { "OldMeterStatusAtInstallation", "OldMeterStatusAtInstallation" },
        { "CtRatio", "CtRatio" },
        { "OldMeterReadingCumKVAH", "OldMeterReadingCumKVAH" },
        { "OldMeterReadingCumKWH", "OldMeterReadingCumKWH" }

    };
        }
        private static IDictionary<string, string> TEMP_UPDATED_DATA_TABLE()
        {
            return columns = new Dictionary<string, string>()
            {
                { "KeyType", "PARENTKEY" },
                { "KeyValue", "PARENTVALUE" },
                { "LocationType", "LOCATIONTYPE" },
                { "ColumnName", "COLUMNNAME" },
                { "ColumnValue", "COLUMNVALUE" },
            };
        }

        private static IDictionary<string, string> TempFeederNewMeterTable()
        {
            return columns = new Dictionary<string, string>()
            {
                { "ConsumerId", "ConsumerId" },
                { "BookNo", "BookNo" },
                { "InstallationDateTime", "InstallationDateTime" },
                { "OldReadingKWH", "OldReadingKWH" },
                { "OldReadingKVAH", "OldReadingKVAH" },
                { "OldReadingKVA", "OldReadingKVA" },
                { "MeterNo", "MeterNo" },
                { "InitialReadingKWH", "InitialReadingKWH" },
                { "InitialReadingKVAH", "InitialReadingKVAH" },
                { "InitialReadingKVA", "InitialReadingKVA" },
                { "NewMeterPhoto", "NewMeterPhoto" },
                { "FeederControlPanelPhoto", "FeederControlPanelPhoto" },
                { "OldMeterNo","OldMeterNo" },
                { "MeterMaker","MeterMaker" },
                { "MeterType", "MeterType" },
                { "MeterBoxSealNo", "MeterBoxSealNo" },
                { "ICCID", "ICCID" },
                { "SIMNW","SIMNW" },
            };
        }

        private static IDictionary<string, string> TempConsumerNewMeterTable()
        {
            return columns = new Dictionary<string, string>()
            {
                { "ConsumerId", "ConsumerId" },
                { "NewConsumerId", "NewConsumerId" },
                { "OldMeterNo", "OldMeterNo" },
                { "OldMeterType", "OldMeterType" },
                { "NewSmartMeterSrNo", "NewSmartMeterSrNo" },
                { "MeterType", "MeterType" },
                { "BookNoOrSlipNo", "BookNoOrSlipNo" },
                { "DateOfInstallation", "DateOfInstallation" },
                { "OldMeterReadingKwh", "OldMeterReadingKwh" },
                { "OldMeterReadingKvah", "OldMeterReadingKvah" },
                { "BalanceOfPrepaidMeter", "BalanceOfPrepaidMeter" },
                { "ExistingReturnMeter", "ExistingReturnMeter" },
                { "InitialReadingKwh", "InitialReadingKwh" },
                { "InitialReadingKvah", "InitialReadingKvah" },
                { "NewMeterPhoto", "NewMeterPhoto" },
                { "MeterPhotoWithConsumer", "MeterPhotoWithConsumer" },
                { "InstallerName", "InstallerName" },
                { "InstallerMobileNo", "InstallerMobileNo" },
                { "MeterMaker", "MeterMaker" },
                { "MeterBoxSealNo", "MeterBoxSealNo" },
                { "ICCID", "ICCID" },
                { "SIMNW", "SIMNW" },
            };
        }

        private static IDictionary<string, string> TempDTNewMeterTable()
        {
            return columns = new Dictionary<string, string>()
            {
                { "ConsumerID", "ConsumerId" },
                { "MeterNo","MeterNo" },
                { "OldMeterNo", "OldMeterNo" },
                { "BookNo", "BookNo" },
                { "InstallationDateTime","InstallationDateTime" },
                { "OldReadingKWH","OldReadingKWH" },
                { "OldReadingKVAH", "OldReadingKVAH" },
                { "OldReadingKVA", "OldReadingKVA" },
                { "OldMeterStatus", "OldMeterStatus" },
                { "LTCTRating","LTCTRating" },
                { "NoOfLTCTInstalled", "NoOfLTCTInstalled" },
                { "LugsSize", "LugsSize" },
                { "NoOfLugsInstalled", "NoOfLugsInstalled" },
                { "CableSize", "CableSize" },
                { "LengthOfCable", "LengthOfCable" },
                { "NewSmartMeterSrNo", "NewSmartMeterSrNo" },
                { "SMCBoxSealNo", "SMCBoxSealNo" },
                { "InitialReadingKWH", "InitialReadingKWH" },
                { "InitialReadingKVAH", "InitialReadingKVAH" },
                { "InitialReadingKVA", "InitialReadingKVA" },
                { "InstallerName", "InstallerName" },
                { "InstallerMobileNo", "InstallerMobileNo" },
                { "MeterType", "MeterType" },
                { "MeterMaker", "MeterMaker" },
                { "MeterBoxSealNo", "MeterBoxSealNo" },
                { "ICCID", "ICCID" },
                { "SIMNW", "SIMNW" },
                { "LTCTMake", "LTCTMake" },
                { "LTCTType", "LTCTType" },
                { "NewMMF", "NewMMF" },
                { "LengthOfCopperCable", "LengthOfCopperCable" },
                { "NewMeterPhoto", "NewMeterPhoto" },
                { "SMCBoxPhoto", "SMCBoxPhoto" },
            };
        }
        private static IDictionary<string, string> TempProductionMeterTable()
        {
            return columns = new Dictionary<string, string>()
            {
                { "MeterNo", "MeterNo" },
                { "CreatedBy", "CreatedBy" },
                { "MeterType", "MeterType" },
                { "MeterStatus", "MeterStatus" },
                { "MSSIDN", "MSSIDN" },
                { "ICCID", "ICCID" },
                { "IMSINO", "IMSINO" },
                { "IPV6", "IPV6" },
                { "SIMNW", "SIMNW" },
                { "SimStatus", "SimStatus" },
                { "GPRSSealNo", "GPRSSealNo" },
                { "MeterSealNo", "MeterSealNo" },
                { "MeterBoxSealNo", "MeterBoxSealNo" },
                 { "MeterTerminalSealNo", "MeterTerminalSealNo" },
                 { "BoxId", "BoxId" },
            };
        }
        private static IDictionary<string, string> TempOldMeterTable()
        {
            return columns = new Dictionary<string, string>()
            {
                { "CustomerId", "CustomerNumber" },
                { "Oldmeterserialno", "OldMeterSerialNumber" },
                { "OldMetersealno", "MeterSealNo" },
                { "Oldmeterouterboxno", "MeterBoxNo" },
                { "OldmeterType", "MeterType" },
                { "OldmeterReadingKWHonLCD", "ReadingKWHonLCD" },
                { "OldmeterReadingKW", "ReadingKW" },
                { "OldmeterReadingKvah", "ReadingKvah" },
                { "ContractorName", "ContractorName" },
                { "CreatedBy", "CreatedBy" },
            };
        }
        private static IDictionary<string, string> TempNewMeterTable()
        {
            return columns = new Dictionary<string, string>()
            {
                { "CustomerId", "CustomerNo" },
                { "Newmeterserialno", "MeterSerialNumber" },
                { "ReadingKWHonLCD", "FinalReadingOnLCDKWH" },
                { "ReadingKVAH", "FinalReadingKVAH" },
                { "CTratio", "CTRatio" },
                { "PTratio", "PTRatio" },
                { "Sanctionload", "SectionLoad" },
                { "Anytemper", "MeterStatus" },
                { "MeterType", "MeterType" },
                { "Metermake", "MeterManufacture" },
                { "Typeofsmartmeter", "SmartMeterType" },
                { "Installationdate", "Created" },
                { "Latitude", "Latitude" },
                { "Longtitude", "Longitude" },
                { "Contractorname", "ContractorName" },
                { "Bookno", "BookNo" },
                { "ArmoredCableInstalled", "ArmoredCableInstalled" },
                { "Cablelength", "CableLength" },
                { "NeturalAvaibility", "NeturalAvability" },
                { "CreatedBy", "CreatedBy" },
            };
        }
        private static IDictionary<string, string> TempDataTable()
        {
            return columns = new Dictionary<string, string>()
            {
                { "MeterSerialNo", "MeterSerialNo" },
                { "MSISDN", "MSISDN" },
                { "ICCID", "ICCID" },
                { "IMSINO", "IMSINO" },
                { "IPV6", "IPV6" },
                { "GPRSSealNo", "GPRSSealNo" },
                { "MeterSealNo", "MeterSealNo" },
                { "BoxSealNo", "BoxSealNo" },
                { "SimStatus", "SimStatus" },
                { "CreatedBy", "CreatedBy" },
            };
        }
        private static IDictionary<string, string> TempDataSimStatusTable()
        {
            return columns = new Dictionary<string, string>()
            {
                { "MeterSerialNo", "MeterSerialNo" },
                { "Status", "SimStatus" },
                { "CreatedBy", "CreatedBy" },
            };
        }
        private static IDictionary<string, string> TempFullCustomerDataTable()
        {
            return columns = new Dictionary<string, string>()
            {
                { "CustomerId", "CustomerNo" },
                { "Consumername", "ConsumerName" },
                { "Address", "CustomerAddress" },
                { "Zone", "ZoneName" },
                { "Circlewise", "CircleName" },
                { "Division", "Division" },
                { "Subdivision", "SubDivision" },
                { "Substation", "SubStation" },
                { "DTFeeder", "Feeder" },
                { "DTname", "DT" },
                { "District", "District" },
                { "Houseownername", "HouseOwnername" },
                { "ConsumerDetailFather/husband/proprietorname", "ConsumerRelativeName" },
                { "Consumertype", "ConsumerType" },
                { "Adhar/EpicNo", "AadharNo" },
                { "Consumermobileno", "ConsumerMobileNo" },
                { "Villagename", "VillageName" },
                { "Block", "Block" },
                { "Tehsilname", "TehsilName" },
                { "Substationcode", "SubstationCode" },
                { "DTFeedercode", "DTFeederCode" },
                { "DTcode", "DtCode" },
                { "LTpolecode", "LTPoleCode" },
                { "Noofconnectioninpremise", "NoOfConnectionInPremise" },
                { "Categoryofusedomestic/commercial", "CategoryOfUse" },
                { "BillingCategory", "BillingCategory" },
                { "CreatedBy", "CreatedBy" },
            };
        }
        private static IDictionary<string, string> TempFeederSurveyTable()
        {
            return columns = new Dictionary<string, string>()
            {
                { "FeederID", "ConsumerId" },
                { "FeederName","Name"},
                { "DISCOMName","DISCOMName"},
                { "DistrictName","DistrictName"},
                { "NameofBlock(aspercensus)","NameOfBlock"},
                { "Latitude","Latitude"},
                { "Longitude","Longitude"},
                { "CircleName","CircleName"},
                { "DivisionName","DivisionName"},
                { "DivisionID","DivisionId"},
                { "Sub-DivisionName","SubDivisionName"},
                { "Sub-DivisionID","SubDivisionId"},
                { "FeedingSub-stationName","FeedingSubStationName"},
                { "FeedingSub-stationID","FeedingSubStationId"},
                { "VoltageRatio(132/33KVOr33/11KV)","VoltageRatio"},
                { "CapacityinMVA","CapacityMVA"},
                { "Configuration(inNo#sXMVA)","Configuration"},
                { "FeederCode","Code"},
                { "FeederCategory(Rural/Urban/DedicatedIndustrial/Agricultural/M","Category"},
                { "FeederLength(km)","Length"},
                { "FeederVoltageLevel(KV)","VoltageLevel"},
                { "FeederConductorType/CableType","ConductorType"},
                { "OriginalCommissioningDate","OriginalCommissioningDate"},
                { "S/sIncomingFeederName","IncomingFeederName"},
                { "FeedermonitoredthroughRT-DAS(Yes/No)","MonitoredRTDAS"},
                { "Meteravailable(ifyes)","MeterAvailable"},
                { "MeterSerialNo#","MeterSerialNo"},
                { "MeterManufacturer'sName/Make","MeterManufacturersName"},
                { "MeterRating","MeterRating"},
                { "MeterType","MeterType"},
                { "MeterWorking(Yes/No)","MeterWorking"},
                { "MeterBoxavailable","MeterBoxAvailable"},
                { "MeterMultiplicationfactorprovidedbymetermanufacturer","MeterMultiplication"},
                { "DateandTimeofFeederMeterInstallation","MeterInstallationDate"},
                { "CapturingofCBstatusthroughNA/Meter/Modem","CbStatus"},
                { "line-PTarrangement(Yes/No)","LinePtArrangement"},
                { "CTPTavailable(ifYes)","CtptAvailable"},
                { "CTMake","CtMake"},
                { "CTRating","CtRating"},
                { "CTType","CtType"},
                { "CTConnectedratio","CtConnectedRatio"},
                { "PTMake","PtMake"},
                { "PTRating","PtRating"},
                { "PTType","PtType"},
                { "PTcorrectedratio","PtCorrectedRatio"},
                { "Sitephotoofmeter","CTPTMeterPhoto"},
                { "SitephotoofCT","CTMeterAvailable"},
                { "SitephotoofPT","PTMeterAvailable"},
                { "Nos#ofDTsconnected","DTConnected"},
                { "TotalTransformationCapacityofDTs(kVA)","TotalTransformationCapacityDT"},
                { "RuralConsumer","RuralConsumers"},
                { "UrbanConsumer","UrbanConsumers"},
                { "IndustrialConsumer","IndustrialConsumers"},
                { "AgricultureConsumer","AgriculturalConsumers"},
                { "PriorityConsumers(WaterSupply/Hospitaletc#)","PriorityConsumers"},
                { "NameoftheBlockbeingsuppliedbythefeeder","BlockName"},
                { "MobileNoJE","MobileNoJE"},
                { "MobileNoEE","MobileNoEE"},
                { "GPRSsignalstrength","GprsSignalStrength"},
                { "SIMType","SimType"},
                { "Remarks","Remarks"},
                { "CreatedBy", "CreatedBy" }
        };
        }
        private static IDictionary<string, string> TempDTSurveyTable()
        {
            return columns = new Dictionary<string, string>()
           {
    { "ConsumerId", "ConsumerId" },
    { "MeterAvailable", "MeterAvailable" },
    { "MeterManufacturer", "MeterManufacturer" },
    { "MeterRating", "MeterRating" },
    { "MeterType", "MeterType" },
    { "MeterWorking", "MeterWorking" },
    { "MeterBoxAvailable", "MeterBoxAvailable" },
    { "TTBAvailable", "TTBAvailable" },
    { "LTCTAvailable", "LTCTAvailable" },
    { "NoOfLTCTAvailable", "NoOfLTCTAvailable" },
    { "LTCTMake", "LTCTMake" },
    { "LTCTRating", "LTCTRating" },
    { "LTCTType", "LTCTType" },
    { "LTCTMeterFittedInSMC", "LTCTMeterFittedInSMC" },
    { "LTPowerCableAvailable", "LTPowerCableAvailable" },
    { "CableSize", "CableSize" },
    { "LTPowerCableType", "LTPowerCableType" },
    { "LTPowerCableRating", "LTPowerCableRating" },
    { "LTPowerCableTerminatingAt", "LTPowerCableTerminatingAt" },
    { "ExistingLTCableSuitable", "ExistingLTCableSuitable" },
    { "AluminiumCableLugsRequired", "AluminiumCableLugsRequired" },
    { "LugSize", "LugSize" },
    { "NoOfLugs", "NoOfLugs" },
    { "GPRSSignalStrength", "GPRSSignalStrength" },
    { "GPRSSignalStrengthJio", "GPRSSignalStrengthJio" },
    { "DTLocationName", "DTLocationName" },
    { "DTRatingKVA", "DTRatingKVA" },
    { "NamePlateVisibility", "NamePlateVisibility" },
    { "DTMountingAt", "DTMountingAt" },
    { "DTAccessibility", "DTAccessibility" },
    { "DTCode", "DTCode" },
    { "DTType", "DTType" },
    { "MeterSerialNo", "MeterSerialNo" },
    { "Remark", "Remark" },
    { "Latitude", "Latitude" },
    { "Longitude", "Longitude" },
    { "Surveyagencyname", "Surveyagencyname" },
    { "AdditionalRemark", "AdditionalRemark" },
    { "MeterSitePhoto", "MeterSitePhoto" },
    { "CTSitePhoto", "CTSitePhoto" },
    { "SurveyorName", "SurveyorName" },
    { "MeterPhoto", "MeterPhoto" },
    { "DateOfSurvey", "DateOfSurvey" },
    { "ZoneName", "ZoneName" },
    { "CircleName", "CircleName" },
    { "DivisionName", "DivisionName" },
    { "SubDivisionName", "SubDivisionName" },
    { "FeedingSubStationName", "FeedingSubStationName" },
    { "FeedingSubStationCode", "FeedingSubStationCode" },
    { "FeederName", "FeederName" },
    { "FeederCode", "FeederCode" },
    { "VillageName", "VillageName" },
    { "VillageCensusCode", "VillageCensusCode" },
    { "StreetColonyLocality", "StreetColonyLocality" },
    { "Block", "Block" },
    { "District", "District" },
    { "ZoneCode", "ZoneCode" },
    { "CircleCode", "CircleCode" },
    { "DivisionCode", "DivisionCode" },
    { "SubDivisionCode", "SubDivisionCode" },
    { "NewConsumerId", "NewConsumerId" }
};
        }
        private static IDictionary<string, string> TempConsumerSurveyTable()
        {
            return columns = new Dictionary<string, string>()
            {
                { "ConsumerAccountno", "ConsumerId" },
                { "NewConsumerId", "NewConsumerId" },
                { "Zone", "Zone" },
                { "Circle", "CircleWise" },
                { "Division", "Division" },
                { "Subdivision", "Subdivision" },
                { "33/11kvSubstationname", "Substation" },
                { "33/11kvSubstationcode", "SubstationCode" },
                { "11KVFeedername", "DTFeeder" },
                { "11KVFeedercode", "DTFeederCode" },
                { "11/0#440KVSubstation/DTRName", "DTName" },
                { "11/0#44KVSubstation/DTRCode", "DTCode" },
                { "ConsumerName", "ConsumerName" },
                { "ConsumerResponse", "ConsumerResponse" },
                { "Father/Husband/ProprietorNameetc", "ConsumerRelativeName" },
                { "ConsumerIDType", "IdentityType" },
                { "ConsumerIDCardNo", "AadharNo" },
                { "Consumermobilenumber", "ConsumerMobileNo" },
                { "Address", "ConsumerAddress" },
                { "CorrectConsumerAddress", "CorrectConsumerAddress" },
                { "DistrictName", "District" },
                { "TarrifName", "TarrifName" },
                { "Sanctionload(INKW)", "SanctionLoad" },
                { "MeteredConsumer", "MeterdConsumer" },
                { "Connectionstatus", "ConnectionStatus" },
                { "Metermake", "MeterMake" },
                { "OldMeterserialno", "MeterNo" },
                { "CorrectMeterserialno", "CorrectMeterNo" },
                { "Oldmeterboxstatus", "MeterBoxStatus" },
                { "OldMeterstatus", "MeterStatus" },
                { "OldMetersealstatus", "MeterSealStatus" },
                { "OldmeterType", "MeterType" },
                { "Meterlocation", "MeterLocation" },
                { "Oldmeterphoto", "MeterPhotoUrl" },
                { "OldMeterReadingKWH/KVAH/KW", "MeterReading" },
                { "GPRSSignalstrengthAirtel", "GPRSSignalstrengthAirtel" },
                { "GPRSSignalstrengthJIO", "GPRSSignalstrengthJIO" },
                { "Latitude", "Latitude" },
                { "Longitude", "Longitude" },
                { "OldMeterBoxsealNo#", "MeterBoxSealNo" },
                { "OldMeterTerminalSealNo#", "MeterTerminalSealNo" }
            };
        }
    }
}
