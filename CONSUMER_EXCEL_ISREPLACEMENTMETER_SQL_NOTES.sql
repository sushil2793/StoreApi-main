-- ============================================================
-- CONSUMER NEW METER EXCEL UPLOAD: IsReplacementMeter COLUMN
-- Date: 23-Oct-2026
-- Purpose: ConsumerNewMeterDataExcel sheet me user ab ek naya
--          column "IsReplacementMeter" dega. Upload pe wo value
--          TempConsumerNewMeter -> ConsumerNewMeterDetails tak
--          jaye (SYNCCONSUMERNEWMETER proc ke through).
--
-- FLOW (kaise kaam karta hai):
--   UI Excel upload -> ImportConsumerNewMeterDataByExcel (API)
--   -> Hangfire ImportExcelData -> SqlBulkCopy -> dbo.TempConsumerNewMeter
--   -> SYNCCONSUMERNEWMETER proc -> ConsumerNewMeterDetails (main table)
--   -> ProductionMeter insert (unchanged) -> temp TRUNCATE
--
-- NOTE:
--   - ConsumerNewMeterDetails.IsReplacementMeter VARCHAR(255) PEHLE SE HAI
--     (koi change nahi chahiye wahan).
--   - Ye script IDEMPOTENT hai - re-run safe.
--   - Excel me column na ho to bhi upload chalega (NULL jayega) -
--     C# side BulkCopy optional-column guard ke through.
-- ============================================================

USE [StoreDB];
GO

-- [1] Temp table me column add (agar nahi hai)
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.TempConsumerNewMeter')
      AND name = 'IsReplacementMeter'
)
BEGIN
    ALTER TABLE dbo.TempConsumerNewMeter
    ADD IsReplacementMeter VARCHAR(255) NULL;
    PRINT 'IsReplacementMeter column added to TempConsumerNewMeter';
END
ELSE
    PRINT 'IsReplacementMeter already exists on TempConsumerNewMeter - skip';
GO

-- [2] SYNCCONSUMERNEWMETER proc update:
--     INSERT me IsReplacementMeter add kiya (STEP 2), baaki sab unchanged:
--       - STEP 1: existing-consumer DELETE (unchanged)
--       - STEP 2: INSERT ConsumerNewMeterDetails (+ IsReplacementMeter)
--       - STEP 3: INSERT ProductionMeter (unchanged)
--       - STEP 4: TRUNCATE temp (unchanged)
CREATE OR ALTER PROCEDURE [dbo].[SYNCCONSUMERNEWMETER]
    @USERID INT
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

    ---------------------------------------------------
    -- STEP 1: Remove already existing records safely
    ---------------------------------------------------
    DELETE TCNM
    FROM TempConsumerNewMeter TCNM
    INNER JOIN ConsumerNewMeterDetails CNMD
        ON ISNULL(TCNM.ConsumerId, 0) = ISNULL(CNMD.ConsumerId, 0)
        OR ISNULL(TCNM.NewConsumerId, 0) = ISNULL(CNMD.NewConsumerId, 0);

    ---------------------------------------------------
    -- STEP 2: Insert ONLY VALID records (FIXED DATE)
    ---------------------------------------------------
    INSERT INTO ConsumerNewMeterDetails (
        ConsumerId, NewSmartMeterSrNo, BookNoOrSlipNo, DateOfInstallation,
        OldMeterReadingKwh, OldMeterReadingKvah, BalanceOfPrepaidMeter,
        ExistingReturnMeter, InitialReadingKwh, InitialReadingKvah,
        NewMeterPhoto, MeterPhotoWithConsumer, UserID, NewConsumerId,
        IsReplacementMeter
    )
    SELECT
        ConsumerId,
        NewSmartMeterSrNo,
        BookNoOrSlipNo,
        TRY_CONVERT(DATETIME, DateOfInstallation, 103) AS DateOfInstallation,
        OldMeterReadingKwh,
        OldMeterReadingKvah,
        BalanceOfPrepaidMeter,
        ExistingReturnMeter,
        InitialReadingKwh,
        InitialReadingKvah,
        NewMeterPhoto,
        MeterPhotoWithConsumer,
        @USERID,
        NewConsumerId,
        IsReplacementMeter
    FROM TempConsumerNewMeter
    WHERE
        ConsumerId IS NOT NULL
        AND NewSmartMeterSrNo IS NOT NULL
        AND LTRIM(RTRIM(NewSmartMeterSrNo)) <> ''
        AND TRY_CONVERT(DATETIME, DateOfInstallation, 103) IS NOT NULL;

    ---------------------------------------------------
    -- STEP 3: Insert into ProductionMeter safely
    ---------------------------------------------------
    INSERT INTO ProductionMeter (
        MeterNo, MeterType, MeterBoxSealNo, ICCID, SIMNW, CreatedBy
    )
    SELECT
        t.NewSmartMeterSrNo, t.MeterType, t.MeterBoxSealNo, t.ICCID, t.SIMNW, @USERID
    FROM TempConsumerNewMeter t
    WHERE
        t.NewSmartMeterSrNo IS NOT NULL
        AND LTRIM(RTRIM(t.NewSmartMeterSrNo)) <> ''
        AND NOT EXISTS (
            SELECT 1
            FROM ProductionMeter pm
            WHERE pm.MeterNo = t.NewSmartMeterSrNo
        );

    ---------------------------------------------------
    -- STEP 4: Cleanup
    ---------------------------------------------------
    TRUNCATE TABLE TempConsumerNewMeter;

END;
GO

-- [3] VERIFY (expected: TempHasColumn=1, ProcHasIsRep=1, FinalHasColumn=1)
SELECT 'TempHasColumn' CheckName,
       COUNT(*) CheckValue
FROM sys.columns
WHERE object_id = OBJECT_ID('dbo.TempConsumerNewMeter') AND name = 'IsReplacementMeter'
UNION ALL
SELECT 'ProcHasIsRep',
       CASE WHEN OBJECT_DEFINITION(OBJECT_ID('dbo.SYNCCONSUMERNEWMETER')) LIKE '%IsReplacementMeter%'
            THEN 1 ELSE 0 END
UNION ALL
SELECT 'FinalHasColumn',
       COUNT(*)
FROM sys.columns
WHERE object_id = OBJECT_ID('dbo.ConsumerNewMeterDetails') AND name = 'IsReplacementMeter';
