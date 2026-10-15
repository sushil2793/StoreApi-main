using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SmartMeterAdmin.Infrastructure.Helpers;
using SmartMeterAdmin.Services.User;
using SmartMeterAdmin.ViewModel;
using SmartMeterAdmin.ViewModel.Login;
using SmartMeterAdmin.ViewModel.Master;
using static SmartMeterAdmin.Infrastructure.Helpers.ConstantValues;

namespace SmartMeterAdmin.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/Account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        public AccountController(IUser user, IOptions<ApplicationSettings> appSettings, IJsonResponseFromat jsonResponseFromat)
        {
            User = user;

            _appSettings = appSettings.Value;
            _jsonResponseFromat = jsonResponseFromat;

        }

        private CryptoUtility cryptoUtility;
        private readonly IJsonResponseFromat _jsonResponseFromat;
        private readonly ApplicationSettings _appSettings;
        public new IUser User { get; }


        [Route("Login")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                _ = new LoginResponse();
                cryptoUtility ??= new CryptoUtility();
                var result = await User.Login(model.UserName);
                if (result != null)
                {
                    if (result.Password != null && model.Password.Equals(cryptoUtility.Decrypt(result.Password)))
                    {
                        var tokenDescriptor = new SecurityTokenDescriptor
                        {
                            Subject = new ClaimsIdentity(new Claim[]
                               {
                        new ("UserID",result.UserID.ToString()),
                        new ("DeviceType",model.DeviceType),
                        new ("RoleName",result.RoleType),
                        new ("Name",result.FullName),
                        new("Access", result.Access.ToString())
                               }),
                            Expires = DateTime.UtcNow.AddDays(390),
                            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWT_Secret)), SecurityAlgorithms.HmacSha256Signature)

                        };


                        var tokenHandler = new JwtSecurityTokenHandler();
                        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                        var token = tokenHandler.WriteToken(securityToken);
                        var jsonresult = new
                        {
                            username = result.FullName,
                            role = result.RoleType,
                            token,
                            access = result.Access ? 1 : 0,
                            userid = result.UserID

                        };
                        return Ok(_jsonResponseFromat.JsonMessage(true, LoginSuccess, jsonresult));
                    }
                    else
                    {
                        return Ok(_jsonResponseFromat.JsonMessage(false, CredentialsNotMatch));
                    }
                }
                else
                {
                    return Ok(_jsonResponseFromat.JsonMessage(false, CredentialsNotMatch));
                }
            }
            else
            {
                return BadRequest(_jsonResponseFromat.JsonMessage(false, SomethingWrong));
            }
        }

        [Route("GetDashboard")]
        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            DashboardViewModels obj = await User.GetDashboard(base.User.Identity.GetUserId(), base.User.Identity.GetUserRoleName());
            return Ok(_jsonResponseFromat.JsonMessage(true, RecordSyncSuccess, obj));
        }


        [Route("ChangePassword")]
        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] ChangeUserPasswordViewModel model)
        {
            cryptoUtility ??= new CryptoUtility();

            var password = cryptoUtility.Encrypt(model.NewPassword);
            var currentpassword = cryptoUtility.Encrypt(model.CurrentPassword);
            var msg = await User.ChangeUserPassword(base.User.Identity.GetUserId(), base.User.Identity.GetUserId(), password, currentpassword, false);
            if (!string.IsNullOrEmpty(msg))
                return Ok(_jsonResponseFromat.JsonMessage(true, PasswordChangeSuccess));
            return Ok(_jsonResponseFromat.JsonMessage(false, PasswordNotMatch));
        }

        [Route("GetStoreDashboard")]
        [HttpGet]
        public async Task<IActionResult> GetStoreDashboard()
        {
            StoreDashBoardViewModel obj = await User.GetStoreDashBoard(base.User.Identity.GetUserId(), base.User.Identity.GetUserRoleName());
            return Ok(_jsonResponseFromat.JsonMessage(true, RecordSyncSuccess, obj));
        }

        [AllowAnonymous]
        [Route("EncryptText")]
        [HttpGet]
        public IActionResult EncryptText(string text)
        {
            cryptoUtility ??= new CryptoUtility();
            var encryptBytes = cryptoUtility.Encrypt(text);
            var encryptedText = Convert.ToBase64String(encryptBytes);
            return Ok(_jsonResponseFromat.JsonMessage(true, DataDetails, encryptedText));
        }

    }
}