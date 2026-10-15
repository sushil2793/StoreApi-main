using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SmartMeterAdmin.Data.HelpEntity;
using SmartMeterAdmin.Infrastructure.Helpers;
using SmartMeterAdmin.Services.Help;
using static SmartMeterAdmin.Infrastructure.Helpers.ConstantValues;
namespace SmartMeterAdmin.Controllers
{
    [Produces("application/json")]
    [Route("api/HelpDocument")]
    public class HelpDocumentController : ControllerBase
    {
        private readonly IHelpDocument _helpDocument;
        private readonly IJsonResponseFromat _jsonResponseFromat;
        private readonly IConfiguration _configuration;

        public HelpDocumentController(IHelpDocument helpDocument, IJsonResponseFromat jsonResponseFromat, IConfiguration configuration)
        {
            _helpDocument = helpDocument;
            _jsonResponseFromat = jsonResponseFromat;
            _configuration = configuration;
        }


        #region New Meter




        [Route("ManageHelpDocument")]
        [HttpPost]
        public async Task<IActionResult> ManageHelpDocument([FromBody]HelpDocument model)
        {
            if (model.HelpDocumentID > 0)
            {
                HelpDocument helpDocument = await _helpDocument.GetHelpDocumentById(model.HelpDocumentID);
                helpDocument.DocumentName = model.DocumentName;
              
                await _helpDocument.Update(helpDocument);
            }
            else
            {
                model.Created = DateTime.Now;
                await _helpDocument.Create(model);

            }
            return Ok(_jsonResponseFromat.JsonMessage(true, UpdateSuccess));
        }
       
        [Route("GetAllHelpDocument")]
        [HttpGet]
        public async Task<IActionResult> GetAllHelpDocument(int createdbyid)
        {
            
                var result = await _helpDocument.GetHelpDocumentById(createdbyid);
                return Ok(_jsonResponseFromat.JsonMessage(true, MeterList, result));
           

        }
        [Route("GetHelpDocument")]
        [HttpGet]
        public async Task<IActionResult> GetHelpDocument(int id)
        {
           
                var result = await _helpDocument.GetHelpDocumentById(id);
                return Ok(_jsonResponseFromat.JsonMessage(true, UserList, result));
           


        }
        [Route("DeleteHelpDocument")]
       
        [HttpGet]
        public async Task<IActionResult> DeleteNewMeter(int id, string password)
        {
            string AuthenticationPassword = _configuration.GetSection("Authentication")["Password"];
            if (AuthenticationPassword != password)
                return Ok(_jsonResponseFromat.JsonMessage(false, PasswordNotMatch));

            var result = await _helpDocument.GetHelpDocumentById(id);
                result.IsActive = false;
                result.IsDelete = true;
                result.UpdatedBy = 1;
                result.Updated = DateTime.Now;
                await _helpDocument.Update(result);
                return Ok(_jsonResponseFromat.JsonMessage(true, DeletedUserList, result));
           


        }


        #endregion


    }
}