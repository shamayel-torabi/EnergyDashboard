using System.Text;
using System.Net.Http.Headers;
using Microsoft.Extensions.Caching.Distributed;
using MeterService.Domain.ValueObjects;
using MeterService.Application.Models;
using MeterService.Application.Interfaces;
using Notification.gRPC.Services;
using Notification.gRPC;
using Newtonsoft.Json;

#nullable disable

namespace MeterService.Infrastructure.Services;

public sealed class ModamService : IModamService
{
    private readonly ILogger<ModamService> _logger;
    private readonly HttpClient _httpClient;
    private readonly IMessageService _messageService;
    private readonly IDistributedCache _cache;

    public ModamService(
        ILogger<ModamService> logger,
        HttpClient httpClient,
        IMessageService messageService,
        IDistributedCache cache)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task UpdateMeterInstantAsync(DateTime startDate, DateTime endDate, string serialNumber, CancellationToken cancellationToken)
    {
        var body = new OnlineEnergyParam
        {
            StartDate = startDate,
            EndDate = endDate,
            SerialNumber = serialNumber,
        };

        var result = await SendMeterInstantRequestAsync(body, cancellationToken);
        if (result != null)
        {
            await CacheMeterInstantParameter(result, cancellationToken);
            await SendMeterInstantEvent(result);
        }
    }

    private async Task<IEnumerable<MeterInstantParameter>> SendMeterInstantRequestAsync(OnlineEnergyParam body, CancellationToken cancellationToken)
    {
        string messaage;

        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
        };

        try
        {
            using (var request = new HttpRequestMessage())
            {
                var content = new StringContent(JsonConvert.SerializeObject(body, settings));
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                request.Content = content;
                request.Method = new HttpMethod("POST");

                HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                var disposeResponse = true;
                try
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        ModamResponse modamResponse = JsonConvert.DeserializeObject<ModamResponse>(responseData);

                        if (modamResponse.Result.Count > 0)
                        {

                            var meterInstants = modamResponse.Result
                                .GroupBy(g => g.SerialNumber)
                                .Select(s => s.OrderByDescending(o => o.MeterTime))
                                .Select(x => x.First());

                            return meterInstants;
                        }
                        else
                        {
                            messaage = $"ModamResponse empty {modamResponse.Message} at {body.StartDate.ToString("g")}";
                            _logger.LogWarning(messaage);
                            messaage = $"اطلاعات دریافتی از سامانه مدام در تاریخ {body.StartDate.ToString("yyyy/MM/dd ساعت HH:mm:ss")} خالی است.";
                            await _messageService.SendMessageAsync(messaage, MessageScope.General, MessageType.Warning, false);
                        }
                    }
                    else
                    {
                        var status = (int)response.StatusCode;

                        var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                        messaage = $"IGMC Modam Service Error {responseData} with status code {status}";
                        _logger.LogWarning(messaage);
                        messaage = $"دریافت پیام با کد خطای {status} از سامانه مدام در تاریخ {body.StartDate.ToString("yyyy/MM/dd HH:mm:ss")}";
                        await _messageService.SendMessageAsync(messaage, MessageScope.General, MessageType.Warning, false);
                    }
                }
                finally
                {
                    if (disposeResponse)
                        response.Dispose();
                }
            }
        }
        catch (Exception)
        {
            messaage = "Unable to Connect IGMC Modam Service Check Network Connection !";
            _logger.LogWarning(messaage);
            messaage = "خطا در برقرای ارتباط با سایت مدام مدیریت شبکه !";
            await _messageService.SendMessageAsync(messaage, MessageScope.General, MessageType.Warning, false);
        }

        return null;
    }

    private async Task SendMeterInstantEvent(IEnumerable<MeterInstantParameter> meterInstants)
    {
        _logger.LogInformation($"MeterInstants Read {meterInstants.Count()} Meters at {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}");
        await _messageService.SendMessageAsync("به روز رسانی سامانه مدام در ساعت " + DateTime.Now.ToString("HH:mm:ss"), MessageScope.General, MessageType.Info, save: false);
        await _messageService.SendRefreshMeterInstantAsync();
    }

    private async Task CacheMeterInstantParameter(IEnumerable<MeterInstantParameter> meterInstantParameter, CancellationToken cancellationToken)
    {
        DistributedCacheEntryOptions option = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(10.0))
                .SetAbsoluteExpiration(DateTimeOffset.Now.AddMinutes(30.0));

        var meterInstants = MapToMeterInstant(meterInstantParameter);

        foreach (var mi in meterInstants)
        {
            string serializedInstants = JsonConvert.SerializeObject(mi);
            byte[] encodedMeterInstants = Encoding.UTF8.GetBytes(serializedInstants);
            await _cache.SetAsync(mi.SerialNumber, encodedMeterInstants, option, cancellationToken);
        }
    }

    private List<MeterInstant> MapToMeterInstant(IEnumerable<MeterInstantParameter> meterInstantParameters)
    {
        List<MeterInstant> mms = new List<MeterInstant>();
        foreach (var mi in meterInstantParameters)
        {
            mms.Add(new MeterInstant(
                mi.SerialNumber,
                mi.MeterTime,
                mi.Power_Active_Export ?? 0,
                mi.Power_Active_Import ?? 0,
                mi.Power_Reactive_Export ?? 0,
                mi.Power_Reactive_Import ?? 0,
                mi.Voltage_A ?? 0,
                mi.Voltage_B ?? 0,
                mi.Voltage_C ?? 0,
                mi.Current_A ?? 0,
                mi.Current_B ?? 0,
                mi.Current_C ?? 0,
                mi.EstimateStatus ?? false));
        }
        return mms;
    }
}