/* ============================================================================
   DT EXCEL IMPORT FIX - TempDTSurvey missing columns + SYNCDTSURVEY proc
   ----------------------------------------------------------------------------
   ISSUE 1: "Import DT Data" upload fails in Hangfire job with
            System.InvalidOperationException: The given ColumnMapping does not
            match up with any column in the source or destination.
            Cause: dbo.TempDTSurvey has only 56 columns. C# mapping
            (ColumnNameByTable.TempDTSurveyTable) + sample Excel template both
            carry 61 columns - missing from table:
              ZoneCode, CircleCode, DivisionCode, SubDivisionCode, NewConsumerId

   ISSUE 2: proc dbo.SYNCDTSURVEY fails with
            Msg 213: Column name or number of supplied values does not match
            table definition.
            Cause: INSERT INTO DTRecivedConsumer supplies 15 values but the
            table has 19 columns (ZoneCode/CircleCode/DivisionCode/
            SubDivisionCode added 13-Jun-2026). Fails even with 0 rows.
            No transaction in proc -> partial commits would leave orphan rows
            in DTRecivedSurvey/DTRecivedMeter (invisible in GETALLDTDATA due
            to INNER JOIN) when a later statement fails.

   FIX IN THIS SCRIPT (idempotent, safe to re-run):
     [1] ALTER dbo.TempDTSurvey ADD the 5 missing columns (guarded).
     [2] CREATE OR ALTER dbo.SYNCDTSURVEY:
           - SET XACT_ABORT ON + BEGIN TRY / TRAN / COMMIT / CATCH / ROLLBACK
             / THROW (atomic: all-or-nothing, error visible to Hangfire).
           - first UPDATE (DTRecivedConsumer) now also carries the 4 codes
             with existing ISNULL(...) pattern.
           - all 3 INSERTs use explicit column lists (prevents this class of
             schema-drift bug in future).
           - DTRecivedSurvey.NewConsumerId keeps current behavior: NULL
             (do NOT carry Excel NewConsumerId - unchanged by design).
     [3] Verify queries + expected output.

   NO CODE/UI CHANGES NEEDED: C# mapping (61) and Excel template (61) already
   match the fixed table (61). No rebuild required - SQL only.

   PROD RUN ORDER:
     1. SSMS se purane SYNCDTSURVEY ka "Script Stored Procedure as CREATE"
        lekar save kar lo (rollback ke liye).
     2. Ye file StoreDB par run karo (USE [StoreDB] already in file - DB name
        verify kar lena).
     3. Verify section chalao - expected output niche likhi hai.
     4. App restart ki zaroorat NAHI. Deploy/stop ki zaroorat NAHI.

   ROLLBACK (agar chahiye):
     1. Step 1 me script kiya hua purana CREATE proc wapas execute karo.
     2. ALTER TABLE dbo.TempDTSurvey DROP COLUMN ZoneCode, CircleCode,
        DivisionCode, SubDivisionCode, NewConsumerId;
        (sirf tab jab tak koi naya data in columns me na aaya ho)
   ========================================================================== */

USE [StoreDB];
GO

/* ---------------------------------------------------------------------------
   [1] TempDTSurvey: add 5 missing columns (idempotent, dynamic SQL guard)
   Target types match destination tables:
     ZoneCode/CircleCode/DivisionCode/SubDivisionCode -> nvarchar(100)
       (DTRecivedConsumer codes are nvarchar(100); varchar->nvarchar implicit
        conversion is safe)
     NewConsumerId -> varchar(255) (TempDTSurvey convention = varchar(255);
       proc never reads this temp value - it inserts NULL into
       DTRecivedSurvey - so this column only has to accept the Excel value
       at bulk-copy time)
   ------------------------------------------------------------------------ */
IF COL_LENGTH('dbo.TempDTSurvey', 'ZoneCode') IS NULL
    EXEC ('ALTER TABLE dbo.TempDTSurvey ADD ZoneCode nvarchar(100) NULL');
IF COL_LENGTH('dbo.TempDTSurvey', 'CircleCode') IS NULL
    EXEC ('ALTER TABLE dbo.TempDTSurvey ADD CircleCode nvarchar(100) NULL');
IF COL_LENGTH('dbo.TempDTSurvey', 'DivisionCode') IS NULL
    EXEC ('ALTER TABLE dbo.TempDTSurvey ADD DivisionCode nvarchar(100) NULL');
IF COL_LENGTH('dbo.TempDTSurvey', 'SubDivisionCode') IS NULL
    EXEC ('ALTER TABLE dbo.TempDTSurvey ADD SubDivisionCode nvarchar(100) NULL');
IF COL_LENGTH('dbo.TempDTSurvey', 'NewConsumerId') IS NULL
    EXEC ('ALTER TABLE dbo.TempDTSurvey ADD NewConsumerId varchar(255) NULL');
GO

/* ---------------------------------------------------------------------------
   [2] SYNCDTSURVEY - fixed proc
   Changes vs old version (statement-by-statement same order, same joins):
     - SET XACT_ABORT ON + TRY/CATCH + single transaction (atomic sync).
     - UPDATE DTRecivedConsumer: + 4 code columns (ISNULL carry pattern).
     - INSERT DTRecivedSurvey: explicit 19-col list, NewConsumerId = NULL
       (unchanged behavior).
     - INSERT DTRecivedMeter: explicit 25-col list (unchanged values).
     - INSERT DTRecivedConsumer: explicit 19-col list, + 4 codes from
       TempDTSurvey (this was the Msg 213 crash).
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
    DTRC.District = ISNULL(TDTS.District, DTRC.District),
    DTRC.ZoneCode = ISNULL(TDTS.ZoneCode, DTRC.ZoneCode),
    DTRC.CircleCode = ISNULL(TDTS.CircleCode, DTRC.CircleCode),
    DTRC.DivisionCode = ISNULL(TDTS.DivisionCode, DTRC.DivisionCode),
    DTRC.SubDivisionCode = ISNULL(TDTS.SubDivisionCode, DTRC.SubDivisionCode)
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
        ConsumerId,
        DTLocationName,
        DTRatingKVA,
        NamePlateVisibility,
        DTMountingAt,
        DTAccessibility,
        DTCode,
        DTType,
        MeterSerialNo,
        Remark,
        Latitude,
        Longitude,
        Surveyagencyname,
        AdditionalRemark,
        MeterSitePhoto,
        CTSitePhoto,
        SurveyorName,
        MeterPhoto,
        NULL
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
        ConsumerId,
        MeterAvailable,
        MeterManufacturer,
        MeterRating,
        MeterType,
        MeterWorking,
        MeterBoxAvailable,
        TTBAvailable,
        LTCTAvailable,
        NoOfLTCTAvailable,
        LTCTMake,
        LTCTRating,
        LTCTType,
        LTCTMeterFittedInSMC,
        LTPowerCableAvailable,
        CableSize,
        LTPowerCableType,
        LTPowerCableRating,
        LTPowerCableTerminatingAt,
        ExistingLTCableSuitable,
        AluminiumCableLugsRequired,
        LugSize,
        NoOfLugs,
        GPRSSignalStrength,
        GPRSSignalStrengthJio
    FROM TempDTSurvey

    INSERT INTO DTRecivedConsumer (
        ConsumerId, DateOfSurvey, ZoneName, CircleName, DivisionName,
        SubDivisionName, FeedingSubStationName, FeedingSubStationCode,
        FeederName, FeederCode, VillageName, VillageCensusCode,
        StreetColonyLocality, Block, District,
        ZoneCode, CircleCode, DivisionCode, SubDivisionCode)
    SELECT
        ConsumerId,
        DateOfSurvey,
        ZoneName,
        CircleName,
        DivisionName,
        SubDivisionName,
        FeedingSubStationName,
        FeedingSubStationCode,
        FeederName,
        FeederCode,
        VillageName,
        VillageCensusCode,
        StreetColonyLocality,
        Block,
        District,
        ZoneCode,
        CircleCode,
        DivisionCode,
        SubDivisionCode
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
   [3] VERIFY - run these after the script. Expected output in comments.
   ------------------------------------------------------------------------ */

-- 3a. TempDTSurvey must now have 61 columns (56 old + 5 new)
-- Expected: TempDT_ColCount = 61
SELECT COUNT(*) AS TempDT_ColCount
FROM sys.columns
WHERE object_id = OBJECT_ID('dbo.TempDTSurvey');

-- 3b. All 5 new columns exist with expected types
-- Expected: 5 rows, bytes: ZoneCode/CircleCode/DivisionCode/SubDivisionCode = 200,
--           NewConsumerId = 255
SELECT name,
       TYPE_NAME(system_type_id) AS type_name,
       max_length AS bytes
FROM sys.columns
WHERE object_id = OBJECT_ID('dbo.TempDTSurvey')
  AND name IN ('ZoneCode','CircleCode','DivisionCode','SubDivisionCode','NewConsumerId')
ORDER BY name;

-- 3c. DTRecivedConsumer untouched (should still be 19 columns - defensive
--     column adds NOT needed, they already exist since 13-Jun-2026)
-- Expected: Consumer_ColCount = 19
SELECT COUNT(*) AS Consumer_ColCount
FROM sys.columns
WHERE object_id = OBJECT_ID('dbo.DTRecivedConsumer');

-- 3d. New proc deployed (definition must mention XACT_ABORT + ZoneCode)
-- Expected: ProcExists = 1, HasTransaction = 1, HasCodes = 1
SELECT CASE WHEN OBJECT_ID('dbo.SYNCDTSURVEY','P') IS NOT NULL THEN 1 ELSE 0 END AS ProcExists,
       CASE WHEN OBJECT_DEFINITION(OBJECT_ID('dbo.SYNCDTSURVEY','P')) LIKE '%XACT_ABORT%' THEN 1 ELSE 0 END AS HasTransaction,
       CASE WHEN OBJECT_DEFINITION(OBJECT_ID('dbo.SYNCDTSURVEY','P')) LIKE '%DTRC.ZoneCode%' THEN 1 ELSE 0 END AS HasCodes;

-- 3e. Orphan report (report-only, DO NOT auto-delete):
--     DTRecivedSurvey rows whose ConsumerId is missing in DTRecivedConsumer.
--     Post-fix expected: 0 new orphans. On prod, historical orphans from the
--     broken period (13-Jun-2026 onward, partial commits) may show here -
--     business decision, not auto-fixed by this script.
SELECT COUNT(*) AS OrphanSurveyRows
FROM DTRecivedSurvey s
WHERE NOT EXISTS (SELECT 1 FROM DTRecivedConsumer c WHERE c.ConsumerId = s.ConsumerId);

-- 3f. Sanity: procs/tables unaffected by the ALTER (should return 0 rows)
-- Expected: no rows (no errors)
SELECT OBJECT_NAME(m.object_id) AS obj, o.name
FROM sys.sql_modules m
JOIN sys.objects o ON o.object_id = m.object_id
WHERE m.definition LIKE '%TempDTSurvey%'
  AND o.name NOT IN ('SYNCDTSURVEY','TRUNCATE_TEMP_TABLE');
