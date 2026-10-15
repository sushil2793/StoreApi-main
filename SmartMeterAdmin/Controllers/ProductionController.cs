using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SmartmeterAdmin.Hangfire.ExcelJobs;
using SmartMeterAdmin.Data;
using SmartMeterAdmin.Infrastructure.Enumeration;
using SmartMeterAdmin.Infrastructure.Helpers;
using SmartMeterAdmin.Services.Production;
using SmartMeterAdmin.ViewModel.DataTable;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static SmartMeterAdmin.Infrastructure.Helpers.ConstantValues;

namespace SmartMeterAdmin.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/Production")]
    public class ProductionController : ControllerBase
    {
        private readonly IWebHostEnvironment env;
        private readonly IConfiguration configuration;
        private readonly IJsonResponseFromat jsonResponseFromat;
        private readonly IProduction production;

        public ProductionController(IWebHostEnvironment env, IConfiguration configuration, IJsonResponseFromat jsonResponseFromat, IProduction production)
        {
            this.env = env;
            this.configuration = configuration;
            this.jsonResponseFromat = jsonResponseFromat;
            this.production = production;
        }

        [HttpPost]
        [Route("ImportProductionMeterByExcel")]
        public IActionResult ImportProductionMeterByExcel(IFormFile postedFile, int storeId)
        {
            if (postedFile != null)
            {
                int userId = User.Identity.GetUserId();
                // Create a Folder
                string path = Path.Combine(env.WebRootPath, "ProductionMeterExcel");
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
                var jobId = BackgroundJob.Enqueue<ImportExcelDataIntoDatabase>(x => x.ImportExcelData(filePath, User.Identity.GetUserId(), TableNameEnum.TempProductionMeter));
                BackgroundJob.ContinueJobWith(jobId, () => SyncProductionMeters(User.Identity.GetUserId(), storeId));

                return Ok(jsonResponseFromat.JsonMessage(true, FileUploaded));
            }

            return Ok(jsonResponseFromat.JsonMessage(false, SelectValidExcel));
        }

        [Route("GetAllProductionMeter")]
        [HttpPost]
        public async Task<IActionResult> GetAllProductionMeter([FromBody] AgGridTable dataTable)
        {
            dataTable.RoleType = string.IsNullOrEmpty(dataTable.RoleType) ? User.Identity.GetUserRoleName() : dataTable.RoleType;
            dataTable.UserId = User.Identity.GetUserId();
            var data = await production.GetProductionMetersList(dataTable);
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, data));
        }

        [Route("GetDashboardCount")]
        [HttpGet]
        public async Task<IActionResult> GetDashboardCount()
        {
            // Ensure BoxID is considered in dashboard calculations
            var data = await production.GetDashboardCount(User.Identity.GetUserId(), User.Identity.GetUserRoleName());
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, data));
        }

        [Route("GetDashboardList")]
        [HttpGet]
        public async Task<IActionResult> GetDashboardList()
        {
            // Ensure BoxID is included in the dashboard list response
            var data = await production.GetDashboardList(User.Identity.GetUserId(), User.Identity.GetUserRoleName());
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, data));
        }

        [AutomaticRetry(Attempts = 0)]
        [NonAction]
        public async Task SyncProductionMeters(int userId, int storeId)
        {
            await production.SyncProductionMeter(userId, storeId);
        }

        [Route("GetMeterNo")]
        [HttpGet]
        public async Task<IActionResult> GetMeterNo(string meterNo)
        {
            var data = await production.GetMeterNoList(meterNo, User.Identity.GetUserRoleName(), User.Identity.GetUserId());
            return Ok(jsonResponseFromat.JsonMessage(true, DataList, data));
        }

    }
}
