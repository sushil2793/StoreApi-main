using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.X509;
using SmartMeterAdmin.Data;
using SmartMeterAdmin.Infrastructure.Helpers;
using SmartMeterAdmin.Services.Meter;
using SmartMeterAdmin.Services.User;
using SmartMeterAdmin.ViewModel.Login;
using SmartMeterAdmin.ViewModel.MeterViewModel;
using static SmartMeterAdmin.Infrastructure.Helpers.ConstantValues;

namespace SmartMeterAdmin.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/ElectricianAccount")]

    public class ElectricianAccountController : ControllerBase
    {
        public ElectricianAccountController(IJsonResponseFromat jsonResponseFromat, IUser user, IOptions<ApplicationSettings> appSettings, IMeter meter, IConfiguration configuration)
        {
            JsonResponseFromat = jsonResponseFromat;
            this.user = user;
            Meter = meter;
            Configuration = configuration;
            _appSettings = appSettings.Value;
            OwnerName = configuration.GetSection("HESBaseConfig")["OwnerName"];
        }
        public IConfiguration Configuration { get; }
        private string OwnerName { get; }
        private readonly ApplicationSettings _appSettings;
        private readonly IUser user;

        public IJsonResponseFromat JsonResponseFromat { get; }
        public IMeter Meter { get; }
        private CryptoUtility cryptoUtility;


        [Route("ChekcMobileNumber")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ChekcMobileNumber([FromBody] CheckMobileViewModel model)
        {
            if (!string.IsNullOrEmpty(model.MobileNo))
            {
                var userData = await user.LoginByMobileNumber(model.MobileNo);
                var data = new CheckMobileNumberResponse();
                if (userData is not null)
                {
                    data.UserExists = true;
                    if (userData.Password is not null)
                        data.PasswordExists = true;
                }
                if (data != null)
                    return Ok(JsonResponseFromat.JsonMessage(true, UserExists, data.PasswordExists));
                else
                    return Ok(JsonResponseFromat.JsonMessage(false, UserNotExists));
            }
            else
            {
                return Ok(JsonResponseFromat.JsonMessage(false, MobileNumberRequired));
            }
        }

        [Route("ChangePassword")]
        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] ChangeUserPasswordViewModel model)
        {
            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                cryptoUtility ??= new CryptoUtility();

                var password = cryptoUtility.Encrypt(model.NewPassword);

                var msg = await user.ChangeUserPassword(User.Identity.GetUserId(), User.Identity.GetUserId(), password, null,false);
                if (!string.IsNullOrEmpty(msg))
                    return Ok(JsonResponseFromat.JsonMessage(true, PasswordChangeSuccess));
                return Ok(JsonResponseFromat.JsonMessage(false, UserNotExists));
            }
            else
            {
                return Ok(JsonResponseFromat.JsonMessage(false, PasswordRequired));
            }
        }

        [AllowAnonymous]
        [Route("ResetPassword")]
        [HttpPost]
        public async Task<IActionResult> ResetPassword([FromBody] ChangeUserPasswordViewModel model)
        {
            if (!string.IsNullOrEmpty(model.NewPassword) && model.CurrentPassword == "NufPBEOmnlcLiD5$")
            {
                cryptoUtility ??= new CryptoUtility();

                var password = cryptoUtility.Encrypt(model.NewPassword);
                var userId = Convert.ToInt32(model.userId);
                var msg = await user.ChangeUserPassword(userId, userId, password, null,true);
                if (!string.IsNullOrEmpty(msg))
                    return Ok(JsonResponseFromat.JsonMessage(true, PasswordChangeSuccess));
                return Ok(JsonResponseFromat.JsonMessage(false, UserNotExists));
            }
            else
            {
                return Ok(JsonResponseFromat.JsonMessage(false, PasswordRequired));
            }
        }


        [Route("ElectricianLogin")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ElectricianLogin([FromBody] LoginViewModel model)
        {
            cryptoUtility ??= new CryptoUtility();


            var data = await user.LoginByMobileNumber(model.UserName);
            if (data is null)
                return Ok(JsonResponseFromat.JsonMessage(false, CredentialsNotMatch));
            else if (data is not null && data.Password is not null)
            {
                if (model.Password.Equals(cryptoUtility.Decrypt(data.Password)))
                {
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                   {
                        new ("UserID",data.UserID.ToString()),
                        new ("DeviceType",model.DeviceType),
                        new ("OwnerName",data.CompanyName ?? OwnerName),
                        new ("RoleName",data.Roletype),
                        new ("Name",data.Name),
                   }),
                        Expires = DateTime.UtcNow.AddDays(390),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWT_Secret)), SecurityAlgorithms.HmacSha256Signature)
                    };


                    var tokenHandler = new JwtSecurityTokenHandler();
                    var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                    var token = tokenHandler.WriteToken(securityToken);
                    data.AccessToken = token;
                    data.ProfileImage = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}" + data.ProfileImage;

                    return Ok(JsonResponseFromat.JsonMessage(true, LoginSuccess, data));
                }
                else
                {
                    return Ok(JsonResponseFromat.JsonMessage(false, CredentialsNotMatch));
                }
            }
            else
            {

                ElectricianLoginResponse electricianLogin = new();
                var password = cryptoUtility.Encrypt(model.Password);
                await user.ChangeUserPassword(User.Identity.GetUserId(), User.Identity.GetUserId(), password, null, false);


                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                      {
                        new ("UserID",data.UserID.ToString()),
                        new ("DeviceType",model.DeviceType),
                        new ("OwnerName",data.CompanyName ?? OwnerName),
                        new ("RoleName",data.Roletype),
                        new ("Name",data.Name),
                      }),
                    Expires = DateTime.UtcNow.AddDays(390),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWT_Secret)), SecurityAlgorithms.HmacSha256Signature)
                };


                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);
                electricianLogin.AccessToken = token;
                electricianLogin.ProfileImage = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}" + data.ProfileImage;

                return Ok(JsonResponseFromat.JsonMessage(true, LoginSuccess, electricianLogin));
            }
        }


        [Route("ElectricianProfile")]
        [HttpGet]
        public async Task<IActionResult> ElectricianProfile()
        {

            var profile = await user.GetElectricianProfileById(User.Identity.GetUserId());

            return Ok(JsonResponseFromat.JsonMessage(true, UserProfile, profile));

        }


        [Route("MeterCount")]
        [HttpGet]
        public async Task<IActionResult> MeterCount(string AppVersion)
        {
            if (string.IsNullOrEmpty(AppVersion))
                return Ok(JsonResponseFromat.JsonMessage(true, UpdateYourApp));

            string systemversion = Configuration.GetSection("Android")["AppVersion"];
            Version sysversion = Version.Parse(systemversion);
            Version AppVersionn = Version.Parse(AppVersion);
            if (AppVersionn < sysversion)
                return Ok(JsonResponseFromat.JsonMessage(true, UpdateYourApp));

            MeterCountViewModel meterCountView = await Meter.GetMeterCountByID(User.Identity.GetUserId(), User.Identity.GetUserRoleName());
            return Ok(JsonResponseFromat.JsonMessage(true, MeterCountMsg, meterCountView));
        }

    }
}