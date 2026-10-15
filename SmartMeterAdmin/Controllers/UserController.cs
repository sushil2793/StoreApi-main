using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SmartMeterAdmin.Infrastructure.Enumeration;
using SmartMeterAdmin.Infrastructure.Helpers;
using SmartMeterAdmin.Services.User;
using SmartMeterAdmin.ViewModel.AuditLog;
using SmartMeterAdmin.ViewModel.UserViewModel;

namespace SmartMeterAdmin.Controllers
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/User")]
    public class UserController : Controller
    {
        public UserController(IUser user, IJsonResponseFromat jsonResponseFromat, IBasicUtility basicUtility)
        {
            BasicUtility = basicUtility;
            this.user = user;
            JsonResponseFromat = jsonResponseFromat;
        }
        public IBasicUtility BasicUtility { get; }
        private CryptoUtility cryptoUtility;
        private readonly IUser user;

        public IJsonResponseFromat JsonResponseFromat { get; }

        [Route("ManageUser")]
        [HttpPost]
        public async Task<IActionResult> ManageUser([FromBody] UserViewModel model)
        {
            cryptoUtility ??= new CryptoUtility();

            model.ProfileImage = !string.IsNullOrEmpty(model.ProfileImage) ? BasicUtility.Base64ToImage(model.ProfileImage, "ProfileImage") : null;
            model.DocumentUrl = !string.IsNullOrEmpty(model.DocumentUrl) ? BasicUtility.Base64ToImage(model.DocumentUrl, "DocumentUrl") : null;
            model.AreaValue = GetAreaValue(model);
            model.CreatedBy = model.CreatedBy > 0 ? model.CreatedBy : User.Identity.GetUserId();
            var Password = model.Password is not null ? cryptoUtility.Encrypt(model.Password) : null;

            await user.ManageUser(model, Password);
            return Ok(JsonResponseFromat.JsonMessage(true, "Data Save Successfully"));
        }

        [Route("GetAllUserByRole")]
        [HttpGet]
        public async Task<IActionResult> GetAllUserByRole(string roletype)
        {

            var result = await user.GetAllUserByRole(roletype, User.Identity.GetUserId(), User.Identity.GetUserRoleName());
            return Ok(JsonResponseFromat.JsonMessage(true, "User List", result));


        }        [Route("GetAllDeactivatedUser")]
        [HttpGet]
        public async Task<IActionResult> GetAllDeactivatedUser(string roletype)
        {

            var result = await user.GetAllDeactivatedUserByRole(roletype);
            return Ok(JsonResponseFromat.JsonMessage(true, "User List", result));



        }

        // ==================== USER AUDIT ACCESS (UI se manage) ====================
        // SIRF master Superadmin (UserID = 3) ke liye. Baaki SAB (Admin + baaki Superadmins) ko 403.
        // NOTE: UserID 3 = 'Superadmin' master account (local + production same restore hai).
        private const int AccessManageAllowedUserId = 3; // agar future me change karna ho to yahin karo

        private bool IsAccessManager()
        {
            var role = User.Identity?.GetUserRoleName();
            if (!string.Equals(role, "Superadmin", StringComparison.OrdinalIgnoreCase))
                return false;

            return User.Identity?.GetUserId() == AccessManageAllowedUserId;
        }

        [Route("GetUserAuditAccessList")]
        [HttpGet]
        public async Task<IActionResult> GetUserAuditAccessList(string roleType, string search)
        {
            if (!IsAccessManager())
                return StatusCode(403, JsonResponseFromat.JsonMessage(false, "Access denied."));

            var result = await user.GetUserAuditAccessList(roleType, search);
            return Ok(JsonResponseFromat.JsonMessage(true, "Access List", result));
        }

        [Route("SaveUserAuditAccess")]
        [HttpPost]
        public async Task<IActionResult> SaveUserAuditAccess([FromBody] SaveUserAuditAccessRequest model)
        {
            if (!IsAccessManager())
                return StatusCode(403, JsonResponseFromat.JsonMessage(false, "Access denied."));

            if (model == null || model.UserID <= 0)
                return Ok(JsonResponseFromat.JsonMessage(false, "Invalid user."));

            var updatedBy = User.Identity?.GetName();
            // CanDelete hata diya - CanEdit hi combined "Edit & Delete" right hai (2 permissions: View | Edit & Delete)
            var result = await user.SaveUserAuditAccess(model.UserID, model.CanView, model.CanEdit, updatedBy);

            // Audit access cache (5-min) turant invalidate karo - changes turant effective hon
            AuditController.ClearCanViewCache(model.UserID);

            return Ok(JsonResponseFromat.JsonMessage(true, "Access saved successfully", result));
        }

        [Route("GetUserByID")]
        [HttpGet]
        public async Task<IActionResult> GetUserByID(int id)
        {
            var userData = await user.GetUserById(id);
            return Ok(JsonResponseFromat.JsonMessage(true, "User Details", userData));



        }


        [Route("GetUserProfileByID")]
        [HttpGet]
        public async Task<IActionResult> GetUserProfileByID(int id)
        {

            var result = await user.GetUserProfileByID(id);
            return Ok(JsonResponseFromat.JsonMessage(true, "User List", result));



        }


        [Route("DeleteUser")]
        [HttpGet]
        public async Task<IActionResult> DeleteUser(int id)
        {

            await user.ActivateUser(id, User.Identity.GetUserId(), true);
            return Ok(JsonResponseFromat.JsonMessage(true, "Delete User List", id));



        }

        [Route("ActivateUser")]
        [HttpGet]
        public async Task<IActionResult> ActivateUser(int id)
        {
            await user.ActivateUser(id, User.Identity.GetUserId(), false);
            return Ok(JsonResponseFromat.JsonMessage(true, "Active User List", id));
        }


        [Route("GetUserDropDown")]
        [HttpGet]
        public async Task<IActionResult> GetUserDropDown(RoleEnum role,int userId)
        {
            userId = userId == 0 ? User.Identity.GetUserId() : userId;
            var data = await user.GetUserDropDown(role, userId, "USER");
            return Ok(JsonResponseFromat.JsonMessage(true, "Active User List", data));
        }

        private static string GetAreaValue(UserViewModel model)
        {
            return model.AreaType switch
            {
                "Zone" => model.ZoneName,
                "Circle" => model.CircleName,
                "Division" => model.DivisionName,
                "Subdivision" => model.SubstationName,
                "Substation" => model.SubstationName,
                "Feeder" => model.FeederName,
                "DT" => model.DTName,
                _ => string.Empty,
            };
        }
    }
}