using MeterService.Application.Interfaces;
using MeterService.Domain.Entities;
using MeterService.Application.Models;
using Notification.gRPC.Services;
using Microsoft.Extensions.Options;
using Notification.gRPC;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text;

namespace MeterService.Infrastructure.Services;

public sealed class SepacService : ISepacService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SepacService> _logger;
    private readonly IMeteringDataService _meteringDataService;
    private readonly IMessageService _messageService;
    private readonly ICryptoService _cryptoService;

    private readonly int METER_SAVE_COUNT;
    private Dictionary<int, string> _meters = new Dictionary<int, string>();

    public SepacService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<SepacService>();
        _meteringDataService = serviceProvider.GetRequiredService<IMeteringDataService>();
        _messageService = serviceProvider.GetRequiredService<IMessageService>();
        _cryptoService = serviceProvider.GetRequiredService<ICryptoService>();

        var option = serviceProvider.GetRequiredService<IOptions<IGMCOptions>>();

        METER_SAVE_COUNT = option.Value.MeterSaveCount;
        UpdateMeterDictionary();
    }

    public async Task<bool> UpdateMetersTableAsync(CancellationToken cancellationToken)
    {
        TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
        cancellationToken.Register(() =>
        {
            taskCompletionSource.TrySetCanceled();
        });

        try
        {
            var request = new BaseInfoParam();
            var task = _meteringDataService.GetBaseInfoAsync(request);

            if ((await Task.WhenAny(task, taskCompletionSource.Task)).IsCompleted)
            {
                var response = await task;
                if (response.Success)
                {
                    Tools[] tools = response.Result.Tools;
                    string r = await UpdateMeters(tools, cancellationToken);
                    UpdateMeterDictionary();
                    _logger.LogInformation(r);
                    taskCompletionSource.TrySetResult(true);
                }
                else
                {
                    foreach (var error in response.Errors)
                        _logger.LogError($"Error Code:{error.ErrorCode} - {error.ErrorMessage}");

                    taskCompletionSource.TrySetResult(false);
                }
            }
            else
            {
                _logger.LogError("Error UpdateMetersTableAsync task Completions");
                taskCompletionSource.TrySetResult(false);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"UpdateMetersTableAsync Error:{ex.Message}");
            taskCompletionSource.TrySetResult(false);
        }

        return await taskCompletionSource.Task;
    }
    public async Task<bool> DownloadMetersEnergy(DateTime startDate, DateTime endDate, bool update, CancellationToken cancellationToken)
    {
        int meterCount = 0;
        int totalRecordAdd = 0;
        int totalRecordUpdate = 0;
        string message;
        List<MeterEnergyEntity> meterEnergy = new List<MeterEnergyEntity>();

        var (meterRecords, msg) = await GetMeterEnergyAsync(startDate.Date, endDate.Date, null, cancellationToken);

        if (meterRecords == null)
        {
            _logger.LogWarning(msg);
            await _messageService.SendMessageAsync(msg, MessageScope.UpdateMeterTable, MessageType.Warning);
            return false;
        }

        //await PublishMetersEnergy(meterRecords);

        _logger.LogInformation($"GetMeterEnergyAsync Reads {meterRecords.Count()} Records from {startDate.ToString("yyyy-MM-dd")} to {endDate.ToString("yyyy-MM-dd")}");
        message = $"تعداد {meterRecords.Count()} رکورد از تاریخ {startDate.ToString("dd MMM yyyy")} لغایت {endDate.ToString("dd MMM yyyy")} از سامانه سپاک دریافت شد.";
        await _messageService.SendMessageAsync(message, MessageScope.UpdateMeterTable, MessageType.Success);

        var meterGroups = meterRecords
            .GroupBy(g => g.MeterId)
            .Select(s => new
            {
                MeterId = s.Key,
                Records = s.GroupBy(gg => gg.RecordDate).Select(x => x.First())
                .Select(ss => new
                {
                    RecordDate = ss.RecordDate,
                    ToolTypeId = ss.ToolTypeId,
                    EnergyActiveExport = ss.EnergyActiveExport,
                    EnergyActiveImport = ss.EnergyActiveImport,
                    EnergyReactiveExport = ss.EnergyReactiveExport,
                    EnergyReactiveImport = ss.EnergyReactiveImport
                }).OrderBy(o => o.RecordDate)
            });

        int totalMeterCount = meterGroups.Count();

        using var scope = _serviceProvider.CreateScope();
        var _context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

        int meterSaveCount = 0;
        var context = _context as DbContext;
        context.ChangeTracker.AutoDetectChangesEnabled = false;

        foreach (var meterRecord in meterGroups)
        {
            int meterRecordAdd = 0, meterRecordUpdate = 0;
            string serialNumber;

            if (!_meters.TryGetValue(meterRecord.MeterId, out serialNumber))
            {
                await UpdateMetersTableAsync(cancellationToken);

                if (!_meters.TryGetValue(meterRecord.MeterId, out serialNumber))
                {
                    message = $"Meter with Id: {meterRecord.MeterId} doesn't exist in Database.";
                    _logger.LogWarning(message);
                    message = $"میتر با شناسه {meterRecord.MeterId} در بانک اطلاعاتی موجود نیست.";
                    await _messageService.SendMessageAsync(message, MessageScope.UpdateMeterTable, MessageType.Warning);
                    continue;
                }
            }

            foreach (var record in meterRecord.Records)
            {
                var guid = _cryptoService.GetDeterministicGuid($"{meterRecord.MeterId}-{record.RecordDate.Ticks}");

                if (_context.MeterEnergys.Any(e => e.MeterEnergyId == guid))
                {
                    MeterEnergyEntity me = await _context.MeterEnergys.FirstOrDefaultAsync(x => x.MeterEnergyId == guid);

                    if (update && me != null)
                    {

                        me.MeterId = meterRecord.MeterId;
                        me.RecordDate = record.RecordDate;
                        me.ToolTypeId = record.ToolTypeId;
                        me.EnergyActiveExport = record.EnergyActiveExport;
                        me.EnergyActiveImport = record.EnergyActiveImport;
                        me.EnergyReactiveExport = record.EnergyReactiveExport;
                        me.EnergyReactiveImport = record.EnergyReactiveImport;

                        _context.MeterEnergys.Update(me);

                        totalRecordUpdate++;
                        meterRecordUpdate++;
                    }
                }
                else
                {
                    MeterEnergyEntity me = new MeterEnergyEntity();
                    me.MeterEnergyId = guid;
                    me.MeterId = meterRecord.MeterId;
                    me.RecordDate = record.RecordDate;
                    me.ToolTypeId = record.ToolTypeId;
                    me.EnergyActiveExport = record.EnergyActiveExport;
                    me.EnergyActiveImport = record.EnergyActiveImport;
                    me.EnergyReactiveExport = record.EnergyReactiveExport;
                    me.EnergyReactiveImport = record.EnergyReactiveImport;

                    await _context.MeterEnergys.AddAsync(me, cancellationToken);
                    meterEnergy.Add(me);

                    totalRecordAdd++;
                    meterRecordAdd++;
                }
            }

            meterCount++;
            meterSaveCount++;

            message = $"افزودن {meterRecordAdd} و به روز رسانی {meterRecordUpdate} رکورد به جدول انرژی میتر  {serialNumber} از تاریخ {startDate.ToString("dd MMM yyyy")} لفایت {endDate.ToString("dd MMM yyyy")}";
            await _messageService.SendProgressTextAsync(message, MessageScope.UpdateMeterTable);

            if (meterSaveCount <= METER_SAVE_COUNT)
                continue;

            try
            {
                context.ChangeTracker.DetectChanges();
                await _context.SaveChangesAsync(cancellationToken);
                int progress = meterCount * 100 / totalMeterCount;
                meterSaveCount = 0;
                await _messageService.SendProgressAsync(progress, MessageScope.UpdateMeterTable);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"DbUpdateException Error:{ex.Message}");
                message = $"خطا در به روز رسانی جدول انرژی میتر {serialNumber}";
                await _messageService.SendMessageAsync(message, MessageScope.UpdateMeterTable, MessageType.Error);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception Error:{ex.Message}");
                message = $"خطا عمومی در به روز رسانی جدول انرژی میتر {serialNumber}";
                await _messageService.SendMessageAsync(message, MessageScope.UpdateMeterTable, MessageType.Error);
            }
        }

        context.ChangeTracker.AutoDetectChangesEnabled = true;

        _logger.LogInformation($"Added {totalRecordAdd} and Updated {totalRecordUpdate} Records for {meterCount} Meter from {startDate.ToString("yyyy-MM-dd")} to {endDate.ToString("yyyy-MM-dd")}");
        message = $"افزودن {totalRecordAdd} و به روزرسانی {totalRecordUpdate} رکورد برای تعداد {meterCount} میتر از تاریخ {startDate.ToString("dddd dd MMMM yyyy")} لغایت {endDate.ToString("dddd dd MMMM yyyy")}";
        await _messageService.SendMessageAsync(message, MessageScope.UpdateMeterTable, MessageType.Success);
        await _messageService.SendRefreshEnergyProfileAsync(startDate.Date, endDate.Date);

        return true;
    }
    public async Task<bool> DownloadMeterEnergy(DateTime startDate, DateTime endDate, string serialNumber, bool update, CancellationToken cancellationToken)
    {

        int recordAdd = 0;
        int recordUpdate = 0;
        string message;

        List<MeterEnergyEntity> meterEnergy = new List<MeterEnergyEntity>();

        var (meterRecords, msg) = await GetMeterEnergyAsync(startDate.Date, endDate.Date, serialNumber, cancellationToken);

        if (meterRecords == null)
        {
            _logger.LogWarning(msg);
            await _messageService.SendMessageAsync(msg, MessageScope.UpdateMeterTable, MessageType.Warning);
            return false;
        }

        var meterGroups = meterRecords
            .GroupBy(g => g.MeterId)
            .Select(s => new
            {
                MeterId = s.Key,
                Records = s.GroupBy(gg => gg.RecordDate).Select(x => x.First())
                .Select(ss => new
                {
                    RecordDate = ss.RecordDate,
                    ToolTypeId = ss.ToolTypeId,
                    EnergyActiveExport = ss.EnergyActiveExport,
                    EnergyActiveImport = ss.EnergyActiveImport,
                    EnergyReactiveExport = ss.EnergyReactiveExport,
                    EnergyReactiveImport = ss.EnergyReactiveImport
                })
            });

        using var scope = _serviceProvider.CreateScope();
        var _context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

        foreach (var meterRecord in meterGroups)
        {
            foreach (var record in meterRecord.Records)
            {
                var guid = _cryptoService.GetDeterministicGuid($"{meterRecord.MeterId}-{record.RecordDate.Ticks}");

                if (_context.MeterEnergys.Any(e => e.MeterEnergyId == guid))
                {
                    MeterEnergyEntity me = await _context.MeterEnergys.FirstOrDefaultAsync(x => x.MeterEnergyId == guid);

                    if (update && me != null)
                    {

                        me.MeterId = meterRecord.MeterId;
                        me.RecordDate = record.RecordDate;
                        me.ToolTypeId = record.ToolTypeId;
                        me.EnergyActiveExport = record.EnergyActiveExport;
                        me.EnergyActiveImport = record.EnergyActiveImport;
                        me.EnergyReactiveExport = record.EnergyReactiveExport;
                        me.EnergyReactiveImport = record.EnergyReactiveImport;

                        _context.MeterEnergys.Update(me);
                        recordUpdate++;
                    }
                }
                else
                {
                    MeterEnergyEntity me = new MeterEnergyEntity();
                    me.MeterEnergyId = guid;
                    me.MeterId = meterRecord.MeterId;
                    me.RecordDate = record.RecordDate;
                    me.ToolTypeId = record.ToolTypeId;
                    me.EnergyActiveExport = record.EnergyActiveExport;
                    me.EnergyActiveImport = record.EnergyActiveImport;
                    me.EnergyReactiveExport = record.EnergyReactiveExport;
                    me.EnergyReactiveImport = record.EnergyReactiveImport;

                    await _context.MeterEnergys.AddAsync(me, cancellationToken);
                    meterEnergy.Add(me);

                    recordAdd++;
                }
            }
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"DbUpdateException Error:{ex.Message}");
                message = "خطای به روز رسانی جدول پروفایل انرژی";
                await _messageService.SendMessageAsync(message, MessageScope.UpdateMeterTable, MessageType.Error);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception Error:{ex.Message}");
                message = "خطای عمومی در به روز رسانی جدول پروفایل انرژی";
                await _messageService.SendMessageAsync(message, MessageScope.UpdateMeterTable, MessageType.Error);
                return false;
            }
        }

        _logger.LogInformation($"Added {recordAdd} and Updated {recordUpdate} Records for {serialNumber} Meter from {startDate.ToString("yyyy-MM-dd")} to {endDate.ToString("yyyy-MM-dd")}");
        message = $"افزودن {recordAdd} و به روزرسانی {recordUpdate} رکورد برای میتر با شماره سریال {serialNumber} از تاریخ {startDate.ToString("dddd dd MMMM yyyy")} لغایت {endDate.ToString("dddd dd MMMM yyyy")}";
        await _messageService.SendMessageAsync(message, MessageScope.UpdateMeterTable, MessageType.Success);
        await _messageService.SendRefreshEnergyProfileAsync(startDate, endDate);


        return true;
    }

    private async Task<(IEnumerable<MeterEnergyEntity>, string)> GetMeterEnergyAsync(DateTime startDate, DateTime endDate, string serialNumber, CancellationToken cancellationToken)
    {
        TaskCompletionSource<(IEnumerable<MeterEnergyEntity>, string)> taskCompletionSource = new TaskCompletionSource<(IEnumerable<Domain.Entities.MeterEnergyEntity>, string)>();
        cancellationToken.Register(() =>
        {
            taskCompletionSource.TrySetCanceled();
        });

        try
        {
            MeterEnergyParam parm = new MeterEnergyParam
            {
                StartDate = startDate,
                EndDate = endDate,
                SerialNumber = serialNumber
            };

            var task = _meteringDataService.GetMeterEnergyAsync(parm);
            if ((await Task.WhenAny(task, taskCompletionSource.Task)).IsCompleted)
            {
                var result = await task;
                if (result.Success)
                {
                    MeterEnergy[] meterEnergys = result.Result;

                    var meterEnergylist = new List<MeterEnergyEntity>();

                    foreach (var me in meterEnergys)
                    {
                        meterEnergylist.Add(new MeterEnergyEntity
                        {
                            RecordDate = me.GregorianDate,
                            MeterId = me.MeterId,
                            ToolTypeId = me.ToolTypeId,
                            EnergyActiveExport = me.Energy_Active_Export * 1000000,
                            EnergyActiveImport = me.Energy_Active_Import * 1000000,
                            EnergyReactiveExport = me.Energy_Reactive_Export * 1000000,
                            EnergyReactiveImport = me.Energy_Reactive_Import * 1000000
                        });
                    }

                    taskCompletionSource.TrySetResult((meterEnergylist, string.Empty));
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var error in result.Errors)
                    {
                        sb.AppendLine($"Error Code:{error.ErrorCode} - {error.ErrorMessage}");
                    }
                    taskCompletionSource.TrySetResult((null, sb.ToString()));
                }
            }
            else
            {
                taskCompletionSource.TrySetResult((null, "Error in GetMeterEnergyAsync task Completion"));
            }

        }
        catch (Exception ex)
        {
            taskCompletionSource.TrySetResult((null, ex.Message));
        }
        return await taskCompletionSource.Task;
    }
    private async Task<string> UpdateMeters(IEnumerable<Tools> tools, CancellationToken cancellationToken)
    {
        string msg = string.Empty;
        int meterCount = 0;
        int meterAdd = 0;
        int meterUpdate = 0;

        var meters = tools
            .OrderByDescending(o => o.FormulaMountDate)
            .GroupBy(o => o.MeterId)
            .Select(s => s.First())
            .Select(x => new
            {
                MeterId = x.MeterId,
                SerialNumber = x.SerialNumber,
                Name = x.ToolName,
                StationId = x.StationId,
                StationName = x.StationName,
                StartOperationDate = x.FormulaMountDate,
                EndOperationDate = x.FormulaDismountDate ?? new DateTime(2099, 1, 1, 0, 0, 0),
                Active = x.ActiveStatus,
            })
            .OrderBy(o => o.MeterId)
            .ToList();

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

            MeterEntity meter;
            foreach (var m in meters)
            {
                if (_context.Meters.Any(e => e.MeterId == m.MeterId))
                {
                    meter = await _context.Meters.SingleOrDefaultAsync(e => e.MeterId == m.MeterId);
                    if (meter != null)
                    {
                        meter.MeterId = m.MeterId;
                        meter.SerialNumber = m.SerialNumber;
                        meter.Name = m.Name;
                        meter.StationId = m.StationId;
                        meter.StationName = m.StationName;
                        meter.Active = m.Active;
                        meter.StartOperationDate = m.StartOperationDate;
                        meter.EndOperationDate = m.EndOperationDate;

                        _context.Meters.Update(meter);

                        meterUpdate++;
                    }
                }
                else
                {
                    meter = new MeterEntity
                    {
                        MeterId = m.MeterId,
                        SerialNumber = m.SerialNumber,
                        Name = m.Name,
                        StationId = m.StationId,
                        StationName = m.StationName,
                        Active = m.Active,
                        Dismount = false,
                        StartOperationDate = m.StartOperationDate,
                        EndOperationDate = m.EndOperationDate
                    };

                    await _context.Meters.AddAsync(meter);
                    meterAdd++;
                }
                meterCount++;
            }
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, $"DbUpdateException Error:{ex.Message}");
            msg = "خطا در به روز رسانی جدول میتر";
            return msg;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception Error:{ex.Message}");
            msg = "خطا در به روز رسانی جدول میتر";
            return msg;
        }
        msg = $"افزودن {meterAdd} و به روزرسانی {meterUpdate} میتر به بانک اطلاعاتی میتر با موفقیت انجام شد.";
        _logger.LogInformation($"Added {meterAdd} Updated {meterUpdate} for Total {meterCount} Meters");

        return msg;
    }
    private void UpdateMeterDictionary()
    {
        using var scope = _serviceProvider.CreateScope();
        var _context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

        var meters = _context.Meters.ToList();
        _meters.Clear();
        foreach (var m in meters)
            _meters.Add(m.MeterId, m.SerialNumber);
    }

    //private async Task PublishMetersEnergy(IEnumerable<MeterEnergyEntity> meterEnergyEntitys)
    //{
    //    try
    //    {
    //        var meterEnergyDtos = meterEnergyEntitys
    //            .GroupBy(g => new { g.MeterId, g.RecordDate.Date })
    //            .Select(s => new MeterEnergyMessage
    //            {
    //                MeterId = s.Key.MeterId,
    //                SerialNumber = _meters[s.Key.MeterId],
    //                RecordDate = s.Key.Date,
    //                HourEnergys = s.GroupBy(gg => gg.RecordDate).Select(x => x.First()).Select(m => new HourEnergy(
    //                    m.RecordDate.Hour,
    //                    m.EnergyActiveExport,
    //                    m.EnergyActiveImport,
    //                    m.EnergyReactiveExport,
    //                    m.EnergyReactiveImport)).ToList(),
    //                Anomaly = s.Count() < 24 ? true : false,
    //            });

    //        foreach (MeterEnergyMessage meterEnergyDto in meterEnergyDtos)
    //        {
    //            await _messagePublisher.PublishAsync<MeterEnergyMessage>(meterEnergyDto);
    //        }

    //        _logger.LogInformation($"Publish {meterEnergyDtos.Count()} meter energy records");
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError("Error", ex);
    //    }
    //}
    private async Task SaveToFile(string fileName, IEnumerable<MeterEnergyEntity> meterEnergylist)
    {
        var meterGroups = meterEnergylist
            .GroupBy(g => g.MeterId)
            .Select(s => new
            {
                MeterId = s.Key,
                Records = s.Select(ss => new
                {
                    RecordDate = ss.RecordDate,
                    ToolTypeId = ss.ToolTypeId,
                    EnergyActiveExport = ss.EnergyActiveExport,
                    EnergyActiveImport = ss.EnergyActiveImport,
                    EnergyReactiveExport = ss.EnergyReactiveExport,
                    EnergyReactiveImport = ss.EnergyReactiveImport
                }).OrderBy(o => o.RecordDate)
            });

        var json = JsonSerializer.Serialize(meterGroups);
        await File.AppendAllTextAsync(fileName, json);
    }
}
