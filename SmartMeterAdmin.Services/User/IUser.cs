using SmartMeterAdmin.Data.User;
using SmartMeterAdmin.Infrastructure.Enumeration;
using SmartMeterAdmin.ViewModel.DropDown;
using SmartMeterAdmin.ViewModel.AuditLog;
using SmartMeterAdmin.ViewModel.ElectricianProfile;
using SmartMeterAdmin.ViewModel.Login;
using SmartMeterAdmin.ViewModel.Master;
using SmartMeterAdmin.ViewModel.UserViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartMeterAdmin.Services.User
{
    public interface IUser
    {
        Task<IEnumerable<DropDown>> GetUserDropDown(RoleEnum role, int userId, string tableName);
        Task<LoginResultData> Login(string Username);
        Task<IEnumerable<UserViewModel>> GetAllUserByRole(string roletype, int userId,string loginRole);
        Task<IList<Users>> GetAllDeactivatedUserByRole(string roletype);
        Task<UserViewModel> GetUserById(int id);
        Task<UserViewModel> GetUserProfileByID(int id);
        Task ManageUser(UserViewModel data, byte[] password);
        Task<string> ChangeUserPassword(int userId, int updatedBy, byte[] password, byte[] currentPassword,bool isResetPassword);
        Task ActivateUser(int userId, int updatedBy, bool isDelete);

        #region UserAuditAccess (UI se access manage)

        Task<IEnumerable<UserAuditAccessListViewModel>> GetUserAuditAccessList(string roleType, string search);
        Task<UserAuditAccessListViewModel> SaveUserAuditAccess(int targetUserId, bool canView, bool canEdit, string updatedBy);

        #endregion


        #region Electrician

        Task<ElectricianLoginResponse> LoginByMobileNumber(string mobileno);
        Task<bool> IsSuspendedUser(int userId);
        Task<ElectricianProfile> GetElectricianProfileById(int id);
        #endregion

        #region Dashboard

        Task<DashboardViewModels> GetDashboard(int userid, string roletype);
        Task<StoreDashBoardViewModel> GetStoreDashBoard(int userid, string roletype);

        #endregion
    }
}