using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using SmartMeterAdmin.Infrastructure.Enumeration;
using SmartMeterAdmin.Infrastructure.Helpers;
using SmartMeterAdmin.Services.AuditLog;
using SmartMeterAdmin.ViewModel.AuditLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace SmartMeterAdmin.Middleware
{
    /// <summary>
    /// Endpoint pe lagao to us request ka audit log NAHI banega.
    /// High-frequency/polling endpoints (dashboard, dropdowns) ke liye.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class SkipAuditLogAttribute : Attribute
    {
    }

    /// <summary>
    /// Global action filter: har controller action ka audit log banata hai
    /// (kaun, kya, kab, kahan se, kitni der me). Fail-safe - audit fail ho
    /// to main request flow kabhi nahi tootta.
    /// </summary>
    public class AuditLogActionFilter : IAsyncActionFilter
    {
        private readonly IAuditLogger _auditLogger;

        // Sensitive keys - ye fields RequestData me kabhi nahi jaate
        private static readonly string[] SensitiveKeys =
        {
            "password", "currentpassword", "newpassword", "confirmpassword",
            "token", "accesstoken", "refreshtoken", "secret", "apikey",
            "profileimage", "documenturl", "base64"
        };

        // IMPORTANT GET endpoints (view trail) - baaki GET skip hote hain
        private static readonly string[] ImportantGetActions =
        {
            "login", "logout", "export", "download", "getalluser", "getuser",
            "getreplacementmeters", "getswapmeterlist", "getlogs", "getsummary",
            "getdeactivateduser", "getpendinghesmetersync", "bulkupdatelineratios"
        };

        public AuditLogActionFilter(IAuditLogger auditLogger)
        {
            _auditLogger = auditLogger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var executed = await next();

            // Skip flag wale endpoints log nahi hote
            if (context.Filters.OfType<SkipAuditLogAttribute>().Any()) return;
            if (executed.Exception != null && !executed.ExceptionHandled) return; // exception handler /error pe jayega

            try
            {
                var http = context.HttpContext;
                var descriptor = context.ActionDescriptor;
                var controllerName = descriptor.RouteValues.TryGetValue("controller", out var c) ? c : "Unknown";
                var actionName = descriptor.RouteValues.TryGetValue("action", out var a) ? a : "Unknown";
                var httpMethod = http.Request.Method.ToUpperInvariant();

                bool isAuthenticated = http.User.Identity?.IsAuthenticated == true;
                int userId = isAuthenticated ? http.User.Identity.GetUserId() : -1;
                string role = isAuthenticated ? http.User.Identity.GetUserRoleName() : null;

                // GET policy: sirf important GETs log hote hain (user decision)
                if (httpMethod == "GET")
                {
                    bool important = ImportantGetActions.Any(k => actionName.ToLowerInvariant().Contains(k))
                                     || (http.Request.Path.Value?.ToLowerInvariant().Contains("export") ?? false);
                    if (!important) return;
                }

                var entry = new UserActivityLogViewModel
                {
                    UserID = userId > 0 ? userId : (int?)null,
                    UserName = isAuthenticated ? GetClaim(http, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name") : TryGetLoginName(context),
                    FullName = isAuthenticated ? GetClaim(http, "Name") : null,
                    RoleType = role,
                    Action = ResolveAction(httpMethod, actionName, executed.Result),
                    Module = controllerName,
                    ActionName = actionName,
                    EntityID = http.Items.ContainsKey(EntityIdItemKey)
                        ? http.Items[EntityIdItemKey]?.ToString()
                        : ResolveEntityId(context),
                    Description = BuildDescription(httpMethod, controllerName, actionName, executed.Result)
                        + (http.Items.ContainsKey(DescSuffixItemKey) ? http.Items[DescSuffixItemKey]?.ToString() : null),
                    RequestData = BuildSanitizedRequestData(context, httpMethod),
                    IPAddress = GetClientIp(http),
                    UserAgent = http.Request.Headers.ContainsKey("User-Agent")
                        ? http.Request.Headers["User-Agent"].ToString() : null,
                    StatusCode = http.Response.StatusCode,
                    DurationMs = GetDurationMs(http)
                };

                await _auditLogger.LogAsync(entry);
            }
            catch
            {
                // Audit kabhi main flow nahi tootne dega
            }
        }

        private static string GetClaim(HttpContext http, string claimType)
        {
            return http.User.Claims.FirstOrDefault(c =>
                c.Type.Equals(claimType, StringComparison.OrdinalIgnoreCase)
                || c.Type.EndsWith("/" + claimType, StringComparison.OrdinalIgnoreCase))?.Value;
        }

        private static string TryGetLoginName(ActionExecutingContext context)
        {
            // Login/failed-login ke waqt identity nahi hoti - request body se naam nikaal lo
            foreach (var arg in context.ActionArguments.Values)
            {
                var prop = arg?.GetType().GetProperty("UserName") ?? arg?.GetType().GetProperty("MobileNo");
                var value = prop?.GetValue(arg) as string;
                if (!string.IsNullOrEmpty(value)) return value;
            }
            return null;
        }

        private static string ResolveAction(string httpMethod, string actionName, IActionResult result)
        {
            var name = actionName.ToLowerInvariant();

            if (name.Contains("login")) return "LOGIN";
            if (name.Contains("logout")) return "LOGOUT";
            if (name.Contains("export") || name.Contains("download")) return "EXPORT";
            if (name.Contains("upload") || name.Contains("import")) return "UPLOAD";
            if (name.Contains("delete") || name.Contains("deactivate")) return "DELETE";
            if (name.Contains("activate")) return "EDIT";
            if (name.Contains("update") || name.Contains("edit") || name.Contains("bulkupdate")) return "EDIT";
            if (name.Contains("save") || name.Contains("create") || name.Contains("add") ||
                name.Contains("manage") || name.Contains("swap") || name.Contains("replace")) return "CREATE";
            if (name.Contains("reset")) return "EDIT";

            return httpMethod switch
            {
                "POST" => "CREATE",
                "PUT" => "EDIT",
                "DELETE" => "DELETE",
                _ => "VIEW"
            };
        }

        private static string ResolveEntityId(ActionExecutingContext context)
        {
            // route/body me id jaisa kuch dikhe to capture
            foreach (var key in new[] { "id", "userID", "userId", "consumerId", "meterNo", "meterno" })
            {
                var match = context.ActionArguments.FirstOrDefault(kv =>
                    kv.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
                if (match.Key != null && match.Value != null)
                    return match.Value.ToString();
            }
            return null;
        }

        private static string BuildDescription(string httpMethod, string controller, string action, IActionResult result)
        {
            string outcome;
            if (result is OkObjectResult ok && ok.Value != null)
            {
                var json = JsonConvert.SerializeObject(ok.Value);
                outcome = json.Contains("\"success\":true", StringComparison.OrdinalIgnoreCase) ? "SUCCESS" : "FAILED";
            }
            else if (result is ObjectResult)
            {
                outcome = "SUCCESS";
            }
            else if (result is BadRequestObjectResult || result is UnauthorizedObjectResult
                     || result is NotFoundObjectResult || result is ForbidResult)
            {
                outcome = "BLOCKED";
            }
            else
            {
                outcome = "SUCCESS";
            }

            return $"{httpMethod} {controller}/{action} -> {outcome}";
        }

        private static string BuildSanitizedRequestData(ActionExecutingContext context, string httpMethod)
        {
            try
            {
                if (context.ActionArguments == null || context.ActionArguments.Count == 0) return null;

                var dict = new Dictionary<string, object>();
                foreach (var arg in context.ActionArguments)
                {
                    if (arg.Value is Microsoft.AspNetCore.Http.IFormFile formFile)
                    {
                        dict[arg.Key] = new { fileName = formFile.FileName, sizeBytes = formFile.Length };
                        continue;
                    }

                    // primitive params seedhi log karo
                    var type = arg.Value?.GetType();
                    if (type == null || type.IsPrimitive || type == typeof(string) || type == typeof(decimal))
                    {
                        dict[arg.Key] = arg.Value;
                        continue;
                    }

                    // complex object: public props -> dict, sensitive hatao, IFormFile summarize
                    var props = type.GetProperties()
                        .Where(p => p.CanRead && p.GetIndexParameters().Length == 0)
                        .Where(p => !SensitiveKeys.Contains(p.Name.Replace(" ", "").ToLowerInvariant()));
                    var inner = new Dictionary<string, object>();
                    foreach (var p in props.Take(40))
                    {
                        try
                        {
                            var v = p.GetValue(arg.Value);
                            if (v is Microsoft.AspNetCore.Http.IFormFile ff)
                                inner[p.Name] = new { fileName = ff.FileName, sizeBytes = ff.Length };
                            else if (v == null || IsSimple(v.GetType()))
                                inner[p.Name] = v;
                            else if (v is System.Collections.IEnumerable enumVal && v is not string)
                            {
                                // Lists/arrays: actual items log karo (e.g. pushed consumer IDs)
                                var items = enumVal.Cast<object>().Take(50).Select(x =>
                                    IsSimple(x?.GetType() ?? typeof(object)) ? x : x?.ToString()).ToArray();
                                inner[p.Name] = items;
                            }
                            else
                            {
                                var serialized = JsonConvert.SerializeObject(v);
                                inner[p.Name] = serialized.Length > 2000 ? serialized.Substring(0, 2000) : serialized;
                            }
                        }
                        catch { /* property read fail - ignore */ }
                    }
                    dict[arg.Key] = inner;
                }

                var json = JsonConvert.SerializeObject(dict);
                return json.Length > 40000 ? json.Substring(0, 40000) : json;
            }
            catch
            {
                return null;
            }
        }

        private static bool IsSimple(Type type)
        {
            return type.IsPrimitive
                || type == typeof(string)
                || type == typeof(decimal)
                || type == typeof(DateTime)
                || type == typeof(DateTimeOffset)
                || type == typeof(Guid)
                || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        private static int GetDurationMs(HttpContext http)
        {
            if (http.Items.TryGetValue(StartTimeKey, out var value) && value is DateTimeOffset start)
                return (int)(DateTimeOffset.UtcNow - start).TotalMilliseconds;
            return 0;
        }

        public const string StartTimeKey = "AuditLog_StartTime";
        public const string EntityIdItemKey = "Audit_EntityId";
        public const string DescSuffixItemKey = "Audit_DescSuffix";

        private static string GetClientIp(HttpContext http)
        {
            // IIS / reverse-proxy ke peeche X-Forwarded-For sahi client IP deta hai
            var forwarded = http.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwarded))
            {
                var first = forwarded.Split(',')[0].Trim();
                if (!string.IsNullOrEmpty(first)) return first;
            }
            return http.Connection.RemoteIpAddress?.ToString();
        }
    }
}