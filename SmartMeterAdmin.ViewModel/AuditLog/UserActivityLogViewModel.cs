using System;

namespace SmartMeterAdmin.ViewModel.AuditLog
{
    public class UserActivityLogViewModel
    {
        public long LogID { get; set; }
        public int? UserID { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string RoleType { get; set; }
        public string Action { get; set; }
        public string Module { get; set; }
        public string ActionName { get; set; }
        public string EntityID { get; set; }
        public string Description { get; set; }
        public string RequestData { get; set; }
        public string UserAgent { get; set; }
        public string IPAddress { get; set; }
        public int? StatusCode { get; set; }
        public int? DurationMs { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int TotalCount { get; set; }
    }

    public class UserActivityLogFilter
    {
        public int? UserID { get; set; }
        public string Action { get; set; }
        public string Module { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Search { get; set; }
        public int PageNo { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }

    public class UserActivitySummaryViewModel
    {
        public string Action { get; set; }
        public int TotalCount { get; set; }
    }

    public class UserActivityTopUserViewModel
    {
        public int? UserID { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string RoleType { get; set; }
        public int TotalActions { get; set; }
        public int ChangeCount { get; set; }
        public DateTime? LastActivity { get; set; }
    }

    /// <summary>
    /// GET_USER_AUDIT_ACCESS proc ka result - UserAuditAccess table se.
    /// CanView: audit report dekhne ka access. CanEdit = Edit & Delete (combined).
    /// </summary>
    public class UserAuditAccessViewModel
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public bool CanView { get; set; }
        public bool CanEdit { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    /// <summary>
    /// GET_USER_AUDIT_ACCESS_LIST proc ka result - Admin User List UI se
    /// access manage karne ke liye (RoleType + user details ke saath).
    /// </summary>
    public class UserAuditAccessListViewModel
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string RoleType { get; set; }
        public bool CanView { get; set; }
        public bool CanEdit { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    /// <summary>
    /// SaveUserAuditAccess API ka request body (Admin User List UI se).
    /// </summary>
    public class SaveUserAuditAccessRequest
    {
        public int UserID { get; set; }
        public bool CanView { get; set; }
        public bool CanEdit { get; set; }
    }
}