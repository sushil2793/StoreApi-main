using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NPOI.SS.Formula.Functions;
using SmartMeterAdmin.Services.Dapper_ORM;
using SmartMeterAdmin.Services.HTCSurvey;
using SmartMeterAdmin.ViewModel.DataTable;
using SmartMeterAdmin.ViewModel.Store;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SmartMeterAdmin.Services.Store
{
    public class StoreService : IStore
    {
        private readonly IDapper dapper;
        private readonly HttpClient httpClient;
        private readonly ILogger<StoreService> logger;
        private readonly IConfiguration Configuration;

        public StoreService(IDapper dapper, HttpClient httpClient, ILogger<StoreService> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.dapper = dapper;
            this.httpClient = httpClient;
            this.Configuration = configuration;
        }


        public async Task SyncIssueMeterByMeterNo(int userId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@USERID", userId);

            await dapper.ExecuteProcedure("SYNC_ISSUE_METER_BY_METERNO", parameters);
        }

        public async Task AcceptProductionMeter(ListOfMeterNumber request)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@ISBOX", request.IsBox);
            parameter.Add("@NUMBERS", string.Join(",", request.Number));
            await dapper.ExecuteProcedure("ACCEPT_PRODUCTION_METER", parameter);
        }
        public async Task IssueProductionMeter(ListOfMeterNumber request)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@CONTRACTORID", request.Id);
            parameter.Add("@NUMBERS", string.Join(",", request.Number));
            await dapper.ExecuteProcedure("ISSUE_PRODUCTION_METER", parameter);
        }

        public async Task<IEnumerable<T>> GetListOfIssueMeters<T>(AgGridTable request, int storeId)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@STOREID", storeId);
            parameter.Add("@STATUS", request.SearchValue ?? string.Empty);
            parameter.Add("@FROMDATE", request.FromDate);
            parameter.Add("@TODATE", request.ToDate);
            parameter.Add("@FromMeter", request.FromMeter);
            parameter.Add("@ToMeter", request.ToMeter);
            parameter.Add("@RoleType", request.RoleType ?? string.Empty);
            parameter.Add("@Length", request.Length);

            return await dapper.ExecuteProcedure<T>("GET_ISSUE_METERS", parameter);
        }
        public async Task<IEnumerable<T>> GetRetunMeters<T>(AgGridTable request, int storeId)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@STOREID", storeId);
            parameter.Add("@FROMDATE", request.FromDate);
            parameter.Add("@TODATE", request.ToDate);
            parameter.Add("@FromMeter", request.FromMeter);
            parameter.Add("@ToMeter", request.ToMeter);
            return await dapper.ExecuteProcedure<T>("GET_RETURN_METERS", parameter);
        }

        public async Task<IEnumerable<T>> GetListOfMeterBox<T>(MeterRequest request)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@STOREID", request.StoreId);
            parameter.Add("@BOXID", request.BoxId);
            parameter.Add("@STATUS", request.Status);
            parameter.Add("@RoleType", request.RoleType);
            parameter.Add("@FromDate", request.FromDate);
            parameter.Add("@ToDate", request.ToDate);
            parameter.Add("@Length", request.Length);
            return await dapper.ExecuteProcedure<T>("GET_STORE_INCOMING_METER", parameter);
        }
        public async Task UpdateReturnMeter(string areaType, string areaValue)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@AREA_TYPE", areaType);
            parameters.Add("@AREA_VALUE", areaValue);

            await dapper.ExecuteProcedure("UPDATE_RETURN_METER", parameters);
        }

        public async Task DeleteDataWithBackupProcedure(string actionType, List<string> keyValues, string deletedBy)
        {
            var table = new DataTable();
            table.Columns.Add("KeyValue", typeof(string));

            foreach (var val in keyValues)
            {
                table.Rows.Add(val);
            }

            var parameters = new DynamicParameters();
            parameters.Add("@ActionType", actionType);
            parameters.Add("@KeyValues", table.AsTableValuedParameter("TVP_KeyValues"));
            parameters.Add("@DeletedBy", deletedBy);

            await dapper.ExecuteProcedure("DELETE_DATA_WITH_BACKUP_MULTIPLE_TABLES", parameters);
        }



        public async Task<IEnumerable<ContractorSummaryDto>> GetContractorData(int userId, string roleType)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@USERID", userId);
            parameter.Add("@RoleType", roleType);

            return await dapper.ExecuteProcedure<ContractorSummaryDto>("GetContractorMeterSummary", parameter);
        }

        // =================== HES Meter Replacement ===================

        public async Task<JsonElement?> GetMeterReplacementDataDynamicAsync(string accountId)
        {
            logger.LogInformation("➡ Starting GetMeterReplacementDataDynamicAsync for AccountId: {AccountId}", accountId);

            try
            {
                var requestUrl = "https://hesapi.mizopower.com:6011/api/wfm/getnsc/accountId";
                logger.LogInformation("➡ Request URL: {Url}", requestUrl);

                // Prepare Basic Auth
                var authValue = Convert.ToBase64String(Encoding.ASCII.GetBytes("MZR:mzr@123"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authValue);

                logger.LogInformation("✔ Basic Authentication added successfully.");

                // JSON payload
                var payload = new { accountId };
                var json = JsonSerializer.Serialize(payload);
                logger.LogInformation("➡ Request Payload: {Payload}", json);

                var jsonContent = new StringContent(json, Encoding.UTF8, "application/json");

                // Send POST Request
                logger.LogInformation("➡ Sending POST request...");
                using var response = await httpClient.PostAsync(requestUrl, jsonContent);

                logger.LogInformation("➡ Response Status Code: {StatusCode}", response.StatusCode);

                // Ensure success
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                logger.LogInformation("⬅ Raw Response: {Response}", jsonResponse);

                using var doc = JsonDocument.Parse(jsonResponse);

                // Extract data element
                if (doc.RootElement.TryGetProperty("data", out var dataElement) &&
                    dataElement.ValueKind != JsonValueKind.Null)
                {
                    logger.LogInformation("✔ Data element found. Returning parsed JsonElement.");

                    return JsonDocument.Parse(dataElement.GetRawText()).RootElement;
                }

                logger.LogWarning("⚠ No 'data' property found in response JSON!");

                return null;
            }
            catch (HttpRequestException ex)
            {
                logger.LogError(ex, "❌ HTTP Request error while calling HES API for AccountId: {AccountId}", accountId);
                return null;
            }
            catch (JsonException ex)
            {
                logger.LogError(ex, "❌ JSON Parsing error for AccountId: {AccountId}", accountId);
                return null;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "❌ Unexpected error occurred for AccountId: {AccountId}", accountId);
                return null;
            }
        }



        public async Task SaveMeterReplacementDataDynamicAsync(Dictionary<string, string> data)
        {
            // Ensure keys always exist
            string accountId = data.ContainsKey("AccountId") ? data["AccountId"] : null;

            string meterType = data.ContainsKey("Meter Type") && !string.IsNullOrWhiteSpace(data["Meter Type"])
                               ? data["Meter Type"] : null;

            string ConsumerName = data.ContainsKey("Consumer Name") && !string.IsNullOrWhiteSpace(data["Consumer Name"])
                                  ? data["Consumer Name"] : null;

            string ConsumerAddress = data.ContainsKey("Consumer Address") && !string.IsNullOrWhiteSpace(data["Consumer Address"])
                                     ? data["Consumer Address"] : null;

            string District = data.ContainsKey("District") && !string.IsNullOrWhiteSpace(data["District"])
                              ? data["District"] : null;

            string Zone = data.ContainsKey("Zone") && !string.IsNullOrWhiteSpace(data["Zone"])
                          ? data["Zone"] : null;

            string CircleWise = data.ContainsKey("Circle Wise") && !string.IsNullOrWhiteSpace(data["Circle Wise"])
                                ? data["Circle Wise"] : null;

            string Division = data.ContainsKey("Division") && !string.IsNullOrWhiteSpace(data["Division"])
                              ? data["Division"] : null;

            string Subdivision = data.ContainsKey("Subdivision") && !string.IsNullOrWhiteSpace(data["Subdivision"])
                                 ? data["Subdivision"] : null;

            string Substation = data.ContainsKey("Substation") && !string.IsNullOrWhiteSpace(data["Substation"])
                                ? data["Substation"] : null;

            string SubstationCode = data.ContainsKey("Substation Code") && !string.IsNullOrWhiteSpace(data["Substation Code"])
                                    ? data["Substation Code"] : null;

            string DTFeeder = data.ContainsKey("DT Feeder") && !string.IsNullOrWhiteSpace(data["DT Feeder"])
                              ? data["DT Feeder"] : null;

            string DTFeederCode = data.ContainsKey("DT Feeder Code") && !string.IsNullOrWhiteSpace(data["DT Feeder Code"])
                                  ? data["DT Feeder Code"] : null;

            string DTName = data.ContainsKey("DT Name") && !string.IsNullOrWhiteSpace(data["DT Name"])
                            ? data["DT Name"] : null;

            string DTCode = data.ContainsKey("DT Code") && !string.IsNullOrWhiteSpace(data["DT Code"])
                            ? data["DT Code"] : null;

            string Latitude = data.ContainsKey("Latitude") && !string.IsNullOrWhiteSpace(data["Latitude"])
                              ? data["Latitude"] : null;

            string Longitude = data.ContainsKey("Longitude") && !string.IsNullOrWhiteSpace(data["Longitude"])
                               ? data["Longitude"] : null;



            var meterParam = new DynamicParameters();
            meterParam.Add("@ConsumerId", accountId);
            meterParam.Add("@MeterType", meterType);
            await dapper.ExecuteProcedure("INSERT_SURVEY_RECEIVED_METER", meterParam);


            var dataParam = new DynamicParameters();
            dataParam.Add("@ConsumerId", accountId);
            dataParam.Add("@NewConsumerId", accountId);
            dataParam.Add("@Latitude", Latitude);
            dataParam.Add("@Longitude", Longitude);
            dataParam.Add("@RecivedFrom", "HES_API");
            await dapper.ExecuteProcedure("INSERT_SURVEY_RECEIVED_DATA", dataParam);


            var consumerParam = new DynamicParameters();
            consumerParam.Add("@ConsumerId", accountId);
            consumerParam.Add("@ConsumerName", ConsumerName);
            consumerParam.Add("@ConsumerAddress", ConsumerAddress);
            consumerParam.Add("@District", District);
            consumerParam.Add("@Zone", Zone);
            consumerParam.Add("@CircleWise", CircleWise);
            consumerParam.Add("@Division", Division);
            consumerParam.Add("@Subdivision", Subdivision);
            consumerParam.Add("@Substation", Substation);
            consumerParam.Add("@SubstationCode", SubstationCode);
            consumerParam.Add("@DTFeeder", DTFeeder);
            consumerParam.Add("@DTFeederCode", DTFeederCode);
            consumerParam.Add("@DTName", DTName);
            consumerParam.Add("@DTCode", DTCode);

            await dapper.ExecuteProcedure("INSERT_SURVEY_RECEIVED_CONSUMER", consumerParam);
        }

        public async Task<string> UpdateConsumerRemark(string consumerId, string tableName, string remark, string remarkDescription, ClaimsPrincipal userContext)
        {
            string updatedBy = userContext?.FindFirst("Name")?.Value;

            if (string.IsNullOrWhiteSpace(updatedBy))
                return JsonSerializer.Serialize(new { result = "TOKEN_USERNAME_MISSING" });

            if (string.IsNullOrWhiteSpace(remark))
                return JsonSerializer.Serialize(new { result = "Remark is required." });

            remark = remark.Trim().ToUpper();

            if (remark != "ACCEPT" && remark != "REJECT" && remark != "REAPPROVED")
                return JsonSerializer.Serialize(new { result = "Invalid remark. Only 'ACCEPT', 'REJECT', or 'REAPPROVED' allowed." });

            if (remark == "REJECT" && string.IsNullOrWhiteSpace(remarkDescription))
                return JsonSerializer.Serialize(new { result = "Reject reason is required." });

            if (remark != "REJECT")
                remarkDescription = null;

            var p = new DynamicParameters();
            p.Add("@ConsumerId", consumerId);
            p.Add("@TableName", tableName);
            p.Add("@Remark", remark);
            p.Add("@RemarkDescription", remarkDescription);
            p.Add("@UpdatedBy", updatedBy);

            var resultList = await dapper.ExecuteProcedure<string>(
                "sp_UpdateConsumerRemark",
                p
            );

            var result = resultList.FirstOrDefault();

            return result ?? "ERROR";
        }

        // =================== Replacement Meter Update ===================

        public async Task<string> UpdateReplacementMeter(
    string areaType,
    string areaValue,
    string newAreaValue,
    string newInitialNetMeterImportReadingKWH,
    string newInitialNetMeterExportReadingKWH,
    string newInitialNetMeterImportReadingKVAH,
    string newInitialNetMeterExportReadingKVAH,
    string oldInitialNetMeterImportReadingKWH,
    string oldInitialNetMeterExportReadingKWH,
    string oldInitialNetMeterImportReadingKVAH,
    string oldInitialNetMeterExportReadingKVAH,
    string oldMeterPhotoUrl,
    string newMeterPhotoUrl)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@AREA_TYPE", areaType);
            parameters.Add("@AREA_VALUE", areaValue);
            parameters.Add("@NewAreaValue", newAreaValue);
            parameters.Add("@NewInitialNetMeterImportReadingKWH", newInitialNetMeterImportReadingKWH);
            parameters.Add("@NewInitialNetMeterExportReadingKWH", newInitialNetMeterExportReadingKWH);
            parameters.Add("@NewInitialNetMeterImportReadingKVAH", newInitialNetMeterImportReadingKVAH);
            parameters.Add("@NewInitialNetMeterExportReadingKVAH", newInitialNetMeterExportReadingKVAH);
            parameters.Add("@OldInitialNetMeterImportReadingKWH", oldInitialNetMeterImportReadingKWH);
            parameters.Add("@OldInitialNetMeterExportReadingKWH", oldInitialNetMeterExportReadingKWH);
            parameters.Add("@OldInitialNetMeterImportReadingKVAH", oldInitialNetMeterImportReadingKVAH);
            parameters.Add("@OldInitialNetMeterExportReadingKVAH", oldInitialNetMeterExportReadingKVAH);
            parameters.Add("@OldMeterPhoto", oldMeterPhotoUrl);
            parameters.Add("@NewMeterPhoto", newMeterPhotoUrl);

            await dapper.ExecuteProcedure(
                "UPDATE_REPLACEMENT_METER",
                parameters
            );

            return "SUCCESS";
        }

        // =================== Get All Replacement Meters ===================

        public async Task<IEnumerable<T>> GetReplacementMeters<T>()
        {
            logger.LogInformation("➡ Calling GET_REPLACEMENT_METERS (No Parameters)");

            return await dapper.ExecuteProcedure<T>(
                "GET_REPLACEMENT_METERS",
                null
            );
        }

        // =================== HES Proxy (old meter readings) ===================
        // Uses the project's existing HES login pattern (like HesMeter.cs) with the
        // MZR service account and fetches daily load profile data using Bearer auth.
        // No cookies needed - apikey is a JWT valid for 24h, cached + auto re-login.

        private string? _hesApiKey;
        private DateTime _hesApiKeyFetchedAtUtc;
        private string? _hesBaseUrl;
        private static readonly SemaphoreSlim _hesApiKeyLock = new(1, 1);
        private static readonly TimeSpan _hesApiKeyLifetime = TimeSpan.FromHours(22); // token 24h hota hai, 22 pe refresh

        // BaseUrl resolution: HesUiConfig:BaseUrl -> derive host from LoginUrl -> built-in default.
        // Login URL works on the server (port 6005), so deriving from it keeps daily-readings
        // calls on the same reachable host even if BaseUrl is missing from appsettings.
        private const string HesDefaultBaseUrl = "https://hesapi.mizopower.com:6011";

        private static string ResolveHesBaseUrl(IReadOnlyDictionary<string, string> config)
        {
            var baseUrl = config.GetValueOrDefault("BaseUrl", string.Empty);
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                // Derive from LoginUrl (e.g. http://localhost:6005/api/v1/auth/login -> http://localhost:6005)
                var loginUrl = config.GetValueOrDefault("LoginUrl", string.Empty);
                if (!string.IsNullOrWhiteSpace(loginUrl) &&
                    Uri.TryCreate(loginUrl, UriKind.Absolute, out var loginUri) &&
                    !string.IsNullOrWhiteSpace(loginUri.Authority))
                {
                    baseUrl = $"{loginUri.Scheme}://{loginUri.Authority}";
                }
            }
            if (string.IsNullOrWhiteSpace(baseUrl)) baseUrl = HesDefaultBaseUrl;
            return baseUrl.TrimEnd('/');
        }

        private string HesDailyUrl
        {
            get
            {
                var config = Configuration!.GetSection("HesUiConfig").GetChildren()
                    .ToDictionary(c => c.Key!, c => c.Value ?? string.Empty);
                return $"{ResolveHesBaseUrl(config)}/api/v1/daily/getUiDailyLoadProfileData";
            }
        }

        private async Task<string> GetHesApiKeyAsync()
        {
            // Cached + not expired -> reuse
            if (!string.IsNullOrEmpty(_hesApiKey) &&
                DateTime.UtcNow - _hesApiKeyFetchedAtUtc < _hesApiKeyLifetime)
            {
                return _hesApiKey!;
            }

            await _hesApiKeyLock.WaitAsync();
            try
            {
                // Double-check after lock
                if (!string.IsNullOrEmpty(_hesApiKey) &&
                    DateTime.UtcNow - _hesApiKeyFetchedAtUtc < _hesApiKeyLifetime)
                {
                    return _hesApiKey!;
                }

                var config = Configuration!
                    .GetSection("HesUiConfig").GetChildren()
                    .ToDictionary(c => c.Key!, c => c.Value ?? string.Empty);

                // BUILT-IN DEFAULTS - works even if config is missing (production safe)
                // BaseUrl is configurable: if port 6011 is not reachable in production,
                // set "BaseUrl": "http://localhost:6011" in appsettings.json
                var baseUrl = ResolveHesBaseUrl(config);

                var loginUrl = config.GetValueOrDefault("LoginUrl", $"{baseUrl}/api/v1/auth/login");
                if (string.IsNullOrWhiteSpace(loginUrl)) loginUrl = $"{baseUrl}/api/v1/auth/login";

                _hesBaseUrl = baseUrl;

                var email = config.GetValueOrDefault("Email", "");
                var password = config.GetValueOrDefault("Password", "");

                if (string.IsNullOrWhiteSpace(email)) email = "MZR";
                if (string.IsNullOrWhiteSpace(password)) password = "mzr@123";

                logger.LogInformation("➡ Logging in to HES ({Url}) to get apikey", loginUrl);

                using var request = new HttpRequestMessage(HttpMethod.Post, loginUrl);
                request.Content = new StringContent(
                    JsonSerializer.Serialize(new { email, password }),
                    Encoding.UTF8,
                    "application/json");

                using var response = await httpClient.SendAsync(request);
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    throw new Exception($"HES login failed ({(int)response.StatusCode}): {body}");

                using var doc = JsonDocument.Parse(body);
                var root = doc.RootElement;

                // Response: {"result":true,"apikey":"...","message":"Login Success"}
                bool loginOk = root.TryGetProperty("result", out var resultEl) && resultEl.ValueKind == JsonValueKind.True;
                bool hasKey  = root.TryGetProperty("apikey", out var apiKeyEl) && apiKeyEl.ValueKind == JsonValueKind.String;

                if (!loginOk || !hasKey)
                {
                    throw new Exception($"HES login response invalid: {body}");
                }

                var apiKey = apiKeyEl.GetString();
                if (string.IsNullOrEmpty(apiKey))
                    throw new Exception("HES login succeeded but apikey empty");

                _hesApiKey = apiKey;
                _hesApiKeyFetchedAtUtc = DateTime.UtcNow;

                logger.LogInformation("✔ HES apikey cached (valid ~22h)");
                return apiKey;
            }
            finally
            {
                _hesApiKeyLock.Release();
            }
        }

        public async Task<string?> GetOldMeterReadingsFromHesAsync(string meterNo)
        {
            try
            {
                var apiKey = await GetHesApiKeyAsync();

                var url = HesDailyUrl;
                var payload = JsonSerializer.Serialize(new
                {
                    levelName = "METER",
                    levelValue = meterNo,
                    startDate = "2025-01-01 00:00",
                    endDate = DateTime.Now.ToString("yyyy-MM-dd 23:59"),
                    devType = "1P"
                });

                using var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

                using var response = await httpClient.SendAsync(request);
                var body = await response.Content.ReadAsStringAsync();

                // apikey expired/invalid -> re-login once and retry
                if ((int)response.StatusCode == 401 || body.Contains("Missing or invalid authorization"))
                {
                    logger.LogWarning("⚠ HES apikey expired - re-login and retry");
                    _hesApiKey = null;
                    apiKey = await GetHesApiKeyAsync();

                    using var retryRequest = new HttpRequestMessage(HttpMethod.Post, url);
                    retryRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                    retryRequest.Content = new StringContent(payload, Encoding.UTF8, "application/json");

                    using var retryResponse = await httpClient.SendAsync(retryRequest);
                    body = await retryResponse.Content.ReadAsStringAsync();

                    if (!retryResponse.IsSuccessStatusCode)
                    {
                        logger.LogError("❌ HES retry bhi fail: {Status} {Body}", (int)retryResponse.StatusCode, body);
                        return JsonSerializer.Serialize(new { result = false, message = "HES readings fetch failed", data = (object?)null });
                    }
                }

                // Raw JSON as-is return (controller Content() se bhejega)
                return body;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "❌ GetOldMeterReadingsFromHesAsync failed for meter {MeterNo}", meterNo);
                return JsonSerializer.Serialize(new { result = false, message = ex.Message, data = (object?)null });
            }
        }

    }
}
