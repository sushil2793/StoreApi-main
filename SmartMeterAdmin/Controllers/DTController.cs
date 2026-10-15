using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMeterAdmin.Infrastructure.Helpers;
using SmartMeterAdmin.Services.DT;
using SmartMeterAdmin.ViewModel.DT;
using static SmartMeterAdmin.Infrastructure.Helpers.ConstantValues;
using SmartmeterAdmin.Hangfire.ExcelJobs;
using SmartMeterAdmin.Infrastructure.Enumeration;
using Microsoft.AspNetCore.Hosting;
using Hangfire;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SmartMeterAdmin.ViewModel.DataTable;
using SmartMeterAdmin.ViewModel.DTCSurvey;

namespace SmartMeterAdmin.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/DT")]
    public class DTController : ControllerBase
    {
        private readonly IWebHostEnvironment env;
        private readonly IConfiguration configuration;
        private readonly IJsonResponseFromat jsonResponseFromat;
        private readonly IBasicUtility basicUtility;
        private readonly IDT dt;
        private readonly ILogger<DTController> logger;

        public DTController(IWebHostEnvironment env, IConfiguration configuration, IJsonResponseFromat jsonResponseFromat, IBasicUtility basicUtility, IDT dt, ILogger<DTController> logger)
        {
            this.env = env;
            this.configuration = configuration;
            this.jsonResponseFromat = jsonResponseFromat;
            this.basicUtility = basicUtility;
            this.dt = dt;
            this.logger = logger;
        }

        [HttpPost]
        [Route("ImportDTDataByExcel")]
        public IActionResult ImportDTDataByExcel(IFormFile postedFile)
        {
            if (postedFile != null)
            {
                int userId = User.Identity.GetUserId();
                // Create a Folder
                string path = Path.Combine(env.WebRootPath, "DTDataExcel");
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
                var jobId = BackgroundJob.Enqueue<ImportExcelDataIntoDatabase>(x => x.ImportExcelData(filePath, User.Identity.GetUserId(), TableNameEnum.TempDTSurvey));
                BackgroundJob.ContinueJobWith(jobId, () => SyncDTData(User.Identity.GetUserId()));

                return Ok(jsonResponseFromat.JsonMessage(true, FileUploaded));
            }

            return Ok(jsonResponseFromat.JsonMessage(false, SelectValidExcel));
        }

        [HttpPut]
        [Route("UpdateDTDetails")]
        public async Task<IActionResult> UpdateDTDetails([FromForm] DTDetails dtDetails)
        {

            var fileUrl = string.Empty;
            if (dtDetails.MeterPhoto is not null)
            {
                fileUrl = await basicUtility.UploadFile(dtDetails.MeterPhoto, "DTMeterSurvey", AllowedFileExtensions.GetAllowedFile("Image"));

                if (string.IsNullOrEmpty(fileUrl))
                    return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
            }
            dtDetails.MeterSitePhoto = fileUrl;
            dtDetails.SurveyAgencyName = User.Identity.GetCompanyName();
            dtDetails.Action = "UPDATE";
            await dt.SaveDTDetails(dtDetails, User.Identity.GetUserId());
            return Ok(jsonResponseFromat.JsonMessage(true, CreatedSuccess));
        }


        [HttpPost]
        [Route("SaveDTDetails")]
        public async Task<IActionResult> SaveDTDetails([FromForm] DTDetails dtDetails)
        {
            logger.LogCritical("SaveDTDetails Data : {payLoad}",
                JsonConvert.SerializeObject(dtDetails));

            if (dtDetails == null)
                return Ok(jsonResponseFromat.JsonMessage(false, SomethingWrong));

            // 🔴 MANDATORY FIELDS VALIDATION (ONE MESSAGE)
            if (
                !Request.Form.ContainsKey("ZoneName") ||
                !Request.Form.ContainsKey("CircleName") ||
                !Request.Form.ContainsKey("DivisionName") ||
                !Request.Form.ContainsKey("SubDivisionName") ||
                !Request.Form.ContainsKey("FeedingSubStationName") ||
                !Request.Form.ContainsKey("FeedingSubStationCode") ||
                !Request.Form.ContainsKey("FeederName") ||
                !Request.Form.ContainsKey("FeederCode") ||
                !Request.Form.ContainsKey("Latitude") ||
                !Request.Form.ContainsKey("Longitude") ||

                string.IsNullOrWhiteSpace(dtDetails.ZoneName) ||
                string.IsNullOrWhiteSpace(dtDetails.CircleName) ||
                string.IsNullOrWhiteSpace(dtDetails.DivisionName) ||
                string.IsNullOrWhiteSpace(dtDetails.SubDivisionName) ||
                string.IsNullOrWhiteSpace(dtDetails.FeedingSubStationName) ||
                string.IsNullOrWhiteSpace(dtDetails.FeedingSubStationCode) ||
                string.IsNullOrWhiteSpace(dtDetails.FeederName) ||
                string.IsNullOrWhiteSpace(dtDetails.FeederCode) ||
                string.IsNullOrWhiteSpace(dtDetails.Latitude) ||
                string.IsNullOrWhiteSpace(dtDetails.Longitude)
               )
            {
                return Ok(jsonResponseFromat.JsonMessage(
                    false,
                    "Please fill all mandatory fields."
                ));
            }

            // 📷 File upload
            var fileUrl = string.Empty;
            if (dtDetails.MeterPhoto != null)
            {
                fileUrl = await basicUtility.UploadFile(
                    dtDetails.MeterPhoto,
                    "DTMeterSurvey",
                    AllowedFileExtensions.GetAllowedFile("Image"));

                if (string.IsNullOrEmpty(fileUrl))
                    return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
            }

            dtDetails.MeterSitePhoto = fileUrl;
            dtDetails.SurveyAgencyName = User.Identity.GetCompanyName();
            dtDetails.Action = "INSERT";

            await dt.SaveDTDetails(dtDetails, User.Identity.GetUserId());

            return Ok(jsonResponseFromat.JsonMessage(true, CreatedSuccess));
        }




        [HttpGet]
        [Route("GetDTListForApp")]
        public async Task<IActionResult> GetDTListForApp(DateTime? fromDate, DateTime? toDate)
        {
            var request = new AgGridTable
            {
                UserId = User.Identity.GetUserId(),
                FromDate = fromDate ?? DateTime.Now.AddDays(-30),
                ToDate = toDate ?? DateTime.Now,
                RoleType = User.Identity.GetUserRoleName()
            };
            var data = await dt.GetDTList(request);
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, data));
        }

        [HttpGet]
        [Route("GetDTDetails")]
        public async Task<IActionResult> GetDTDetails(string customerId)
        {
            var data = await dt.GetDTDetails(customerId);
            return Ok(jsonResponseFromat.JsonMessage(true, DataDetails, data));
        }

        [Route("GetAllDTMeterRender")]
        [HttpPost]
        public async Task<IActionResult> GetAllDTMeterRender([FromBody] AgGridTable dataTable)
        {
            dataTable.UserId = dataTable.UserId == 0 ? User.Identity.GetUserId() : dataTable.UserId;
            dataTable.RoleType = string.IsNullOrEmpty(dataTable.RoleType) ? User.Identity.GetUserRoleName() : dataTable.RoleType;

            var data = await dt.GetDTMetersList( dataTable);
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, data));
        }

        [Route("GetAllDTDataRender")]
        [HttpPost]
        public async Task<IActionResult> GetAllDTDataRender([FromBody] AgGridTable dataTable)
        {
            var data = await dt.GetDTDataList(User.Identity.GetUserRoleName(), User.Identity.GetUserId(), dataTable);
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, data));
        }

        [HttpDelete]
        [Route("DeleteDTRecord")]
        public async Task<IActionResult> DeleteDTRecord(string consumerId, string password)
        {
            string AuthenticationPassword = configuration.GetSection("Authentication")["Password"];
            if (AuthenticationPassword != password)
                return Ok(jsonResponseFromat.JsonMessage(false, PasswordNotMatch));

            await dt.DeleteDTRecord(consumerId);
            return Ok(jsonResponseFromat.JsonMessage(true, DeletedSuccess));
        }

        [AutomaticRetry(Attempts = 0)]
        [NonAction]
        public async Task SyncDTData(int userId)
        {
            // Sync process should handle BoxID updates
            await dt.SyncDTSurvey(userId);
        }
    }
}
