using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMeterAdmin.Infrastructure.Helpers;
using SmartMeterAdmin.Services.HTCSurvey;
using SmartMeterAdmin.ViewModel.HTCSurvey;
using System;
using System.Threading.Tasks;
using static SmartMeterAdmin.Infrastructure.Helpers.ConstantValues;
using SmartmeterAdmin.Hangfire.ExcelJobs;
using SmartMeterAdmin.Infrastructure.Enumeration;
using Microsoft.AspNetCore.Hosting;
using Hangfire;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using SmartMeterAdmin.ViewModel.DataTable;
using DocumentFormat.OpenXml.Office2013.Excel;
using SmartMeterAdmin.ViewModel.DTCSurvey;
using SmartMeterAdmin.ViewModel.DT;
using SmartMeterAdmin.ViewModel.Survey;
using SmartMeterAdmin.ViewModel.Feeder;

namespace SmartMeterAdmin.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/HTC")]
    public class HTCSurveyController : ControllerBase
    {
        private readonly IJsonResponseFromat jsonResponseFromat;
        private readonly IBasicUtility basicUtility;
        private readonly IHTC htc;
        private readonly IWebHostEnvironment env;
        private readonly ILogger<HTCSurveyController> logger;

        public HTCSurveyController(IJsonResponseFromat jsonResponseFromat, IBasicUtility basicUtility, IHTC htc, IWebHostEnvironment env, ILogger<HTCSurveyController> logger)
        {
            this.jsonResponseFromat = jsonResponseFromat;
            this.basicUtility = basicUtility;
            this.htc = htc;
            this.env = env;
            this.logger = logger;
        }

        [HttpPost]
        [Route("ImportHTCSurveyByExcel")]
        public IActionResult ImportHTCSurveyByExcel(IFormFile postedFile)
        {
            if (postedFile != null)
            {
                int userId = User.Identity.GetUserId();
                // Create a Folder
                string path = Path.Combine(env.WebRootPath, "DTCExcel");
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
                using (FileStream stream = new(filePath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                // Background Processing
                var jobId = BackgroundJob.Enqueue<ImportExcelDataIntoDatabase>(x => x.ImportExcelData(filePath, User.Identity.GetUserId(), TableNameEnum.TempHTCSurvey));
                BackgroundJob.ContinueJobWith(jobId, () => SyncHTC(User.Identity.GetUserId()));
                return Ok(jsonResponseFromat.JsonMessage(true, FileUploaded));
            }

            return Ok(jsonResponseFromat.JsonMessage(false, SelectValidExcel));
        }

        [HttpPut]
        [Route("UpdateHTCDetails")]
        public async Task<IActionResult> UpdateHTCDetails([FromForm] HTCDetails htcDetails)
        {

            var fileUrl = string.Empty;
            if (htcDetails.MeterPhotoFile is not null)

                if (htcDetails.PhotoWithCumKVAH is not null)
                {
                    fileUrl = await basicUtility.UploadFile(htcDetails.PhotoWithCumKVAH, "HTCSurvey", AllowedFileExtensions.GetAllowedFile("Image"));

                    if (string.IsNullOrEmpty(fileUrl))
                        return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
                }
            if (htcDetails.PhotoWithCumKWH is not null)
            {
                fileUrl = await basicUtility.UploadFile(htcDetails.PhotoWithCumKWH, "htCSurvey", AllowedFileExtensions.GetAllowedFile("Image"));

                if (string.IsNullOrEmpty(fileUrl))
                    return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
            }
            if (htcDetails.PhotoWithKVRH is not null)
            {
                fileUrl = await basicUtility.UploadFile(htcDetails.PhotoWithKVRH, "htCSurvey", AllowedFileExtensions.GetAllowedFile("Image"));

                if (string.IsNullOrEmpty(fileUrl))
                    return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
            }

            htcDetails.PhotoWithCumKVAHUrl = fileUrl;
            htcDetails.PhotoWithCumKWHUrl = fileUrl;
            htcDetails.PhotoWithKVRHUrl = fileUrl;
            htcDetails.SurveyAgency = User.Identity.GetCompanyName();
            htcDetails.Action = "UPDATE";
            await htc.SaveHTCDetails(htcDetails, User.Identity.GetUserId());
            return Ok(jsonResponseFromat.JsonMessage(true, UpdateSuccess));
        }

        [HttpPost]
        [Route("SaveHTCDetails")]
        public async Task<IActionResult> SaveHTCDetails([FromForm] HTCDetails htcDetails)
        {
            logger.LogCritical("SaveHTCDetails Data : {payload}",
                JsonConvert.SerializeObject(htcDetails));

            if (htcDetails == null)
                return Ok(jsonResponseFromat.JsonMessage(false, SomethingWrong));

            // 🔴 MANDATORY FIELD VALIDATION (ONE MESSAGE)
            if (
                !Request.Form.ContainsKey("Zone") ||
                !Request.Form.ContainsKey("CircleWise") ||
                !Request.Form.ContainsKey("Division") ||
                !Request.Form.ContainsKey("Subdivision") ||
                !Request.Form.ContainsKey("Substation") ||
                !Request.Form.ContainsKey("SubstationCode") ||
                !Request.Form.ContainsKey("DTFeeder") ||
                !Request.Form.ContainsKey("DTFeederCode") ||
                !Request.Form.ContainsKey("DTName") ||
                !Request.Form.ContainsKey("DTCode") ||
                !Request.Form.ContainsKey("Latitude") ||
                !Request.Form.ContainsKey("Longitude") ||
                !Request.Form.ContainsKey("ConsumerMobileNo") ||
                !Request.Form.ContainsKey("SanctionLoad") ||
                !Request.Form.ContainsKey("CTRating") ||
                !Request.Form.ContainsKey("CTConnectedRatio") ||
                !Request.Form.ContainsKey("PTRating") ||
                !Request.Form.ContainsKey("PTConnectedRatio") ||

                string.IsNullOrWhiteSpace(htcDetails.Zone) ||
                string.IsNullOrWhiteSpace(htcDetails.CircleWise) ||
                string.IsNullOrWhiteSpace(htcDetails.Division) ||
                string.IsNullOrWhiteSpace(htcDetails.Subdivision) ||
                string.IsNullOrWhiteSpace(htcDetails.Substation) ||
                string.IsNullOrWhiteSpace(htcDetails.SubstationCode) ||
                string.IsNullOrWhiteSpace(htcDetails.DTFeeder) ||
                string.IsNullOrWhiteSpace(htcDetails.DTFeederCode) ||
                string.IsNullOrWhiteSpace(htcDetails.DTName) ||
                string.IsNullOrWhiteSpace(htcDetails.DTCode) ||
                string.IsNullOrWhiteSpace(htcDetails.Latitude) ||
                string.IsNullOrWhiteSpace(htcDetails.Longitude) ||
                string.IsNullOrWhiteSpace(htcDetails.ConsumerMobileNo) ||
                string.IsNullOrWhiteSpace(htcDetails.SanctionLoad) ||
                string.IsNullOrWhiteSpace(htcDetails.CTRating) ||
                string.IsNullOrWhiteSpace(htcDetails.CTConnectedRatio) ||
                string.IsNullOrWhiteSpace(htcDetails.PTRating) ||
                string.IsNullOrWhiteSpace(htcDetails.PTConnectedRatio)
               )
            {
                return Ok(jsonResponseFromat.JsonMessage(
                    false,
                    "Please fill all mandatory fields."
                ));
            }

            var fileUrl = string.Empty;

            if (htcDetails.MeterPhotoFile != null)
            {
                fileUrl = await basicUtility.UploadFile(
                    htcDetails.MeterPhotoFile,
                    "HTCSurvey",
                    AllowedFileExtensions.GetAllowedFile("Image"));

                if (string.IsNullOrEmpty(fileUrl))
                    return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
            }

            if (htcDetails.PhotoWithCumKVAH != null)
            {
                fileUrl = await basicUtility.UploadFile(
                    htcDetails.PhotoWithCumKVAH,
                    "HTCSurvey",
                    AllowedFileExtensions.GetAllowedFile("Image"));

                if (string.IsNullOrEmpty(fileUrl))
                    return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
            }

            if (htcDetails.PhotoWithCumKWH != null)
            {
                fileUrl = await basicUtility.UploadFile(
                    htcDetails.PhotoWithCumKWH,
                    "HTCSurvey",
                    AllowedFileExtensions.GetAllowedFile("Image"));

                if (string.IsNullOrEmpty(fileUrl))
                    return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
            }

            if (htcDetails.PhotoWithKVRH != null)
            {
                fileUrl = await basicUtility.UploadFile(
                    htcDetails.PhotoWithKVRH,
                    "HTCSurvey",
                    AllowedFileExtensions.GetAllowedFile("Image"));

                if (string.IsNullOrEmpty(fileUrl))
                    return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
            }

            htcDetails.PhotoWithCumKVAHUrl = fileUrl;
            htcDetails.PhotoWithCumKWHUrl = fileUrl;
            htcDetails.PhotoWithKVRHUrl = fileUrl;
            htcDetails.SurveyAgency = User.Identity.GetCompanyName();
            htcDetails.Action = "INSERT";

            await htc.SaveHTCDetails(htcDetails, User.Identity.GetUserId());

            return Ok(jsonResponseFromat.JsonMessage(true, CreatedSuccess));
        }


        [HttpGet]
        [Route("GetHTCDetails")]
        public async Task<IActionResult> GetHTCDetails(string customerId)
        {

            var data = await htc.GetHTCDetails(customerId);
            return Ok(jsonResponseFromat.JsonMessage(true, DataDetails, data));


        }

       
        [Route("GetAllHTCMeterRender")]
        [HttpPost]
        public async Task<IActionResult> GetAllHTCMeterRender([FromBody] AgGridTable dataTable)
        {

            dataTable.UserId = dataTable.UserId == 0 ? User.Identity.GetUserId() : dataTable.UserId;
            dataTable.RoleType = string.IsNullOrEmpty(dataTable.RoleType) ? User.Identity.GetUserRoleName() : dataTable.RoleType;

            var data = await htc.GetHTCMetersList(dataTable);
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, data));
        }

       


        [AutomaticRetry(Attempts = 0)]
        [NonAction]
        public async Task SyncHTC(int userId)
        {
            await htc.SyncHTC(userId);
        }


    }
}
