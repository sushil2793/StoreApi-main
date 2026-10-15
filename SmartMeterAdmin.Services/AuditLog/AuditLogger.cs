using Dapper;
using SmartMeterAdmin.Infrastructure.Helpers;
using SmartMeterAdmin.Services.Dapper_ORM;
using SmartMeterAdmin.ViewModel.AuditLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartMeterAdmin.Services.AuditLog
{
    public interface IAuditLogger
    {
        Task LogAsync(UserActivityLogViewModel entry);
        Task<IEnumerable<UserActivityLogViewModel>> GetLogs(UserActivityLogFilter filter);
        Task<IEnumerable<UserActivitySummaryViewModel>> GetSummary(DateTime? fromDate, DateTime? toDate);
        Task<IEnumerable<UserActivityTopUserViewModel>> GetTopUsers(DateTime? fromDate, DateTime? toDate);
    }

    public class AuditLogger : IAuditLogger
    {
        private readonly IDapper _dapper;

        public AuditLogger(IDapper dapper)
        {
            _dapper = dapper;
        }

        public async Task LogAsync(UserActivityLogViewModel entry)
        {
            if (entry == null) return;

            var parameters = new DynamicParameters();
            parameters.Add("@UserID", entry.UserID);
            parameters.Add("@UserName", Truncate(entry.UserName, 150));
            parameters.Add("@FullName", Truncate(entry.FullName, 150));
            parameters.Add("@RoleType", Truncate(entry.RoleType, 50));
            parameters.Add("@Action", Truncate(entry.Action, 30));
            parameters.Add("@Module", Truncate(entry.Module, 100));
            parameters.Add("@ActionName", Truncate(entry.ActionName, 150));
            parameters.Add("@EntityID", Truncate(entry.EntityID, 50));
            parameters.Add("@Description", Truncate(entry.Description, 500));
            parameters.Add("@RequestData", entry.RequestData);
            parameters.Add("@IPAddress", Truncate(entry.IPAddress, 50));
            parameters.Add("@UserAgent", Truncate(entry.UserAgent, 300));
            parameters.Add("@StatusCode", entry.StatusCode);
            parameters.Add("@DurationMs", entry.DurationMs);

            try
            {
                await _dapper.ExecuteProcedure("SAVE_USER_ACTIVITY_LOG", parameters);
            }
            catch
            {
                // Audit logging fail-safe: log fail ho to application flow kabhi na toote.
            }
        }

        public async Task<IEnumerable<UserActivityLogViewModel>> GetLogs(UserActivityLogFilter filter)
        {
            filter ??= new UserActivityLogFilter();

            var parameters = new DynamicParameters();
            parameters.Add("@UserID", filter.UserID);
            parameters.Add("@Action", filter.Action);
            parameters.Add("@Module", filter.Module);
            parameters.Add("@FromDate", filter.FromDate);
            parameters.Add("@ToDate", filter.ToDate);
            parameters.Add("@Search", filter.Search);
            parameters.Add("@PageNo", filter.PageNo <= 0 ? 1 : filter.PageNo);
            parameters.Add("@PageSize", filter.PageSize <= 0 ? 50 : Math.Min(filter.PageSize, 5000));

            return await _dapper.ExecuteProcedure<UserActivityLogViewModel>("GET_USER_ACTIVITY_LOGS", parameters);
        }

        public async Task<IEnumerable<UserActivitySummaryViewModel>> GetSummary(DateTime? fromDate, DateTime? toDate)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@FromDate", fromDate);
            parameters.Add("@ToDate", toDate);
            return await _dapper.ExecuteProcedure<UserActivitySummaryViewModel>("GET_USER_ACTIVITY_SUMMARY", parameters);
        }

        public async Task<IEnumerable<UserActivityTopUserViewModel>> GetTopUsers(DateTime? fromDate, DateTime? toDate)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@FromDate", fromDate);
            parameters.Add("@ToDate", toDate);
            // proc ke do resultset hain: [0] = action summary, [1] = top users
            var gridReader = await _dapper.ExecuteProcedureMultiple("GET_USER_ACTIVITY_SUMMARY", parameters);
            using (gridReader)
            {
                gridReader.Read<UserActivitySummaryViewModel>(); // pehla resultset skip
                return await gridReader.ReadAsync<UserActivityTopUserViewModel>();
            }
        }

        private static string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
    }
}