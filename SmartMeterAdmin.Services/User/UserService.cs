using Dapper;
using Microsoft.EntityFrameworkCore;
using SmartMeterAdmin.Data.User;
using SmartMeterAdmin.Infrastructure.Data;
using SmartMeterAdmin.Infrastructure.Enumeration;
using SmartMeterAdmin.Services.Dapper_ORM;
using SmartMeterAdmin.ViewModel.DropDown;
using SmartMeterAdmin.ViewModel.AuditLog;
using SmartMeterAdmin.ViewModel.ElectricianProfile;
using SmartMeterAdmin.ViewModel.Login;
using SmartMeterAdmin.ViewModel.Master;
using SmartMeterAdmin.ViewModel.UserViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SmartMeterAdmin.Services.User
{
    public class Userservice : IUser
    {
        private readonly SmartMeterContext _context;
        private readonly IDapper _dapper;
        public Userservice(SmartMeterContext context, IDapper dapper)
        {
            _context = context;
            _dapper = dapper;
        }
        public async Task<IEnumerable<DropDown>> GetUserDropDown(RoleEnum role, int userId, string tableName)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ROLETYPE", role.ToString());
            parameters.Add("@USERID", userId);
            parameters.Add("@TABLENAME", tableName);
            return await _dapper.ExecuteProcedure<DropDown>("GET_DROPDOWN", parameters);
        }
        public async Task<LoginResultData> Login(string Username)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Username", Username);

            string query = @"SELECT TOP 1 
                        UserID,
                        RoleType,
                        FullName,
                        Password,
                        Access
                     FROM USERS WITH(NOLOCK) 
                     WHERE UserName = @Username 
                     AND IsDelete = 0";

            return await _dapper.GetFirstOrDefaultAsync<LoginResultData>(query, parameters);
        }
        public async Task<IEnumerable<UserViewModel>> GetAllUserByRole(string roletype, int userID, string loginRole)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@USER_ID", userID);
            parameters.Add("@ROLE_TYPE", roletype);
            parameters.Add("@LOGIN_ROLE_TYPE", loginRole);
            return await _dapper.ExecuteProcedure<UserViewModel>("GET_USER_LIST", parameters);
        }
        public async Task<IList<Users>> GetAllDeactivatedUserByRole(string roletype)
        {
            return await _context.Users.Where(a => a.IsDelete == true && a.RoleType == roletype).ToListAsync();
        }
        public async Task<UserViewModel> GetUserById(int id)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@USER_ID", id);

            return await _dapper.ExecuteProcedureFirstOrDefault<UserViewModel>("GET_USER_BY_ID", parameters);
        }
        public async Task<UserViewModel> GetUserProfileByID(int id)
        {
            return await _context.Set<UserViewModel>().FromSqlRaw($"UserDetails {id}").FirstOrDefaultAsync();
        }
        public async Task ManageUser(UserViewModel data, byte[] password)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@UserID", data.UserID);
            parameters.Add("@UserName", data.Username);
            parameters.Add("@MobileNo", data.MobileNo);
            parameters.Add("@Email", data.Email);
            parameters.Add("@City", data.City);
            parameters.Add("@State", data.State);
            parameters.Add("@CreatedBy", data.CreatedBy);
            parameters.Add("@UpdatedBy", data.UpdatedBy);

            parameters.Add("@Roletype", data.RoleType);
            parameters.Add("@FullName", data.FullName);
            parameters.Add("@CompanyName", data.CompanyName);
            parameters.Add("@AreaType", data.AreaType);
            parameters.Add("@AreaValue", data.AreaType);
            parameters.Add("@Address", data.Address);
            parameters.Add("@Pincode", data.Pincode);
            parameters.Add("@Dob", data.Dob);
            parameters.Add("@LastQualification", data.LastQualification);
            parameters.Add("@DocumentType", data.DocumentType);
            parameters.Add("@DocumentNo", data.DocumentNo);
            parameters.Add("@DocumentUrl", data.DocumentUrl);
            parameters.Add("@ProfileImage", data.ProfileImage);
            if (password is not null)
                parameters.Add("@Password", password);
            // Audit access (Add New form se) - proc me default 1/1 hai; null bhejne par bhi default lagega
            // CanDelete hata diya - CanEdit hi combined "Edit & Delete" right hai
            parameters.Add("@CanView", data.CanView ?? true);
            parameters.Add("@CanEdit", data.CanEdit ?? true);
            try
            {
                await _dapper.ExecuteProcedure("MANAGE_USER", parameters);
            }
            catch (Exception)
            {

                throw;
            }

        }
        public async Task<string> ChangeUserPassword(int userId, int updatedBy, byte[] password, byte[] currentPassword, bool isResetPassword)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@USER_ID", userId);
            parameters.Add("@PASSWORD", password);
            parameters.Add("@CURRENT_PASSWORD", currentPassword, DbType.Binary);
            parameters.Add("@UPDATED_BY", updatedBy);
            parameters.Add("@IS_RESET_PASSWORD", isResetPassword);

            return await _dapper.ExecuteProcedureFirstOrDefault<string>("CHANGE_USER_PASSWORD", parameters);
        }
        public async Task ActivateUser(int userId, int updatedBy, bool isDelete)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@USER_ID", userId);
            parameters.Add("@UPDATED_BY", updatedBy);
            parameters.Add("@IS_DELETE", isDelete);

            await _dapper.ExecuteProcedure("ACTIVATE_USER", parameters);
        }

        #region UserAuditAccess (UI se access manage)

        public async Task<IEnumerable<UserAuditAccessListViewModel>> GetUserAuditAccessList(string roleType, string search)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ROLE_TYPE", string.IsNullOrWhiteSpace(roleType) ? null : roleType.Trim());
            parameters.Add("@SEARCH", string.IsNullOrWhiteSpace(search) ? null : search.Trim());
            return await _dapper.ExecuteProcedure<UserAuditAccessListViewModel>("GET_USER_AUDIT_ACCESS_LIST", parameters);
        }

        public async Task<UserAuditAccessListViewModel> SaveUserAuditAccess(int targetUserId, bool canView, bool canEdit, string updatedBy)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UserID", targetUserId);
            parameters.Add("@CanView", canView);
            parameters.Add("@CanEdit", canEdit);
            parameters.Add("@UpdatedBy", updatedBy ?? "system");

            return await _dapper.ExecuteProcedureFirstOrDefault<UserAuditAccessListViewModel>("SAVE_USER_AUDIT_ACCESS", parameters);
        }

        #endregion

        #region Electricain
        public async Task<ElectricianProfile> GetElectricianProfileById(int id)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@USER_ID", id);

            return await _dapper.ExecuteProcedureFirstOrDefault<ElectricianProfile>("GET_ELECTRICIAN_PROFILE", parameters);
        }
        public async Task<ElectricianLoginResponse> LoginByMobileNumber(string mobileno)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@MOBILE_NO", mobileno);

            return await _dapper.ExecuteProcedureFirstOrDefault<ElectricianLoginResponse>("ELECTRICIAN_LOGIN", parameters);
        }
        public async Task<bool> IsSuspendedUser(int userId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@USER_ID", userId);
            return await _dapper.ExecuteProcedureFirstOrDefault<bool>("CHECK_USER_SUSPENDED", parameters);
        }
        #endregion

        public async Task<DashboardViewModels> GetDashboard(int userid, string roletype)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@UserID", userid);
            parameters.Add("@RoleType", roletype);

            return await _dapper.ExecuteProcedureFirstOrDefault<DashboardViewModels>("Pro_GetDashboard", parameters);
        }
        public async Task<StoreDashBoardViewModel> GetStoreDashBoard(int userid, string roletype)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@UserID", userid);
            parameters.Add("@RoleType", roletype);

            return await _dapper.ExecuteProcedureFirstOrDefault<StoreDashBoardViewModel>("SP_StoreDashBoard", parameters);
        }
    }
}