using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartmeterAdmin.Hangfire.ExcelJobs;
using SmartMeterAdmin.Infrastructure.Helpers;
using SmartMeterAdmin.Services.Feeder;
using SmartMeterAdmin.Infrastructure.Enumeration;
using SmartMeterAdmin.ViewModel.Feeder;
using static SmartMeterAdmin.Infrastructure.Helpers.ConstantValues;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using SmartMeterAdmin.ViewModel.DataTable;
namespace SmartMeterAdmin.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/Feeder")]
    public class FeederController : ControllerBase
    {
        private readonly IWebHostEnvironment env;
        private readonly IConfiguration configuration;
        private readonly IJsonResponseFromat jsonResponseFromat;
        private readonly IBasicUtility basicUtility;
        private readonly IFeeder feeder;
        private readonly ILogger<FeederController> logger;

        public FeederController(IWebHostEnvironment env, IConfiguration configuration, IJsonResponseFromat jsonResponseFromat, IBasicUtility basicUtility, IFeeder feeder, ILogger<FeederController> logger)
        {
            this.env = env;
            this.configuration = configuration;
            this.jsonResponseFromat = jsonResponseFromat;
            this.basicUtility = basicUtility;
            this.feeder = feeder;
            this.logger = logger;
        }
        [HttpPost]
        [Route("ImportFeederDataByExcel")]
        public IActionResult ImportFeederDataByExcel(IFormFile postedFile)
        {
            if (postedFile != null)
            {
                int userId = User.Identity.GetUserId();
                // Create a Folder
                string path = Path.Combine(env.WebRootPath, "FeederDataExcel");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                // Save the uploaded Excel file
                string fileName = Path.GetFileNameWithoutExtension(postedFile.FileName);
                string extension = Path.GetExtension(postedFile.FileName);

                if (!AllowedFileExtensions.GetAllowedFile("Excel").Contains(extension))
                    return Ok(jsonResponseFromat.JsonMessage(false, SelectValidExcel));

                string filePath = Path.Combine(path, fileName + DateTime.Now.ToString("yyyyMMddHHmmss") + extension);
                using (FileStream stream = new (filePath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                // Background Processing
                var jobId = BackgroundJob.Enqueue<ImportExcelDataIntoDatabase>(x => x.ImportExcelData(filePath, User.Identity.GetUserId(), TableNameEnum.TempFeederSurvey));
                BackgroundJob.ContinueJobWith(jobId, () => SyncFeederData(User.Identity.GetUserId()));

                return Ok(jsonResponseFromat.JsonMessage(true, FileUploaded));
            }

            return Ok(jsonResponseFromat.JsonMessage(false, SelectValidExcel));
        }

        [HttpPost]
        [Route("SaveFeederDetails")]
        public async Task<IActionResult> SaveFeederDetails([FromForm] FeederDetails feederDetails)
        {
            logger.LogCritical("SaveFeederDetails Data : {payLoad}",
                JsonConvert.SerializeObject(feederDetails));

            if (feederDetails == null)
                return Ok(jsonResponseFromat.JsonMessage(false, SomethingWrong));

            // 🔴 MANDATORY FIELDS VALIDATION (ONE MESSAGE)
            if (
                !Request.Form.ContainsKey("CTRating") ||
                !Request.Form.ContainsKey("CTConnectedRatio") ||
                !Request.Form.ContainsKey("PTRating") ||
                !Request.Form.ContainsKey("PTCorrectedRatio") ||
                !Request.Form.ContainsKey("Latitude") ||
                !Request.Form.ContainsKey("Longitude") ||
                !Request.Form.ContainsKey("CircleName") ||
                !Request.Form.ContainsKey("DivisionName") ||
                !Request.Form.ContainsKey("DivisionId") ||
                !Request.Form.ContainsKey("SubDivisionName") ||
                !Request.Form.ContainsKey("SubDivisionId") ||
                !Request.Form.ContainsKey("FeedingSubStationName") ||
                !Request.Form.ContainsKey("FeedingSubStationId") ||

                string.IsNullOrWhiteSpace(feederDetails.CTRating) ||
                string.IsNullOrWhiteSpace(feederDetails.CTConnectedRatio) ||
                string.IsNullOrWhiteSpace(feederDetails.PTRating) ||
                string.IsNullOrWhiteSpace(feederDetails.PTCorrectedRatio) ||
                string.IsNullOrWhiteSpace(feederDetails.Latitude) ||
                string.IsNullOrWhiteSpace(feederDetails.Longitude) ||
                string.IsNullOrWhiteSpace(feederDetails.CircleName) ||
                string.IsNullOrWhiteSpace(feederDetails.DivisionName) ||
                string.IsNullOrWhiteSpace(feederDetails.DivisionId) ||
                string.IsNullOrWhiteSpace(feederDetails.SubDivisionName) ||
                string.IsNullOrWhiteSpace(feederDetails.SubDivisionId) ||
                string.IsNullOrWhiteSpace(feederDetails.FeedingSubStationName) ||
                string.IsNullOrWhiteSpace(feederDetails.FeedingSubStationId)
               )
            {
                return Ok(jsonResponseFromat.JsonMessage(
                    false,
                    "Please fill all mandatory fields."
                ));
            }

            // 📷 File upload
            var fileUrl = string.Empty;
            if (feederDetails.MeterPhotoFile != null)
            {
                fileUrl = await basicUtility.UploadFile(
                    feederDetails.MeterPhotoFile,
                    "CTPTMeter",
                    AllowedFileExtensions.GetAllowedFile("Image"));

                if (string.IsNullOrEmpty(fileUrl))
                    return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
            }

            feederDetails.CTPTMeterPhoto = fileUrl;
            feederDetails.Action = "INSERT";

            try
            {
                await feeder.SaveFeederDetails(feederDetails, User.Identity.GetUserId());
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(jsonResponseFromat.JsonMessage(true, CreatedSuccess));
        }

        [HttpPut]
        [HttpPut]
        [Route("UpdateFeederDetails")]
        public async Task<IActionResult> UpdateFeederDetails([FromForm] FeederDetails feederDetails)
        {
            try
            {
                // Handle photo upload if provided
                if (feederDetails.MeterPhotoFile is not null)
                {
                    var fileUrl = await basicUtility.UploadFile(
                        feederDetails.MeterPhotoFile,
                        "CTPTMeter",
                        AllowedFileExtensions.GetAllowedFile("Image")
                    );

                    if (string.IsNullOrEmpty(fileUrl))
                        return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));

                    feederDetails.CTPTMeterPhoto = fileUrl;
                }

                // Get current user id
                var userId = User.Identity.GetUserId();

                // Call repository (void return)
                await feeder.SaveFeederDetails(feederDetails, userId);
                feederDetails.Action = "UPDATE";

                // Always success if no exception
                return Ok(jsonResponseFromat.JsonMessage(true, "Feeder details updated successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    jsonResponseFromat.JsonMessage(false, "Error updating feeder details: " + ex.Message));
            }
        }



        [HttpGet]
        [Route("GetFeederListForApp")]
        public async Task<IActionResult> GetFeederListForApp(DateTime? fromDate, DateTime? toDate)
        {
            var request = new AgGridTable
            {
                UserId = User.Identity.GetUserId(),
                FromDate = fromDate ?? DateTime.Now.AddDays(-30),
                ToDate = toDate ?? DateTime.Now,
                RoleType = User.Identity.GetUserRoleName()
            };

            var data = await feeder.GetFeederList(request);
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, data));

        }

        [HttpGet]
        [Route("GetFeederDetails")]
        public async Task<IActionResult> GetFeederDetails(string customerId, string meterNo)
        {
            var data = await feeder.GetFeederDetails(customerId, meterNo);
            return Ok(jsonResponseFromat.JsonMessage(true, DataDetails, data));
        }

        [Route("GetAllFeederMeterRender")]
        [HttpPost]
        public async Task<IActionResult> GetAllFeederMeterRender([FromBody] AgGridTable dataTable)
        {

            dataTable.UserId = dataTable.UserId == 0 ? User.Identity.GetUserId() : dataTable.UserId;
            dataTable.RoleType = string.IsNullOrEmpty(dataTable.RoleType) ? User.Identity.GetUserRoleName() : dataTable.RoleType;

            var data = await feeder.GetFeederMetersList(dataTable);
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, data));
        }

        [Route("GetAllFeederDataRender")]
        [HttpPost]
        public async Task<IActionResult> GetAllFeederDataRender([FromBody] AgGridTable dataTable)
        {
            var data = await feeder.GetFeederDataList(User.Identity.GetUserRoleName(), User.Identity.GetUserId(), dataTable);
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, data));
        }

        [HttpDelete]
        [Route("DeleteFeederRecord")]
        public async Task<IActionResult> DeleteFeederRecord(string consumerId) // Update parameter name
        {
            await feeder.DeleteFeederRecord(consumerId); // Update parameter name
            return Ok(jsonResponseFromat.JsonMessage(true, DeletedSuccess));
        }
        [AutomaticRetry(Attempts = 0)]
        [NonAction]
        public async Task SyncFeederData(int userId)
        {
            // Sync process should handle BoxID updates
            await feeder.SyncFeederData(userId);
        }
    }
}
