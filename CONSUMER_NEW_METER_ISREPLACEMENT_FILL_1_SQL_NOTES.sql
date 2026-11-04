/* ==========================================================================================
   CONSUMER NEW METER - IsReplacementMeter FILL = '1'  (Book2.xlsx 101 meters)
   PRODUCTION SCRIPT - user khud prod par run karega

   KYA KARTA HAI:
   Book2.xlsx ke 101 unique new-smart-meter numbers ki rows me ConsumerNewMeterDetails.IsReplacementMeter
   = '1' bharta hai - SIRF jahan abhi NULL hai.
   - JIS ROW ME PEHLE SE VALUE HAI (1/3/4/kuch bhi) - USKO BILKUL NAHI CHHUTTA (no overwrite)
   - JIS ROW ME NULL HAI - usme '1' set hota hai

   Kaise chalaye (SSMS):
   1. StoreDB select karke run (USE [StoreDB] me hai - prod DB naam alag ho to change kar lena)
   2. Sequence: [0] column check -> [1] meter load -> [2] BEFORE -> [3] UPDATE -> [4] AFTER -> [5] detail
   3. ROLLBACK chahiye to APPENDIX-R

   EXPECTED (local DB basis - prod me actual counts alag ho sakte hai, before/after zaroor check):
   - Unique meters: 101
   - Pre-existing value: 1 row (meter '1001054' = already '1') -> UNTOUCHED rahegi
   - NULL rows: ~100 -> '1' banenge
   ========================================================================================== */

USE [StoreDB];
SET NOCOUNT ON;

/* [0] Pre-check: column exist karta hai?
   Agar ye ERROR de = column hi nahi hai -> script mat chalana, pehle batao. */
SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH AS Len
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'ConsumerNewMeterDetails' AND COLUMN_NAME = 'IsReplacementMeter';

/* [1] Book2 ke 101 meters temp table me (list EK hi jagah - aage sab usi se) */
IF OBJECT_ID('tempdb..#Book2Meters') IS NOT NULL DROP TABLE #Book2Meters;
CREATE TABLE #Book2Meters (NewSmartMeterSrNo varchar(50) PRIMARY KEY);
INSERT INTO #Book2Meters (NewSmartMeterSrNo) VALUES
    ('1001054'),('MH1002084'),('MH1008473'),('MH1008474'),('MH1008475'),('MH1008476'),('MH1008477'),('MH1008478'),
    ('MH1008479'),('MH1008480'),('MH1008750'),('MH1009748'),('MH1009765'),('MH1012073'),('MH1012164'),('MH1012166'),
    ('MH1012415'),('MH1012699'),('MH1013460'),('MH1013785'),('MH1013983'),('MH1014050'),('MH1014052'),('MH1014931'),
    ('MZ1002682'),('MZ1008136'),('MZ1008738'),('MZ1008744'),('MZ1008750'),('MZ1010530'),('MZ1011424'),('MZ1011541'),
    ('MZ1011674'),('MZ1011753'),('MZ1012230'),('MZ1016009'),('MZ1017570'),('MZ1017806'),('MZ1017991'),('MZ1017992'),
    ('MZ1017994'),('MZ1017998'),('MZ1018165'),('MZ1019536'),('MZ1022237'),('MZ1022460'),('MZ2000743'),('MZ2001253'),
    ('MZ2003474'),('MZ2004249'),('MZ2004744'),('MZ2006574'),('MZ2010592'),('MZ2011933'),('MZ2012391'),('MZ2013315'),
    ('MZ2013420'),('MZ2014036'),('MZ2014037'),('MZ2014038'),('MZ2016313'),('MZ2017210'),('MZ2017453'),('MZ2017454'),
    ('MZ2017455'),('MZ2017513'),('MZ2017514'),('MZ2017515'),('MZ2017619'),('MZ2017772'),('MZ2017773'),('MZ2019113'),
    ('MZ2019240'),('MZ2020111'),('MZ2020113'),('MZ2020117'),('MZ2020168'),('MZ2020318'),('MZ2020518'),('MZ2020522'),
    ('MZ2020524'),('MZ2020542'),('MZ2021364'),('MZ2022057'),('MZ2022606'),('MZ2022610'),('MZ2022763'),('MZ2022765'),
    ('MZ2022772'),('MZ2023107'),('MZ2023144'),('MZ3000226'),('MZ3000997'),('MZ3003304'),('MZ3004099'),('MZ3005788'),
    ('MZ3005969'),('MZ3006317'),('MZ3007556'),('MZ3008636'),('MZ3008869');

SELECT 'METERS_LOADED' AS CheckName, COUNT(*) AS Cnt FROM #Book2Meters;  -- expected 101

/* [2] BEFORE - in 101 meters ka current IsReplacementMeter distribution
   Expected local: '(NULL)'=100, '1'=1 (1001054) */
SELECT 'BEFORE' AS Stage,
       ISNULL(c.IsReplacementMeter, '(NULL)') AS Code,
       COUNT(*) AS Cnt
FROM ConsumerNewMeterDetails c
JOIN #Book2Meters m ON m.NewSmartMeterSrNo = c.NewSmartMeterSrNo
GROUP BY c.IsReplacementMeter
ORDER BY Code;

-- [2b] kitne meters DB me mile (missing list - file me ho par row na ho)
SELECT 'DB_MATCH' AS CheckName, COUNT(DISTINCT c.NewSmartMeterSrNo) AS MetersFound,
       (SELECT COUNT(*) FROM #Book2Meters) - COUNT(DISTINCT c.NewSmartMeterSrNo) AS MetersMissing
FROM ConsumerNewMeterDetails c
JOIN #Book2Meters m ON m.NewSmartMeterSrNo = c.NewSmartMeterSrNo;

/* [3] UPDATE - SIRF NULL wali rows me '1'
   Guard: AND IsReplacementMeter IS NULL -> jisme pehle se value hai SKIP (1001054 chhoot jayegi)
   Expected affected: ~100 */
UPDATE c
SET IsReplacementMeter = '1'
FROM ConsumerNewMeterDetails c
JOIN #Book2Meters m ON m.NewSmartMeterSrNo = c.NewSmartMeterSrNo
WHERE c.IsReplacementMeter IS NULL;
-- Messages pane me "N rows affected" dekhna (expected ~100)

/* [4] AFTER - verify
   Expected: '1' = 101, '(NULL)' = 0 */
SELECT 'AFTER' AS Stage,
       ISNULL(c.IsReplacementMeter, '(NULL)') AS Code,
       COUNT(*) AS Cnt
FROM ConsumerNewMeterDetails c
JOIN #Book2Meters m ON m.NewSmartMeterSrNo = c.NewSmartMeterSrNo
GROUP BY c.IsReplacementMeter
ORDER BY Code;

/* [5] Detail: jin row me pehle se koi AUR value thi (1 ke alawa) - wo UNTOUCHED rahi
   Expected: meter '1001054' (jo pehle se '1' tha). Agar prod pe 3/4/YES wali row mili to yahi dikhegi. */
SELECT c.NewSmartMeterSrNo, c.ConsumerId, c.IsReplacementMeter, c.DateOfInstallation
FROM ConsumerNewMeterDetails c
JOIN #Book2Meters m ON m.NewSmartMeterSrNo = c.NewSmartMeterSrNo
WHERE c.IsReplacementMeter <> '1'
ORDER BY c.IsReplacementMeter, c.NewSmartMeterSrNo;

DROP TABLE #Book2Meters;

/* ==========================================================================================
   [APPENDIX-R] ROLLBACK - is script ke sirf NULL->'1' set kiye gaye rows wapas NULL
   NOTE: ye sirf tab safe hai jab aapne STEP-[3] ke baad aur kisi ne value na badli ho.
   Sabse pehle STEP-[5] ki tarah "pehle se alag value wali rows" ki list nikal lo, phir:

   -- 1) pehle ki non-'1' rows list banao (backup)
   -- 2) sirf UpdatedAt >= aaj ka UPDATE_TIMESTAMP wale '1' rows ko NULL karo
   DECLARE @since datetime = '2026-11-01 00:00:00';  -- <-- script run ka time yahan daalo
   UPDATE c SET IsReplacementMeter = NULL
   FROM ConsumerNewMeterDetails c
   JOIN #Book2Meters m ON m.NewSmartMeterSrNo = c.NewSmartMeterSrNo
   WHERE c.IsReplacementMeter = '1' AND c.UpdatedAt >= @since
     AND c.ConsumerId NOT IN ( ...jo rows pehle se '1'/3/4 thi unki list... );
   ========================================================================================== */
