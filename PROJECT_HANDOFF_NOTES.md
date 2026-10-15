# REPLACEMENT METER - PROJECT STATUS (Handoff Notes)
# Last updated: 10-Oct-2026 - AUDIT ACCESS: PERMISSIONS 3 -> 2 (View | Edit & Delete), CanDelete REMOVED - DB/API/UI + LOCAL TEST + PROD DEPLOY (DB+API DONE, UI DIST COPY BAKI)
# >>> 03-Oct: Consumer push (UpdateHesStatus CONSUMER) ab sync 1-by-1 hota hai aur response me poora push log (payload + HES response + status) aata hai. UI ME KOI CHANGE NAHI. Backend 4 files, build 0 errors, DEPLOY PENDING (neeche Session 03-Oct). <<<
# >>> 10-Oct: Audit access permissions 3 se 2 hui - ab sirf View aur combined "Edit & Delete" hai. CanDelete DB column/API param/UI checkbox SAB JAGAH HATA DIYA (user ne Option B chuna). Migration script USER_AUDIT_ACCESS_DROP_CANDELETE_SQL_NOTES.sql (repo root) - local+prod DB run ho gaya, backend 5 files deploy ho gaya (DONETAPI publish). PROD UI dist copy BAKI (neeche Session 10-Oct). <<<
# >>> 30-Sep: 3 naye Satnam admin users production pe deploy ho gaye (user ne). Admin full-scope ka SQL fix READY - production pe RUN BAKI hai. Production UI bundle bhi PURANA hai - dist copy BAKI. EDIT BUTTON FIX = Users.Access=1 UPDATE (production pe RUN BAKI) - alag script file user ne DELETE kar di, fix ab USER_MANAGE_TEMPLATE_SQL_NOTES.sql ke sath run hoga (neeche dekho). <<<

## ============================================================ SESSION 10-Oct: AUDIT ACCESS 2-PERMISSION (View | Edit & Delete) - CanDelete REMOVED - DB+API+UI, PROD DB/API DEPLOYED, PROD UI DIST COPY BAKI ============================================================

USER REQUIREMENT: Audit access permission model 3 permissions (View / Edit / Delete) se 2 me
convert karo - sirf View aur combined "Edit & Delete" (ek checkbox dono kaam kare). TABLE, UI
and API TINO jagah sirf 2 options. User ne OPTION B chuna: CanDelete column DB se PURI TARAH
hatao (null/migrate nahi karna). Iska canAccess (existing Users audit flag) wale feature se koi
lena dena nahi - wo alag cheez hai.

### MIGRATION SQL: USER_AUDIT_ACCESS_DROP_CANDELETE_SQL_NOTES.sql (repo root, final version 3x local tested)
- [1] Data safety-fix: CanDelete=1 & CanEdit=0 wale rows -> CanEdit=1 (warna drop ke baad CanDelete
  access ka data kho jata). IMPORTANT FIX: initial script me ye UPDATE re-run pe
  'Msg 207 Invalid column name CanDelete' deta tha (harmless 0-rows but ugly) - IF/ELSE guard se
  bhi fix NAHI hua (SQL Server dono branches compile-time parse karta hai) => FINAL FIX:
  sp_executesql DYNAMIC SQL me column-exist guard. Script idempotent hai - jitni baar marzi re-run.
- [2] Constraint fix: DF_UAA_CanDelete default constraint DROP COLUMN block karta tha (Msg 5074) =>
  script ab sys.default_constraints se dynamically drop karta hai, phir ALTER TABLE DROP COLUMN.
- [3] 4 proc rewrites (CREATE OR ALTER, clean 2-param versions, purane full-scope
  USER_MANAGE_TEMPLATE_SQL_NOTES.sql ke tarah pattern):
  * MANAGE_USER: @CanDelete param nahi
  * GET_USER_AUDIT_ACCESS_LIST: canView/canEdit two columns, canDelete=0 matches
  * SAVE_USER_AUDIT_ACCESS (targetUserId, canView, canEdit, updatedBy)
  * GET_USER_AUDIT_ACCESS (targetUserId) -> canView/canEdit
- [4] Built-in verify query at end (expected: CanDeleteCol=0, SaveCanDeleteParam=0, ManageAuditParams=2, AllProcsExist=4)
- USER ka SSMS screenshot (local + prod dono): 0/0/2/4 + 'No issues found' + 'CanDelete column dropped + procs clean - deployment OK'

### BACKEND (5 files, dotnet build 0 errors, 29 warnings sab pre-existing CS0436 - is session se related nahi)
- SmartMeterAdmin.ViewModel/UserViewModel/UserViewModel.cs: CanDelete property removed
- SmartMeterAdmin.ViewModel/AuditLog/UserActivityLogViewModel.cs: 3 classes se CanDelete removed
  (UserAuditAccessViewModel, UserAuditAccessListViewModel, SaveUserAuditAccessRequest)
- SmartMeterAdmin.Services/User/UserService.cs: ManageUser me @CanDelete param removed;
  SaveUserAuditAccess signature ab (targetUserId, canView, canEdit, updatedBy)
- SmartMeterAdmin.Services/User/IUser.cs: interface updated
- SmartMeterAdmin/Controllers/UserController.cs: SaveUserAuditAccess call 4-arg;
  IsAccessManager() guard (RoleName=='Superadmin' AND UserID==3) existing hai;
  AuditController.ClearCanViewCache() call existing hai
- SmartMeterAdmin/Controllers/AuditController.cs: UNCHANGED (sirf CanView use karta hai, verified)

### UI (ANGULAR REPO = D:/UI/StoreUI - NOTE: purane notes me 'D:/WFM software/StoreUI/src/src' likha tha, AB WAHI REPO NAHI HAI; Angular root repo me package.json hai)
- IMPORTANT: is repo me audit-access feature KAHIN THA HI NAHI (purana repo) - feature FRESH ADD hua
  is session me (user ka screenshot confirm: "view and edit & Delete ka check box nahi hai"
  pehle). UI Angular 11.2 + Node 22 => ng serve pe `export NODE_OPTIONS=--openssl-legacy-provider` chahiye.
- 4 files changed:
  * src/app/Model/user.ts: +canView?/canEdit? (koi canDelete nahi)
  * src/app/Service/admin-user.service.ts: +getUserAuditAccessList(roleType, search),
    +saveUserAuditAccess(userId, canView, canEdit)
  * src/app/Views/create-admin-user/create-admin-user.component.html + .ts: naya Audit Access
    section (2 checkbox: View | Edit & Delete); .ts me ngOnInit default true/true, onAccessChange
    cascade: edit ON -> view auto-ON; view OFF -> edit auto-OFF
  * src/app/Views/admin-user/admin-user.component.html + .ts: access-modal (2 checkbox) + Action
    column label 'Edit & Delete' + onRowAction -> openAccessModal; .ts se unused Router import+
    ctor param hataya (TS6138 fix)
- ng serve final: 'Compiled successfully'
- environment.ts ab PROD pe point karta hai (user ki local-UI testing ke liye): apiUrl
  'https://wfmapi.mizopower.com/api' + meterPhotoUrl/imageUrl bhi wfmapi (prod build pe koi asar nahi -
  environment.prod.ts hi use hota hai)

### LOCAL TEST RESULTS (user ne khud kiya, 2 baar restore+re-run zaroori tha)
- Local DB localhost/StoreDB (user sushil / Sushil@123 - appsettings DevConnection) se 2 BAAR delete+'
  restore hua - har restore ke baad CanDelete column WAPAS aata tha => migration re-apply zaroori thi
  (script idempotent hui - easily re-runnable)
- FINAL LOCAL DB STATE: CanDeleteCol=0, 161 rows (153 view-only, 8 Edit&Delete, 0 no-access), 4 procs clean
- Local: dotnet build 0 errors + ng serve 'Compiled successfully' verified

### PRODUCTION DEPLOY STATUS (user ne kiya, verify ho gaya)
- PROD DB: migration run ho gaya (user SSMS screenshot = 0/0/2/4 verify + 'No issues found')
- PROD API: deploy ho gaya (user IIS/DONETAPI copy) - Buffy VERIFY: swagger.json me canDelete 0
  matches + SaveUserAuditAccess + canView/canEdit present
- PROD UI: dist copy BAKI - live index.html abhi bhi main.0efdb638499e45380fa0.js (30-Sep wala PURANA
  bundle 777KB, canEdit=0 matches)... USER ko dist copy karna hai (neeche REMAINING)
- PATH NOTE: dotnet publish to C:/Users/sushi/OneDrive/Desktop/DONETAPI (wahi purana path user ne
  diya, 110 files) - publish ke BAAD appsettings.json + appsettings.Development.json DELETE
  (deploy rule), web.config present. Temp Desktop DONETAPI_PUBLISH folder delete kar diya

### DEPLOY ORDER (critical): SQL pehle => turant API (purana API unknown @CanDelete param bhejke
  proc fail karega) => UI dist last. PROD UI COPY LESSON (03-Oct wala):
  dist/SmartMeterInstallation ke ANDAR KI FILES us folder me copy karo jahan prod index.html pada hai
  (folder-in-folder copy se bachna)

### REMAINING (user karega)
1. PROD UI dist copy: local D:/UI/StoreUI se `ng build --prod` (Node 22 pe --openssl-legacy-provider
   env chahiye) => dist/SmartMeterInstallation ke andar ki files PROD server wale folder me copy
   karo jahan index.html + main.0efdb... pada hai => purane hashed js overwrite => Ctrl+Shift+R
   (naya bundle hash live index.html me match check karo)
2. OPTIONAL (future): admin-user.component.html me purana BULK-DELETE toolbar button
   (deleteSelectedRows) abhi bhi hai - gating decide nahi hui, user decide karega
3. UI create-admin-user form me Audit Access section ka visual confirmation user-side nahi hua
   (screenshot me sirf form fields dikhe, section nahi) - section HTML me added hai, user ne
   'ALL DONE' bola - next session me coordinate kare

## ============================================================ SESSION 03-Oct: CONSUMER HES PUSH - SYNC 1-BY-1 + RESPONSE ME FULL PUSH LOG (LOCAL DONE, DEPLOY PENDING) ============================================================

USER REQUIREMENT: Consumer New Meter push (POST /api/Meter/UpdateHesStatus, areaType=CONSUMER)
pe response sirf {"success":true,"message":"Updated Successfully"} deta tha - user ko POORA push
log chahiye jo pehle sirf log file (LogCritical) me jaata tha. UI ME KOI CHANGE NAHI (user ne
bola jaisa hai waisa rahe - log response JSON me hi dekhega). Push 1-BY-1 (user requirement).
DT/DTC/FEEDER/HTC push UNCHANGED (purana async Hangfire flow).

### BACKEND (4 files, build 0 errors - 29 warnings sab pre-existing CS0436)
- NEW SmartMeterAdmin.ViewModel/Hes/HesPushLogEntry.cs: HesPushLogEntry (ConsumerId, MeterNo,
  Status SUCCESS/FAILED, HesHttpStatus, HesResponse, Message, Payload) + HesPushResult
  (AreaType, Total, Success, Failed, Error, Logs)
- SmartmeterAdmin.Hangfire/SyncHesMeter/HesMeter.cs:
  * NAYA PushSelectedNow(areaType, commaIds): HES login -> GetPendingHesMeter se SELECTED ids
    C# me filter (koi SQL change NAHI) -> 1-by-1 push -> har meter ka log entry + success pe
    UpdateHesStatus COMPLETED. MeterNo = payload ka DeviceSerialNo
  * SendToHesApi ab wrapper hai - naya SendToHesApiDetailed (HTTP status + response body return).
    Background SendToHes flow BILKUL untouched
  * HesPayloadSerializerSettings() helper - dono flows ka payload format same (camelCase+indented)
- MeterController.cs: HesMeter ctor-inject + UpdateHesStatus me CONSUMER branch: sync push ->
  response {success, message:"HES Push Completed: X/Y success", data:{summary, logs}}
  + 5-min delayed BackgroundJob.Schedule<HesMeter>(SendToHes) = fail/leftover PENDING ka
  safety-net (delay se double-push race avoid). Total==0 -> "koi PENDING meter nahi mila";
  HES login fail -> success=false + error message. Baaki areaTypes purane flow pe
- Program.cs: builder.Services.AddTransient<HesMeter>() add (pehle sirf Hangfire activator
  banata tha; controller constructor-inject ke liye DI registration zaroori thi)

### RESPONSE SHAPE (CONSUMER push)
{"success":true,"message":"HES Push Completed: 2/2 success",
 "data":{"summary":{"areaType":"CONSUMER","total":2,"success":2,"failed":0,"error":null},
         "logs":[{"consumerId":"...","meterNo":"MZ...","status":"SUCCESS","hesHttpStatus":"200",
                  "hesResponse":"<HES body>","message":"HES push done, DB status COMPLETED",
                  "payload":"<poora HES JSON camelCase indented>"}]}}

### LOCAL TEST (user khud karega)
- API: cd SmartMeterAdmin && dotnet run --no-launch-profile --urls https://localhost:60592
- UI: ng serve 4200 (environment.ts already local pe hai) -> Consumer New Meter -> rows select ->
  Update Selected Rows -> F12 > Network > UpdateHesStatus > Response -> data.logs
- UI toaster same rahega - log sirf API response JSON me hai

### DEPLOY PENDING (is feature ka): sirf backend publish (4 files) - koi SQL NAHI, koi UI change NAHI

### DEPLOY PREP DONE (03-Oct evening) - PUBLISH PACKAGES READY
- Local API/UI band kiye. dotnet build-server shutdown + clean (Release+Debug) + Release build: 0 errors (29 pre-existing CS0436 warnings)
- Publish: dotnet publish SmartMeterAdmin.csproj -c Release -o C:/Users/sushi/OneDrive/Desktop/DONETAPI -> 577 files (fresh Oct 3 14:57-14:58), appsettings.json + appsettings.Development.json DELETE ho gaye (deploy rule)
- UI prod build: D:/WFM software/StoreUI/dist/SmartMeterInstallation/ (Oct 3 15:03) - markers verified: 'audit-report' main.js me (4), 'AuditLog/GetLogs' src-app-Module-manage-meter-module.js me
- NOTE: Angular root D:/WFM software/StoreUI/ hai (package.json wahan) - src/src me NAHI. ng serve bhi wahi se chalta hai (npm start = cross-env openssl-legacy ng serve)
- environment.ts abhi LOCAL pe hai (apiUrl=localhost:60592) - prod build environment.prod.ts use karta hai, koi asar nahi
- Bacha hai (user karega): SSMS SQL scripts -> DONETAPI copy -> IIS restart -> dist copy -> verify (plan user ko de diya)

### PRODUCTION DEPLOY (03-Oct) - USER NE KAR DIYA + VERIFY RESULTS
- USER CONFIRM: "sab ho gya" - SQL (5 runs incl. Users.Access=1 update), backend copy + IIS restart, UI dist copy
- VERIFY (Buffy, 03-Oct evening):
  * PROD API = LIVE NAYA: swagger 165 endpoints, AuditLog/GetLogs + UpdateHesStatus dono present
  * PROD UI = DOUBTFUL: curl https://wfm.mizopower.com/main.js -> sirf 13.5KB + 'audit-report' marker 0
    (local dist main.js me marker 4 tha) - YA TO purana bundle live hai, YA UI dusre path/CDN-cache se serve ho raha
    -> USER BROWSER ME CHECK KARE: Ctrl+Shift+R -> Audit Report menu dikhe + Network me UpdateHesStatus response me data.logs
- Note: local API/UI dono band kar diye (03-Oct end) - koi process nahi chal raha
- UI VERIFY FINAL: live index.html -> main.0efdb638499e45380fa0.js (30-Sep wala PURANA bundle, 777KB, audit-report marker 0)
  -> dist copy EFFECTIVE NAHI HUI. FIX: dist/SmartMeterInstallation ke ANDAR KI FILES us folder me copy karo jahan prod
  index.html + main.0efdb... pada hai (folder-in-folder copy se bachna) -> purane hashed js overwrite -> Ctrl+Shift+R


## ============================================================ SESSION 30-Sep (LATER): NEW CONSUMER METER 'EDIT' BUTTON NAHI DIKH RAHA THA ============================================================

### ROOT CAUSE: Users.Access column = 0 (role ya audit access se KOI LENA DENA NAHI)
- Edit button UI me EditButtonComponent (Shared/AgGrid/edit-button) render karta hai.
  Template: *ngIf="params?.visible !== false && isAccess" -> isAccess FALSE = button render HI NAHI hota.
- isAccess logic: localStorage 'Access' (login pe res.data.access se AES-encrypted store hota hai)
  -> access !== '0' (kuch bhi aur ho - '1', 'true', garbage - sab chalta hai; sirf '0' hide karta hai).
- Access backend se AccountController.Login -> Users table ka BIT Access column -> JWT claim + response.
- 3 naye Satnam users INSERT me Access column set NAHI kiya gaya tha -> BIT default 0 -> login ke
  baad Access='0' store -> saare grids me Edit/HES buttons hide (HES/delete button consumer page pe
  alag checks hain: HES=!isSurveyInspect && isAccess, delete=isSuperadmin && isAccess).
- Yehi 'Access column 0 hona NORMAL hai' wala purana note GALAT tha - wo sirf LOGIN ke liye normal
  tha, UI buttons ispe depend karte hain. Purana Admin (hliraazl 609) ka bhi Access=0 hi hai ->
  unhe bhi kabhi Edit nahi dikhta tha (kisi ne notice nahi kiya hoga). Superadmins ka Access=1 hai.

### FIX (LOCAL DONE + VERIFIED, PRODUCTION PE RUN BAKI)
- Alag script file (EDIT_ACCESS_FIX_SQL_NOTES.sql) user ne DELETE kar di - FILE AB EXIST NAHI KARTI.
  FIX AB AISE RUN HOGA (2 tarike, koi bhi ek):
  (a) Direct SSMS query:
      UPDATE dbo.Users SET Access = 1 WHERE RoleType IN ('Superadmin','Admin') AND Access = 0;
      (Saare Superadmin+Admin cover - role-based hai, UserID pe depend nahi.)
  (b) YA USER_MANAGE_TEMPLATE_SQL_NOTES.sql run karke proc se:
      EXEC dbo.MANAGE_APP_USER @Action='EDIT', @Email='harshpathak@satnamglobal.in', @Access=1;
      EXEC dbo.MANAGE_APP_USER @Action='EDIT', @Email='saurabhsati@satnamglobal.in', @Access=1;
      EXEC dbo.MANAGE_APP_USER @Action='EDIT', @Email='msbhardwaj510@gmail.com',  @Access=1;
  (a) recommended - ek query me sab cover.
- LOCAL: teeno Satnam users (611/612/613) Access 0->1 UPDATE + verify PASS.
- IMPORTANT: Access JWT (390 din) + localStorage me AES-encrypted store hota hai -> SQL ke baad
  LOGOUT + LOGIN (dobara) zaroori, warna purana Access=0 token hi chalega.
- EK KAAM AUR BAKI (UI): EditButtonComponent me pehla isAccess check `decryptedAccess === '1'`
  hai (strict '1'), dusra `access !== '0'` - pehla dead code hai, harmless hai par glt result
  console.log karta hai. Kabhi saaf karna ho to dono ko `access !== '0'` kar dena. Abhi kaam
  chal raha hai (Access=1 pe dono path pass) - production UI copy ke saath ye bhi chala sakte ho.

### MASTER USER TEMPLATE: USER_MANAGE_TEMPLATE_SQL_NOTES.sql (30-Sep, LOCAL E2E TESTED)
- Ek baar run karo -> dbo.MANAGE_APP_USER proc ban jayegi (CREATE OR ALTER, idempotent)
- Har kaam ab chhote EXEC se: CREATE / EDIT (email,mobile,name,role,password,Access,audit rights) /
  VIEW / DELETE (soft) / RESTORE - examples script ke comments me ready hain
- CREATE built-in safety: Access=1 + UserAuditAccess 1/1/1 + duplicate-email guard
- Password plaintext SQL me KABHI nahi - PwdHex (CryptoUtility TripleDES hex) Buffy se banwana
- Test: CREATE(615) -> EDIT(mobiel/name/role/Access/email-change) -> VIEW -> DELETE -> RESTORE ->
  cleanup - SAB PASS. Local pe proc abhi bani hui hai (production me run karna hai).

### ADMIN USER LIST UI FIX: ADMIN_USER_UI_FIX_SQL_NOTES.sql (30-Sep, LOCAL E2E TESTED, PRODUCTION RUN BAKI)
- USER REQUIREMENT: /user/adminuser (Admin User List) pe Superadmin + Admin dikhenge (abhi 'No Rows
  To Show' aa raha tha) aur Add New se naya user ban sake with full access
- ROOT CAUSE (2 problems, dono DB procs me - UI/backend code change NAHI chahiye):
  1. UI AdminUserService.getAllAdminUser() -> GetAllUserByRole?roletype=Admin -> GET_USER_LIST proc.
     Sirf Admin role aata tha; production me proc ka ADMIN-bypass shayad purana hai (local me tha).
  2. MANAGE_USER proc INSERT me Users.Access set NAHI karti (naya user Access=0 = wahi 30-Sep
     Edit-button bug future me dobara!) + UserAuditAccess row bhi nahi banti.
- FIX (CREATE OR ALTER, idempotent, no IIS restart):
  [A] GET_USER_LIST: @ROLE_TYPE='Admin' me Admin + Superadmin DONO (baaki roles unchanged)
  [B] MANAGE_USER: INSERT me [Access]=1 column + SCOPE_IDENTITY se UserAuditAccess 1/1/1 row
      (UPDATE branch untouched; duplicate guard pehle se tha: UserName+Roletype NOT EXISTS)
  [C] Backfill: purane Admin/Superadmin jinke paas audit row nahi - unki 1/1/1 ban gayi
- LOCAL E2E TEST PASS: list me Admin+Superadmin dono (OtherRoles=0), test user MANAGE_USER se
  banaya -> Access=1 + audit 1/1/1 auto, test user cleanup done
- NOTE: Add New form se roleType hamesha 'Admin' hi jata hai (create-admin-user.component.ts
  hardcoded RoleEnum.Admin) - Superadmin UI se nahi banega, SQL template se ban sakta hai

### ACCESS UI SE MANAGE: USER_AUDIT_ACCESS_UI_SQL_NOTES.sql + API + UI (30-Sep, LOCAL E2E TESTED, PROD DEPLOY BAKI)
- USER REQUIREMENT: Admin User List se hi kisi bhi user ko audit View/Edit/Delete access UI se de/pa sake
- SQL (USER_AUDIT_ACCESS_UI_SQL_NOTES.sql - 2 NAYI procs, existing GET_USER_AUDIT_ACCESS untouched):
  * GET_USER_AUDIT_ACCESS_LIST(@ROLE_TYPE,@SEARCH): sab users + LEFT JOIN access (na ho to 0/0/0)
  * SAVE_USER_AUDIT_ACCESS(@UserID,@CanView,@CanEdit,@CanDelete,@UpdatedBy): upsert + verify row wapas
- BACKEND (5 files changed, build 0 errors):
  * UserActivityLogViewModel.cs: +UserAuditAccessListViewModel, +SaveUserAuditAccessRequest
  * IUser.cs / UserService.cs: GetUserAuditAccessList + SaveUserAuditAccess (proc calls)
  * UserController.cs: GET api/User/GetUserAuditAccessList (roleType,search),
    POST api/User/SaveUserAuditAccess - DONO pe IsAdminLevelRole() guard (Superadmin/Admin only, 403)
  * IdentityExtensions.cs: +GetName() (Name claim)
  * AuditController.cs: +public static ClearCanViewCache(userId) - save pe cache turant invalidate
- ANGULAR (3 files, prod build SUCCESS 4.13 MB, marker verified in user-module bundle):
  * admin-user.service.ts: getUserAuditAccessList + saveUserAuditAccess
  * admin-user.component.ts: Access column (purple button har row me) + openAccessModal +
    loadAccessList + onAccessCheckboxChange (Edit->View auto, Delete-> teeno auto, View off-> sab off)
  * admin-user.component.html: header me purple 'Access' button + modal (role filter, search,
    table UserID/Username/Name/Role/View/Edit/Delete checkboxes, instant save per checkbox)
- LOCAL E2E TESTS (sab PASS, API localhost:60592):
  * Admin (saurabhsati 611) login -> access list aayi (Contractor filter: 564 CanView=1 tha)
  * REVOKE 564+101 (CanView=0) -> DB 0/0/0 -> 101 ka GetLogs TURANT 'Access denied' (cache clear kaam)
  * GRANT wapas 101 -> GetLogs TURANT success+data (232 logs)
  * 403: Contractor (101) se GetUserAuditAccessList -> HTTP 403 Access denied
  * Cleanup: 101 aur 564 original state (CanView=1, Edit/Delete=0) restore
- PRODUCTION DEPLOY ORDER: [1] SQL script (2 procs) -> [2] Backend publish (DONETAPI copy,
  appsettings skip, restart) -> [3] Angular dist/SmartMeterInstallation copy + hard refresh
  -> [4] Admin User List kholo -> Access button (header + har row) -> modal me checkbox se grant/revoke
  -> save turant effective (revoke wale ko 'Access denied' mil jayega agle request pe)
- NOTE: access changes ab AUDIT ME BHI jayenge (AuditLogActionFilter POST /api/User/SaveUserAuditAccess
  ko log karega - kaun kisne kisko kya diya, sab record hoga)
- UPDATE (30-Sep late): ACCESS AB SIRF MASTER SUPERADMIN (UserID 3) KE LIYE - final requirement:
  * Backend (UserController.cs): IsAccessManager() = RoleName=='Superadmin' AND UserID==3
    (AccessManageAllowedUserId const = 3 - future me change karna ho to yahin karo)
  * Baaki SAB (Admin + doosre Superadmins jaise 219/579/580/611) -> 403 Access denied
  * UI (admin-user component): header Access button *ngIf=isAccessManager (RoleName=='Superadmin'
    AND UserID==3, UserID localStorage 'UserID' se) + row Access column bhi sirf manager ko
  * IMPORTANT: UI check ke liye UserID localStorage me hona zaroori (login.component set karta hai)
  * LOCAL: saurabhsati (611) ko test ke liye Superadmin bana diya tha (Admin tha), par UserID 3 na
    hone se usko Access button/milna NAHI chahiye - sirf 'Superadmin' username (UserID 3) wala
  * E2E PASS (local API): UserID 3 -> list 200 + save 200 | 611 (Superadmin, not 3) -> 403 |
    612 (Admin) -> 403 | prod dist rebuilt
  * NOTE: production me UserID 3 = 'Superadmin' master account hai (same restore)
  * IMPORTANT: kisi bhi role/user change ke baad RE-LOGIN zaroori (JWT+localStorage me purana hai)
- UPDATE (30-Sep night): ADD NEW FORM SE ACCESS + SUPERADMIN PASSWORD CHANGE:
  * Superadmin (UserID 3) ka password ab P@ssword123321 hai (local) - purana P@ssword123 rejected
    (production me ye password abhi nahi gaya - deploy ke waqt SQL me karna hoga ya rehne dena)
  * MANAGE_USER proc me 3 NAYE optional params END me: @CanView/@CanEdit/@CanDelete (default 1/1/1)
    -> INSERT ke waqt hi UserAuditAccess row form ki values se banti hai
  * Backend: UserViewModel +CanView/CanEdit/CanDelete (bool?, null=default) + UserService wire
  * UI: create-admin-user form me 'Audit Access' checkboxes (View/Edit/Delete, Edit->View auto,
    Delete->teeno auto) + user.ts model me canView/canEdit/canDelete
  * E2E PASS: user created via API with View=1/Edit=0/Delete=0 -> DB 1/0/0 -> login -> GetLogs
    ALLOW (View=1) -> test user cleanup done | prod dist rebuilt
  * USER_AUDIT_ACCESS_UI_SQL_NOTES.sql ab FULL deploy file hai (MANAGE_USER + list/save procs)
  * DEPLOY NOTE: MANAGE_USER me params END me hain (positional-safe) - purana UI bhi chalega

## ============================================================ SESSION 30-Sep: SATNAM ADMIN USERS + ADMIN FULL DATA-SCOPE FIX ============================================================

### 1. 3 NAYE ADMIN USERS - CREATE + PRODUCTION DEPLOY DONE (user ne SSMS me run kiya)
- Script: SATNAM_ADMIN_USERS_SQL_NOTES.sql (project root)
- Users (teeno RoleType='Admin' + UserAuditAccess CanView/CanEdit/CanDelete=1/1/1):
  * saurabhsati@satnamglobal.in  (Saurabh Sati)
  * harshpathak@satnamglobal.in  (Harsh Pathak)
  * msbhardwaj510@gmail.com      (M S Bhardwaj)
- Passwords alag-alag set kiye (user ne khud diye) - encrypted hex CryptoUtility TripleDES
  (same key/iv) se generate, roundtrip decrypt test PASS. Plaintext passwords script ke
  comments me hain - deploy ke baad comment delete karne bola tha user ko.
- LOCAL UserIDs: 611/612/613. PRODUCTION UserID ALAG HAIN (screenshot: harshpathak prod pe 613,
  local pe 612) - UserID server-specific hai, username se hi check karna.
- Production login verify hua (screenshot): success, role=Admin, access:0 (Access column 0
  hona NORMAL hai - purana Admin 609 ka bhi 0 hai; login check sirf password decrypt pe hai)

### 2. PROBLEM: LOGIN KE BAAD SAB DATA 0 DIKHTA THA (dashboard counts, lists khali)
- ROOT CAUSE: dbo.GET_USER_ID(@USERID,@ROLETYPE) function - ye 24 dashboard/list/render procs
  me data-scope decide karta hai. Purana logic: sirf own + CreatedBy=@USERID users ka data,
  bypass sirf 'Superadmin'/'SurveyInspect' ke liye. ADMIN ROLE KAHIN FULL-SCOPE NAHI THA
  (purana Admin 609 ka bhi same behavior - Admin ko kabhi full data nahi milta tha).
- Proof: production API direct curl - GetNewMeterDashboardCount?userId=0&roleType=Admin ->
  HTTP 200 par saare counts 0. UI/API/code sab sahi the - sirf DB scope function ka issue.
- Superadmin ko isliye sab dikhta tha kyunki procs me IN ('Superadmin',...) literal bypass hai.

### 3. FIX: ADMIN_FULL_ACCESS_SQL_NOTES.sql (project root) - PRODUCTION PE RUN BAKI !!!
- GET_USER_ID() function CREATE OR ALTER: ab 'Admin' bhi full scope (sab users) -
  WHERE @ROLETYPE IN ('Superadmin','Admin') OR (CreatedBy=@USERID for baaki roles)
- EK FIX SE SAARE 24 procs cover (GET_NEWMETER_COUNT_WEB, GET_SURVEY_COUNT_WEB, saare
  *Render/*List procs jo sys.sql_expression_dependencies me GET_USER_ID use karte hain)
- Baaki roles (Contractor/Subcontractor/Subdivision/Store) ka behavior BILKUL UNCHANGED
- LOCAL TEST PASS: Admin 613 -> totalConsumerNewMeter=87139 (pehle 0), DT=1744, Feeder=326;
  Contractor 36 -> scope same as before (1)
- IDEMPOTENT (CREATE OR ALTER). ROLLBACK def script ke comment me hai.
- NOTE: function change INSTANT hai - IIS restart ki zaroorat NAHI.

### 4. REUSABLE TEMPLATE: NEW_USER_CREATE_TEMPLATE_SQL_NOTES.sql (project root)
- Har naye user ke liye process: Buffy ko bolo "naya user banao: <email>, <name>, password <x>
  ya generate karo" -> encrypted hex bhar ke final script milega -> SSMS me run
- Template: [A] user insert (NOT EXISTS guard) + [B] UserAuditAccess 1/1/1 + [C] verify
- E2E TESTED (local, 30-Sep): test user 614 banaaya template-pattern se -> login PASS +
  dashboard 87139 meters + audit logs 218 -> test user CLEAN kar diya (ab exist nahi karta)

### 5. DOOSRA ISSUE (ALAG): PRODUCTION UI BUNDLE PURANA HAI - DIST COPY BAKI
- Production main.js (main.0efdb638499e45380fa0.js) me AUDIT CODE HI NAHI hai:
  'AuditLog/GetLogs' / 'audit-report' / 'GetSummary' / 'ExportExcel' sab 0 matches
- Local dist (29-Sep build): D:/WFM software/StoreUI/dist/SmartMeterInstallation/ me
  audit markers PRESENT hain (src-app-Module-manage-meter-module.js + main.js me)
- FIX (user karega): dist/SmartMeterInstallation/ ka content production UI folder me
  copy/overwrite + purane hashed js delete + browser hard refresh (Ctrl+Shift+R)
- Verify: curl -sk https://wfm.mizopower.com/main.js | grep -c 'AuditLog/GetLogs'  (>=1 aana chahiye)
- NOTE: data-scope fix (section 3) ke baad CURRENT purane UI me bhi data dikh jayega -
  UI copy audit-report screen ke liye chahiye, dashboard ke liye nahi.

### 6. BACKEND/API VERIFY (30-Sep, sab sahi mila)
- wfmapi.mizopower.com (prod UI ka main API) pe NAYA backend live: swagger me
  AuditLog/GetLogs + GetSummary + ExportExcel present (163 endpoints)
- wfm.mizopower.com:5032 is machine se connection-refused deta hai (firewall) - wfmapi se test karo
- Production login (harshpathak) PASS; Local E2E (saurabhsati) PASS
- Local API test ke liye: cd SmartMeterAdmin && dotnet run --no-launch-profile --urls
  "https://localhost:60592" (nohup background; test ke baad kill kar diya - PID 3780)

### 7. PRODUCTION DEPLOY CHECKLIST (AGLA STEP - ISI ORDER ME)
- [ ] SSMS me ADMIN_FULL_ACCESS_SQL_NOTES.sql run karo (USE [StoreDB] check)
- [ ] Verify: Admin_FullScope = sab users count (~156+), Superadmin same, OtherRole unchanged
- [ ] Production UI: dist/SmartMeterInstallation/ copy (section 5) + hard refresh
- [ ] Satnam users se login -> dashboard counts NON-ZERO + Audit Report me data
- [ ] (Satnam users production me already hain - SATNAM script dobara RUN MAT KARNA)

### KEY DECISIONS (30-Sep)
- Admin role ab full data-scope (Superadmin jaisa) - role change karne ki zaroorat nahi padi,
  function-level single fix chuna (37 procs me Superadmin literal tha, 24 GET_USER_ID use karte hain)
- User ne bola: aage se naye users ke liye ready script dega (template process section 4)
- Access=0 Users column me normal hai (koi kaam ka nahi) - JWT me Access claim jata hai par
  login/account logic uspe depend nahi karta

## ============================================================ SESSION 29-Sep: PRODUCTION DEPLOY COMPLETE + VERIFY ============================================================

### 1. LOCAL PRE-DEPLOY VERIFY (29-Sep, sab PASS)
- dotnet build -c Release (sln): 0 errors. dotnet run + swagger 200 + Login API ne DB query ki
  (valid response) + Hangfire dashboard 200 (SQL storage OK) - DB connectivity RESTORE KE BAAD BHI VERIFIED.
- Serilog me aaj ka koi naya error log nahi bana. (Kal 28-Sep 02:32 wala Hangfire Error Number:-2
  connection timeout DB restore ke time ka tha - ab clean.)

### 2. SQL SCRIPTS PRODUCTION PE RUN + VERIFIED (29-Sep, user ne SSMS me run kiye)
- USER_ACTIVITY_LOG_SQL_NOTES.sql: RUN DONE.
- USER_AUDIT_ACCESS_SQL_NOTES.sql: RUN DONE - screenshot verify:
  SeededRows=155 | TableExists=1 | RowCount=155 | ViewCount=155 | EditCount=3 | DeleteCount=3 |
  ProcExists=1 | OrphanUserCount=0 (GATE PASS - production UserIDs match) | Proc tests:
  219->1,1,1 | 609->1,0,0 | 3(Superadmin)->1,0,0 (sahi - EDIT sirf 219/579/580 ko hai)
- UserNameMismatchCount=1 = KNOWN HARMLESS (UserID 218 Excel 'Swami@gmail' vs DB 'Swami@gmail,com'
  comma typo - access UserID se hota hai, koi asar nahi. Rehne do.)

### 3. BACKEND DEPLOY + CONFIRMED LIVE (29-Sep)
- DONETAPI folder C:/Users/sushi/OneDrive/Desktop/DONETAPI (110 files) -> production copy.
- appsettings.json + appsettings.Development.json DONETAPI se REMOVE kiye gaye (production wala
  overwrite na ho - deploy rule). web.config rehne diya (IIS ke liye zaroori).
- CONFIRM LIVE: https://wfm.mizopower.com:5032/swagger/v1/swagger.json me /api/AuditLog/GetLogs,
  GetSummary, ExportExcel teeno endpoints present (163 total endpoints).

### 4. VS PUBLISH ISSUE (agar dobara aaye to iska fix yaad rakho)
- VS IDE ka 'Publish - Build failed' DONO BAAR aaya, jabki CLI + VS ka apna MSBuild.exe command
  line se 0 errors me pass the = code ka issue NAHI tha.
- ROOT CAUSE: stale MSBuild build-server processes (dotnet build-server shutdown se fix) +
  OneDrive conflict - DONETAPI OneDrive/Desktop me hai; VS publish ne DeleteExistingFiles se
  folder empty kiya aur OneDrive sync ne copy step fail kiya (web.config deletion bhi revert hui thi).
- FIX/WORKAROUND: CLI se publish - dotnet publish SmartMeterAdmin.csproj -c Release
  -o "C:/Users/sushi/OneDrive/Desktop/DONETAPI" -> fir appsettings*.json delete kar lo.
  (Ya permanent: .pubxml me PublishUrl OneDrive se bahar shift karo, e.g. D:\Deploy\DONETAPI.)

### 5. ANGULAR FRESH PROD BUILD (29-Sep 17:40, SUCCESS)
- cd "D:/WFM software/StoreUI/src" && NODE_OPTIONS=--openssl-legacy-provider npx ng build --source-map=false
- OUTPUT PATH NOTE: is baar dist D:/WFM software/StoreUI/dist/SmartMeterInstallation/ me bana
  (26 MB; project-name folder ke saath - purane 28-Sep wale path se alag ho sakta hai).
- Verified: prod URLs baked (wfmapi.mizopower.com + wfm.mizopower.com:5032), 'audit-report' route
  bundle me present. Prod UI https://wfm.mizopower.com pe khud bhi audit-report string confirm hui.

### 6. LOCAL UI TEST ATTEMPT (29-Sep) - START KIYA THA FIR STOP KAR DIYA
- Local API (https://localhost:60592) + ng serve (4200) start kiya, fir user ne stop bola -
  teeno processes kill + ports free verified. Abhi kuch bhi local pe chal nahi raha.

### 7. ABHI KA STATUS / NEXT
- Production: SQL done + backend live + UI deploy (user ne deploy kiya). USER TESTING PENDING.
- Testing checklist (Step 5 wali): non-admin ko Audit menu dikhe, report data + LOGIN entry,
  meter push pe Meter No + Consumer ID + Result columns, Refresh/Action filter/Export Excel,
  CanView=0 deny test (5-min cache). UI purana cache ho to Ctrl+Shift+R.
- Test creds note (LOCAL ONLY): users 101/36 password P@ssword123 - production pe applicable NAHI.
- Test meters (local DB) MZ0004190/92/89 wale local the - production pe mat dhundo.

## ============================================================ NEXT SESSION (29-Sep): PRODUCTION DEPLOY - STEP BY STEP (ISI ORDER ME) ============================================================
SAB READY HAI (28-Sep ko implement + test ho chuka - neeche Session 28-Sep section me details).
Kal bas ye 5 steps chalane hain:

### STEP 1: SQL Script 1 - USER_ACTIVITY_LOG_SQL_NOTES.sql (project root)
- SSMS me production DB pe run karo. PEHLE check: script me 'USE [StoreDB]' - production
  ka DB naam sahi ho to hi run karo.
- EXPECTED VERIFY OUTPUT: TableExists=1 | IndexCount=3 | SaveProc=1 | LogsProc=1 |
  SummaryProc=1 | SaveProcReturnValue=0
- End me test row aayegi (UserName='verify-script') - optional delete:
  DELETE FROM UserActivityLog WHERE UserName='verify-script';

### STEP 2: SQL Script 2 - USER_AUDIT_ACCESS_SQL_NOTES.sql (project root)
- EXPECTED VERIFY OUTPUT: TableExists=1 | RowCount=155 | ViewCount=155 | EditCount=3 |
  DeleteCount=3 | ProcExists=1 | OrphanUserCount=0
- !!! AGAR OrphanUserCount != 0: production ke UserIDs local se alag hain -> RUKEIN,
  mujhe (AI) ko batana, seed UserID-wise fix karna padega. UserNameMismatchCount bhi
  dekhna (0 ideal; kuch aaye to note karke batana).

### STEP 3: Backend Publish
- VS Release publish -> DONETAPI folder -> production API folder me COPY/OVERWRITE
- RULES: appsettings.json SKIP (production wala overwrite NAHI), folder DELETE NAHI
- IIS restart (app pool recycle)
- Is build me naya: AuditController (CanView check - UserAuditAccess se),
  AuditLogActionFilter, AuditLogger, MeterServices, IMeter, MeterController, IDapper,
  Dapper_ORM, Program.cs + NEW: MeterPushInfo.cs (ViewModel/Hes/),
  UserActivityLogViewModel.cs (4+1 viewmodels)
- TIP: deploy se pehle production folder ka BACKUP copy rakh lena (rollback easy)

### STEP 4: Angular dist -> Production
- DIST ALREADY BUILT (28-Sep, 4.13 MB, 0 errors): D:/WFM software/StoreUI/src/dist/
- Fresh build chahiye to: cd "D:/WFM software/StoreUI/src" &&
  NODE_OPTIONS=--openssl-legacy-provider npx ng build --source-map=false
- dist folder production server pe copy karo (jahan pehle dist copy hota hai)

### STEP 5: Live Verify (production pe, deploy ke baad)
- [ ] Koi bhi NON-ADMIN user (Subcontractor/Contractor koi bhi) login kare ->
      sidebar me 'Audit > Audit Report' DIKHE (ab sabko dikhta hai)
- [ ] Non-admin report khole -> DATA DIKHE (CanView=1 sabki hai)
- [ ] Koi login kare -> Audit Report me LOGIN entry aaye
- [ ] Meter push karo -> entry me Meter No + Consumer ID + Result columns bhare
- [ ] Refresh button + Action filter + Export Excel test
- [ ] OPTIONAL deny test: DB me kisi user ka CanView=0 karo -> us user ko
      'Access denied' mile (5-min cache expire ke baad). Test ke baad CanView=1 wapas.

### ROLLBACK (agar kuch toote):
- Backend: backup copy wapas overwrite + IIS restart
- SQL: UserAuditAccess table safe DROP ho sakta hai (koi dependency nahi - sirf access)
- UserActivityLog table DROP MAT KARNA (agar logs ban chuke hain - immutable data)

### KAL KE BAAD KE OPTIONAL KAAM:
- EDIT/DELETE rights (3 superadmins: 219/579/580) abhi sirf STORE hain - purge/edit
  capability banani ho to alag se (audit table immutable by design)
- users_access_list.txt + csv files stale hain (26-Sep) - chaho to .xlsx se regenerate

## ============================================================ SESSION 28-Sep: USER AUDIT ACCESS + FULL PRE-DEPLOY VERIFY ============================================================

## ============================================================ SESSION 28-Sep: USER AUDIT ACCESS + FULL PRE-DEPLOY VERIFY ============================================================

### 1. USER NE EXCEL BHAR DIYA (UserAccessList/UserAccessList.xlsx, 28-Sep 13:52 update)
- PARSED + VERIFIED: 155 users, 0 blank, 0 duplicate UserID, 0 invalid value
- FINAL MARKS:
  * VIEW=1   -> SABHI 155 users (USER NE CONFIRM KIYA: sabko audit report dikhani hai)
  * EDIT=1   -> sirf 3: UserID 219 (Nageshsharma), 579 (Asanga), 580 (Damtea) - teeno Superadmin
  * DELETE=1 -> wahi 3 (EDIT/DELETE abhi sirf STORE honge - audit table immutable hai,
               koi update/delete function NAHI bana; user ne khud option select kiya)
- NOTE: sirf .xlsx update hua - users_access_list.txt + csv PURANE (26-Sep) hain, stale.
  Script .xlsx se banaya (sahi source).

### 2. NAYA SCRIPT: USER_AUDIT_ACCESS_SQL_NOTES.sql (project root) - PRODUCTION PE RUN KARNA HAI
- [A] UserAuditAccess table (UserID PK, UserName ref, CanView/CanEdit/CanDelete BIT,
      UpdatedDate/UpdatedBy)
- [B] Seed 155 rows EXACTLY Excel se (automated 155/155 diff verify PASS).
      INSERT-only WHERE NOT EXISTS -> dobara run safe, purani rows overwrite NAHI hoti.
- [C] GET_USER_AUDIT_ACCESS proc (@UserID -> rights; 0 rows = access denied)
- [D] Verify: TableExists=1, RowCount=155, ViewCount=155, EditCount=3, DeleteCount=3,
      ProcExists=1, OrphanUserCount=0 (production me 0 NAHI aaya to UserID mismatch - RUKEIN)
- FIXES during make: UserID 41 email typo (csakshi745), RowCount reserved word -> [RowCount]
- LOCAL RUN + VERIFIED (28-Sep): sab PASS, re-run idempotent, proc tests:
  219 -> 1,1,1 | 609 -> 1,0,0 | 999999 -> 0 rows
- Known harmless: UserID 218 Excel 'Swami@gmail' vs Users 'Swami@gmail,com' (comma typo,
  DB quirk) - access UserID key se hota hai, koi asar nahi.

### 3. DEPLOY SE PEHLE FULL VERIFY (28-Sep) - SAB PASS
- Dono SQL scripts local re-run: idempotent PASS (ActivityLog: TableExists=1, IndexCount=3,
  procs 1,1,1, SaveProcReturnValue=0; local rows 192+)
- Backend dotnet build -c Release: 0 ERRORS (30 warnings sab pre-existing CS0436
  ImportExcelDataIntoDatabase type-conflict - purana known issue, audit se related nahi)
- Saari 11 backend deploy files EXIST verified. NOTE: MeterPushInfo.cs
  SmartMeterAdmin/ViewModel/Hes/ me hai (notes me Controllers likha tha - galat tha)
- Program.cs wiring verified: IAuditLogger DI + AuditLogActionFilter global
  (AddControllers options) + AuditLog_StartTime middleware - key names match
- MeterController enrichment verified: GetMeterNosByIds (MeterServices line 581, IMeter 80)
  + Audit_EntityId / Audit_DescSuffix Items keys (lines 1325-1334)
- Dapper ExecuteProcedureMultiple verified (Dapper_ORM line 60, IDapper interface me)
- Angular 8/8 files verified (D:/WFM software/StoreUI/src/src):
  audit-log.model.ts, audit.service.ts (actionType rename OK), audit-report component
  ts/html/scss, module declare + routing 'audit-report', menu.ts me 2 entries (line 423+634)
- ENVIRONMENTS: environment.prod.ts -> production (wfmapi.mizopower.com) OK;
  environment.ts (dev) -> localhost:60592 (local). ng build prod config use karta hai
  -> prod file jayegi, theek. Local ng serve pe dev file chalegi.

### 4. ROLE-CHECK UPDATE IMPLEMENTED + E2E TESTED (28-Sep, user ke 'kar do' ke baad)
- BACKEND (2 files changed):
  * AuditController.cs: IsAuthorizedRole() -> async IsAuthorizedAsync()
    - Superadmin/Admin fast-path (DB hit nahi hota)
    - Baaki roles: GET_USER_AUDIT_ACCESS proc se CanView check
    - Per-request cache (HttpContext.Items[Audit_CanView]) + static 5-min
      ConcurrentDictionary cache (UserID -> CanView)
    - Fail-closed: DB fail = deny; proc fail = deny
    - 3 endpoints (GetLogs/GetSummary/ExportExcel) sab await IsAuthorizedAsync() use karte hain
    - NEW DI: IDapper inject hua
  * UserActivityLogViewModel.cs: + UserAuditAccessViewModel class (CanView/CanEdit/CanDelete)
- ANGULAR (3 files changed):
  * menu.ts: shared auditMenuBlock property banaya + contractor/store/survey/production/
    subdivision menus ke end me add (ab 7 menus me Audit hai)
  * layout.component.ts: getLoginStatus() me else branch - Subcontractor/jo-bhi-role
    menu-less hai usko sirf Audit menu dikhega (menuData = [auditMenuBlock])
  * random.guard.ts: checkUserHasAccess() me '/manage-meter/audit-report' URL sab roles
    ke liye open (asli control backend CanView pe hai)
- E2E TEST RESULTS (local, 28-Sep, sab PASS):
  * Test setup: users 101 (Subcontractor) + 36 (Contractor) ko P@ssword123 set kiya
    (CryptoUtility TripleDES same key/iv, python pyCryptodome se encrypt karke DB me daala)
  * Login 101 -> JWT OK (UserID+RoleName claims confirm)
  * GetLogs (CanView=1) -> SUCCESS, totalCount=196, user ka apna LOGIN entry dikha
  * GetSummary -> SUCCESS (VIEW=148, LOGIN=20, CREATE=15...)
  * CanView=0 kiya (101) -> 5-min cache expire (310s wait) -> 'Access denied.' FAIL-CLOSED OK
  * CanView wapas 1 kiya (restore). API band kar di test ke baad.
  * Backend build: 0 errors | Angular prod build: SUCCESS (4.13 MB initial, no errors)
- TEST CREDENTIALS NOTE: users 101/36 ka password ab P@ssword123 hai (LOCAL DB ONLY)

### 4a. PURANA DECISION RECORD (28-Sep first half)
- 28-Sep ko user ne pucha 'Excel bhar diya to hardcoded kyu hai' - explanation diya:
  Excel -> SQL -> DB table ho gaya, par backend code table ko read nahi karta (Step 2 adhura)
- Pehle user ne 'Abhi sirf store rakho' chuna tha; baad me decision badla ->
  'karke test karke bata do' -> upar wala implementation hua (section 4)

### 5. PRODUCTION DEPLOY ORDER (SQL scripts dono, fir backend, fir Angular)
1. SQL: USER_ACTIVITY_LOG_SQL_NOTES.sql -> verify: TableExists=1, IndexCount=3,
   procs 1,1,1, SaveProcReturnValue=0
2. SQL: USER_AUDIT_ACCESS_SQL_NOTES.sql -> verify: RowCount=155, EditCount=3,
   ProcExists=1, OrphanUserCount=0 (0 nahi aaya = production UserIDs alag - RUKEIN)
3. Backend publish -> DONETAPI copy/overwrite (appsettings.json SKIP, folder DELETE nahi)
   -> IIS restart
4. Angular: cd 'D:/WFM software/StoreUI/src' -> NODE_OPTIONS=--openssl-legacy-provider
   npx ng build --source-map=false -> dist copy
5. Live verify: login entry, Audit Report menu (AB SAB ROLES ko dikhega), meter push pe
   Meter No + Consumer ID + Result columns, Refresh + Action filter + Export Excel,
   non-CanView user ko Access denied (ab UserAuditAccess se control hota hai)

## ============================================================ CURRENT STATUS / NEXT SESSION TODO (26-Sep evening) ============================================================

## ============================================================ CURRENT STATUS / NEXT SESSION TODO (26-Sep evening) ============================================================

### ✅ Aaj ka complete kaam (26-Sep): USER ACTIVITY AUDIT LOG FEATURE
Neeche full details hain. Ye feature abhi PRODUCTION PE DEPLOY NAHI HUA. Next session me deploy karna hai.

### 🚀 NEXT SESSION - PRODUCTION DEPLOY (4 STEPS, ISI ORDER ME):
1. **SQL**: USER_ACTIVITY_LOG_SQL_NOTES.sql production pe run karo (USE [StoreDB] naam check).
   Expected: TableExists=1, IndexCount=3, teeno procs=1, SaveProcReturnValue=0.
   End me verify-script test row aati hai - delete optional:
   DELETE FROM UserActivityLog WHERE UserName='verify-script';
2. **Backend publish**: Release publish -> DONETAPI folder -> production me COPY/OVERWRITE
   (appsettings.json SKIP, folder DELETE nahi) -> IIS restart.
   Changed: AuditController.cs, AuditLogActionFilter.cs, AuditLogger.cs, MeterServices.cs,
   IMeter.cs, MeterController.cs, IDapper.cs, Dapper_ORM.cs, Program.cs
   + NEW: MeterPushInfo.cs, UserActivityLogViewModel.cs
3. **Angular build**: cd "D:\WFM software\StoreUI\src"
   NODE_OPTIONS=--openssl-legacy-provider npx ng build --source-map=false -> dist copy
4. **Live verify**:
   - Login -> Audit Report me LOGIN entry
   - Sidebar: Audit > Audit Report (Superadmin + Admin ko dikhta hai)
   - Meter push karo -> entry me Meter No + Consumer ID + Result columns bhare
   - Refresh button + Action filter + Export Excel test
   - Non-admin user ko "Access denied" aana chahiye

### 📋 PENDING TASK: USER-WISE AUDIT ACCESS (user ne list banwai hai)
- Sabhi 155 users ki list ready: UserAccessList/users_access_list.txt (txt)
  + UserAccessList/UserAccessList.xlsx (Excel, VIEW?/EDIT?/DELETE? blank columns)
- Users summary: Subcontractor 83, Subdivision 36, Contractor 13, Superadmin 7,
  Production 6, SurveyInspect 5, Store 3, Admin 1, Swami 1
- USER KO YE KARNA HAI: Excel me har user ke saamne VIEW/EDIT/DELETE me se access mark karna
- JAB USER EXCEL BHARKE DE: usse UserAuditAccess table + proc script banaunga,
  backend role-check update (sirf Superadmin/Admin nahi - list wale bhi)
- Abhi implementation: sirf Superadmin + Admin hi audit report dekh sakte hain (hardcoded)

### 🐛 AAJ KE BUG FIXES (deploy me included, yaad rakhne layak):
1. `action` query param ASP.NET route value se collide tha -> rename `actionType`
   (AuditController.cs + audit.service.ts)
2. UI ka userId=0 (All Users) filter UserID=0 ban raha tha -> blank grid -> ab 0=ALL
   (AuditController.cs: UserID = userId > 0 ? userId : null)
3. Push audit me list IDs type-name dikh rahe the -> ab actual IDs (AuditLogActionFilter.cs)
4. NEW: push pe Meter No + NewConsumerId audit me - GetMeterNosByIds (MeterServices/IMeter)
   + MeterController UpdateHesStatus enrichment + AuditLogActionFilter EntityId/DescSuffix keys
5. Angular unused import fix (audit.service.ts HttpHeaders)

### 💻 LOCAL SETUP (abhi bhi chal raha hai - next session start pe band kar sakte ho):
- API: https://localhost:60592 (dotnet run, x86) - audit feature ke saath latest build
- UI: http://localhost:4200 (ng serve, background process)
- Local SQL: UserActivityLog table + 3 procs created + tested (155 test logs already there)
- Test users: Superadmin/P@ssword123
- Test meters pushed: MZ0004190, MZ0004192 (1000038289), MZ0004189 (1000038290) - sab log hue

### 📊 AUDIT REPORT UI (aaj improve kiya):
Columns: Date&Time (pinned) | User (Login ID) | User Name | Role | Activity | Screen |
Meter No | Consumer ID | Result (SUCCESS green/FAILED red) | IP | Time Taken | Details | API Endpoint
+ Refresh button + Export Excel + filters (UserID/Action/Module/Date range/Search)

# Ye file padh kar poora context wapas milega - koi bhi naya session me.

## ================= SESSION 26-Sep (EVENING): USER ACTIVITY AUDIT LOG + AUDIT REPORT (COMPLETE + LOCAL TESTED, DEPLOY PENDING) =================
Task: Koi bhi user kuch bhi kare (view/edit/delete/login/upload/export) to automatic log bane
+ Superadmin/Admin ke liye Audit Report screen (filters + grid + Excel export).

### KEY DECISIONS (user-approved)
- Report access: Superadmin + Admin (backend role-check + dono menus me entry)
- GET logging: sirf IMPORTANT GETs (login/export/download/user-list/swap-list etc.) - polling/dashboard GET skip
- Retention: FOREVER - koi purge job nahi
- Approach: Custom Action Filter (koi new package nahi) - project Dapper/proc pattern me fit

### 1. SQL SCRIPT: USER_ACTIVITY_LOG_SQL_NOTES.sql (project root) - PRODUCTION PE RUN KARNA HAI
- [A] UserActivityLog table (LogID BIGINT PK, UserID, UserName, FullName, RoleType, Action, Module,
  ActionName, EntityID, Description, RequestData NVARCHAR(MAX), IPAddress, UserAgent, StatusCode,
  DurationMs, CreatedDate) + 3 indexes (CreatedDate DESC, UserID+CreatedDate, Action+CreatedDate)
- [B] SAVE_USER_ACTIVITY_LOG (insert, RequestData 40KB cap, TRY/CATCH swallow)
- [C] GET_USER_ACTIVITY_LOGS (filters: UserID/Action/Module/FromDate/ToDate/Search + paging;
  Users LEFT JOIN se UserName/FullName enrich; InCurrentPage flag)
- [D] GET_USER_ACTIVITY_SUMMARY (resultset1: action-wise counts, resultset2: top 25 users)
- [E] Verify: TableExists=1, IndexCount=3, teeno procs=1, test insert + cleanup note
- LOCAL RUN + VERIFIED (26-Sep): sab PASS. Table immutable - koi update/delete proc NAHI.

### 2. BACKEND (8 files, build 0 errors, local E2E tested)
- SmartMeterAdmin.ViewModel/AuditLog/UserActivityLogViewModel.cs (NEW) - 4 viewmodels
- SmartMeterAdmin.Services/AuditLog/AuditLogger.cs (NEW) - IAuditLogger + AuditLogger (Dapper,
  fail-safe: log fail ho to main flow kabhi nahi tootta)
- SmartMeterAdmin.Services/Dapper_ORM/IDapper.cs + Dapper_ORM.cs: NAYA ExecuteProcedureMultiple
  (SqlMapper.GridReader - summary proc ke 2 resultsets ke liye)
- SmartMeterAdmin/Middleware/AuditLogActionFilter.cs (NEW):
  * AuditLogActionFilter : IAsyncActionFilter - GLOBAL registered (Program.cs AddControllers options)
  * Action detect: action name keywords (login/export/delete/update/save...) warna HTTP method
  * SENSITIVE sanitize: password/token/secret/apikey/profileimage/documenturl/base64 keys remove
  * GET policy: ImportantGetActions list match tabhi log (user decision: sirf important GETs)
  * SkipAuditLogAttribute (NEW) - kisi endpoint pe lagao to log nahi banta (polling ke liye)
  * IP: X-Forwarded-For aware (IIS/proxy ke peeche sahi); Duration: HttpContext.Items start time
  * Description: 'POST Account/Login -> SUCCESS/FAILED' format (response JSON parse)
- SmartMeterAdmin/Program.cs: IAuditLogger DI + filter global registration + request start-time middleware
- SmartMeterAdmin/Controllers/AuditController.cs (NEW): GET api/AuditLog/GetLogs (paging+filters),
  GetSummary, ExportExcel (NPOI, cap 5000 rows) - SAB pe Superadmin/Admin role-check (IsAuthorizedRole)
- Login logging: global filter se automatic (LOGIN action, success+fail dono) - controllers touch nahi kiye

### 3. ANGULAR (D:\WFM software\StoreUI\src\src - 8 files)
- Model/audit-log.model.ts (NEW), Service/audit.service.ts (NEW - getLogs/getSummary/exportExcel blob)
- Views/MeterSwap/audit-report/ (NEW folder): component.ts (filters + ag-grid + server paging +
  excel download blob), component.html (filter row + grid + paging), component.scss (action color badges)
- Module/manage-meter.module.ts: AuditReportComponent declare
- Routing/manage-meter-routing.module.ts: path 'audit-report'
- Shared/layout/menu.ts: 'Audit > Audit Report' parent menu - superadminMenu + adminMenu DONO me
- NOTE: Angular build nahi chalaya (user khud karega: NODE_OPTIONS=--openssl-legacy-provider npx ng build)

### 4. LOCAL E2E TEST RESULTS (26-Sep, sab PASS)
- LOGIN success + LOGIN failed dono log hue (UserName captured, password sanitize verified)
- VIEW (GetLogs) + EXPORT (ExportExcel) log hue; ExportExcel ne 4.4KB xlsx download kiya
- GetSummary: actionSummary + topUsers dono resultsets sahi
- GetLogs paging + totalCount sahi; users join enrich working

### 5. PRODUCTION DEPLOY ORDER (jab user bole)
1. SQL: USER_ACTIVITY_LOG_SQL_NOTES.sql (USE [StoreDB] naam check) -> verify output
2. Backend: publish -> DONETAPI folder copy/overwrite (appsettings.json skip) -> restart
3. Angular: NODE_OPTIONS=--openssl-legacy-provider npx ng build --source-map=false -> dist copy
4. Live verify: koi bhi user login/edit/delete kare -> Audit Report me dikhe; Excel export;
   non-admin ko report access denied

### Notes
- Audit log user ki REPLACE flows (consumer/dt/etc.) sab modules pe automatic lagu - koi per-controller
  code nahi. Agar koi noisy endpoint mile to bas [SkipAuditLog] attribute laga do.
- Reporting API bhi apna hi access log karta hai (GetLogs/ExportExcel) - by design.

## ================= KYA KARNA THA (purana - 16-Sep, sab complete) =================
Consumer meter replacement flow (StoreApi + StoreUI Angular):
1. UpdateReplacementMeter API naye fields ke saath:
   - AreaType = REASON (free text reason)
   - AreaValue = OLD meter no (jis ko replace karna hai)
   - NewAreaValue = NEW smart meter no
   - 4 "NewinitialNetMeter*" readings (user bharta hai)
   - 4 "OldinitialNetMeter*" readings (HES API se AUTO-FILL)
   - OldMeterPhoto + NewMeterPhoto (file upload)
2. Old meter readings HES API (getUiDailyLoadProfileData) se auto-fill jab user old meter no dale
3. Grid (GetReplacementMeters) me:
   - Old Meter No + New Meter No alag columns
   - UserID ki jagah installer ka NAME (Users table se)
   - NewConsumerId sahi (NULL ho to ConsumerId fallback)

## ================= FILE CHANGES (COMPLETE) =================
### Backend (StoreApi-main):
1. SmartMeterAdmin.ViewModel/UpdateReplacementMeterRequest/UpdateReplacementMeterRequest.cs
   - Naye 8 reading fields + 2 IFormFile photos (purane KWH/KVAH fields removed)
2. SmartMeterAdmin/Controllers/StoreController.cs
   - UpdateReplacementMeter: [FromBody] -> [FromForm] (multipart), photo upload
     (basicUtility.UploadFile, folder "ReplacementMeter"), 11 field validations
   - NEW endpoint: [HttpGet("GetOldMeterReadingsFromHes?meterNo=...")]
3. SmartMeterAdmin.Services/Store/IStore.cs
   - UpdateReplacementMeter 13-param signature
   - Task<string?> GetOldMeterReadingsFromHesAsync(string meterNo)
4. SmartMeterAdmin.Services/Store/StoreService.cs
   - 13 params -> proc UPDATE_REPLACEMENT_METER
   - HES proxy: GetHesApiKeyAsync() (login MZR/mzr@123 via appsettings HesUiConfig,
     apikey 22h cache, SemaphoreSlim thread-safe) + GetOldMeterReadingsFromHesAsync()
     (Bearer auth, 401 pe auto re-login + retry, raw JSON string return)
5. SmartMeterAdmin/appsettings.json
   - NEW section HesUiConfig: LoginUrl (6011/api/v1/auth/login), Email=MZR, Password=mzr@123
   - IMPORTANT: ye cookie approach NAHI hai - apikey (JWT Bearer) use hota hai jo
     existing HesMeter.cs pattern se mila. Cookie ki zaroorat hi nahi, kal bhi chalega.

### Angular (D:\WFM software\StoreUI - NOTE: nested path src/src/):
1. src/src/app/Model/replacement-meter.model.ts - new payload + HesDailyRow + grid model
2. src/src/app/Service/production.service.ts
   - updateReplacementMeter: FormData (multipart) bhejta hai
   - getOldMeterReadingsFromHes: BACKEND PROXY pe call (direct HES nahi)
   - extractLatestHesReading: last row = latest reading (format: [meterNo,date,mdas,ImpKWH,ImpKVAH,ExpKWH,ExpKVAH])
3. src/src/app/Views/MDM/meters-replacement/meters-replacement.component.ts
   - form model naye fields, onOldMeterBlur() -> HES autofill, photo handlers, validations
4. ...component.html - old/new readings sections, HES status messages, photo file inputs
5. ...component.scss - hes-status + file-input styles
6. Grid columns: OldMeterNo, NewMeterNo, UserName (Installed By), UserEmail (UserID hata)

## ================= SQL (ReplacementMeter_SQL_NOTES.sql - project root) =================
FINAL SCRIPT - DB restore ke baad YEHI run karna (PARSE_OK verified):
- [A] ReplacementMeter table me 6 columns add:
  OldInitialNetMeter{Import,Export}Reading{KWH,KVAH} VARCHAR(250) + OldMeterNo, NewMeterNo VARCHAR(50)
- [A] Backfill: purani rows me Old/New meter no bharne ke 2 UPDATE
- [B] UPDATE_REPLACEMENT_METER (13 params):
  * 3-STEP LOOKUP (user requirement): pehle NewSmartMeterSrNo=@AREA_VALUE,
    na mile to NewConsumerId=@AREA_VALUE, phir ConsumerId=@AREA_VALUE,
    tab bhi na mile to "Consumer not found"
  * Original logic preserved: history INSERT + survey update + readings/photos update
    + NewSmartMeterSrNo swap + IsReplacementMeter='3'
  * COALESCE guard on photos (photo na do to purani photo overwrite nahi hogi)
  * OldMeterNo=@AREA_VALUE, NewMeterNo=@NewAreaValue store hota hai history me
- [B2] GET_REPLACEMENT_METERS:
  * OldMeterNo fallback: ISNULL(NULLIF(rm.OldMeterNo,''), rm.NewSmartMeterSrNo)
  * NewMeterNo fallback: ISNULL(NULLIF(rm.NewMeterNo,''), cnm.NewSmartMeterSrNo) (join ConsumerNewMeterDetails)
  * NewConsumerId fallback: ISNULL(NULLIF(rm.NewConsumerId,''), rm.ConsumerId)
  * UserName = ISNULL(u.FullName, u.UserName) (Users join on UserID)
  * UserEmail = u.UserName
- [C] Verify queries (6 columns, 13 params dikhne chahiye)

## ================= LIVE TEST RESULTS (local StoreDB) =================
- Full E2E API test PASS: MZ1006146 -> MZ3009391, HES readings 1440.322/1564.685,
  photos upload, main table + history dono me reflect
- 3-step lookup PASS (ConsumerId se bhi replace hua)
- HES proxy PASS (apikey login + Bearer, MZ1006146/MZ2015932 dono test kiye)
- IMPORTANT BEHAVIOR: replace success hone ke baad old meter dobara se replace NAHI
  hota (DB me swap ho chuka) - "Consumer not found" aayega. Ye by-design hai.
- Local DB me test data RESTORE kiya gaya tha (consumer 02680427 = MZ1006146 wapas)

## ================= ENVIRONMENT NOTES =================
- API: https://localhost:60592 (x86/Debug build, launchSettings me SSL port)
- DB: StoreDB @ localhost, Windows Auth (-E) chalta hai; appsettings me sushil/Sushil@123
- User table = dbo.Users (UserID, UserName=email, FullName, Roletype)
- Angular: Angular ye purana webpack hai -> build/serve ke liye
  NODE_OPTIONS=--openssl-legacy-provider zaroori (package.json start script me already hai)
- Angular build: NODE_OPTIONS=--openssl-legacy-provider npx ng build --source-map=false
- If app bin folder se manually start ho to photo upload ke liye:
  --contentRoot="D:/WFM software/working/FINAL/New folder/StoreApi-main/SmartMeterAdmin"
  (warna WebRootPath null -> "path1" error)
- VS se run karne pe ye problem nahi aati

## ================= PENDING / TODO =================
1. Production DB pe ReplacementMeter_SQL_NOTES.sql run karna (restore ke baad)
2. Production publish me appsettings.json me HesUiConfig include karna
3. DT/DTC/Feeder/HTC replacement flows me bhi same changes chahiye to alag se karna
   (aapne bola tha sirf consumer wala karna) - unke tables/procs alag hain
4. Optional: grid me photo preview columns
5. Optional: fallback lookup me strictness (galat ID se galat consumer replace na ho)

## ================= KEY DECISIONS =================
- Cookie (APP_SESSION) approach DROP - apikey Bearer approach (project ke existing
  MZR service account se). Reason: cookie browser me ho hi nahi sakta + 24h expire.
- FormData (multipart) hi bhejna - JSON nahi chalega ([FromForm] hai)
- HES response last row = latest reading
- Photos optional hain (text fields required hain)

## ================= SESSION 16-Sep (EVENING): METER SWAP =================
Feature complete (SQL + backend), UI pending (user ne bola UI ka rahne do).

### MeterSwap_SQL_NOTES.sql (project root, PARSE-ready) - PRODUCTION PE RUN KARNA HAI
- SwapMeter table (IF NOT EXISTS guard) - swap history + full meter snapshot
  (consumer detail, old/new meter readings, installer, zone/circle/division, photos)
- SAVE_SWAP_METER_DETAIL proc - 3-step lookup (NewConsumerId -> NewSmartMeterSrNo -> ConsumerId)
- GET_SWAP_METER_LIST proc - history list, search + paging, latest first
- End me [4] VERIFY queries - expected: SwapMeterTable=1, SaveSwapProc=1, GetSwapListProc=1

### Backend changes (already in build)
- MeterController.MeterSwap - swap success ke BAAD snapshot save; fail ho to bhi swap
  success, response me snapshotSaved:false + error detail
- NEW endpoint: GET /api/Meter/GetSwapMeterList (search/pageSize)
- MeterServices.SaveSwapMeterDetail + GetSwapMeterList (Dapper procs)
- SwapMeterDetailViewModel + SwapMeterSaveResultDto viewmodels

## ================= SESSION 17-Sep: HES RATIO FIX + DEPLOY PREP =================

### 1. UI environment -> PRODUCTION switch kiya gaya
- File: D:\WFM software\StoreUI\src\src\environments\environment.ts (dev file, ng serve isko use karta hai)
- Ab production pe point karta hai (local values comments me saved hain):
  apiUrl   = https://wfm.mizopower.com:5032/api
  apiUrl2  = https://hesapi.mizopower.com:6011/api (WFM endpoints)
  apiUrl3  = https://wfm.mizopower.com:5032/api (MeterSwap endpoints isi se jate hain)
  imageUrl = https://wfm.mizopower.com:5032
- environment.prod.ts pehle se production pe tha, chheda nahi
- Wapas local karna ho to comments me values ready hain (uncomment/comment swap)
- meterPhotoUrl sirf environment.prod.ts me tha, koi component use nahi karta - skip

### 2. UPDATE_HES_SEND proc analysis (koi change NAHI kiya)
- Ye sirf HesSend status flag update karta hai (PENDING/COMPLETED + HesPushedBy/At=GETDATE())
- 5 branches: CONSUMER/FEEDER/DT/DTC/HTC - *NewMeterDetails tables me SET
- single/3-phase/CT/CTC/HT alag se exist NAHI karte - wo UI categories hain
- Auth check (Superadmin/Subdivision) commented out hai
- C# code: MeterServices.UpdateHesStatus sahi hai, koi change nahi chahiye

### 3. GET_PENDING_HES_METER_SYNC - ISSUE aur FIX
ISSUE: lineCtRatio/linePtRatio hardcoded '1' the (FEEDER me meter ratios real the,
line wale '1'), table me real data para tha - HES ko galat ratio ja raha tha.

FIX SCRIPT: GET_PENDING_HES_METER_SYNC_FIX.sql (project root + Desktop pe copy)
- Sirf line ratios change hue (aapke confirm kiye rules ke hisaab se):
  * lineCtRatio: table value (LineCTRatio) + A suffix GUARANTEE (200/5 -> 200/5A;
    pehle se A ho to as-is); NULL/empty -> '1/1A'
  * linePtRatio: table value (LinePTRatio) + V suffix GUARANTEE (240/240 -> 240/240V);
    NULL/empty -> '1/1V'
  * CONSUMER branch: hardcoded '1/1' -> '1/1A' / '1/1V' (aapne confirm kiya)
  * meterCTRatio/meterPTRatio: KAHIN HAATH NAHI (CONSUMER 1/1, DT/DTC/HTC 1,
    FEEDER real - FeederMeter.CtConnectedRatio/PtCorrectedRatio)
- UPDATE_HES_SEND + C# code untouched
- Script me USE [StoreDB] hai - production me DB naam adjust karna
- A/V suffix append logic CASE-based hai - NULLIF/LTRIM/RTRIM guards ke saath

### 4. Teeno meters ka data (17-Sep ki position)
| Meter | Table | DB LineCT/PTRatio | Status |
|---|---|---|---|
| MZ0007041 | DTNewMeterDetails (ConsumerId 105HBW0106) | 800/5 + 240/240 | LIVE RE-PUSH TEST DONE ✅ |
| MZ0001133 | DTCNewMeterDetails (ConsumerId 10702950) | 800/5 + 240/240 | COMPLETED (purane '1' ratio ke saath) |
| MZ0000033 | FeederNewMeterDetails (ConsumerId 10HBW21) | 600/5 + 11000/110V | COMPLETED (purane '1' ratio ke saath) |
- DT row me LTCTRating=800/5A alag field hai, HES payload me nahi jata - ignore

### 5. LIVE TEST (local, 17-Sep 20:31) - PASS
- Local API start kiya (dotnet run), Superadmin/P@ssword123 se login, UpdateHesStatus
  API se MZ0007041 (DT, ConsumerId 105HBW0106) PENDING kiya
- Hangfire SendToHes job ne push kiya -> HES COMPLETED -> log me payload confirm:
  meterCTRatio:1, meterPTRatio:1, lineCtRatio:800/5A, linePtRatio:240/240V ✅
- Test ke baad local API band kar diya (port 60592 free)
- Login flow note: password DB me encrypted hai (CryptoUtility), login API se hi token lena

### 6. PRODUCTION DEPLOY PACKAGE (READY)
- Publish files: C:\Users\sushi\OneDrive\Desktop\DONETAPI (93 files, build 17-Sep 16:56)
  * appsettings.json DONETAPI me HAI HI NAHI (production wala overwrite nahi hoga)
  * Deploy rule: production folder DELETE nahi - sirf copy/overwrite; app restart (IIS pool/service)
- SQL scripts (project root + Desktop pe copy):
  * MeterSwap_SQL_NOTES.sql (SwapMeter feature)
  * GET_PENDING_HES_METER_SYNC_FIX.sql (line ratio fix)
  * (ReplacementMeter_SQL_NOTES.sql - 16-Sep wala, agar pehle run nahi hua)
- PRODUCTION RE-PUSH PENDING (purane '1' ratio ke saath ja chuke hain):
  MZ0007041 (DT), MZ0001133 (DTC), MZ0000033 (FEEDER)
  -> script run hone ke baad UI/UpdateHesStatus se PENDING karo, Hangfire push kar dega
- HTC: production me bhi 0 rows - koi action nahi, future data automatic sahi jayega

## ================= PENDING (26-Sep: kuch nahi bacha - sab complete) =================
- 17-Sep ke sab items complete ho gaye (deploy, re-push, MeterSwap UI).
- 16-Sep ke optional items (DT/DTC/Feeder/HTC replacement flows, photo preview columns)
  bhi user ne "all done" bata diye (26-Sep).

## ================= KEY DECISIONS (17-Sep additions) =================
- HES ratio format LOCKED: lineCtRatio hamesha A suffix (800/5A), linePtRatio hamesha
  V suffix (240/240V); null fallback 1/1A / 1/1V - ye CONSUMER me bhi
- meterCTRatio/meterPTRatio ko kabhi chhedna nahi (FEEDER me real, baaki me '1')
- GET_PENDING_HES_METER_SYNC me hi fix hoga, UPDATE_HES_SEND sirf status ke liye hai
- Deploy DONETAPI se hota hai (folder overwrite, appsettings.json isme nahi rehta)

## ================= SESSION 24-Sep (EVENING): FEEDER 404 FIX + PRODUCTION DEPLOY COMPLETE =================

### 1. FEEDER 404 FIX (root cause + fix, production pe deploy ho gaya)
ISSUE: POST /api/Meter/GetFullNewFeederMeterRender 404 de raha tha (UI feeder render page khali).
ROOT CAUSE: MeterController.GetFullNewFeederMeterRender me sirf [HttpPost] tha, [Route] attribute MISSING.
  Controller route template "api/Meter" hai ({action} token nahi) -> action sirf bare POST /api/Meter pe map tha.
  (Compare: GetAllNewFeederMeterRender etc. sab me [Route] laga hai - sirf yehi ek chhoot gaya tha.)
FIX: [Route("GetFullNewFeederMeterRender")] add kiya -> rebuild 0 errors -> local verify (200 + data) -> production deploy DONE.
  Poore MeterController scan kiya - koi aur action affected nahi tha.

### 2. PRODUCTION DEPLOY STATUS (24-Sep evening) - SAB COMPLETE
- [x] ReplacementMeter_SQL_NOTES.sql (16-Sep)
- [x] MeterSwap_SQL_NOTES.sql + GET_PENDING_HES_METER_SYNC_FIX.sql (17-Sep)
- [x] LINE_RATIO_UI_UPDATE_SQL_NOTES.sql (19-Sep)
- [x] METER_SWAP_SWAPREMARK_SQL_NOTES.sql (24-Sep)
- [x] Backend publish -> DONETAPI -> production copy -> restart (appsettings.json skip hua)
- [x] Angular production build -> dist copy
- Ab production me live: replacement meter flow, meter swap + swap history, line ratio UI + bulk,
  SwapRemark, feeder render fix

### 3. VERIFY / FOLLOWUP (26-Sep: SAB DONE - user confirm kiya)
- [x] HES RE-PUSH: MZ0007041 (DT), MZ0001133 (DTC), MZ0000033 (FEEDER) - done
- [x] MZ2020643 (consumer) - HES me device dikh raha hai - done
- [x] Line ratio prefill DT/DTC/Feeder edit screens - verified
- [x] Swap modal me Swap Remark input + grid me value - verified

## ================= SESSION 24-Sep: METERSWAP SwapRemark (COMPLETE, DEPLOY PENDING) =================
Task: MeterSwap me user apna remark de sake (UI payload se). Column name = SwapRemark.
History grid ke existing "Remark" column me ab SwapRemark dikhana hai (purana CNMD-Remark hidden).

### 1. SQL SCRIPT: METER_SWAP_SWAPREMARK_SQL_NOTES.sql (project root) - PRODUCTION PE RUN KARNA HAI
- [A] SwapMeter table + SwapRemark VARCHAR(250) NULL (IF NOT EXISTS guard)
- [B] SAVE_SWAP_METER_DETAIL: +1 param @SwapRemark VARCHAR(250) = NULL (END me, positional-safe);
  INSERT me SwapRemark column, value NULLIF(LTRIM(RTRIM(@SwapRemark)),'') (empty->NULL).
  Stub-CREATE + ALTER pattern (fresh restore pe bhi chalega). CNMD.Remark copy as-is (varchar(20) status field).
- [C] GET_SWAP_METER_LIST: s.Remark AS Remark -> s.SwapRemark AS Remark (grid ka Remark ab user swap-remark)
- [D] Verify: HasSwapRemark=1, SAVE proc 5 params, alias check, purani rows SwapRemark=NULL
- LOCAL RUN + VERIFIED (24-Sep): sab PASS. E2E proc test bhi kiya (SwapId 9 trim-ke-sath save, fir delete).

### 2. BACKEND (4 files, build 0-errors)
- MeterController.cs: MeterSwapRequest DTO + SwapRemark property; SaveSwapMeterDetail ko pass
- IMeter.cs / MeterServices.cs: SaveSwapMeterDetail + optional string swapRemark = null;
  parameters.Add("@SwapRemark", null if whitespace else trim)
- SwapMeterDetailViewModel.cs: SwapRemark property add (Remark me bhi wahi value aati hai ab)

### 3. ANGULAR (D:\WFM software\StoreUI\src\src - 3 files)
- Model/meter-swap.model.ts: MeterSwapRequest + swapRemark; SwapMeter + remark comment + swapRemark
- Views/MeterSwap/meter-swap/component.ts: form model/reset me swapRemark; grid column header 'Remark'->'Swap Remark' (width 160)
- component.html: modal me optional 'Swap Remark' input (maxlength 250, ngModel swapRemark, required nahi)
- NOTE: Angular build nahi chalaya (user khud karega: NODE_OPTIONS=--openssl-legacy-provider npx ng build)

### 4. PRODUCTION DEPLOY ORDER (jab user bole)
1. SQL: METER_SWAP_SWAPREMARK_SQL_NOTES.sql (USE [StoreDB] naam check) -> verify output
2. Backend: publish -> DONETAPI folder copy/overwrite (appsettings.json skip) -> restart
3. Angular: build -> dist copy
4. Live verify: swap modal me Swap Remark input; swap ke baad history grid ke Swap Remark column me value

### KEY DECISIONS (24-Sep)
- Naya column SwapRemark (250) - CNMD.Remark (varchar 20, 'ACCEPT' jaisa status) ko touch nahi kiya
- SwapRemark optional hai - purana payload (3 fields) bhi chalega (proc param NULL default)
- Grid me alag column nahi - existing Remark column hi SwapRemark dikhayega (user requirement)
- Purani SwapMeter rows me SwapRemark NULL - grid me blank dikhega (backfill nahi kiya)

## ================= SESSION 19-Sep: LINE CT/PT RATIO UI FEATURE (COMPLETE, DEPLOY PENDING) =================
Task: DT/DTC/Feeder me LineCTRatio + LinePTRatio UI se edit ho sake + bulk Excel upload.
Sab code + SQL complete aur local pe E2E tested. Production pe kuch bhi run NAHI hua.

### 1. SQL SCRIPT: LINE_RATIO_UI_UPDATE_SQL_NOTES.sql (project root) - PRODUCTION PE RUN KARNA HAI
- [A] UpdateDTNewMeterDetails ALTER: +2 params @LineCTRatio/@LinePTRatio VARCHAR(50) NULL
  (END me rakhe, positional-safe) + SET me COALESCE(NULLIF(@x,''), col) guard
- [B] UpdateDTCNewMeterDetails ALTER: same 2 params, existing IF @Action='UPDATE' block ke andar
- [C] UpdateFeederNewMeterDetails ALTER: same 2 params (CASE-guard style) + AUDIT BUGFIX:
  pre-existing crash - jab koi audited column (BookNo/Readings) change na ho to audit string
  '[]' ban jata tha aur SYSTEM_AUDIT_LOG ke OPENJSON pe "']' at position 0" error aata tha.
  Fix: guard NOT IN ('', '[', ']', '[]') - real change pe audit abhi bhi lagta hai (verified)
- [E] NAYA proc BULK_UPDATE_LINE_RATIOS (4 params: MeterNo, LineCTRatio, LinePTRatio + extra):
  MeterNo pehle DTNewMeterDetails, phir DTCNewMeterDetails, phir FeederNewMeterDetails me
  dhundhta hai; jahan mile wahan guarded update. Output: UPDATED / NOT_FOUND / SKIPPED.
- [D] Verify queries - EXPECTED OUTPUT: DT=26 params, DTC=18, Feeder=14, sab RatioParams=2,
  BULK_UPDATE_LINE_RATIOS=4 params/2 ratio
- Tables me koi change NAHI (columns varchar(50) pehle se the). GET procs me change NAHI
  (ratio columns already return hote hain - prefill free me chala). Restart ki zaroorat NAHI.
- Script me USE [StoreDB] hai - production DB naam check karke run karna. SQL PEHLE run karo,
  naye params optional hain isliye purana UI kuch nahi tootega.

### 2. BACKEND CHANGES (3 files, build 0-errors, local E2E tested)
- SmartMeterAdmin.Services/Meter/MeterServices.cs:
  * UpdateDTNewMeterDetails / UpdateDTCNewMeterDetails / UpdateFeederNewMeterDetails me
    parameters.Add("@LineCTRatio", ...) + @LinePTRatio (2-2 lines each)
  * NAYA: BulkUpdateLineRatiosAsync() - proc call per row
- SmartMeterAdmin.Services/Meter/IMeter.cs: BulkUpdateLineRatiosAsync signature
- SmartMeterAdmin/Controllers/MeterController.cs - 2 NAYE endpoints:
  * POST /api/Meter/BulkUpdateLineRatios - Excel (NPOI) upload, parse, row-by-row proc,
    result summary {updated, notFound, skipped, failed, rows[]}; cap 5000 rows
  * GET /api/Meter/DownloadBulkLineRatioSample - NPOI se sample xlsx generate (koi static
    file dependency nahi; header MeterNo|LineCTRatio|LinePTRatio + 3 sample rows)

### 3. ANGULAR CHANGES (D:\WFM software\StoreUI\src\src - 10 files)
- Model/new-meter.ts: DtNewMeter, DtcNewMeter, FeederNewMeter me lineCTRatio/linePTRatio
- Service/feedersurvey.service.ts: NAYA BulkUpdateLineRatios() method
- update-dtnewmeter html+ts: 2 inputs (SMC Box Seal No ke baad) + formData.append 2
- update-dtcnewmeter html+ts: 2 inputs (CT Ratio ke baad) + formData.append
- update-feedernewmeter html+ts: 2 inputs (Book No ke baad) + formData.append
- htcnewmeter html+ts: HEADER me 2 naye icons - sample Excel download + bulk upload
  (Swal confirm -> upload -> result table popup -> OK pe grid refresh). BULK KA GHAR YAHI HAI.
- Shared/layout/menu.ts: "Bulk Line Ratio Update" sidebar entry REMOVE kar di (user ne bola
  alag screen nahi chahiye). Views/MeterInfoList/upload-line-ratio/ component files exist
  karti hain par ab unreachable hain (delete nahi ki) - chahiye to route+menu wapas jodna.

### 4. LOCAL TEST RESULTS (sab PASS)
- Teeno Update procs: value update + empty/NULL guard (purani value safe) + restore kar diya
- Bulk E2E (real Excel upload API se): 3 updated (DT/DTC/Feeder auto-detect sahi), 1 NOT_FOUND
- DTC + Feeder Update APIs curl se test: success:true, DB me values gayi
- Audit regression: real change (BookNo NULL->457D) log hota hai; ratio-only/no-change pe
  skip, crash nahi
- FINAL LOCAL DB VALUES: MZ0000845(DT)=10/5A+240/240V, MZ0000644(DTC)=150/5A+240/240V,
  MZ0000019(Feeder)=100/5A+11000/110V (user ki original 3 UPDATE queries ka target pura)

### 5. ENVIRONMENT / INFRA NOTES (is session ki)
- environment.ts (dev) ab LOCAL API pe point karta hai (localhost:60592) - production values
  comments me saved hain. Prod build environment.prod.ts use karta hai (untouched, production
  pe hi hai) - deploy pe asar nahi. Local ng serve ke liye comments swap karne honge.
- src/node_modules par JUNCTION banaya tha (pre-existing bug: angular.json sourceRoot=src/src
  nested hai, styles.scss ka ../node_modules resolve nahi hota tha) - ag-grid CSS isi se mila.
  Junction abhi pada hai; ager clean checkout ho to dobara banana padega:
  cd "D:/WFM software/StoreUI/src" && cmd //c "mklink /J node_modules ..\\node_modules"
- Session end pe local API (60592) + UI (4200) dono STOP kar diye - user live server pe khud
  test karega

### 6. PRODUCTION DEPLOY ORDER (jab user bole)
1. SQL: LINE_RATIO_UI_UPDATE_SQL_NOTES.sql (USE [StoreDB] naam check) -> verify output dekho
2. Backend: publish -> DONETAPI folder copy/overwrite (appsettings.json skip) -> restart
3. Angular: NODE_OPTIONS=--openssl-legacy-provider npx ng build --source-map=false -> dist copy
4. Live verify: edit screens me ratio prefill; New HTC Meter page ke header icons (sample
   download + upload); Feeder ratio-only update pe koi error nahi (audit bugfix ka proof)

### KEY DECISIONS (19-Sep)
- NULLIF/empty guard har proc me: UI blank bheje to purani value safe (approve/reject flows
  ratio bhejte hi nahi - unpe zero asar)
- Naye proc params END me + NULL default - purane positional callers break nahi honge
- Bulk me meter type (DT/DTC/Feeder) auto-detect - user alag-alag file nahi bharega
- Sample Excel API se generate hota hai (static file nahi) - dev+prod dono me chalega
