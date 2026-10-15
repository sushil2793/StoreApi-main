using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMeterAdmin.Infrastructure.Helpers;
using SmartMeterAdmin.Services.Survey;
using SmartMeterAdmin.ViewModel.Survey;
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
using SmartMeterAdmin.ViewModel.Feeder;

namespace SmartMeterAdmin.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/Survey")]
    public class SurveyController : ControllerBase
    {
        private readonly IJsonResponseFromat jsonResponseFromat;
        private readonly IBasicUtility basicUtility;
        private readonly ISurvey survey;
        private readonly IWebHostEnvironment env;
        private readonly ILogger<SurveyController> logger;

        public SurveyController(IJsonResponseFromat jsonResponseFromat, IBasicUtility basicUtility, ISurvey survey, IWebHostEnvironment env, ILogger<SurveyController> logger)
        {
            this.jsonResponseFromat = jsonResponseFromat;
            this.basicUtility = basicUtility;
            this.survey = survey;
            this.env = env;
            this.logger = logger;
        }
        [HttpPost]
        [Route("ImportSurveyByExcel")]
        public IActionResult ImportSurveyByExcel(IFormFile postedFile)
        {
            if (postedFile == null)
                return Ok(jsonResponseFromat.JsonMessage(false, SelectValidExcel));

            int userId = User.Identity.GetUserId();

            // 1️⃣ Save uploaded file
            string path = Path.Combine(env.WebRootPath, "TempConsumerSurveyAllTable");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string fileName = Path.GetFileNameWithoutExtension(postedFile.FileName);
            string extension = Path.GetExtension(postedFile.FileName);

            if (!AllowedFileExtensions.GetAllowedFile("Excel").Contains(extension))
                return Ok(jsonResponseFromat.JsonMessage(false, SelectValidExcel));

            string filePath = Path.Combine(path, fileName + DateTime.Now.ToString("yyyyMMddHHmmss") + extension);
            using (FileStream stream = new(filePath, FileMode.Create))
            {
                postedFile.CopyTo(stream);
            }

            // 2️⃣ Enqueue Hangfire job with all required parameters
            BackgroundJob.Enqueue<ImportExcelDataIntoDatabase>(
                x => x.ImportExcelData(filePath, userId, TableNameEnum.TempSurveyAllTable)
            );

            // 3️⃣ Return immediate success response
            return Ok(jsonResponseFromat.JsonMessage(true, "File uploaded successfully. Processing in background."));
        }

        [HttpPost]
        [Route("ImportConsumerSurveyByExcel")]
        public IActionResult ImportConsumerSurveyByExcel(IFormFile postedFile)
        {
            if (postedFile != null)
            {
                int userId = User.Identity.GetUserId();
                // Create a Folder
                string path = Path.Combine(env.WebRootPath, "SurveyExcel");
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
                var jobId = BackgroundJob.Enqueue<ImportExcelDataIntoDatabase>(x => x.ImportExcelData(filePath, User.Identity.GetUserId(), TableNameEnum.TempConsumerSurvey));
                BackgroundJob.ContinueJobWith(jobId, () => SyncSurvey(User.Identity.GetUserId()));
                return Ok(jsonResponseFromat.JsonMessage(true, FileUploaded));
            }

            return Ok(jsonResponseFromat.JsonMessage(false, SelectValidExcel));
        }

        [HttpPost]
        [Route("SaveSurveyDetails")]
        public async Task<IActionResult> SaveSurveyDetails([FromForm] SurveyDetails surveyDetails)
        {
            logger.LogCritical("SaveSurveyDetails Data : {payLoad}",
                JsonConvert.SerializeObject(surveyDetails));

            if (surveyDetails == null)
                return Ok(jsonResponseFromat.JsonMessage(false, SomethingWrong));

            // 🔴 ONE common mandatory validation
            bool isInvalid =
                surveyDetails.Latitude == null ||
                surveyDetails.Longitude == null ||
                string.IsNullOrWhiteSpace(surveyDetails.Zone) ||
                string.IsNullOrWhiteSpace(surveyDetails.CircleWise) ||
                string.IsNullOrWhiteSpace(surveyDetails.Division) ||
                string.IsNullOrWhiteSpace(surveyDetails.Subdivision) ||
                string.IsNullOrWhiteSpace(surveyDetails.Substation) ||
                string.IsNullOrWhiteSpace(surveyDetails.SubstationCode) ||
                string.IsNullOrWhiteSpace(surveyDetails.DTFeeder) ||
                string.IsNullOrWhiteSpace(surveyDetails.DTFeederCode) ||
                string.IsNullOrWhiteSpace(surveyDetails.DTName) ||
                string.IsNullOrWhiteSpace(surveyDetails.DTCode) ||
                string.IsNullOrWhiteSpace(surveyDetails.SanctionLoad);


            if (isInvalid)
            {
                return Ok(jsonResponseFromat.JsonMessage(
                    false,
                    "Please fill all mandatory fields."
                ));
            }

            var fileUrl = string.Empty;

            if (surveyDetails.MeterPhotoFile != null)
            {
                fileUrl = await basicUtility.UploadFile(
                    surveyDetails.MeterPhotoFile,
                    "OldMeterSurvey",
                    AllowedFileExtensions.GetAllowedFile("Image"));

                if (string.IsNullOrEmpty(fileUrl))
                    return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
            }

            if (surveyDetails.MeterPhotoFileKVAH != null)
            {
                fileUrl = await basicUtility.UploadFile(
                    surveyDetails.MeterPhotoFileKVAH,
                    "OldMeterSurvey",
                    AllowedFileExtensions.GetAllowedFile("Image"));

                if (string.IsNullOrEmpty(fileUrl))
                    return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
            }

            surveyDetails.MeterPhotoUrl = fileUrl;
            surveyDetails.MeterPhotoUrlKVAH = fileUrl;
            surveyDetails.SurveyAgency = User.Identity.GetCompanyName();
            surveyDetails.Action = "INSERT";

            await survey.SaveSurveyDetails(surveyDetails, User.Identity.GetUserId());

            return Ok(jsonResponseFromat.JsonMessage(true, CreatedSuccess));
        }


        [HttpPut]
        [Route("UpdateSurveyDetails")]
        public async Task<IActionResult> UpdateSurveyDetails([FromForm] SurveyDetails surveyDetails)
        {

            var fileUrl = string.Empty;
            if (surveyDetails.MeterPhotoFile is not null)
            {
                fileUrl = await basicUtility.UploadFile(surveyDetails.MeterPhotoFile, "OldMeterSurvey", AllowedFileExtensions.GetAllowedFile("Image"));

                if (string.IsNullOrEmpty(fileUrl))
                    return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
            }
            surveyDetails.MeterPhotoUrl = fileUrl;
            surveyDetails.SurveyAgency = User.Identity.GetCompanyName();
            surveyDetails.Action = "UPDATE";
            await survey.SaveSurveyDetails(surveyDetails, User.Identity.GetUserId());
            return Ok(jsonResponseFromat.JsonMessage(true, UpdateSuccess));
        }

        [HttpGet]
        [Route("GetSurveyListForApp")]
        public async Task<IActionResult> GetSurveyListForApp(DateTime? fromDate, DateTime? toDate)
        {
            var request = new AgGridTable
            {
                UserId = User.Identity.GetUserId(),
                FromDate = fromDate ?? DateTime.Now.AddDays(-30),
                ToDate = toDate ?? DateTime.Now,
                RoleType = User.Identity.GetUserRoleName()
            };
            var data = await survey.GetSurveyList(request);
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, data));
        }

        [HttpGet]
        [Route("GetSurveyDetails")]
        public async Task<IActionResult> GetSurveyDetails(string customerId)
        {

            var data = await survey.GetSurveyDetails(customerId);
            return Ok(jsonResponseFromat.JsonMessage(true, DataDetails, data));


        }

        [Route("GetAllSurveyUtilityData")]
        [HttpPost]
        public async Task<IActionResult> GetAllSurveyUtilityData([FromBody] AgGridTable dataTable)
        {

            var data = await survey.GetSurveyUtilityData(User.Identity.GetUserRoleName(), User.Identity.GetUserId(), dataTable);
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, data));

        }

        [Route("GetAllSurveyMeterRender")]
        [HttpPost]
        public async Task<IActionResult> GetAllSurveyMeterRender([FromBody] AgGridTable dataTable)
        {

            dataTable.UserId = dataTable.UserId == 0 ? User.Identity.GetUserId() : dataTable.UserId;
            dataTable.RoleType = string.IsNullOrEmpty(dataTable.RoleType) ? User.Identity.GetUserRoleName() : dataTable.RoleType;

            var data = await survey.GetSurveyMetersList(dataTable);
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, data));
        }

        [Route("GetSurveyDashboardCount")]
        [HttpGet]
        public async Task<IActionResult> GetSurveyDashboardCount(int userId,string roleType)
        {
            userId = userId == 0 ? User.Identity.GetUserId() : userId;
            roleType = string.IsNullOrEmpty(roleType) ? User.Identity.GetUserRoleName() : roleType; 
            var data = await survey.GetSurveyDashboardCount(roleType, userId);
            return Ok(jsonResponseFromat.JsonMessage(true, DataDetails, data));
        }

        [AutomaticRetry(Attempts = 0)]
        [NonAction]
        public async Task SyncSurvey(int userId)
        {
            await survey.SyncSurvey(userId);
        }
    }
}
