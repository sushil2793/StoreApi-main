using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.UserModel;
using SmartMeterAdmin.Infrastructure.Enumeration;
using SmartMeterAdmin.Infrastructure.Helpers;
using SmartMeterAdmin.Services.AuditLog;
using SmartMeterAdmin.Services.Dapper_ORM;
using SmartMeterAdmin.ViewModel.AuditLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SmartMeterAdmin.Controllers
{
    /// <summary>
    /// Audit report endpoints.
    /// Access rule: Superadmin/Admin hamesha allowed, baaki users ko
    /// UserAuditAccess table me CanView=1 chahiye (GET_USER_AUDIT_ACCESS proc).
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/AuditLog")]
    public class AuditController : ControllerBase
    {
        private readonly IAuditLogger _auditLogger;
        private readonly IExportUtility _exportUtility;
        private readonly IJsonResponseFromat _jsonResponseFromat;
        private readonly IDapper _dapper;

        // Per-request-cache: ek HTTP request me proc sirf 1 baar call ho
        private const string CanViewItemKey = "Audit_CanView";

        // 5-min static cache: DB proc call har request pe na ho (UserID -> (CanView, cachedAt))
        private static readonly ConcurrentDictionary<int, (bool CanView, DateTime CachedAt)> CanViewCache
            = new ConcurrentDictionary<int, (bool, DateTime)>();
        private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);

        /// <summary>
        /// Access UI se change hone par cache turant invalidate karo,
        /// warna 5 min tak purana CanView chalta rahega.
        /// userId <= 0 ho to poora cache saaf.
        /// </summary>
        public static void ClearCanViewCache(int userId)
        {
            if (userId > 0)
                CanViewCache.TryRemove(userId, out _);
            else
                CanViewCache.Clear();
        }

        public AuditController(IAuditLogger auditLogger, IExportUtility exportUtility, IJsonResponseFromat jsonResponseFromat, IDapper dapper)
        {
            _auditLogger = auditLogger;
            _exportUtility = exportUtility;
            _jsonResponseFromat = jsonResponseFromat;
            _dapper = dapper;
        }

        /// <summary>
        /// Superadmin/Admin -> allowed (fast path, DB hit nahi).
        /// Baaki roles -> UserAuditAccess.CanView (proc, 5-min cache).
        /// DB fail ho to DENY (fail-closed) - audit access ka main flow se koi lena-dena nahi.
        /// </summary>
        private async Task<bool> IsAuthorizedAsync()
        {
            var role = User.Identity.GetUserRoleName();
            if (role == RoleEnum.Superadmin.ToString() || role == RoleEnum.Admin.ToString())
                return true;

            int userId = User.Identity.GetUserId();
            if (userId <= 0) return false;

            if (HttpContext.Items.TryGetValue(CanViewItemKey, out var cachedItem) && cachedItem is bool perRequest)
                return perRequest;

            bool canView = false;
            try
            {
                var now = DateTime.UtcNow;
                if (CanViewCache.TryGetValue(userId, out var entry) && (now - entry.CachedAt) < CacheTtl)
                {
                    canView = entry.CanView;
                }
                else
                {
                    var parameters = new Dapper.DynamicParameters();
                    parameters.Add("@UserID", userId);
                    var access = await _dapper.ExecuteProcedureFirstOrDefault<UserAuditAccessViewModel>(
                        "GET_USER_AUDIT_ACCESS", parameters);
                    canView = access?.CanView == true;
                    CanViewCache[userId] = (canView, now);
                }
            }
            catch
            {
                canView = false; // fail-closed
            }

            HttpContext.Items[CanViewItemKey] = canView;
            return canView;
        }

        [HttpGet("GetLogs")]
        public async Task<IActionResult> GetLogs(int? userId, string actionType, string module,
            DateTime? fromDate, DateTime? toDate, string search, int pageNo = 1, int pageSize = 50)
        {
            if (!await IsAuthorizedAsync())
                return Ok(_jsonResponseFromat.JsonMessage(false, "Access denied."));

            var filter = new UserActivityLogFilter
            {
                UserID = userId > 0 ? userId : null, // 0 = All Users
                Action = actionType,
                Module = module,
                FromDate = fromDate,
                ToDate = toDate,
                Search = search,
                PageNo = pageNo,
                PageSize = pageSize
            };

            var logs = (await _auditLogger.GetLogs(filter)).ToList();
            int totalCount = logs.FirstOrDefault()?.TotalCount ?? 0;

            return Ok(_jsonResponseFromat.JsonMessage(true, "Audit logs", new
            {
                totalCount,
                pageNo = filter.PageNo,
                pageSize = filter.PageSize,
                rows = logs
            }));
        }

        [HttpGet("GetSummary")]
        public async Task<IActionResult> GetSummary(DateTime? fromDate, DateTime? toDate)
        {
            if (!await IsAuthorizedAsync())
                return Ok(_jsonResponseFromat.JsonMessage(false, "Access denied."));

            var summary = await _auditLogger.GetSummary(fromDate, toDate);
            var topUsers = await _auditLogger.GetTopUsers(fromDate, toDate);

            return Ok(_jsonResponseFromat.JsonMessage(true, "Audit summary", new
            {
                actionSummary = summary,
                topUsers
            }));
        }

        [HttpGet("ExportExcel")]
        public async Task<IActionResult> ExportExcel(int? userId, string actionType, string module,
            DateTime? fromDate, DateTime? toDate, string search)
        {
            if (!await IsAuthorizedAsync())
                return Ok(_jsonResponseFromat.JsonMessage(false, "Access denied."));

            var filter = new UserActivityLogFilter
            {
                UserID = userId > 0 ? userId : null, // 0 = All Users
                Action = actionType,
                Module = module,
                FromDate = fromDate,
                ToDate = toDate,
                Search = search,
                PageNo = 1,
                PageSize = 5000 // export cap
            };

            var logs = (await _auditLogger.GetLogs(filter)).ToList();
            var rows = logs.Select(l => new AuditExportRow
            {
                LogID = l.LogID,
                Date = l.CreatedDate,
                UserName = l.UserName,
                FullName = l.FullName,
                RoleType = l.RoleType,
                Action = l.Action,
                Module = l.Module,
                ActionName = l.ActionName,
                EntityID = l.EntityID,
                IPAddress = l.IPAddress,
                StatusCode = l.StatusCode,
                DurationMs = l.DurationMs,
                Description = l.Description
            }).ToList();

            IWorkbook workbook = _exportUtility.WriteExcelWithNPOI(rows, "xlsx");
            using var ms = new MemoryStream();
            workbook.Write(ms);

            return File(ms.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"AuditReport_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }

        private class AuditExportRow
        {
            public long LogID { get; set; }
            public DateTime? Date { get; set; }
            public string UserName { get; set; }
            public string FullName { get; set; }
            public string RoleType { get; set; }
            public string Action { get; set; }
            public string Module { get; set; }
            public string ActionName { get; set; }
            public string EntityID { get; set; }
            public string IPAddress { get; set; }
            public int? StatusCode { get; set; }
            public int? DurationMs { get; set; }
            public string Description { get; set; }
        }
    }
}