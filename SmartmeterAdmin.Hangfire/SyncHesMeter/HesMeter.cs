using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SmartMeterAdmin.Infrastructure.Helpers;
using SmartMeterAdmin.Services.Meter;
using SmartMeterAdmin.ViewModel.Hes;
using System.Diagnostics.Metrics;
using System.Text;

namespace SmartmeterAdmin.Hangfire.SyncHesMeter
{
    public class HesMeter
    {
        private readonly IMeter meter;
        private readonly ILogger<HesMeter> logger;
        private readonly HesBaseConfig hesConfig;
        private HesLoginDTO hesLoginData = new();

        public HesMeter(IMeter meter, ILogger<HesMeter> logger, IOptions<HesBaseConfig> hesConfig)
        {
            this.meter = meter;
            this.logger = logger;
            this.hesConfig = hesConfig.Value;
        }

        [AutomaticRetry(Attempts = 0)]
        public async Task SendToHes(string areaType)
        {
            var settings = HesPayloadSerializerSettings();
            try
            {
                await HesLoginApi();
                if (hesLoginData.Result)
                {
                    var data = await meter.GetPendingHesMeter(areaType);
                    foreach (var item in data)
                    {
                        logger.LogCritical("SendToHes PayLoad Bind Data : {consumerId}", item.ConsumerNo);
                        item.Description = "Add By App";
                        item.OwnerName = hesConfig.OwnerName ?? item.OwnerName;
                        var serializeData = JsonConvert.SerializeObject(item, settings);
                        logger.LogCritical("SendToHes PayLoad : {payLoad}", serializeData);
                        await UpdateStatus(serializeData, item.ConsumerNo, areaType);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogCritical("SendToHes Exception : {exception}", ex.Message);
            }
        }

        private async Task<bool> SendToHesApi(StringContent content)
        {
            var (success, _, _) = await SendToHesApiDetailed(content);
            return success;
        }

        // Detailed version: HES ka HTTP status + response body bhi wapas deta hai (push log ke liye)
        private async Task<(bool Success, string HttpStatus, string Body)> SendToHesApiDetailed(StringContent content)
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + hesLoginData.ApiKey);
            var response = await client.PostAsync(hesConfig.DeviceUrl + "devices/addDevice", content);
            string responseBody = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return (true, ((int)response.StatusCode).ToString(), responseBody);

            logger.LogCritical("Exception From HES : {exception}", responseBody);
            return (false, ((int)response.StatusCode).ToString(), responseBody);
        }

        // SendToHes (background) aur PushSelectedNow (sync) dono ka HES payload format same hai
        private static JsonSerializerSettings HesPayloadSerializerSettings()
        {
            return new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented // Optional: Makes the JSON output more readable
            };
        }

        /// <summary>
        /// CONSUMER push: selected ids ko 1-by-1 HES pe sync push karta hai aur har meter ka
        /// log entry banata hai (UpdateHesStatus response me wapas jata hai).
        /// Background SendToHes flow isse untouched hai.
        /// </summary>
        public async Task<HesPushResult> PushSelectedNow(string areaType, string commaIds)
        {
            var result = new HesPushResult { AreaType = areaType };
            var idSet = (commaIds ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => x.Length > 0)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            try
            {
                await HesLoginApi();
                if (!hesLoginData.Result)
                {
                    result.Error = "HES login failed";
                    logger.LogCritical("PushSelectedNow : HES login failed");
                    return result;
                }

                var pending = await meter.GetPendingHesMeter(areaType);
                var selected = pending
                    .Where(x => idSet.Contains((x.ConsumerNo ?? string.Empty).Trim()))
                    .ToList();
                result.Total = selected.Count;

                var settings = HesPayloadSerializerSettings();

                foreach (var item in selected) // 1-by-1 push (user requirement)
                {
                    var entry = new HesPushLogEntry
                    {
                        ConsumerId = item.ConsumerNo,
                        MeterNo = item.DeviceSerialNo
                    };
                    try
                    {
                        item.Description = "Add By App";
                        item.OwnerName = hesConfig.OwnerName ?? item.OwnerName;
                        entry.Payload = JsonConvert.SerializeObject(item, settings);
                        logger.LogCritical("PushSelectedNow PayLoad : {consumerId}", item.ConsumerNo);

                        var content = new StringContent(entry.Payload, Encoding.UTF8, "application/json");
                        var (success, httpStatus, body) = await SendToHesApiDetailed(content);
                        entry.HesHttpStatus = httpStatus;
                        entry.HesResponse = body;

                        if (success)
                        {
                            await meter.UpdateHesStatus(areaType, item.ConsumerNo, "COMPLETED");
                            entry.Status = "SUCCESS";
                            entry.Message = "HES push done, DB status COMPLETED";
                            result.Success++;
                            logger.LogCritical("PushSelectedNow Updated Status In Database : {consumerId}", item.ConsumerNo);
                        }
                        else
                        {
                            entry.Status = "FAILED";
                            entry.Message = "HES push failed, meter PENDING rahega (background retry hoga)";
                            result.Failed++;
                        }
                    }
                    catch (Exception ex)
                    {
                        entry.Status = "FAILED";
                        entry.HesResponse = ex.Message;
                        entry.Message = "Exception during push, meter PENDING rahega";
                        result.Failed++;
                        logger.LogCritical("PushSelectedNow Exception : {exception}", ex.Message);
                    }
                    result.Logs.Add(entry);
                }
            }
            catch (Exception ex)
            {
                result.Error = ex.Message;
                logger.LogCritical("PushSelectedNow Exception : {exception}", ex.Message);
            }
            return result;
        }
        private async Task HesLoginApi()
        {
            var loginData = new
            {
                email = hesConfig.Email,
                password = hesConfig.Password
            };

            var content = new StringContent(JsonConvert.SerializeObject(loginData), Encoding.UTF8, "application/json");

            using HttpClient client = new();
            var response = await client.PostAsync(hesConfig.DeviceUrl + "auth/login", content);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                hesLoginData = JsonConvert.DeserializeObject<HesLoginDTO>(result) ?? new HesLoginDTO();
                hesConfig.OwnerName = hesConfig.OwnerName;
            }
            else
                logger.LogCritical("Hes Login Failed : {exception}", response);
        }

        private async Task UpdateStatus(string serializeData, string ConsumerNo, string areaType)
        {
            try
            {
                var content = new StringContent(serializeData, Encoding.UTF8, "application/json");
                var response = await SendToHesApi(content);
                logger.LogCritical("SendToHes Response Recived : {consumerId}", ConsumerNo);
                if (response)
                {
                    await meter.UpdateHesStatus(areaType, ConsumerNo, "COMPLETED");
                    logger.LogCritical("SendToHes Updated Status In Database : {consumerId}", ConsumerNo);
                }
            }
            catch (Exception ex)
            {
                logger.LogCritical("SendToHes Exception : {exception}", ex.Message);
            }
        }
    }
}
