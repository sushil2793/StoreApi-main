using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartMeterAdmin.Infrastructure.Helpers;
using SmartMeterAdmin.ViewModel.MeterViewModel;
using static SmartMeterAdmin.Infrastructure.Helpers.ConstantValues;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Hangfire;
using SmartmeterAdmin.Hangfire.ExcelJobs;
using SmartmeterAdmin.Hangfire.SyncHesMeter;
using Microsoft.AspNetCore.Http;
using SmartMeterAdmin.Infrastructure.Enumeration;
using System.IO;
using System.Linq;
using SmartMeterAdmin.ViewModel.Hes;
using System;
using System.Collections.Generic;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using Microsoft.AspNetCore.Hosting;
using SmartMeterAdmin.Services.Meter;
using SmartMeterAdmin.ViewModel.DataTable;
using SmartMeterAdmin.ViewModel.Master;

namespace SmartMeterAdmin.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/Meter")]
    public class MeterController : ControllerBase
    {
        readonly IWebHostEnvironment env;
        private readonly IMeter meter;
        private readonly IJsonResponseFromat jsonResponseFromat;
        private readonly IBasicUtility basicUtility;
        private readonly ILogger<MeterController> logger;
        private readonly HesMeter hesMeter;

        public MeterController(
             ILogger<MeterController> logger,
             IMeter meter,
             IJsonResponseFromat jsonResponseFromat,
             IBasicUtility basicUtility,
             IWebHostEnvironment env,
             HesMeter hesMeter
             )
        {
            this.env = env;
            this.meter = meter;
            this.jsonResponseFromat = jsonResponseFromat;
            this.basicUtility = basicUtility;
            this.logger = logger;
            this.hesMeter = hesMeter;
        }

        #region HTCMeter New Meter

        [HttpPost]
        [Route("ImportHTCNewMeterDataByExcel")]
        public IActionResult ImportHTCNewMeterDataByExcel(IFormFile postedFile)
        {
            if (postedFile != null)
            {
                int userId = User.Identity.GetUserId();
                // Create a Folder
                string path = Path.Combine(env.WebRootPath, "HTCNewMeterExcel");
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
                var jobId = BackgroundJob.Enqueue<ImportExcelDataIntoDatabase>(x => x.ImportExcelData(filePath, User.Identity.GetUserId(), TableNameEnum.TempHTCNewMeter));
                BackgroundJob.ContinueJobWith(jobId, () => SyncConsumerNewMeterData(User.Identity.GetUserId()));

                return Ok(jsonResponseFromat.JsonMessage(true, FileUploaded));
            }

            return Ok(jsonResponseFromat.JsonMessage(false, SelectValidExcel));
        }

        [Route("GetFullNewHTCMeterRender")]
        [HttpPost]
        public async Task<IActionResult> GetFullNewHTCMeterRender([FromBody] AgGridTable dataTable)
        {

            dataTable.UserId = dataTable.UserId == 0 ? User.Identity.GetUserId() : dataTable.UserId;
            dataTable.RoleType = string.IsNullOrEmpty(dataTable.RoleType) ? User.Identity.GetUserRoleName() : dataTable.RoleType;

            var result = await meter.GetFullNewHTCMeterRender(dataTable);
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, result));
        }
       
        [Route("GetHTCNewMeterByConsumerId")]
        [HttpGet]
        public async Task<IActionResult> GetHTCNewMeterByConsumerId(string consumerId)
        {
            if (string.IsNullOrEmpty(consumerId))
                return BadRequest(jsonResponseFromat.JsonMessage(false, "ConsumerId is required", null));

            var data = await meter.GetHTCNewMeterByConsumerId(consumerId);
            return Ok(jsonResponseFromat.JsonMessage(true, "Data fetched successfully", data));
        }


        [Route("GetAllNewHTCMeterRender")]
        [HttpPost]
        public async Task<IActionResult> GetAllNewHTCMeterRender([FromBody] AgGridTable dataTable)
        {

            dataTable.UserId = dataTable.UserId == 0 ? User.Identity.GetUserId() : dataTable.UserId;
            dataTable.RoleType = string.IsNullOrEmpty(dataTable.RoleType) ? User.Identity.GetUserRoleName() : dataTable.RoleType;

            var data = await meter.GetAllNewHTCMeterRender(dataTable);
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, data));
        }

        [Route("GetNewHTCMeterDetails")]
        [HttpGet]
        public async Task<IActionResult> GetNewHTCMeterDetails(string consumerId)
        {
            var result = await meter.GetHTCMeterDetails(consumerId);
            return Ok(jsonResponseFromat.JsonMessage(true, "HTC New Meter", result));
        }


        //[HttpPut]
        //[Route("UpdateHTCNewMeterDetails")]
        //public async Task<IActionResult> UpdateHTCNewMeterDetails([FromForm] HTCNewMeterDetails model)
        //{
        //    logger.LogCritical("UpdateHTCNewMeterDetails Data : {Data}", JsonConvert.SerializeObject(model));

        //    var fileUrl = string.Empty;
        //    if (model.NewMeterPhoto is not null)
        //    {
        //        fileUrl = await basicUtility.UploadFile(model.NewMeterPhoto, "NewHTCMeterPhoto", AllowedFileExtensions.GetAllowedFile("Image"));

        //        if (string.IsNullOrEmpty(fileUrl))
        //            return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
        //    }
        //    model.NewMeterPhotoUrl = fileUrl;
        //    fileUrl = string.Empty;
        //    if (model.MeterPhotoWithConsumer is not null)
        //    {
        //        fileUrl = await basicUtility.UploadFile(model.MeterPhotoWithConsumer, "MeterPhotoWithConsumer", AllowedFileExtensions.GetAllowedFile("Image"));

        //        if (string.IsNullOrEmpty(fileUrl))
        //            return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
        //    }
        //    model.MeterPhotoWithConsumerUrl = fileUrl;
        //    if (model.MeterPhotoWithKVAH is not null)
        //    {
        //        fileUrl = await basicUtility.UploadFile(model.MeterPhotoWithKVAH, "MeterPhotoWithKVAH", AllowedFileExtensions.GetAllowedFile("Image"));

        //        if (string.IsNullOrEmpty(fileUrl))
        //            return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
        //    }
        //    model.MeterPhotoWithKVAHUrl = fileUrl;
        //    if (model.MeterPhotoWithKWH is not null)
        //    {
        //        fileUrl = await basicUtility.UploadFile(model.MeterPhotoWithKWH, "MeterPhotoWithKWH", AllowedFileExtensions.GetAllowedFile("Image"));

        //        if (string.IsNullOrEmpty(fileUrl))
        //            return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
        //    }
        //    model.MeterPhotoWithKWHUrl = fileUrl;
        //    model.Action = "UPDATE";
        //    var message = await meter.SaveHTCNewMeterDetails(model, User.Identity.GetUserId(), User.Identity.GetUserRoleName());
        //    if (!string.IsNullOrEmpty(message))
        //    {
        //        logger.LogCritical("SaveHTCNewMeterDetails Data Validation : {Data} ", message);
        //        return Ok(jsonResponseFromat.JsonMessage(false, message));
        //    }
        //    return Ok(jsonResponseFromat.JsonMessage(true, CreatedSuccess));
        //}


        [HttpPut]
        [Route("UpdateHTCNewMeterDetails")]
        public async Task<IActionResult> UpdateHTCNewMeterDetails([FromForm] HTCNewMeterDetails model)
        {
            logger.LogCritical("UpdateHTCNewMeterDetails Data : {Data}", JsonConvert.SerializeObject(model));

            var message = await meter.UpdateHTCNewMeterDetails(model, User.Identity.GetUserId());

            if (!string.IsNullOrEmpty(message))
            {
                logger.LogCritical("SaveHTCNewMeterDetails Validation: {Data}", message);
                return Ok(jsonResponseFromat.JsonMessage(false, message));
            }

            return Ok(jsonResponseFromat.JsonMessage(true, "Updated Successfully"));
        }

        [HttpPost]
        [Route("SaveHTCNewMeterDetails")]
        public async Task<IActionResult> SaveHTCNewMeterDetails([FromForm] HTCNewMeterDetails model)
        {
            logger.LogCritical("SaveHTCNewMeterDetails Data : {Data}", JsonConvert.SerializeObject(model));

            if (model == null)
                return Ok(jsonResponseFromat.JsonMessage(false, SomethingWrong));

            // 🔴 MANDATORY FIELD VALIDATION (ONE MESSAGE)
            if (
                !Request.Form.ContainsKey("OldMeterReadingKwh") ||
                !Request.Form.ContainsKey("OldMeterReadingKvah") ||
                !Request.Form.ContainsKey("InitialReadingKwh") ||
                !Request.Form.ContainsKey("InitialReadingKvah") ||

                string.IsNullOrWhiteSpace(model.OldMeterReadingKwh) ||
                string.IsNullOrWhiteSpace(model.OldMeterReadingKvah) ||
                string.IsNullOrWhiteSpace(model.InitialReadingKwh) ||
                string.IsNullOrWhiteSpace(model.InitialReadingKvah)
               )
            {
                return Ok(jsonResponseFromat.JsonMessage(
                    false,
                    "Please fill all mandatory fields."
                ));
            }

            var fileUrl = string.Empty;
            if (model.NewMeterPhoto is not null)
            {
                fileUrl = await basicUtility.UploadFile(model.NewMeterPhoto, "NewHTCMeterPhoto", AllowedFileExtensions.GetAllowedFile("Image"));

                if (string.IsNullOrEmpty(fileUrl))
                    return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
            }

            model.NewMeterPhotoUrl = fileUrl;
            fileUrl = string.Empty;
            if (model.MeterPhotoWithConsumer is not null)
            {
                fileUrl = await basicUtility.UploadFile(
                    model.MeterPhotoWithConsumer,
                    "MeterPhotoWithConsumer",
                    AllowedFileExtensions.GetAllowedFile("Image"));

                if (string.IsNullOrEmpty(fileUrl))
                    return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
            }

            model.MeterPhotoWithConsumerUrl = fileUrl;
            if (model.MeterPhotoWithKVAH is not null)
            {
                fileUrl = await basicUtility.UploadFile(
                    model.MeterPhotoWithKVAH,
                    "MeterPhotoWithKVAH",
                    AllowedFileExtensions.GetAllowedFile("Image"));

                if (string.IsNullOrEmpty(fileUrl))
                    return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
            }

            model.MeterPhotoWithKVAHUrl = fileUrl;
            if (model.MeterPhotoWithKWH is not null)
            {
                fileUrl = await basicUtility.UploadFile(
                    model.MeterPhotoWithKWH,
                    "MeterPhotoWithKWH",
                    AllowedFileExtensions.GetAllowedFile("Image"));

                if (string.IsNullOrEmpty(fileUrl))
                    return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
            }

            model.MeterPhotoWithKWHUrl = fileUrl;
            model.Action = "INSERT";

            var message = await meter.SaveHTCNewMeterDetails(
                model,
                User.Identity.GetUserId(),
                User.Identity.GetUserRoleName());

            if (!string.IsNullOrEmpty(message))
            {
                logger.LogCritical("SaveHTCNewMeterDetails Data Validation : {Data}", message);
                return Ok(jsonResponseFromat.JsonMessage(false, message));
            }

            return Ok(jsonResponseFromat.JsonMessage(true, CreatedSuccess));
        }

        #endregion HTCMeter New Meter

        #region DTCMeter New Meter

        [HttpPost]
        [Route("ImportDTCNewMeterDataByExcel")]
        public IActionResult ImportDTCNewMeterDataByExcel(IFormFile postedFile)
        {
            if (postedFile != null)
            {
                int userId = User.Identity.GetUserId();
                // Create a Folder
                string path = Path.Combine(env.WebRootPath, "DTCNewMeterExcel");
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
                var jobId = BackgroundJob.Enqueue<ImportExcelDataIntoDatabase>(x => x.ImportExcelData(filePath, User.Identity.GetUserId(), TableNameEnum.TempDTCNewMeter));
                BackgroundJob.ContinueJobWith(jobId, () => SyncDTCNewMeter(User.Identity.GetUserId()));

                return Ok(jsonResponseFromat.JsonMessage(true, FileUploaded));
            }

            return Ok(jsonResponseFromat.JsonMessage(false, SelectValidExcel));
        }

        [Route("GetFullNewDTCMeterRender")]
        [HttpPost]
        public async Task<IActionResult> GetFullNewDTCMeterRender([FromBody] AgGridTable dataTable)
        {

            dataTable.UserId = dataTable.UserId == 0 ? User.Identity.GetUserId() : dataTable.UserId;
            dataTable.RoleType = string.IsNullOrEmpty(dataTable.RoleType) ? User.Identity.GetUserRoleName() : dataTable.RoleType;

            var result = await meter.GetFullNewDTCMeterRender(dataTable);
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, result));
        }

        [Route("GetDTCNewMeterByConsumerId")]
        [HttpGet]
        public async Task<IActionResult> GetDTCNewMeterByConsumerId(string consumerId)
        {
            if (string.IsNullOrEmpty(consumerId))
                return BadRequest(jsonResponseFromat.JsonMessage(false, "ConsumerId is required", null));

            var data = await meter.GetDTCNewMeterByConsumerId(consumerId);
            return Ok(jsonResponseFromat.JsonMessage(true, "Data fetched successfully", data));
        }



        [Route("GetAllNewDTCMeterRender")]
        [HttpPost]
        public async Task<IActionResult> GetAllNewDTCMeterRender([FromBody] AgGridTable dataTable)
        {

            dataTable.UserId = dataTable.UserId == 0 ? User.Identity.GetUserId() : dataTable.UserId;
            dataTable.RoleType = string.IsNullOrEmpty(dataTable.RoleType) ? User.Identity.GetUserRoleName() : dataTable.RoleType;

            var data = await meter.GetAllNewDTCMeterRender(dataTable);
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, data));
        }

        [Route("GetNewDTCMeterDetails")]
        [HttpGet]
        public async Task<IActionResult> GetNewDTCMeterDetails(string consumerId)
        {
            var result = await meter.GetDTCMeterDetails(consumerId);
            return Ok(jsonResponseFromat.JsonMessage(true, "DTC New Meter", result));
        }

        //[HttpPut]
        //[Route("UpdateDTCNewMeterDetails")]
        //public async Task<IActionResult> UpdateDTCNewMeterDetails([FromForm] DTCNewMeterDetails model)
        //{
        //    logger.LogCritical("UpdateDTCNewMeterDetails Data : {Data}", JsonConvert.SerializeObject(model));

        //    var fileUrl = string.Empty;
        //    if (model.NewMeterPhoto is not null)
        //    {
        //        fileUrl = await basicUtility.UploadFile(model.NewMeterPhoto, "NewDTCMeterPhoto", AllowedFileExtensions.GetAllowedFile("Image"));

        //        if (string.IsNullOrEmpty(fileUrl))
        //            return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
        //    }
        //    model.NewMeterPhotoUrl = fileUrl;
        //    fileUrl = string.Empty;
        //    if (model.MeterPhotoWithConsumer is not null)
        //    {
        //        fileUrl = await basicUtility.UploadFile(model.MeterPhotoWithConsumer, "MeterPhotoWithConsumer", AllowedFileExtensions.GetAllowedFile("Image"));

        //        if (string.IsNullOrEmpty(fileUrl))
        //            return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
        //    }
        //    model.MeterPhotoWithConsumerUrl = fileUrl;
        //    model.Action = "UPDATE";
        //    var message = await meter.SaveDTCNewMeterDetails(model, User.Identity.GetUserId(), User.Identity.GetUserRoleName());
        //    if (!string.IsNullOrEmpty(message))
        //    {
        //        logger.LogCritical("SaveDTCNewMeterDetails Data Validation : {Data} ", message);
        //        return Ok(jsonResponseFromat.JsonMessage(false, message));
        //    }
        //    return Ok(jsonResponseFromat.JsonMessage(true, CreatedSuccess));
        //}

        [HttpPut]
        [Route("UpdateDTCNewMeterDetails")]
        public async Task<IActionResult> EditDTCNewMeterDetails([FromForm] DTCNewMeterDetails model)
        {
            logger.LogCritical("UpdateDTCNewMeterDetails Data : {Data}", JsonConvert.SerializeObject(model));

            var message = await meter.UpdateDTCNewMeterDetails(model, User.Identity.GetUserId());
            if (!string.IsNullOrEmpty(message))
            {
                logger.LogCritical("UpdateDTCNewMeterDetails Data Validation : {Data} ", message);
                return Ok(jsonResponseFromat.JsonMessage(false, message));
            }
            return Ok(jsonResponseFromat.JsonMessage(true, CreatedSuccess));
        }

        [HttpPost]
        [Route("SaveDTCNewMeterDetails")]
        public async Task<IActionResult> SaveDTCNewMeterDetails([FromForm] DTCNewMeterDetails model)
        {
            logger.LogCritical("SaveDTCNewMeterDetails Data : {Data}", JsonConvert.SerializeObject(model));

            if (model == null)
                return Ok(jsonResponseFromat.JsonMessage(false, SomethingWrong));

            // 🔴 MANDATORY FIELD VALIDATION (ONE MESSAGE)
            if (
                !Request.Form.ContainsKey("OldMeterReadingKwh") ||
                !Request.Form.ContainsKey("OldMeterReadingKvah") ||
                !Request.Form.ContainsKey("InitialReadingKwh") ||
                !Request.Form.ContainsKey("InitialReadingKvah") ||

                string.IsNullOrWhiteSpace(model.OldMeterReadingKwh) ||
                string.IsNullOrWhiteSpace(model.OldMeterReadingKvah) ||
                string.IsNullOrWhiteSpace(model.InitialReadingKwh) ||
                string.IsNullOrWhiteSpace(model.InitialReadingKvah)
               )
            {
                return Ok(jsonResponseFromat.JsonMessage(
                    false,
                    "Please fill all mandatory fields."
                ));
            }

            var fileUrl = string.Empty;
            if (model.NewMeterPhoto is not null)
            {
                fileUrl = await basicUtility.UploadFile(
                    model.NewMeterPhoto,
                    "NewDTCMeterPhoto",
                    AllowedFileExtensions.GetAllowedFile("Image"));

                if (string.IsNullOrEmpty(fileUrl))
                    return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
            }

            model.NewMeterPhotoUrl = fileUrl;
            fileUrl = string.Empty;
            if (model.MeterPhotoWithConsumer is not null)
            {
                fileUrl = await basicUtility.UploadFile(
                    model.MeterPhotoWithConsumer,
                    "MeterPhotoWithConsumer",
                    AllowedFileExtensions.GetAllowedFile("Image"));

                if (string.IsNullOrEmpty(fileUrl))
                    return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
            }

            model.MeterPhotoWithConsumerUrl = fileUrl;
            model.Action = "INSERT";

            var message = await meter.SaveDTCNewMeterDetails(
                model,
                User.Identity.GetUserId(),
                User.Identity.GetUserRoleName());

            if (!string.IsNullOrEmpty(message))
            {
                logger.LogCritical("SaveDTCNewMeterDetails Data Validation : {Data}", message);
                return Ok(jsonResponseFromat.JsonMessage(false, message));
            }

            return Ok(jsonResponseFromat.JsonMessage(true, CreatedSuccess));
        }

        #endregion DTCMeter New Meter

        #region Consumer New Meter
        [HttpPost]
        [Route("ImportConsumerNewMeterDataByExcel")]
        public IActionResult ImportConsumerNewMeterDataByExcel(IFormFile postedFile)
        {
            if (postedFile != null)
            {
                int userId = User.Identity.GetUserId();
                // Create a Folder
                string path = Path.Combine(env.WebRootPath, "ConsumerNewMeterDataExcel");
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
                var jobId = BackgroundJob.Enqueue<ImportExcelDataIntoDatabase>(x => x.ImportExcelData(filePath, User.Identity.GetUserId(), TableNameEnum.TempConsumerNewMeter));
                BackgroundJob.ContinueJobWith(jobId, () => SyncConsumerNewMeterData(User.Identity.GetUserId()));

                return Ok(jsonResponseFromat.JsonMessage(true, FileUploaded));
            }

            return Ok(jsonResponseFromat.JsonMessage(false, SelectValidExcel));
        }

        [Route("GetFullNewConsumerMeterRender")]
        [HttpPost]
        public async Task<IActionResult> GetFullNewConsumerMeterRender([FromBody] AgGridTable dataTable)
        {

            dataTable.UserId = dataTable.UserId == 0 ? User.Identity.GetUserId() : dataTable.UserId;
            dataTable.RoleType = string.IsNullOrEmpty(dataTable.RoleType) ? User.Identity.GetUserRoleName() : dataTable.RoleType;

            var result = await meter.GetFullNewConsumerMeterRender(dataTable);
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, result));
        }

        [Route("GetConsumerNewMeterByConsumerId")]
        [HttpGet]  // Use GET since we're retrieving by parameter
        public async Task<IActionResult> GetConsumerNewMeterByConsumerId([FromQuery] string consumerId)
        {
            if (string.IsNullOrEmpty(consumerId))
                return BadRequest(jsonResponseFromat.JsonMessage(false, "ConsumerId is required"));

            var data = await meter.GetConsumerNewMeterByConsumerId(consumerId);
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, data));
        }


        [Route("GetAllNewConsumerMeterRender")]
        [HttpPost]
        public async Task<IActionResult> GetAllNewConsumerMeterRender([FromBody] AgGridTable dataTable)
        {

            dataTable.UserId = dataTable.UserId == 0 ? User.Identity.GetUserId() : dataTable.UserId;
            dataTable.RoleType = string.IsNullOrEmpty(dataTable.RoleType) ? User.Identity.GetUserRoleName() : dataTable.RoleType;

            var data = await meter.GetAllNewConsumerMeterRender(dataTable);
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, data));
        }

        [Route("GetNewConsumerMeterDetails")]
        [HttpGet]
        public async Task<IActionResult> GetNewConsumerMeterDetails(string consumerId)
        {
            var result = await meter.GetConsumerMeterDetails(consumerId);
            return Ok(jsonResponseFromat.JsonMessage(true, "Consumer New Meter", result));
        }

        [HttpPost]
        [Route("SaveConsumerNewMeterDetails")]
        public async Task<IActionResult> SaveConsumerNewMeterDetails([FromForm] ConsumerNewMeterDetails model)
        {
            logger.LogCritical("SaveConsumerNewMeterDetails Data : {Data}", JsonConvert.SerializeObject(model));

            if (model == null)
                return Ok(jsonResponseFromat.JsonMessage(false, SomethingWrong));
            var fileUrl = string.Empty;
                if (model.NewMeterPhoto is not null)
            {
                fileUrl = await basicUtility.UploadFile(
                    model.NewMeterPhoto,
                    "NewConsumerMeterPhoto",
                    AllowedFileExtensions.GetAllowedFile("Image"));

                if (string.IsNullOrEmpty(fileUrl))
                    return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
            }

            model.NewMeterPhotoUrl = fileUrl;
            fileUrl = string.Empty;
                if (model.MeterPhotoWithConsumer is not null)
            {
                fileUrl = await basicUtility.UploadFile(
                    model.MeterPhotoWithConsumer,
                    "MeterPhotoWithConsumer",
                    AllowedFileExtensions.GetAllowedFile("Image"));

                if (string.IsNullOrEmpty(fileUrl))
                    return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
            }

            model.MeterPhotoWithConsumerUrl = fileUrl;
            fileUrl = string.Empty;
                if (model.OldMeterPhotoWithCUMkvah is not null)
            {
                fileUrl = await basicUtility.UploadFile(
                    model.OldMeterPhotoWithCUMkvah,
                    "OldMeterPhotoWithCUMkvah",
                    AllowedFileExtensions.GetAllowedFile("Image"));

                if (string.IsNullOrEmpty(fileUrl))
                    return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
            }

            model.OldMeterPhotoWithCUMkvahUrl = fileUrl;
            model.Action = "INSERT";

            var message = await meter.SaveConsumerNewMeterDetails(
                model,
                User.Identity.GetUserId(),
                User.Identity.GetUserRoleName());

            if (!string.IsNullOrEmpty(message))
            {
                logger.LogCritical("SaveConsumerNewMeterDetails Data Validation : {Data}", message);
                return Ok(jsonResponseFromat.JsonMessage(false, message));
            }

            return Ok(jsonResponseFromat.JsonMessage(true, CreatedSuccess));
        }

        //[Route("UpdateConsumerNewMeterDetails")]
        //[HttpPut]
        //public async Task<IActionResult> UpdateConsumerNewMeterDetails([FromForm] ConsumerNewMeterDetails model)
        //{
        //    var fileUrl = string.Empty;
        //    if (model.NewMeterPhoto is not null)
        //    {
        //        fileUrl = await basicUtility.UploadFile(model.NewMeterPhoto, "NewConsumerMeterPhoto", AllowedFileExtensions.GetAllowedFile("Image"));

        //        if (string.IsNullOrEmpty(fileUrl))
        //            return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
        //    }
        //    model.NewMeterPhotoUrl = fileUrl;
        //    fileUrl = string.Empty;
        //    if (model.MeterPhotoWithConsumer is not null)
        //    {
        //        fileUrl = await basicUtility.UploadFile(model.MeterPhotoWithConsumer, "MeterPhotoWithConsumer", AllowedFileExtensions.GetAllowedFile("Image"));

        //        if (string.IsNullOrEmpty(fileUrl))
        //            return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
        //    }
        //    model.MeterPhotoWithConsumerUrl = fileUrl;
        //    fileUrl = string.Empty;
        //    if (model.OldMeterPhotoWithCUMkvah is not null)
        //    {
        //        fileUrl = await basicUtility.UploadFile(model.OldMeterPhotoWithCUMkvah, "OldMeterPhotoWithCUMkvah", AllowedFileExtensions.GetAllowedFile("Image"));

        //        if (string.IsNullOrEmpty(fileUrl))
        //            return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
        //    }
        //    model.OldMeterPhotoWithCUMkvahUrl = fileUrl;
        //    model.Action = "UPDATE";
        //    var message = await meter.SaveConsumerNewMeterDetails(model, User.Identity.GetUserId(), User.Identity.GetUserRoleName());
        //    if (!string.IsNullOrEmpty(message))
        //    {
        //        logger.LogCritical("UpdateConsumerNewMeterDetails Data Validation : {Data} ", message);
        //        return Ok(jsonResponseFromat.JsonMessage(false, message));
        //    }
        //    return Ok(jsonResponseFromat.JsonMessage(true, UpdateSuccess));
        //}
        [Route("UpdateConsumerNewMeterDetails")]
        [HttpPut]
        public async Task<IActionResult> EditConsumerNewMeterDetails([FromForm] ConsumerNewMeterDetails model)
        {
            var fileUrl = string.Empty;
            if (model.NewMeterPhoto is not null)
            {
                fileUrl = await basicUtility.UploadFile(model.NewMeterPhoto, "NewConsumerMeterPhoto", AllowedFileExtensions.GetAllowedFile("Image"));

                if (string.IsNullOrEmpty(fileUrl))
                    return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
            }
            model.NewMeterPhotoUrl = fileUrl;
            fileUrl = string.Empty;
            if (model.MeterPhotoWithConsumer is not null)
            {
                fileUrl = await basicUtility.UploadFile(model.MeterPhotoWithConsumer, "MeterPhotoWithConsumer", AllowedFileExtensions.GetAllowedFile("Image"));

                if (string.IsNullOrEmpty(fileUrl))
                    return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
            }
            model.MeterPhotoWithConsumerUrl = fileUrl;
            fileUrl = string.Empty;
            if (model.OldMeterPhotoWithCUMkvah is not null)
            {
                fileUrl = await basicUtility.UploadFile(model.OldMeterPhotoWithCUMkvah, "OldMeterPhotoWithCUMkvah", AllowedFileExtensions.GetAllowedFile("Image"));

                if (string.IsNullOrEmpty(fileUrl))
                    return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
            }
            model.OldMeterPhotoWithCUMkvahUrl = fileUrl;
            model.Action = "UPDATE";
            var message = await meter.UpdateConsumerNewMeterDetails(model, User.Identity.GetUserId());
            if (!string.IsNullOrEmpty(message))
            {
                logger.LogCritical("UpdateConsumerNewMeterDetails Data Validation : {Data} ", message);
                return Ok(jsonResponseFromat.JsonMessage(false, message));
            }
            return Ok(jsonResponseFromat.JsonMessage(true, UpdateSuccess));
        }
        [Route("GetConsumerNewMeterList")]
        [HttpGet]
        public async Task<IActionResult> GetConsumerNewMeterList()
        {
            var result = await meter.GetConsumerNewMeterList(User.Identity.GetUserId(), User.Identity.GetUserRoleName());
            return Ok(jsonResponseFromat.JsonMessage(true, "Consumer New Meter Details", result));
        }




        #endregion Consumer New Meter

        #region DT New Meter

        [HttpPost]
        [Route("ImportDTNewMeterDataByExcel")]
        public IActionResult ImportDTNewMeterDataByExcel(IFormFile postedFile)
        {
            if (postedFile == null)
            {
                return BadRequest("No file was uploaded.");
            }

            try
            {
                int userId = User.Identity.GetUserId();
                // Create a Folder
                string path = Path.Combine(env.WebRootPath, "DTNewMeterDataExcel");
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
                var jobId = BackgroundJob.Enqueue<ImportExcelDataIntoDatabase>(x => x.ImportExcelData(filePath, userId, SmartMeterAdmin.Infrastructure.Enumeration.TableNameEnum.TempDTNewMeter));
                BackgroundJob.ContinueJobWith(jobId, () => SyncDTNewMeterData(userId));

                return Ok(jsonResponseFromat.JsonMessage(true, FileUploaded));
            }
            catch (Exception ex)
            {
                // Handle exception
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [Route("GetFullNewDTMeterRender")]
        [HttpPost]
        public async Task<IActionResult> GetFullNewDTMeterRender([FromBody] AgGridTable dataTable)
        {

            dataTable.UserId = dataTable.UserId == 0 ? User.Identity.GetUserId() : dataTable.UserId;
            dataTable.RoleType = string.IsNullOrEmpty(dataTable.RoleType) ? User.Identity.GetUserRoleName() : dataTable.RoleType;

            var result = await meter.GetFullNewDTMeterRender(dataTable);
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, result));
        }

       

        [Route("GetDTNewMeterByConsumerId")]
        [HttpGet]
        public async Task<IActionResult> GetDTNewMeterByConsumerId(string consumerId)
        {
            if (string.IsNullOrEmpty(consumerId))
                return BadRequest(jsonResponseFromat.JsonMessage(false, "ConsumerId is required", null));

            var data = await meter.GetDTNewMeterByConsumerId(consumerId);
            return Ok(jsonResponseFromat.JsonMessage(true, "Data fetched successfully", data));
        }


        [Route("GetAllNewDTMeterRender")]
        [HttpPost]
        public async Task<IActionResult> GetAllNewDTMeterRender([FromBody] AgGridTable dataTable)
        {

            dataTable.UserId = dataTable.UserId == 0 ? User.Identity.GetUserId() : dataTable.UserId;
            dataTable.RoleType = string.IsNullOrEmpty(dataTable.RoleType) ? User.Identity.GetUserRoleName() : dataTable.RoleType;

            var data = await meter.GetAllNewDTMeterRender(dataTable);
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, data));
        }

        //[HttpPut]
        //[Route("UpdateDTNewMeterDetails")]
        //public async Task<IActionResult> UpdateDTNewMeterDetails([FromForm] DTNewMeterDetails model)
        //{
        //    logger.LogCritical("SaveDTNewMeterDetails Data : {Data}", JsonConvert.SerializeObject(model));

        //    var fileUrl = string.Empty;
        //    if (model.SMCBoxPhoto is not null)
        //    {
        //        fileUrl = await basicUtility.UploadFile(model.SMCBoxPhoto, "SMCBoxPhoto", AllowedFileExtensions.GetAllowedFile("Image"));

        //        if (string.IsNullOrEmpty(fileUrl))
        //            return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
        //    }
        //    model.SMCBoxPhotourl = fileUrl;
        //    fileUrl = string.Empty;
        //    if (model.NewMeterPhoto is not null)
        //    {
        //        fileUrl = await basicUtility.UploadFile(model.NewMeterPhoto, "NewMeterPhoto", AllowedFileExtensions.GetAllowedFile("Image"));

        //        if (string.IsNullOrEmpty(fileUrl))
        //            return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
        //    }
        //    model.NewMeterPhotourl = fileUrl;
        //    model.Action = "UPDATE";
        //    var message = await meter.SaveDTNewMeterDetails(model, User.Identity.GetUserId(), User.Identity.GetUserRoleName());
        //    if (!string.IsNullOrEmpty(message))
        //    {
        //        logger.LogCritical("SaveDTNewMeterDetails Data Validation : {Data} ", message);
        //        return Ok(jsonResponseFromat.JsonMessage(false, message));
        //    }

        //    return Ok(jsonResponseFromat.JsonMessage(true, CreatedSuccess));
        //}
        [HttpPut]
        [Route("UpdateDTNewMeterDetails")]
        public async Task<IActionResult> EditDTNewMeterDetails([FromForm] DTNewMeterDetails model)
        {
            logger.LogCritical("UpdateDTNewMeterDetails Data : {Data}", JsonConvert.SerializeObject(model));
           
            var message = await meter.UpdateDTNewMeterDetails(model, User.Identity.GetUserId());
            if (!string.IsNullOrEmpty(message))
            {
                logger.LogCritical("UpdateDTNewMeterDetails Data Validation : {Data} ", message);
                return Ok(jsonResponseFromat.JsonMessage(false, message));
            }

            return Ok(jsonResponseFromat.JsonMessage(true, CreatedSuccess));
        }
        [HttpPost]
        [Route("SaveDTNewMeterDetails")]
        public async Task<IActionResult> SaveDTNewMeterDetails([FromForm] DTNewMeterDetails model)
        {
            logger.LogCritical("SaveDTNewMeterDetails Data : {Data}", JsonConvert.SerializeObject(model));

            if (model == null)
                return Ok(jsonResponseFromat.JsonMessage(false, SomethingWrong));

            // 🔴 MANDATORY FIELD VALIDATION (ONE MESSAGE)
            if (
                !Request.Form.ContainsKey("OldReadingKWH") ||
                !Request.Form.ContainsKey("OldReadingKVAH") ||
                !Request.Form.ContainsKey("InitialReadingKWH") ||
                !Request.Form.ContainsKey("InitialReadingKVAH") ||

                string.IsNullOrWhiteSpace(model.OldReadingKWH) ||
                string.IsNullOrWhiteSpace(model.OldReadingKVAH) ||
                string.IsNullOrWhiteSpace(model.InitialReadingKWH) ||
                string.IsNullOrWhiteSpace(model.InitialReadingKVAH)
               )
            {
                return Ok(jsonResponseFromat.JsonMessage(
                    false,
                    "Please fill all mandatory fields."
                ));
            }

            var fileUrl = string.Empty;
            if (model.SMCBoxPhoto is not null)
            {
                fileUrl = await basicUtility.UploadFile(
                    model.SMCBoxPhoto,
                    "SMCBoxPhoto",
                    AllowedFileExtensions.GetAllowedFile("Image"));

                if (string.IsNullOrEmpty(fileUrl))
                    return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
            }

            model.SMCBoxPhotourl = fileUrl;
            fileUrl = string.Empty;
            if (model.NewMeterPhoto is not null)
            {
                fileUrl = await basicUtility.UploadFile(
                    model.NewMeterPhoto,
                    "NewMeterPhoto",
                    AllowedFileExtensions.GetAllowedFile("Image"));

                if (string.IsNullOrEmpty(fileUrl))
                    return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
            }

            model.NewMeterPhotourl = fileUrl;
            model.Action = "INSERT";

            var message = await meter.SaveDTNewMeterDetails(
                model,
                User.Identity.GetUserId(),
                User.Identity.GetUserRoleName());

            if (!string.IsNullOrEmpty(message))
            {
                logger.LogCritical("SaveDTNewMeterDetails Data Validation : {Data}", message);
                return Ok(jsonResponseFromat.JsonMessage(false, message));
            }

            return Ok(jsonResponseFromat.JsonMessage(true, CreatedSuccess));
        }

        [Route("GetDTNewMeterList")]
        [HttpGet]
        public async Task<IActionResult> GetDTNewMeterList()
        {
            var result = await meter.GetDTNewMeterList(User.Identity.GetUserId(), User.Identity.GetUserRoleName());
            return Ok(jsonResponseFromat.JsonMessage(true, "DT New Meter Details", result));
        }

        [Route("GetNewDTMeterDetails")]
        [HttpGet]
        public async Task<IActionResult> GetNewDTMeterDetails(string consumerId)
        {
            var result = await meter.GetDTMeterDetails(consumerId);
            return Ok(jsonResponseFromat.JsonMessage(true, "DT New Meter", result));
        }

        #endregion  DT New Meter

        #region Feeder New Meter


        [HttpPost]
        [Route("ImportFeederNewMeterDataByExcel")]
        public IActionResult ImportFeederNewMeterDataByExcel(IFormFile postedFile)
        {
            if (postedFile != null)
            {
                int userId = User.Identity.GetUserId();
                if (userId == 0)
                {
                    return Unauthorized("User is not authenticated.");
                }

                // Create a Folder
                string path = Path.Combine(env.WebRootPath, "FeederNewMeterDataExcel");
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
                var jobId = BackgroundJob.Enqueue<ImportExcelDataIntoDatabase>(x => x.ImportExcelData(filePath, userId, TableNameEnum.TempFeederNewMeter));
                BackgroundJob.ContinueJobWith(jobId, () => SyncConsumerNewMeterData(userId));

                return Ok(jsonResponseFromat.JsonMessage(true, FileUploaded));
            }

            return Ok(jsonResponseFromat.JsonMessage(false, SelectValidExcel));
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("DownloadBulkLineRatioSample")]
        public IActionResult DownloadBulkLineRatioSample()
        {
            IWorkbook workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet("Sheet1");
            string[][] rows =
            {
                new[] { "MeterNo", "LineCTRatio", "LinePTRatio" },
                new[] { "MZXXXXXXX", "10/5A", "240/240V" },
                new[] { "MZYYYYYYY", "100/5A", "11000/110V" },
                new[] { "MZZZZZZZZ", "150/5A", "240/240V" }
            };
            for (int r = 0; r < rows.Length; r++)
            {
                var row = sheet.CreateRow(r);
                for (int c = 0; c < rows[r].Length; c++)
                    row.CreateCell(c).SetCellValue(rows[r][c]);
            }
            using var ms = new MemoryStream();
            workbook.Write(ms);
            return File(ms.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "BulkLineRatioSample.xlsx");
        }

        [HttpPost]
        [Route("BulkUpdateLineRatios")]
        public async Task<IActionResult> BulkUpdateLineRatios(IFormFile postedFile)
        {
            if (postedFile == null || postedFile.Length == 0)
                return Ok(jsonResponseFromat.JsonMessage(false, "Please select a valid Excel file."));

            string extension = Path.GetExtension(postedFile.FileName).ToLower();
            if (extension != ".xlsx" && extension != ".xls")
                return Ok(jsonResponseFromat.JsonMessage(false, "Only .xlsx / .xls files are allowed."));

            int userId = User.Identity.GetUserId();
            var results = new List<object>();
            int updated = 0, notFound = 0, skipped = 0, failed = 0;

            try
            {
                using var stream = new MemoryStream();
                await postedFile.CopyToAsync(stream);
                stream.Position = 0;
                IWorkbook workbook = extension == ".xls"
                    ? new HSSFWorkbook(stream)
                    : new XSSFWorkbook(stream);
                var sheet = workbook.GetSheetAt(0);

                for (int i = 1; i <= sheet.LastRowNum; i++)
                {
                    var row = sheet.GetRow(i);
                    if (row == null) continue;

                    string GetCell(int idx)
                    {
                        var cell = row.GetCell(idx);
                        if (cell == null) return string.Empty;
                        return cell.CellType == CellType.Numeric && DateUtil.IsCellDateFormatted(cell)
                            ? cell.DateCellValue.ToString()
                        : (cell.CellType == CellType.Numeric
                            ? cell.NumericCellValue.ToString("0.######")
                            : (cell.StringCellValue ?? string.Empty).Trim());
                    }

                    string meterNo = GetCell(0);
                    string ct = GetCell(1);
                    string pt = GetCell(2);

                    if (string.IsNullOrWhiteSpace(meterNo)) { skipped++; continue; }

                    try
                    {
                        string procResult = await meter.BulkUpdateLineRatiosAsync(meterNo, ct, pt, userId);
                        if (procResult == "SKIPPED_EMPTY_RATIO") { skipped++; results.Add(new { meterNo, status = "SKIPPED", detail = "CT & PT both blank" }); }
                        else if (procResult == "INVALID_METER_NO") { skipped++; results.Add(new { meterNo, status = "SKIPPED", detail = "Invalid meter no" }); }
                        else if (procResult == "NOT_FOUND") { notFound++; results.Add(new { meterNo, status = "NOT_FOUND", detail = "Meter not found in DT/DTC/Feeder" }); }
                        else { updated++; results.Add(new { meterNo, status = "UPDATED", detail = procResult }); }
                        logger.LogInformation("BulkUpdateLineRatios Row {Row}: {MeterNo} -> {Result}", i + 1, meterNo, procResult);
                    }
                    catch (Exception rowEx)
                    {
                        failed++;
                        results.Add(new { meterNo, status = "FAILED", detail = rowEx.Message });
                    }
                    if (i >= 5000) break; // safety cap
                }

                string summary = $"{updated} updated, {notFound} not found, {skipped} skipped, {failed} failed";
                return Ok(jsonResponseFromat.JsonMessage(true, summary, results));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "BulkUpdateLineRatios failed");
                return Ok(jsonResponseFromat.JsonMessage(false, "Bulk upload failed: " + ex.Message));
            }
        }

        [Route("GetFullNewFeederMeterRender")]
        [HttpPost]
        public async Task<IActionResult> GetFullNewFeederMeterRender([FromBody] AgGridTable dataTable)
        {

            dataTable.UserId = dataTable.UserId == 0 ? User.Identity.GetUserId() : dataTable.UserId;
            dataTable.RoleType = string.IsNullOrEmpty(dataTable.RoleType) ? User.Identity.GetUserRoleName() : dataTable.RoleType;

            var result = await meter.GetFullNewFeederMeterRender(dataTable);
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, result));
        }

        [Route("GetFeederNewMeterByConsumerId")]
        [HttpGet]
        public async Task<IActionResult> GetFeederNewMeterByConsumerId(string consumerId)
        {
            if (string.IsNullOrEmpty(consumerId))
                return BadRequest(jsonResponseFromat.JsonMessage(false, "ConsumerId is required", null));

            var data = await meter.GetFeederNewMeterByConsumerId(consumerId);

            if (data == null)
                return NotFound(jsonResponseFromat.JsonMessage(false, "No data found for this ConsumerId", null));

            return Ok(jsonResponseFromat.JsonMessage(true, "Data fetched successfully", data));
        }


        [Route("GetAllNewFeederMeterRender")]
        [HttpPost]
        public async Task<IActionResult> GetAllNewFeederMeterRender([FromBody] AgGridTable dataTable)
        {
            dataTable.UserId = dataTable.UserId == 0 ? User.Identity.GetUserId() : dataTable.UserId;
            dataTable.RoleType = string.IsNullOrEmpty(dataTable.RoleType) ? User.Identity.GetUserRoleName() : dataTable.RoleType;

            var data = await meter.GetAllNewFeederMeterRender(dataTable);
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, data));
        }

        //[HttpPut]
        //[Route("UpdateFeederNewMeterDetails")]
        //public async Task<IActionResult> UpdateFeederNewMeterDetails([FromForm] FeederNewMeterDetails model)
        //{
        //    var fileUrl = string.Empty;
        //    if (model.FeederControlPanelPhoto is not null)
        //    {
        //        fileUrl = await basicUtility.UploadFile(model.FeederControlPanelPhoto, "FeederControlPanelPhoto", AllowedFileExtensions.GetAllowedFile("Image"));

        //        if (string.IsNullOrEmpty(fileUrl))
        //            return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
        //    }
        //    model.FeederControlPanelPhotoUrl = fileUrl;
        //    fileUrl = string.Empty;

        //    if (model.NewMeterPhoto is not null)
        //    {
        //        fileUrl = await basicUtility.UploadFile(model.NewMeterPhoto, "NewMeterPhoto", AllowedFileExtensions.GetAllowedFile("Image"));

        //        if (string.IsNullOrEmpty(fileUrl))
        //            return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
        //    }
        //    model.NewMeterPhotoUrl = fileUrl;
        //    model.Action = "UPDATE";
        //    var message = await meter.SaveFeederNewMeterDetails(model, User.Identity.GetUserId(), User.Identity.GetUserRoleName());
        //    if (!string.IsNullOrEmpty(message))
        //    {
        //        logger.LogCritical("SaveFeederNewMeterDetails Data Validation : {Message} ", message);
        //        return Ok(jsonResponseFromat.JsonMessage(false, message));
        //    }
        //    return Ok(jsonResponseFromat.JsonMessage(true, CreatedSuccess));
        //}

        [HttpPut]
        [Route("UpdateFeederNewMeterDetails")]
        public async Task<IActionResult> EditFeederNewMeterDetails([FromForm] FeederNewMeterDetails model)
        {
            
            var message = await meter.UpdateFeederNewMeterDetails(model, User.Identity.GetUserId());
            if (!string.IsNullOrEmpty(message))
            {
                logger.LogCritical("UpdateFeederNewMeterDetails Data Validation : {Message} ", message);
                return Ok(jsonResponseFromat.JsonMessage(false, message));
            }
            return Ok(jsonResponseFromat.JsonMessage(true, CreatedSuccess));
        }
        [HttpPost]
        [Route("SaveFeederNewMeterDetails")]
        public async Task<IActionResult> SaveFeederNewMeterDetails([FromForm] FeederNewMeterDetails model)
        {
            if (model == null)
                return Ok(jsonResponseFromat.JsonMessage(false, SomethingWrong));

            // 🔴 MANDATORY FIELD VALIDATION (ONE MESSAGE)
            if (
                !Request.Form.ContainsKey("OldReadingKWH") ||
                !Request.Form.ContainsKey("OldReadingKVAH") ||
                !Request.Form.ContainsKey("InitialReadingKWH") ||
                !Request.Form.ContainsKey("InitialReadingKVAH") ||

                string.IsNullOrWhiteSpace(model.OldReadingKWH) ||
                string.IsNullOrWhiteSpace(model.OldReadingKVAH) ||
                string.IsNullOrWhiteSpace(model.InitialReadingKWH) ||
                string.IsNullOrWhiteSpace(model.InitialReadingKVAH)
               )
            {
                return Ok(jsonResponseFromat.JsonMessage(
                    false,
                    "Please fill all mandatory fields."
                ));
            }

            var fileUrl = string.Empty;
            if (model.FeederControlPanelPhoto is not null)
            {
                fileUrl = await basicUtility.UploadFile(
                    model.FeederControlPanelPhoto,
                    "FeederControlPanelPhoto",
                    AllowedFileExtensions.GetAllowedFile("Image"));

                if (string.IsNullOrEmpty(fileUrl))
                    return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
            }

            model.FeederControlPanelPhotoUrl = fileUrl;
            fileUrl = string.Empty;

            if (model.NewMeterPhoto is not null)
            {
                fileUrl = await basicUtility.UploadFile(
                    model.NewMeterPhoto,
                    "NewMeterPhoto",
                    AllowedFileExtensions.GetAllowedFile("Image"));

                if (string.IsNullOrEmpty(fileUrl))
                    return Ok(jsonResponseFromat.JsonMessage(false, SelectValidFile));
            }

            model.NewMeterPhotoUrl = fileUrl;
            model.Action = "INSERT";

            var message = await meter.SaveFeederNewMeterDetails(
                model,
                User.Identity.GetUserId(),
                User.Identity.GetUserRoleName());

            if (!string.IsNullOrEmpty(message))
            {
                logger.LogCritical("SaveFeederNewMeterDetails Data Validation : {Message}", message);
                return Ok(jsonResponseFromat.JsonMessage(false, message));
            }

            return Ok(jsonResponseFromat.JsonMessage(true, CreatedSuccess));
        }

        [Route("GetFeederNewMeterList")]
        [HttpGet]
        public async Task<IActionResult> GetFeederNewMeterList()
        {
            var result = await meter.GetFeederNewMeterList(User.Identity.GetUserId(), User.Identity.GetUserRoleName());
            return Ok(jsonResponseFromat.JsonMessage(true, "Feeder New Meter Details", result));
        }

        [Route("GetNewFeederMeterDetails")]
        [HttpGet]
        public async Task<IActionResult> GetNewFeederMeterDetails(string consumerId)
        {
            var result = await meter.GetFeederMeterDetails(consumerId);
            return Ok(jsonResponseFromat.JsonMessage(true, "Feeder New Meter", result));
        }
        #endregion Feeder New Meter

        [Route("GetNewMeterDetails")]
        [HttpGet]
        public async Task<IActionResult> GetNewMeterDetails(string meterNo)
        {
            var result = await meter.GetNewMeterDetials(meterNo);
            return Ok(jsonResponseFromat.JsonMessage(true, "Consumer New Meter Details", result));
        }

        [Route("GetNewMeterDashboardCount")]
        [HttpGet]
        public async Task<IActionResult> GetNewMeterDashboardCount(int userId, string roleType)
        {
            userId = userId == 0 ? User.Identity.GetUserId() : userId;
            roleType = string.IsNullOrEmpty(roleType) ? User.Identity.GetUserRoleName() : roleType;
            var data = await meter.GetNewMeterDashboardCount(roleType, userId);
            return Ok(jsonResponseFromat.JsonMessage(true, DataDetails, data));
        }


        [Route("GetMeterCountReport")]
        [HttpGet]
        public async Task<IActionResult> GetMeterCountReport()
        {
            var data = await meter.GetMeterCountWithName(User.Identity.GetUserRoleName(), User.Identity.GetUserId());
            return Ok(jsonResponseFromat.JsonMessage(true, "Meter Count List", data));
        }


        [Route("UpdateHesStatus")]
        [HttpPost]
        public async Task<IActionResult> UpdateHesStatus([FromBody] ListOfConsumerId data)
        {
            string userName =
                User?.FindFirst("Name")?.Value ??
                User?.Identity?.Name ??
                "SYSTEM";

            var ids = basicUtility.GenrateCommaStringIds(data.ID);

            // Audit enrichment: pushed ids ke new meter no + new consumer id nikalo (audit log me dikhenge)
            try
            {
                var pushInfos = await meter.GetMeterNosByIds(data.AreaType, ids);
                var meterNos = pushInfos.Select(x => x.MeterNo).Where(x => !string.IsNullOrWhiteSpace(x)).Take(20).ToList();
                var newConsumerIds = pushInfos.Select(x => x.NewConsumerId).Where(x => !string.IsNullOrWhiteSpace(x)).Take(20).ToList();

                HttpContext.Items["Audit_EntityId"] = string.Join(", ", meterNos);

                var suffixParts = new List<string>();
                if (meterNos.Count > 0) suffixParts.Add($"NewMeterNo: {string.Join(", ", meterNos)}");
                if (newConsumerIds.Count > 0) suffixParts.Add($"NewConsumerId: {string.Join(", ", newConsumerIds)}");
                HttpContext.Items["Audit_DescSuffix"] = suffixParts.Count > 0 ? " | " + string.Join(" | ", suffixParts) : null;
            }
            catch { /* enrichment fail-safe */ }

            try
            {
                await meter.UpdateHesStatus(data.AreaType, ids, "PENDING", userName);

                // CONSUMER (new consumer meter table): selected meters 1-by-1 SYNC push +
                // response me poora push log (payload + HES response + status) wapas jata hai
                if (string.Equals(data.AreaType?.Trim(), "CONSUMER", StringComparison.OrdinalIgnoreCase))
                {
                    var pushResult = await hesMeter.PushSelectedNow(data.AreaType, ids);

                    // Fail/leftover PENDING rows ka background safety-net (5 min delay -
                    // sync push complete hone ke baad chale, double-push race avoid)
                    BackgroundJob.Schedule<HesMeter>(x => x.SendToHes(data.AreaType), TimeSpan.FromMinutes(5));

                    bool pushed = string.IsNullOrEmpty(pushResult.Error);
                    string message;
                    if (!pushed)
                        message = $"HES Push Failed: {pushResult.Error}";
                    else if (pushResult.Total == 0)
                        message = "HES Push: koi PENDING meter nahi mila (shayad pehle hi COMPLETED)";
                    else
                        message = $"HES Push Completed: {pushResult.Success}/{pushResult.Total} success"
                                  + (pushResult.Failed > 0 ? $", {pushResult.Failed} failed (retry hoga)" : string.Empty);

                    return Ok(jsonResponseFromat.JsonMessage(pushed, message,
                        new
                        {
                            summary = new { pushResult.AreaType, pushResult.Total, pushResult.Success, pushResult.Failed, pushResult.Error },
                            logs = pushResult.Logs
                        }));
                }

                BackgroundJob.Enqueue<HesMeter>(x => x.SendToHes(data.AreaType));

                return Ok(jsonResponseFromat.JsonMessage(true, "Updated Successfully"));
            }
            catch (InvalidOperationException ex)
            {
                return Ok(jsonResponseFromat.JsonMessage(false, ex.Message));
            }
            catch (Exception ex)
            {
                return Ok(jsonResponseFromat.JsonMessage(false, $" {ex.Message}"));
            }
        }


        [AutomaticRetry(Attempts = 0)]
        [NonAction]
        public async Task SyncDTCNewMeter(int userId)
        {
            await meter.SyncDTCNewMeter(userId);
        }
        [AutomaticRetry(Attempts = 0)]
        [NonAction]
        public async Task SyncDTNewMeterData(int userId)
        {
            await meter.SyncDTNewMeter(userId);
        }
        [AutomaticRetry(Attempts = 0)]
        [NonAction]
        public async Task SyncConsumerNewMeterData(int userId)
        {
            await meter.SyncConsumerNewMeter(userId);
        }
        [AutomaticRetry(Attempts = 0)]
        [NonAction]
        public async Task SyncFeederNewMeterData(int userId)
        {
            await meter.SyncFeederNewMeter(userId);
        }

        [HttpPost]
        [Route("MeterSwap")]
        public async Task<IActionResult> MeterSwap([FromBody] MeterSwapRequest request)
        {
            if (request == null)
                return BadRequest(jsonResponseFromat.JsonMessage(false, "Request body is required."));

            var consumerId = request.ConsumerId?.Trim();
            var oldMeterNo = request.OldMeterNo?.Trim();
            var newMeterNo = request.NewMeterNo?.Trim();

            if (string.IsNullOrWhiteSpace(consumerId) || string.IsNullOrWhiteSpace(oldMeterNo) || string.IsNullOrWhiteSpace(newMeterNo))
                return BadRequest(jsonResponseFromat.JsonMessage(false, "All fields are required."));

            int userId = User.Identity.GetUserId();

            var result = await meter.SwapMeter(consumerId, oldMeterNo, newMeterNo, userId);

            if (result == "Meter swapped successfully.")
            {
                // ── Full detail snapshot SwapMeter table me save karo (consumer ke according) ──
                // Snapshot fail ho to swap bhi nahi aana chahiye - dono status UI ko dikhte hain.
                bool snapshotSaved = false;
                string snapshotMessage = null;
                try
                {
                    var saveResult = await meter.SaveSwapMeterDetail(consumerId, oldMeterNo, newMeterNo, userId, request.SwapRemark);

                    if (saveResult is SmartMeterAdmin.ViewModel.MeterViewModel.SwapMeterSaveResultDto dto
                        && int.TryParse(dto.SwapId, out _))
                    {
                        snapshotSaved = true;
                    }
                    else
                    {
                        snapshotMessage = (saveResult as SmartMeterAdmin.ViewModel.MeterViewModel.SwapMeterSaveResultDto)?.SwapId
                                          ?? "Snapshot could not be saved.";
                    }
                }
                catch (Exception ex)
                {
                    snapshotMessage = ex.Message;
                }

                if (snapshotSaved)
                    return Ok(jsonResponseFromat.JsonMessage(true, "Meter swapped successfully."));

                return Ok(jsonResponseFromat.JsonMessage(true, result, new { snapshotSaved, error = snapshotMessage }));
            }

            return Ok(jsonResponseFromat.JsonMessage(false, result));
        }

        // ── Swap Meter History (list + expand snapshot) ──
        [HttpGet]
        [Route("GetSwapMeterList")]
        public async Task<IActionResult> GetSwapMeterList([FromQuery] string search = "", [FromQuery] int pageNumber = 0, [FromQuery] int pageSize = 50)
        {
            try
            {
                var data = await meter.GetSwapMeterList(search, pageNumber, pageSize);
                return Ok(jsonResponseFromat.JsonMessage(true, "Swap Meter List", data));
            }
            catch (Exception ex)
            {
                return Ok(jsonResponseFromat.JsonMessage(false, ex.Message));
            }
        }

        // DTO for MeterSwap request body
        public class MeterSwapRequest
        {
            public string ConsumerId { get; set; }
            public string OldMeterNo { get; set; }
            public string NewMeterNo { get; set; }
            public string SwapRemark { get; set; }   // NEW: optional user remark from UI
        }
    }
}