using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMeterAdmin.Infrastructure.Helpers;
using SmartMeterAdmin.ViewModel.Production;
using SmartMeterAdmin.ViewModel.Store;
using System.Threading.Tasks;
using static SmartMeterAdmin.Infrastructure.Helpers.ConstantValues;
using System.Linq;
using SmartMeterAdmin.Services.Store;
using SmartMeterAdmin.ViewModel.DataTable;
using SmartMeterAdmin.ViewModel.Master;
using System.Collections.Generic;
using Org.BouncyCastle.Utilities.Collections;
using System.Security.Claims;
using System;
using Hangfire;
using Microsoft.AspNetCore.Http;
using SmartmeterAdmin.Hangfire.ExcelJobs;
using System.IO;
using SmartMeterAdmin.Infrastructure.Enumeration;
using Microsoft.AspNetCore.Hosting;

namespace SmartMeterAdmin.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/Store")]
    public class StoreController : ControllerBase
    {
        private readonly IWebHostEnvironment env;
        private readonly IJsonResponseFromat jsonResponseFromat;
        private readonly IStore store;
        private readonly IBasicUtility basicUtility;

        public StoreController(
            IWebHostEnvironment env,
            IJsonResponseFromat jsonResponseFromat,
            IStore store,
            IBasicUtility basicUtility,
            Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            this.env = env;
            this.jsonResponseFromat = jsonResponseFromat;
            this.store = store;
            this.basicUtility = basicUtility;
            this.configuration = configuration;
        }

        private readonly Microsoft.Extensions.Configuration.IConfiguration configuration;

        [HttpPost]
        [Route("ImportIssueMeterByExcel")]
        public IActionResult ImportIssueMeterByExcel(IFormFile postedFile)
        {
            if (postedFile == null)
                return Ok(jsonResponseFromat.JsonMessage(false, SelectValidExcel));

            int userId = User.Identity.GetUserId();

            // Create Folder
            string path = Path.Combine(env.WebRootPath, "IssueMeterExcel");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            // File Info
            string fileName = Path.GetFileNameWithoutExtension(postedFile.FileName);
            string extension = Path.GetExtension(postedFile.FileName);

            if (!AllowedFileExtensions.GetAllowedFile("Excel").Contains(extension))
                return Ok(jsonResponseFromat.JsonMessage(false, SelectValidExcel));

            string filePath = Path.Combine(
                path,
                fileName + DateTime.Now.ToString("yyyyMMddHHmmss") + extension
            );

            using (FileStream stream = new(filePath, FileMode.Create))
            {
                postedFile.CopyTo(stream);
            }

            // Background Job (same pattern)
            var jobId = BackgroundJob.Enqueue<ImportExcelDataIntoDatabase>(
                x => x.ImportExcelData(filePath, userId, TableNameEnum.TempIssueMeter)
            );

            BackgroundJob.ContinueJobWith(
                jobId,
                () => SyncIssueMeterByMeterNo(userId)
            );

            return Ok(jsonResponseFromat.JsonMessage(true, FileUploaded));
        }


        [Route("GetMeterBoxList")]
        [HttpPost]
        public async Task<IActionResult> GetMeterBoxList([FromBody] MeterRequest request)
        {
            request.StoreId = request.StoreId == 0 ? User.Identity.GetUserId() : request.StoreId;
            request.RoleType = string.IsNullOrWhiteSpace(request.RoleType) ? User.Identity.GetUserRoleName() : request.RoleType;
            if (request.BoxId is null)
            {
                var data = await store.GetListOfMeterBox<MeterBox>(request);
                return Ok(jsonResponseFromat.JsonMessage(true, DataList, data));
            }
            else
            {
                var data = await store.GetListOfMeterBox<ProductionMeter>(request);
                return Ok(jsonResponseFromat.JsonMessage(true, DataList, data));
            }
        }

        [Route("AssignMeterBoxList")]
        [HttpPost]
        public async Task<IActionResult> AssignMeterBoxList([FromBody] ListOfMeterNumber data)
        {
            await store.AcceptProductionMeter(data);
            return Ok(jsonResponseFromat.JsonMessage(true, UpdateSuccess, data));
        }

        [Route("IssueMeterToContractor")]
        [HttpPost]
        public async Task<IActionResult> IssueMeterToContractor([FromBody] ListOfMeterNumber data)
        {
            await store.IssueProductionMeter(data);
            return Ok(jsonResponseFromat.JsonMessage(true, UpdateSuccess, data));
        }

        [Route("GetIssueMeterList")]
        [HttpPost]
        public async Task<IActionResult> GetIssueMeterList([FromBody] AgGridTable request)
        {
            var storeId = request.UserId == 0 ? User.Identity.GetUserId() : request.UserId;
            request.RoleType = string.IsNullOrWhiteSpace(request.RoleType) ? User.Identity.GetUserRoleName() : request.RoleType;
            var data = await store.GetListOfIssueMeters<ContractorMeters>(request, storeId);
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, data));
        }

        [Route("UpdateReturnMeter")]
        [HttpPost]
        public async Task<IActionResult> UpdateReturnMeter([FromBody] ListOfConsumerId data)
        {
            var ids = basicUtility.GenrateCommaStringIds(data.ID);
            await store.UpdateReturnMeter(data.AreaType, ids);
            return Ok(jsonResponseFromat.JsonMessage(true, UpdateSuccess));
        }

        [Route("GetReturnMeter")]
        [HttpPost]
        public async Task<IActionResult> GetReturnMeter([FromBody] AgGridTable request)
        {
            var data = await store.GetRetunMeters<ContractorMeters>(request, User.Identity.GetUserId());
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, data));
        }

        [Route("DeleteData")]
        [HttpPost]
        public async Task<IActionResult> DeleteData([FromBody] DeleteRequest request)
        {
            string AuthenticationPassword = configuration.GetSection("Authentication")["Password"];
            if (AuthenticationPassword != request.Password)
                return Ok(jsonResponseFromat.JsonMessage(false, PasswordNotMatch));

            if (string.IsNullOrWhiteSpace(request.DeletedBy) || request.KeyValues == null || request.KeyValues.Count == 0)
            {
                return BadRequest(jsonResponseFromat.JsonMessage(false, "DeletedBy and KeyValues are required."));
            }

            await store.DeleteDataWithBackupProcedure(
                request.ActionType,
                request.KeyValues,
                request.DeletedBy
            );

            return Ok(jsonResponseFromat.JsonMessage(true, "Data deleted successfully"));
        }


        [Route("GetContractorData")]
        [HttpGet]

        public async Task<IActionResult> GetContractorData()
        {
            var data = await store.GetContractorData(User.Identity.GetUserId(), User.Identity.GetUserRoleName());
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, data));
        }

        // ================== HES Meter Replacement ==================

        [Route("FetchAndSaveMeterReplacementAccount")]
        [HttpPost]
        public async Task<IActionResult> FetchAndSaveMeterReplacementAccount([FromQuery] string accountId)
        {
            if (string.IsNullOrWhiteSpace(accountId))
                return BadRequest(jsonResponseFromat.JsonMessage(false, "Account ID is required"));

            // 1️⃣ Fetch dynamic data
            var jsonData = await store.GetMeterReplacementDataDynamicAsync(accountId);
            if (jsonData == null)
                return NotFound(jsonResponseFromat.JsonMessage(false, $"No data found for Account ID {accountId}"));

            // 2️⃣ Convert JsonElement → Dictionary<string, string>
            var dynamicData = new Dictionary<string, string>();
            foreach (var prop in jsonData.Value.EnumerateObject())
                dynamicData[prop.Name] = prop.Value.ToString();

            // Always include AccountId
            dynamicData["AccountId"] = accountId;

            // 3️⃣ Save dynamic data
            await store.SaveMeterReplacementDataDynamicAsync(dynamicData);

            // 4️⃣ Return full data
            return Ok(jsonResponseFromat.JsonMessage(true,
                "Account data fetched and saved successfully",
                dynamicData));
        }

        [HttpPut]
        [Route("UpdateRemark")]
        public async Task<IActionResult> UpdateConsumerRemark([FromBody] ConsumerRemarkUpdate model)
        {
            if (string.IsNullOrWhiteSpace(model.ConsumerId))
                return Ok(jsonResponseFromat.JsonMessage(false, "ConsumerId is required"));

            if (string.IsNullOrWhiteSpace(model.TableName))
                return Ok(jsonResponseFromat.JsonMessage(false, "TableName is required"));

            if (string.IsNullOrWhiteSpace(model.Remark))
                return Ok(jsonResponseFromat.JsonMessage(false, "Remark is required"));

            // Allowed tables
            var validTables = new[]
            {
        "ConsumerNewMeterDetails",
        "DTCNewMeterDetails",
        "FeederNewMeterDetails",
        "DTNewMeterDetails",
        "HTCNewMeterDetails"
    };

            if (!validTables.Contains(model.TableName))
                return Ok(jsonResponseFromat.JsonMessage(false, "Invalid table name"));

            // Call store method
            var result = await store.UpdateConsumerRemark(
                model.ConsumerId,
                model.TableName,
                model.Remark,
                model.RemarkDescription,
                 User
            );

            if (result != "SUCCESS")
                return Ok(jsonResponseFromat.JsonMessage(false, result));

            return Ok(jsonResponseFromat.JsonMessage(true, "Remark updated"));
        }


        [HttpPost("UpdateReplacementMeter")]
        public async Task<IActionResult> UpdateReplacementMeter(
     [FromForm] UpdateReplacementMeterRequest request)
        {
            if (request == null)
                return BadRequest(new
                {
                    Status = false,
                    Message = "Request body is missing."
                });

            if (string.IsNullOrWhiteSpace(request.AreaType))
                return BadRequest(new
                {
                    Status = false,
                    Message = "AreaType is required."
                });

            if (string.IsNullOrWhiteSpace(request.AreaValue))
                return BadRequest(new
                {
                    Status = false,
                    Message = "Old meter number (AreaValue) is required."
                });

            if (string.IsNullOrWhiteSpace(request.NewAreaValue))
                return BadRequest(new
                {
                    Status = false,
                    Message = "New meter number (NewAreaValue) is required."
                });

            if (string.IsNullOrWhiteSpace(request.NewinitialNetMeterImportReadingKWH))
                return BadRequest(new
                {
                    Status = false,
                    Message = "NewinitialNetMeterImportReadingKWH is required."
                });

            if (string.IsNullOrWhiteSpace(request.NewinitialNetMeterExportReadingKWH))
                return BadRequest(new
                {
                    Status = false,
                    Message = "NewinitialNetMeterExportReadingKWH is required."
                });

            if (string.IsNullOrWhiteSpace(request.NewinitialNetMeterImportReadingKVAH))
                return BadRequest(new
                {
                    Status = false,
                    Message = "NewinitialNetMeterImportReadingKVAH is required."
                });

            if (string.IsNullOrWhiteSpace(request.NewinitialNetMeterExportReadingKVAH))
                return BadRequest(new
                {
                    Status = false,
                    Message = "NewinitialNetMeterExportReadingKVAH is required."
                });

            if (string.IsNullOrWhiteSpace(request.OldinitialNetMeterImportReadingKWH))
                return BadRequest(new
                {
                    Status = false,
                    Message = "OldinitialNetMeterImportReadingKWH is required."
                });

            if (string.IsNullOrWhiteSpace(request.OldinitialNetMeterExportReadingKWH))
                return BadRequest(new
                {
                    Status = false,
                    Message = "OldinitialNetMeterExportReadingKWH is required."
                });

            if (string.IsNullOrWhiteSpace(request.OldinitialNetMeterImportReadingKVAH))
                return BadRequest(new
                {
                    Status = false,
                    Message = "OldinitialNetMeterImportReadingKVAH is required."
                });

            if (string.IsNullOrWhiteSpace(request.OldinitialNetMeterExportReadingKVAH))
                return BadRequest(new
                {
                    Status = false,
                    Message = "OldinitialNetMeterExportReadingKVAH is required."
                });

            try
            {
                // Upload old meter photo
                string oldMeterPhotoUrl = null;
                if (request.OldMeterPhoto != null && request.OldMeterPhoto.Length > 0)
                {
                    oldMeterPhotoUrl = await basicUtility.UploadFile(
                        request.OldMeterPhoto,
                        "ReplacementMeter",
                        AllowedFileExtensions.GetAllowedFile("Image"));

                    if (string.IsNullOrEmpty(oldMeterPhotoUrl))
                        return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
                }

                // Upload new meter photo
                string newMeterPhotoUrl = null;
                if (request.NewMeterPhoto != null && request.NewMeterPhoto.Length > 0)
                {
                    newMeterPhotoUrl = await basicUtility.UploadFile(
                        request.NewMeterPhoto,
                        "ReplacementMeter",
                        AllowedFileExtensions.GetAllowedFile("Image"));

                    if (string.IsNullOrEmpty(newMeterPhotoUrl))
                        return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
                }

                await store.UpdateReplacementMeter(
                    request.AreaType,
                    request.AreaValue,
                    request.NewAreaValue,
                    request.NewinitialNetMeterImportReadingKWH,
                    request.NewinitialNetMeterExportReadingKWH,
                    request.NewinitialNetMeterImportReadingKVAH,
                    request.NewinitialNetMeterExportReadingKVAH,
                    request.OldinitialNetMeterImportReadingKWH,
                    request.OldinitialNetMeterExportReadingKWH,
                    request.OldinitialNetMeterImportReadingKVAH,
                    request.OldinitialNetMeterExportReadingKVAH,
                    oldMeterPhotoUrl,
                    newMeterPhotoUrl
                );

                return Ok(new
                {
                    Status = true,
                    Message = "Replacement meter updated successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = false,
                    Message = "An error occurred while updating replacement meter.",
                    Error = ex.Message
                });
            }
        }


        [HttpGet("GetReplacementMeters")]
        public async Task<IActionResult> GetReplacementMeters()
        {
            var result = await store.GetReplacementMeters<object>();
            return Ok(result);
        }

        // Latest readings of the old meter from HES (backend proxy - server handles auth)
        [HttpGet("GetOldMeterReadingsFromHes")]
        public async Task<IActionResult> GetOldMeterReadingsFromHes([FromQuery] string meterNo)
        {
            if (string.IsNullOrWhiteSpace(meterNo))
                return BadRequest(new { Status = false, Message = "meterNo is required." });

            try
            {
                var result = await store.GetOldMeterReadingsFromHesAsync(meterNo.Trim());

                if (string.IsNullOrEmpty(result))
                    return StatusCode(500, new { Status = false, Message = "HES readings fetch failed." });

                // HES ka raw JSON as-is pass-through (Angular ko exactly wahi shape milega)
                return Content(result, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = false, Message = "HES readings fetch failed.", Error = ex.Message });
            }
        }



        [AutomaticRetry(Attempts = 0)]
        [NonAction]
        public async Task SyncIssueMeterByMeterNo(int userId)
        {
            await store.SyncIssueMeterByMeterNo(userId);
        }
    }
}
