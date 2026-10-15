using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMeterAdmin.Infrastructure.Helpers;
using SmartMeterAdmin.Services.DTCSurvey;
using SmartMeterAdmin.ViewModel.DTCSurvey;
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
using SmartMeterAdmin.ViewModel.Survey;
using SmartMeterAdmin.Services.HTCSurvey;
using SmartMeterAdmin.ViewModel.HTCSurvey;
using SmartMeterAdmin.ViewModel.DT;
using SmartMeterAdmin.ViewModel.Feeder;

namespace SmartMeterAdmin.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/DTC")]
    public class DTCSurveyController : ControllerBase
    {
        private readonly IJsonResponseFromat jsonResponseFromat;
        private readonly IBasicUtility basicUtility;
        private readonly IDTC dtc;
        private readonly IWebHostEnvironment env;
        private readonly ILogger<DTCSurveyController> logger;

        public DTCSurveyController(IJsonResponseFromat jsonResponseFromat, IBasicUtility basicUtility, IDTC dtc, IWebHostEnvironment env, ILogger<DTCSurveyController> logger)
        {
            this.jsonResponseFromat = jsonResponseFromat;
            this.basicUtility = basicUtility;
            this.dtc = dtc;
            this.env = env;
            this.logger = (ILogger<DTCSurveyController>)logger;
        }

        [HttpPost]
        [Route("ImportDTCSurveyByExcel")]
        public IActionResult ImportDTCSurveyByExcel(IFormFile postedFile)
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
                var jobId = BackgroundJob.Enqueue<ImportExcelDataIntoDatabase>(x => x.ImportExcelData(filePath, User.Identity.GetUserId(), TableNameEnum.TempDTCSurvey));
                BackgroundJob.ContinueJobWith(jobId, () => SyncDTCSurvey(User.Identity.GetUserId()));
                return Ok(jsonResponseFromat.JsonMessage(true, FileUploaded));
            }

            return Ok(jsonResponseFromat.JsonMessage(false, SelectValidExcel));
        }

        [HttpPut]
        [Route("UpdateDTCDetails")]
        public async Task<IActionResult> UpdateDTCDetails([FromForm] DTCDetails dtcDetails)
        {

            var fileUrl = string.Empty;
            if (dtcDetails.MeterPhotoFile is not null)
            {
                fileUrl = await basicUtility.UploadFile(dtcDetails.MeterPhotoFile, "DTCSurvey", AllowedFileExtensions.GetAllowedFile("Image"));

                if (string.IsNullOrEmpty(fileUrl))
                    return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
            }
            if (dtcDetails.PhotoOfMeterOfCumKVAH is not null)
            {
                fileUrl = await basicUtility.UploadFile(dtcDetails.PhotoOfMeterOfCumKVAH, "DTCSurvey", AllowedFileExtensions.GetAllowedFile("Image"));

                if (string.IsNullOrEmpty(fileUrl))
                    return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
            }
            if (dtcDetails.PhotoWithMeterCumReadingKWH is not null)
            {
                fileUrl = await basicUtility.UploadFile(dtcDetails.PhotoWithMeterCumReadingKWH, "DTCSurvey", AllowedFileExtensions.GetAllowedFile("Image"));

                if (string.IsNullOrEmpty(fileUrl))
                    return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
            }
            dtcDetails.MeterPhotoUrl = fileUrl;
            dtcDetails.PhotoOfMeterOfCumKVAHUrl = fileUrl;
            dtcDetails.PhotoWithMeterCumReadingKWHUrl = fileUrl;
            dtcDetails.SurveyAgency = User.Identity.GetCompanyName();
            dtcDetails.Action = "UPDATE";
            await dtc.SaveDTCSurvey(dtcDetails, User.Identity.GetUserId());
            return Ok(jsonResponseFromat.JsonMessage(true, UpdateSuccess));
        }

        [HttpPost]
        [Route("SaveDTCDetails")]
        public async Task<IActionResult> SaveDTCDetails([FromForm] DTCDetails dtcDetails)
        {
            logger.LogCritical("SaveDTCDetails Data : {payload}",
                JsonConvert.SerializeObject(dtcDetails));

            if (dtcDetails == null)
                return Ok(jsonResponseFromat.JsonMessage(false, SomethingWrong));

            // 🔴 MANDATORY FIELDS VALIDATION (ONE MESSAGE)
            if (
                !Request.Form.ContainsKey("SanctionLoad") ||
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

                string.IsNullOrWhiteSpace(dtcDetails.SanctionLoad) ||
                string.IsNullOrWhiteSpace(dtcDetails.Zone) ||
                string.IsNullOrWhiteSpace(dtcDetails.CircleWise) ||
                string.IsNullOrWhiteSpace(dtcDetails.Division) ||
                string.IsNullOrWhiteSpace(dtcDetails.Subdivision) ||
                string.IsNullOrWhiteSpace(dtcDetails.Substation) ||
                string.IsNullOrWhiteSpace(dtcDetails.SubstationCode) ||
                string.IsNullOrWhiteSpace(dtcDetails.DTFeeder) ||
                string.IsNullOrWhiteSpace(dtcDetails.DTFeederCode) ||
                string.IsNullOrWhiteSpace(dtcDetails.DTName) ||
                string.IsNullOrWhiteSpace(dtcDetails.DTCode) ||
                string.IsNullOrWhiteSpace(dtcDetails.Latitude) ||
                string.IsNullOrWhiteSpace(dtcDetails.Longitude)
               )
            {
                return Ok(jsonResponseFromat.JsonMessage(
                    false,
                    "Please fill all mandatory fields."
                ));
            }

            var fileUrl = string.Empty;

            if (dtcDetails.MeterPhotoFile != null)
            {
                fileUrl = await basicUtility.UploadFile(
                    dtcDetails.MeterPhotoFile,
                    "DTCSurvey",
                    AllowedFileExtensions.GetAllowedFile("Image"));

                if (string.IsNullOrEmpty(fileUrl))
                    return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
            }

            if (dtcDetails.PhotoOfMeterOfCumKVAH != null)
            {
                fileUrl = await basicUtility.UploadFile(
                    dtcDetails.PhotoOfMeterOfCumKVAH,
                    "DTCSurvey",
                    AllowedFileExtensions.GetAllowedFile("Image"));

                if (string.IsNullOrEmpty(fileUrl))
                    return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
            }

            if (dtcDetails.PhotoWithMeterCumReadingKWH != null)
            {
                fileUrl = await basicUtility.UploadFile(
                    dtcDetails.PhotoWithMeterCumReadingKWH,
                    "DTCSurvey",
                    AllowedFileExtensions.GetAllowedFile("Image"));

                if (string.IsNullOrEmpty(fileUrl))
                    return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
            }

            dtcDetails.MeterPhotoUrl = fileUrl;
            dtcDetails.PhotoOfMeterOfCumKVAHUrl = fileUrl;
            dtcDetails.PhotoWithMeterCumReadingKWHUrl = fileUrl;
            dtcDetails.SurveyAgency = User.Identity.GetCompanyName();
            dtcDetails.Action = "INSERT";

            await dtc.SaveDTCSurvey(dtcDetails, User.Identity.GetUserId());

            return Ok(jsonResponseFromat.JsonMessage(true, CreatedSuccess));
        }

        [HttpGet]
        [Route("GetDTCDetails")]
        public async Task<IActionResult> GetDTCDetails(string customerId)
        {

            var data = await dtc.GetDTCSurveyByConsumerId(customerId);
            return Ok(jsonResponseFromat.JsonMessage(true, DataDetails, data));
        }
      
        [Route("GetAllDTCMeterRender")]
        [HttpPost]
        public async Task<IActionResult> GetAllDTCMeterRender([FromBody] AgGridTable dataTable)
        {
            dataTable.UserId = dataTable.UserId == 0 ? User.Identity.GetUserId() : dataTable.UserId;
            dataTable.RoleType = string.IsNullOrEmpty(dataTable.RoleType) ? User.Identity.GetUserRoleName() : dataTable.RoleType;

            var data = await dtc.GetDTCMetersList(dataTable);
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, data));
        }

      

        [AutomaticRetry(Attempts = 0)]
        [NonAction]
        public async Task SyncDTCSurvey(int userId)
        {
            await dtc.SyncDTCSurvey(userId);
        }
    }
}
