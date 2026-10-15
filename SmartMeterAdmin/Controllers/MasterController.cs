using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SmartmeterAdmin.Hangfire.ExcelJobs;
using SmartMeterAdmin.Infrastructure.Enumeration;
using SmartMeterAdmin.Infrastructure.Helpers;
using SmartMeterAdmin.Services.Master;
using SmartMeterAdmin.Services.Meter;
using SmartMeterAdmin.ViewModel.DataTable;
using SmartMeterAdmin.ViewModel.Master;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static SmartMeterAdmin.Infrastructure.Helpers.ConstantValues;
namespace SmartMeterAdmin.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/master")]
    public class MasterController : ControllerBase
    {
        private readonly IJsonResponseFromat jsonResponseFromat;
        private readonly IMaster master;
        private readonly IWebHostEnvironment env;
        private readonly IConfiguration configuration;
        private readonly IExportUtility exportUtility;
        private readonly IMeter meter;
        private readonly IBasicUtility basicUtility;

        public MasterController(IJsonResponseFromat jsonResponseFromat, IMaster master, IWebHostEnvironment env, IConfiguration configuration, IExportUtility exportUtility, IMeter meter, IBasicUtility basicUtility)
        {
            this.jsonResponseFromat = jsonResponseFromat;
            this.master = master;
            this.env = env;
            this.configuration = configuration;
            this.exportUtility = exportUtility;
            this.meter = meter;
            this.basicUtility = basicUtility;
        }

        [Route("GetAreaDropdown")]
        [HttpGet]
        public async Task<IActionResult> GetAreaDropdown(string areaType, string areaValue)
        {
            var data = await master.GetAreaDropDown(areaType, areaValue);
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, data));
        }

        [Route("GetAreaDetails")]
        [HttpGet]
        public async Task<IActionResult> GetAreaDetails(string areaType, string areaValue)
        {
            var data = await master.GetAreaDetails(areaType, areaValue);
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, data));
        }


        [Route("DeleteArea")]
        [HttpPost]
        public async Task<IActionResult> DeleteArea([FromBody] ListOfId data)
        {
            string AuthenticationPassword = configuration.GetSection("Authentication")["Password"];
            if (AuthenticationPassword != data.Password)
                return Ok(jsonResponseFromat.JsonMessage(false, PasswordNotMatch));

            var ids = basicUtility.GenrateCommaStringIds(data.ID);
            await master.DeleteArea(data.Password, ids);
            return Ok(jsonResponseFromat.JsonMessage(true, DeletedSuccess));
        }


        #region Zone


        [Route("ManageZone")]
        [HttpPost]
        public async Task<IActionResult> ManageZone([FromBody] ZoneViewModel model)
        {
            model.CreatedBy = User.Identity.GetUserId();
            await master.ManageZone(model);
            return Ok(jsonResponseFromat.JsonMessage(true, CreatedSuccess));
        }


        [Route("GetZoneList")]
        [HttpGet]
        public async Task<IActionResult> GetZoneList()
        {
            var data = await master.GetAllZone(User.Identity.GetUserRoleName());
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, data));
        }

        #endregion

        #region Circle


        [Route("ManageCircle")]
        [HttpPost]
        public async Task<IActionResult> ManageCircle([FromBody] CircleViewModel model)
        {
            model.CreatedBy = User.Identity.GetUserId();
            await master.ManageCircle(model);
            return Ok(jsonResponseFromat.JsonMessage(true, CreatedSuccess));
        }

        [Route("GetAllCircle")]
        [HttpGet]
        public async Task<IActionResult> GetAllCircle()
        {
            var data = await master.GetAllCircles(User.Identity.GetUserRoleName());
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, data));
        }

        #endregion

        #region Division


        [Route("ManageDivision")]
        [HttpPost]
        public async Task<IActionResult> ManageDivision([FromBody] DivisionViewModel model)
        {
            model.CreatedBy = User.Identity.GetUserId();
            await master.ManageDivision(model);
            return Ok(jsonResponseFromat.JsonMessage(true, CreatedSuccess));
        }

        [Route("GetAllDivision")]
        [HttpGet]

        public async Task<IActionResult> GetAllDivision()
        {
            var data = await master.GetAllDivision(User.Identity.GetUserRoleName());
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, data));
        }

        #endregion

        #region SubDivision


        [Route("ManageSubDivision")]
        [HttpPost]
        public async Task<IActionResult> ManageSubDivision([FromBody] SubdivisionViewModel model)
        {
            model.CreatedBy = User.Identity.GetUserId();
            await master.ManageSubdivision(model);
            return Ok(jsonResponseFromat.JsonMessage(true, CreatedSuccess));
        }

        [Route("GetAllSubdivision")]
        [HttpGet]
        public async Task<IActionResult> GetAllSubdivision()
        {
            var data = await master.GetAllSubdivision(User.Identity.GetUserRoleName());
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, data));
        }

        #endregion

        #region SubSubstation


        [Route("ManageSubSubstation")]
        [HttpPost]
        public async Task<IActionResult> ManageSubSubstation([FromBody] SubstationViewModel model)
        {
            model.CreatedBy = User.Identity.GetUserId();
            await master.ManageSubstation(model);
            return Ok(jsonResponseFromat.JsonMessage(true, CreatedSuccess));
        }

        [Route("GetAllSubstation")]
        [HttpGet]
        public async Task<IActionResult> GetAllSubstation()
        {
            var data = await master.GetAllSubstation(User.Identity.GetUserRoleName());
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, data));
        }

        #endregion

        #region SubFeeder

        [Route("ManageFeeder")]
        [HttpPost]
        public async Task<IActionResult> ManageFeeder([FromBody] FeederViewModel model)
        {
            model.CreatedBy = User.Identity.GetUserId();
            await master.ManageFeeder(model);
            return Ok(jsonResponseFromat.JsonMessage(true, CreatedSuccess));
        }

        [Route("GetAllFeeder")]
        [HttpGet]

        public async Task<IActionResult> GetAllFeeder()
        {
            var data = await master.GetAllFeeder(User.Identity.GetUserRoleName());
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, data));
        }

        #endregion

        #region DT


        [Route("ManageDT")]
        [HttpPost]
        public async Task<IActionResult> ManageDT([FromBody] DTViewModel model)
        {
            model.CreatedBy = User.Identity.GetUserId();
            await master.ManageDT(model);
            return Ok(jsonResponseFromat.JsonMessage(true, CreatedSuccess));
        }

        [Route("GetAllDT")]
        [HttpGet]
        public async Task<IActionResult> GetAllDT()
        {
            var data = await master.GetAllDT(User.Identity.GetUserRoleName());
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, data));
        }

        [Route("GetDtData")]
        [HttpGet]
        public async Task<IActionResult> GetDtData(string DtCode)
        {
            var data = await master.GetDTData(DtCode);
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, data));
        }

        #endregion

        #region SimData



        [Route("GetSimDataRender")]
        [HttpPost]
        public async Task<IActionResult> GetSimDataRender([FromBody] AgGridTable dataTable)
        {

            var data = await master.GetSimDataRender(dataTable);
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, data));


        }



        #endregion

        #region Upload Excel


        [Route("SyncCustomerData")]
        [HttpGet]
        public async Task<IActionResult> SyncCustomerData()
        {
            await master.SyncCustomerData(User.Identity.GetUserId());
            return Ok(jsonResponseFromat.JsonMessage(true, RecordSyncSuccess));
        }



        [HttpPost]
        [Route("UploadExcelProduct")]
        public IActionResult UploadExcelProduct(IFormFile postedFile)
        {

            if (postedFile != null)
            {

                int userid = User.Identity.GetUserId();
                //Create a Folder.
                string path = Path.Combine(env.WebRootPath, "ExcelUploads");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                //Save the uploaded Excel file.
                string fileName = Path.GetFileNameWithoutExtension(postedFile.FileName);
                string extension = Path.GetExtension(postedFile.FileName);

                if (!AllowedFileExtensions.GetAllowedFile("Excel").Contains(extension))
                    return Ok(jsonResponseFromat.JsonMessage(false, SelectValidExcel));

                string filePath = Path.Combine(path, fileName + DateTime.Now.ToString("yyyyMMddHHmmss") + extension);
                using (FileStream stream = new(filePath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }


                //Background Processing
                var jobId = BackgroundJob.Enqueue<ImportExcelDataIntoDatabase>(x => x.ImportExcelData(filePath, User.Identity.GetUserId(), TableNameEnum.TempSimData));
                BackgroundJob.ContinueJobWith(jobId, () => SyncData(User.Identity.GetUserId(), TableNameEnum.TempSimData));

                return Ok(jsonResponseFromat.JsonMessage(true, FileUploaded));
            }


            return Ok(jsonResponseFromat.JsonMessage(false, SelectValidExcel));
        }

        [HttpPost]
        [Route("UploadSimStatusExcel")]
        public IActionResult UploadSimStatusExcel(IFormFile postedFile)
        {

            if (postedFile != null)
            {

                int userid = User.Identity.GetUserId();
                //Create a Folder.
                string path = Path.Combine(env.WebRootPath, "ExcelUploads");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                //Save the uploaded Excel file.
                string fileName = Path.GetFileNameWithoutExtension(postedFile.FileName);
                string extension = Path.GetExtension(postedFile.FileName);

                if (!AllowedFileExtensions.GetAllowedFile("Excel").Contains(extension))
                    return Ok(jsonResponseFromat.JsonMessage(false, SelectValidExcel));

                string filePath = Path.Combine(path, fileName + DateTime.Now.ToString("yyyyMMddHHmmss") + extension);
                using (FileStream stream = new(filePath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                //Background Processing
                var jobId = BackgroundJob.Enqueue<ImportExcelDataIntoDatabase>(x => x.ImportExcelData(filePath, User.Identity.GetUserId(), TableNameEnum.TempDataSimStatus));
                BackgroundJob.ContinueJobWith(jobId, () => SyncData(User.Identity.GetUserId(), TableNameEnum.TempDataSimStatus));

                return Ok(jsonResponseFromat.JsonMessage(true, FileUploaded));
            }

            return Ok(jsonResponseFromat.JsonMessage(false, SelectValidExcel));
        }

        [HttpPost]
        [Route("UploadExcelCustomer")]
        public IActionResult UploadExcelCustomer(IFormFile postedFile)
        {
            if (postedFile != null)
            {

                int userid = User.Identity.GetUserId();
                //Create a Folder.
                string path = Path.Combine(env.WebRootPath, "ExcelUploads");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                //Save the uploaded Excel file.
                string fileName = Path.GetFileNameWithoutExtension(postedFile.FileName);
                string extension = Path.GetExtension(postedFile.FileName);

                if (!AllowedFileExtensions.GetAllowedFile("Excel").Contains(extension))
                    return Ok(jsonResponseFromat.JsonMessage(false, SelectValidExcel));

                string filePath = Path.Combine(path, fileName + DateTime.Now.ToString("yyyyMMddHHmmss") + extension);
                using (FileStream stream = new(filePath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                //Background Processing
                var jobId = BackgroundJob.Enqueue<ImportExcelDataIntoDatabase>(x => x.ImportExcelData(filePath, User.Identity.GetUserId(), TableNameEnum.TempFullCustomerData));
                BackgroundJob.ContinueJobWith(jobId, () => SyncData(User.Identity.GetUserId(), TableNameEnum.TempFullCustomerData));

                return Ok(jsonResponseFromat.JsonMessage(true, FileUploaded));
            }

            return Ok(jsonResponseFromat.JsonMessage(false, SelectValidExcel));
        }

        [HttpPost]
        [Route("GetCustomerData")]
        public async Task<IActionResult> GetCustomerData([FromBody] AgGridTable dataTable)
        {
            var data = await master.GetCustomerData(User.Identity.GetUserRoleName(), User.Identity.GetUserId(), dataTable);
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, data));
        }

        [HttpGet]
        [Route("ExportDataExcel")]
        public async Task<IActionResult> ExportDataExcel(string dataType, string actionType)
        {
            var data = new ExportExcelResponse();
            if (actionType == "Download")
                data = await master.ManageExportDataExcel(actionType, dataType, string.Empty);
            else
                BackgroundJob.Enqueue<ExportDataExcel>(x => x.ExportExcelData(dataType));

            return Ok(jsonResponseFromat.JsonMessage(true, "Data Export Request Submitted", data));

        }

        public class SelectedCustomerID
        {
            public List<int> ID { get; set; }
            public string Password { get; set; }
        }

        [HttpPost]
        [Route("DeleteSelectedCustomer")]
        public async Task<IActionResult> DeleteSelectedCustomer([FromBody] SelectedCustomerID id)
        {
            string AuthenticationPassword = configuration.GetSection("Authentication")["Password"];
            if (AuthenticationPassword != id.Password)
                return Ok(jsonResponseFromat.JsonMessage(false, PasswordNotMatch));
            await master.DeleteSelectedCustomer(id.ID);
            return Ok(jsonResponseFromat.JsonMessage(true, DeletedSuccess));
        }

        [HttpPost]
        [Route("DeleteSelectedSim")]
        public async Task<IActionResult> DeleteSelectedSim([FromBody] SelectedCustomerID id)
        {
            string AuthenticationPassword = configuration.GetSection("Authentication")["Password"];
            if (AuthenticationPassword != id.Password)
                return Ok(jsonResponseFromat.JsonMessage(false, PasswordNotMatch));
            await master.DeleteSelectedSim(id.ID);
            return Ok(jsonResponseFromat.JsonMessage(true, DeletedSuccess));
        }

        [HttpPost]
        [Route("UpdateDataByExcel")]
        public IActionResult UpdateDataByExcel(IFormFile postedFile)
        {
            if (postedFile != null)
            {
                int userId = User.Identity.GetUserId();
                string path = Path.Combine(env.WebRootPath, "UpdateDataByExcel");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string fileName = Path.GetFileNameWithoutExtension(postedFile.FileName);
                string extension = Path.GetExtension(postedFile.FileName);

                if (!AllowedFileExtensions.GetAllowedFile("Excel").Contains(extension))
                    return Ok(jsonResponseFromat.JsonMessage(false, SelectValidExcel));

                string filePath = Path.Combine(path, fileName + DateTime.Now.ToString("yyyyMMddHHmmss") + extension);
                using (FileStream stream = new(filePath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                var jobId = BackgroundJob.Enqueue<ImportExcelDataIntoDatabase>(x => x.ImportExcelData(filePath, userId, TableNameEnum.TEMP_UPDATED_DATA));
                BackgroundJob.ContinueJobWith(jobId, () => SyncData(userId, TableNameEnum.TEMP_UPDATED_DATA));

                return Ok(jsonResponseFromat.JsonMessage(true, FileUploaded));
            }
            return Ok(jsonResponseFromat.JsonMessage(false, SelectValidExcel));
        }
        #endregion



        #region Get data for Mobile 

        [Route("GetSimDataForMobile")]
        [HttpGet]
        public async Task<IActionResult> GetSimDataForMobile(DateTime date)
        {
            if (date == DateTime.MinValue)
            {
                date = DateTime.Now;
            }
            var data = await master.GetSimDataForMobile(date);
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, data));
        }

        [AllowAnonymous]
        [Route("GetCustomerDataForMobile")]
        [HttpGet]
        public async Task<IActionResult> GetCustomerDataForMobile(int recivedata)
        {
            var data = await meter.GetCustomerDetailsByCustomerNumber(recivedata);
            var trecords = await meter.CountCustomerDetails();

            var jsonresult = new
            {
                trecords,
                recivedata = data.Count() + recivedata,
                success = true,
                message = "CustomerData",
                data
            };
            return Ok(jsonresult);
        }




        #endregion

        #region Dynamic Table Column

        [Route("GetTableColumnDropDown")]
        [HttpGet]
        public async Task<IActionResult> GetTableColumnDropDown(TableNameEnum tableName, int templateId)
        {
            var data = await master.GetTableColumnByUserId(tableName, User.Identity.GetUserId(), templateId);
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, data));
        }

        [Route("UpdateUserTableColumn")]
        [HttpPost]
        public async Task<IActionResult> UpdateUserTableColumn([FromBody] IEnumerable<TableColumnDto> tableName)
        {
            await master.UpdateTableColumnByUserId(tableName, User.Identity.GetUserId());
            return Ok(jsonResponseFromat.JsonMessage(true, UpdateSuccess));
        }

        [Route("GetTableTemplate")]
        [HttpGet]
        public async Task<IActionResult> GetTableTemplate()
        {
            var data = await master.GetTableTemplate(User.Identity.GetUserId());
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, data));
        }

        [Route("GetTableTemplateById")]
        [HttpGet]
        public async Task<IActionResult> GetTableTemplateById(int id)
        {
            var data = await master.GetTableTemplate(User.Identity.GetUserId(), id);
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, data));
        }

        [Route("DeleteTableTemplate")]
        [HttpPost]
        public async Task<IActionResult> DeleteTableTemplate([FromBody] ListOfId ids)
        {
            string AuthenticationPassword = configuration.GetSection("Authentication")["Password"];
            if (AuthenticationPassword != ids.Password)
                return Ok(jsonResponseFromat.JsonMessage(false, PasswordNotMatch));

            foreach (var item in ids.ID)
            {
                var data = new TableTemplate
                {
                    Id = item,
                    UserId = User.Identity.GetUserId()
                };
                await master.ManageTableTemplate(data, "DELETE");
            }

            return Ok(jsonResponseFromat.JsonMessage(true, DeletedSuccess));
        }

        [Route("UpdateTableColumn")]
        [HttpPost]
        public async Task<IActionResult> UpdateTableColumn([FromBody] TableTemplate template)
        {
            template.UserId = User.Identity.GetUserId();
            await master.ManageTableTemplate(template, "Manage");
            return Ok(jsonResponseFromat.JsonMessage(true, UpdateSuccess));
        }


        [Route("GetTableTemplateDropDown")]
        [HttpGet]
        public async Task<IActionResult> GetTableTemplateDropDown()
        {
            var role = Enum.Parse(typeof(RoleEnum), User.Identity.GetUserRoleName());
            var data = await master.GetTableTemplateDropDown((RoleEnum)role, User.Identity.GetUserId(), "TABLE_TEMPLATE");
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, data));
        }



        #endregion Dynamic Table Column

        [AutomaticRetry(Attempts = 0)]
        [NonAction]
        public async Task SyncData(int userId, TableNameEnum tableName)
        {
            switch (tableName)
            {
                case TableNameEnum.TempSimData:
                    await master.SyncData(userId);
                    break;
                case TableNameEnum.TempDataSimStatus:
                    _ = await master.SyncSimStatusData(userId);
                    break;
                case TableNameEnum.TempFullCustomerData:
                    await master.SyncCustomerData(userId);
                    break;
                case TableNameEnum.TEMP_UPDATED_DATA:
                    await master.UpdateDataByKeys(userId);
                    break;
            }
        }
    }
}