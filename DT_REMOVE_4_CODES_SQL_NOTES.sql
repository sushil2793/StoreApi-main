/* ============================================================================
   REMOVE 4 DT CODE COLUMNS (ZoneCode/CircleCode/DivisionCode/SubDivisionCode)
   ----------------------------------------------------------------------------
   REASON: Hierarchy masters (Zone/Circle/Division/Subdivision) me alag se code
   column EXISTS NAHI karta (sirf ID + Name). DT Excel import flow me ye 4
   code columns redundant the - user ne remove karne ka decision liya.
   NewConsumerId UNTOUCHED (rahega).

   REMOVES FROM:
     [1] dbo.TempDTSurvey    - DROP 4 code columns (guarded)
     [2] dbo.DTRecivedConsumer - DROP 4 code columns (guarded)
     [3] dbo.SYNCDTSURVEY proc - UPDATE + INSERT se 4 codes hatao
       (transaction/atomic behavior BILKUL same, explicit column lists
        NewConsumerId ke saath continue)
   C#/Excel side (repo me already updated, deploy ke saath):
     - ColumnNameByTable.TempDTSurveyTable() mapping se 4 entries removed
     - DTDataExcel.xlsx template se 4 columns removed (57 cols now)

   PROD RUN ORDER:
     1. Pehle purane SYNCDTSURVEY ka CREATE script SSMS se save (rollback)
     2. Ye file StoreDB par run
     3. Verify section chalao (expected output niche)
     4. Backend deploy: ColumnNameByTable.cs + template xlsx + ImportExcelData
        (* strip) - SAATH ME, warna bulk copy mapping mismatch hoga
   ========================================================================== */

USE [StoreDB];
GO

/* ---------------------------------------------------------------------------
   [1] TempDTSurvey - drop 4 code columns (idempotent)
   ------------------------------------------------------------------------ */
IF COL_LENGTH('dbo.TempDTSurvey', 'ZoneCode') IS NOT NULL
    EXEC ('ALTER TABLE dbo.TempDTSurvey DROP COLUMN ZoneCode');
IF COL_LENGTH('dbo.TempDTSurvey', 'CircleCode') IS NOT NULL
    EXEC ('ALTER TABLE dbo.TempDTSurvey DROP COLUMN CircleCode');
IF COL_LENGTH('dbo.TempDTSurvey', 'DivisionCode') IS NOT NULL
    EXEC ('ALTER TABLE dbo.TempDTSurvey DROP COLUMN DivisionCode');
IF COL_LENGTH('dbo.TempDTSurvey', 'SubDivisionCode') IS NOT NULL
    EXEC ('ALTER TABLE dbo.TempDTSurvey DROP COLUMN SubDivisionCode');
GO

/* ---------------------------------------------------------------------------
   [2] DTRecivedConsumer - drop 4 code columns (idempotent)
   ------------------------------------------------------------------------ */
IF COL_LENGTH('dbo.DTRecivedConsumer', 'ZoneCode') IS NOT NULL
    EXEC ('ALTER TABLE dbo.DTRecivedConsumer DROP COLUMN ZoneCode');
IF COL_LENGTH('dbo.DTRecivedConsumer', 'CircleCode') IS NOT NULL
    EXEC ('ALTER TABLE dbo.DTRecivedConsumer DROP COLUMN CircleCode');
IF COL_LENGTH('dbo.DTRecivedConsumer', 'DivisionCode') IS NOT NULL
    EXEC ('ALTER TABLE dbo.DTRecivedConsumer DROP COLUMN DivisionCode');
IF COL_LENGTH('dbo.DTRecivedConsumer', 'SubDivisionCode') IS NOT NULL
    EXEC ('ALTER TABLE dbo.DTRecivedConsumer DROP COLUMN SubDivisionCode');
GO

/* ---------------------------------------------------------------------------
   [3] SYNCDTSURVEY - 4 codes removed from UPDATE + INSERT
       (transaction + explicit column lists + NewConsumerId=NULL preserved)
   ------------------------------------------------------------------------ */
CREATE OR ALTER PROC [dbo].[SYNCDTSURVEY]
@USERID INT
AS
SET NOCOUNT ON
SET XACT_ABORT ON
BEGIN
BEGIN TRY
    BEGIN TRANSACTION;

    UPDATE DTRC SET
    DTRC.ConsumerId = ISNULL(TDTS.ConsumerId, DTRC.ConsumerId),
    DTRC.DateOfSurvey = ISNULL(TDTS.DateOfSurvey, DTRC.DateOfSurvey),
    DTRC.ZoneName = ISNULL(TDTS.ZoneName, DTRC.ZoneName),
    DTRC.CircleName = ISNULL(TDTS.CircleName, DTRC.CircleName),
    DTRC.DivisionName = ISNULL(TDTS.DivisionName, DTRC.DivisionName),
    DTRC.SubDivisionName = ISNULL(TDTS.SubDivisionName, DTRC.SubDivisionName),
    DTRC.FeedingSubStationName = ISNULL(TDTS.FeedingSubStationName, DTRC.FeedingSubStationName),
    DTRC.FeedingSubStationCode = ISNULL(TDTS.FeedingSubStationCode, DTRC.FeedingSubStationCode),
    DTRC.FeederName = ISNULL(TDTS.FeederName, DTRC.FeederName),
    DTRC.FeederCode = ISNULL(TDTS.FeederCode, DTRC.FeederCode),
    DTRC.VillageName = ISNULL(TDTS.VillageName, DTRC.VillageName),
    DTRC.VillageCensusCode = ISNULL(TDTS.VillageCensusCode, DTRC.VillageCensusCode),
    DTRC.StreetColonyLocality = ISNULL(TDTS.StreetColonyLocality, DTRC.StreetColonyLocality),
    DTRC.Block = ISNULL(TDTS.Block, DTRC.Block),
    DTRC.District = ISNULL(TDTS.District, DTRC.District)
    FROM DTRecivedConsumer DTRC
    INNER JOIN TempDTSurvey TDTS ON DTRC.ConsumerId=TDTS.ConsumerId

    UPDATE DTRC SET
    DTRC.ConsumerId = ISNULL(TDTS.ConsumerId, DTRC.ConsumerId),
    DTRC.DTLocationName = ISNULL(TDTS.DTLocationName, DTRC.DTLocationName),
    DTRC.DTRatingKVA = ISNULL(TDTS.DTRatingKVA, DTRC.DTRatingKVA),
    DTRC.NamePlateVisibility = ISNULL(TDTS.NamePlateVisibility, DTRC.NamePlateVisibility),
    DTRC.DTMountingAt = ISNULL(TDTS.DTMountingAt, DTRC.DTMountingAt),
    DTRC.DTAccessibility = ISNULL(TDTS.DTAccessibility, DTRC.DTAccessibility),
    DTRC.DTCode = ISNULL(TDTS.DTCode, DTRC.DTCode),
    DTRC.DTType = ISNULL(TDTS.DTType, DTRC.DTType),
    DTRC.MeterSerialNo = ISNULL(TDTS.MeterSerialNo, DTRC.MeterSerialNo),
    DTRC.Remark = ISNULL(TDTS.Remark, DTRC.Remark),
    DTRC.Latitude = ISNULL(TDTS.Latitude, DTRC.Latitude),
    DTRC.Longitude = ISNULL(TDTS.Longitude, DTRC.Longitude),
    DTRC.SurveyAgencyName = ISNULL(TDTS.SurveyAgencyName, DTRC.SurveyAgencyName),
    DTRC.AdditionalRemark = ISNULL(TDTS.AdditionalRemark, DTRC.AdditionalRemark),
    DTRC.MeterSitePhoto = ISNULL(TDTS.MeterSitePhoto, DTRC.MeterSitePhoto),
    DTRC.CTSitePhoto = ISNULL(TDTS.CTSitePhoto, DTRC.CTSitePhoto),
    DTRC.SurveyorName = ISNULL(TDTS.SurveyorName, DTRC.SurveyorName),
    DTRC.MeterPhoto = ISNULL(TDTS.MeterPhoto, DTRC.MeterPhoto)
    FROM DTRecivedSurvey DTRC
    INNER JOIN TempDTSurvey TDTS ON DTRC.ConsumerId=TDTS.ConsumerId

    UPDATE DTRC SET
    DTRC.ConsumerId = ISNULL(TDTS.ConsumerId, DTRC.ConsumerId),
    DTRC.MeterAvailable = ISNULL(TDTS.MeterAvailable, DTRC.MeterAvailable),
    DTRC.MeterManufacturer = ISNULL(TDTS.MeterManufacturer, DTRC.MeterManufacturer),
    DTRC.MeterRating = ISNULL(TDTS.MeterRating, DTRC.MeterRating),
    DTRC.MeterType = ISNULL(TDTS.MeterType, DTRC.MeterType),
    DTRC.MeterWorking = ISNULL(TDTS.MeterWorking, DTRC.MeterWorking),
    DTRC.MeterBoxAvailable = ISNULL(TDTS.MeterBoxAvailable, DTRC.MeterBoxAvailable),
    DTRC.TTBAvailable = ISNULL(TDTS.TTBAvailable, DTRC.TTBAvailable),
    DTRC.LTCTAvailable = ISNULL(TDTS.LTCTAvailable, DTRC.LTCTAvailable),
    DTRC.NoOfLTCTAvailable = ISNULL(TDTS.NoOfLTCTAvailable, DTRC.NoOfLTCTAvailable),
    DTRC.LTCTMake = ISNULL(TDTS.LTCTMake, DTRC.LTCTMake),
    DTRC.LTCTRating = ISNULL(TDTS.LTCTRating, DTRC.LTCTRating),
    DTRC.LTCTType = ISNULL(TDTS.LTCTType, DTRC.LTCTType),
    DTRC.LTCTMeterFittedInSMC = ISNULL(TDTS.LTCTMeterFittedInSMC, DTRC.LTCTMeterFittedInSMC),
    DTRC.LTPowerCableAvailable = ISNULL(TDTS.LTPowerCableAvailable, DTRC.LTPowerCableAvailable),
    DTRC.CableSize = ISNULL(TDTS.CableSize, DTRC.CableSize),
    DTRC.LTPowerCableType = ISNULL(TDTS.LTPowerCableType, DTRC.LTPowerCableType),
    DTRC.LTPowerCableRating = ISNULL(TDTS.LTPowerCableRating, DTRC.LTPowerCableRating),
    DTRC.LTPowerCableTerminatingAt = ISNULL(TDTS.LTPowerCableTerminatingAt, DTRC.LTPowerCableTerminatingAt),
    DTRC.ExistingLTCableSuitable = ISNULL(TDTS.ExistingLTCableSuitable, DTRC.ExistingLTCableSuitable),
    DTRC.AluminiumCableLugsRequired = ISNULL(TDTS.AluminiumCableLugsRequired, DTRC.AluminiumCableLugsRequired),
    DTRC.LugSize = ISNULL(TDTS.LugSize, DTRC.LugSize),
    DTRC.NoOfLugs = ISNULL(TDTS.NoOfLugs, DTRC.NoOfLugs),
    DTRC.GPRSSignalStrength = ISNULL(TDTS.GPRSSignalStrength, DTRC.GPRSSignalStrength),
    DTRC.GPRSSignalStrengthJio = ISNULL(TDTS.GPRSSignalStrengthJio, DTRC.GPRSSignalStrengthJio)
    FROM DTRecivedMeter DTRC
    INNER JOIN TempDTSurvey TDTS ON DTRC.ConsumerId=TDTS.ConsumerId

    DELETE TDTS
    FROM TempDTSurvey TDTS
    INNER JOIN DTRecivedSurvey DTRD ON DTRD.ConsumerId=TDTS.ConsumerId

    INSERT INTO DTRecivedSurvey (
        ConsumerId, DTLocationName, DTRatingKVA, NamePlateVisibility,
        DTMountingAt, DTAccessibility, DTCode, DTType, MeterSerialNo,
        Remark, Latitude, Longitude, Surveyagencyname, AdditionalRemark,
        MeterSitePhoto, CTSitePhoto, SurveyorName, MeterPhoto, NewConsumerId)
    SELECT
        ConsumerId, DTLocationName, DTRatingKVA, NamePlateVisibility,
        DTMountingAt, DTAccessibility, DTCode, DTType, MeterSerialNo,
        Remark, Latitude, Longitude, Surveyagencyname, AdditionalRemark,
        MeterSitePhoto, CTSitePhoto, SurveyorName, MeterPhoto, NULL
    FROM TempDTSurvey

    INSERT INTO DTRecivedMeter (
        ConsumerId, MeterAvailable, MeterManufacturer, MeterRating, MeterType,
        MeterWorking, MeterBoxAvailable, TTBAvailable, LTCTAvailable,
        NoOfLTCTAvailable, LTCTMake, LTCTRating, LTCTType,
        LTCTMeterFittedInSMC, LTPowerCableAvailable, CableSize,
        LTPowerCableType, LTPowerCableRating, LTPowerCableTerminatingAt,
        ExistingLTCableSuitable, AluminiumCableLugsRequired, LugSize,
        NoOfLugs, GPRSSignalStrength, GPRSSignalStrengthJio)
    SELECT
        ConsumerId, MeterAvailable, MeterManufacturer, MeterRating, MeterType,
        MeterWorking, MeterBoxAvailable, TTBAvailable, LTCTAvailable,
        NoOfLTCTAvailable, LTCTMake, LTCTRating, LTCTType,
        LTCTMeterFittedInSMC, LTPowerCableAvailable, CableSize,
        LTPowerCableType, LTPowerCableRating, LTPowerCableTerminatingAt,
        ExistingLTCableSuitable, AluminiumCableLugsRequired, LugSize,
        NoOfLugs, GPRSSignalStrength, GPRSSignalStrengthJio
    FROM TempDTSurvey

    INSERT INTO DTRecivedConsumer (
        ConsumerId, DateOfSurvey, ZoneName, CircleName, DivisionName,
        SubDivisionName, FeedingSubStationName, FeedingSubStationCode,
        FeederName, FeederCode, VillageName, VillageCensusCode,
        StreetColonyLocality, Block, District)
    SELECT
        ConsumerId, DateOfSurvey, ZoneName, CircleName, DivisionName,
        SubDivisionName, FeedingSubStationName, FeedingSubStationCode,
        FeederName, FeederCode, VillageName, VillageCensusCode,
        StreetColonyLocality, Block, District
    FROM TempDTSurvey

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    THROW;
END CATCH
END
GO

/* ---------------------------------------------------------------------------
   [4] VERIFY - expected output comments me
   ------------------------------------------------------------------------ */

-- 4a. TempDTSurvey: 4 code columns GONE, NewConsumerId present
-- Expected: 0 rows for codes, 1 row for NewConsumerId
SELECT name FROM sys.columns
WHERE object_id = OBJECT_ID('dbo.TempDTSurvey')
  AND name IN ('ZoneCode','CircleCode','DivisionCode','SubDivisionCode','NewConsumerId');

-- 4b. DTRecivedConsumer: 15 columns (19 - 4 codes)
-- Expected: Consumer_ColCount = 15
SELECT COUNT(*) AS Consumer_ColCount
FROM sys.columns WHERE object_id = OBJECT_ID('dbo.DTRecivedConsumer');

-- 4c. TempDTSurvey: 57 columns (61 - 4)
-- Expected: TempDT_ColCount = 57
SELECT COUNT(*) AS TempDT_ColCount
FROM sys.columns WHERE object_id = OBJECT_ID('dbo.TempDTSurvey');

-- 4d. Proc updated (codes removed)
-- Expected: HasCodes = 0, HasTransaction = 1
SELECT CASE WHEN OBJECT_DEFINITION(OBJECT_ID('dbo.SYNCDTSURVEY','P')) LIKE '%DTRC.ZoneCode%' THEN 1 ELSE 0 END AS HasCodes,
       CASE WHEN OBJECT_DEFINITION(OBJECT_ID('dbo.SYNCDTSURVEY','P')) LIKE '%XACT_ABORT%' THEN 1 ELSE 0 END AS HasTransaction;
