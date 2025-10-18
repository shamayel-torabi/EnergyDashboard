using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using EnergyDashboard.Domain.Entities;
using EnergyDashboard.Application.Interfaces;
using Notification.gRPC.Services;
using Notification.gRPC;
using MeterService.gRPC.Services;
using EnergyDashboard.Domain.Repository;
using EnergyDashboard.Infrastructure.Persistence;

namespace EnergyDashboard.Infrastructure.Services;

public class EquipmentEnergyService : IEquipmentEnergyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AppDbContext _context;
    private readonly ILogger<EquipmentEnergyService> _logger;
    private readonly IMessageService _messageService;
    private readonly ICryptoService _cryptoService;
    private readonly IMeterClient _meterService;

    public EquipmentEnergyService(
        IUnitOfWork unitOfWork,
        AppDbContext context,
        ISubstationRepository substationRepository,
        ILogger<EquipmentEnergyService> logger,
        IMessageService messageService,
        IMeterClient meterService,
        ICryptoService cryptoService)
    {
        _unitOfWork = unitOfWork;
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
        _meterService = meterService ?? throw new ArgumentNullException(nameof(meterService));
        _cryptoService = cryptoService ?? throw new ArgumentNullException(nameof(cryptoService));
    }

    public async Task UpdateEquipmentEnergy(Guid equipmentId, DateTimeOffset date, bool update)
    {
        string message;

        var equipment = await _context.Equipments
            .Include(s => s.Substation)
            .SingleOrDefaultAsync(e => e.Id == equipmentId);

        if (equipment is null)
        {
            _logger.LogWarning($"هیچ تجهیزی با شناسه {equipmentId} پیدا نشد");

            message = $"هیچ تجهیزی با شناسه {equipmentId} پیدا نشد";
            await _messageService.SendMessageAsync(message, MessageScope.UpdateEquipmentEnergyTable, MessageType.Warning);

            return;
        }

        var equipmentRecords = await GetEquipmentEnergy(equipment, date);

        if (equipmentRecords is not null && equipmentRecords.Count() > 0)
        {
            try
            {
                foreach (var dEnergy in equipmentRecords)
                {
                    if (_context.EnergyProfiles.Any(item => item.Id == dEnergy.Id))
                    {
                        if (update)
                        {
                            var ep = await _context.EnergyProfiles.FindAsync(dEnergy.Id);

                            ep.RecordDate = dEnergy.RecordDate;
                            ep.ImportWatt = dEnergy.ImportWatt;
                            ep.ExportWatt = dEnergy.ExportWatt;
                            ep.ImportVar = dEnergy.ImportVar;
                            ep.ExportVar = dEnergy.ExportVar;
                            ep.EquipmentId = dEnergy.EquipmentId;

                            _context.EnergyProfiles.Update(ep);
                        }
                    }
                    else
                    {
                        await _context.EnergyProfiles.AddAsync(dEnergy);
                    }
                }

                await _unitOfWork.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"DbUpdateException Error:{ex.Message}");

                message = "خطای به روز رسانی جدول پروفایل انرژی";
                await _messageService.SendMessageAsync(message, MessageScope.UpdateEquipmentEnergyTable, MessageType.Error);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception Error:{ex.Message}");

                message = "خطای عمومی در به روز رسانی جدول پروفایل انرژی";
                await _messageService.SendMessageAsync(message, MessageScope.UpdateEquipmentEnergyTable, MessageType.Error);
            }
        }

        await CheckMeterFailure(equipmentId, date);
    }

    public async Task UpdateEquipmentsEnergy(DateTimeOffset date, bool update)
    {
        string message;
        int meterCount = 0;

        _logger.LogInformation($"شروع به روز رسانی جدول انرژی تجهیزات در تاریخ {date.ToString("dddd dd MMMM yyyy")}");
        message = $"شروع به روز رسانی جدول انرژی تجهیزات در تاریخ {date.ToString("dddd dd MMMM yyyy")}";
        await _messageService.SendProgressTextAsync(message, MessageScope.UpdateEquipmentEnergyTable);

        try
        {
            var equipments = await _context.Equipments
                .Include(s => s.Substation)
                .ToListAsync();

            foreach (var equipment in equipments)
            {
                var equipmentRecords = await GetEquipmentEnergy(equipment, date);

                if (equipmentRecords is not null && equipmentRecords.Count() > 0)
                {
                    foreach (var dEnergy in equipmentRecords)
                    {
                        if (_context.EnergyProfiles.Any(item => item.Id == dEnergy.Id))
                        {
                            if (update)
                            {
                                var ep = await _context.EnergyProfiles.FindAsync(dEnergy.Id);

                                ep.RecordDate = dEnergy.RecordDate;
                                ep.ImportWatt = dEnergy.ImportWatt;
                                ep.ExportWatt = dEnergy.ExportWatt;
                                ep.ImportVar = dEnergy.ImportVar;
                                ep.ExportVar = dEnergy.ExportVar;
                                ep.EquipmentId = dEnergy.EquipmentId;

                                _context.EnergyProfiles.Update(ep);
                            }
                        }
                        else
                        {
                            await _context.EnergyProfiles.AddAsync(dEnergy);
                        }
                    }
                    meterCount++;
                }
            }

            await _unitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, $"DbUpdateException Error:{ex.Message}");
            message = "خطا در به روز رسانی جدول انرژی تجهیزات.";
            await _messageService.SendMessageAsync(message, MessageScope.UpdateEquipmentEnergyTable, MessageType.Error);
            return;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception Error:{ex.Message}");
            message = "خطای عمومی در به روز رسانی جدول پروفایل انرژی";
            await _messageService.SendMessageAsync(message, MessageScope.UpdateEquipmentEnergyTable, MessageType.Error);
            return;
        }

        _logger.LogInformation($"Update {meterCount} Meter in EnergyProfile Table at Date: {date.ToString("dddd dd MMMM yyyy")}");

        message = $"به روز رسانی  جدول انرژی برای تعداد {meterCount} تجهیز در تاریخ {date.ToString("dddd dd MMMM yyyy")}";
        await _messageService.SendMessageAsync(message, MessageScope.UpdateEquipmentEnergyTable, MessageType.Success);
        await CheckMetersFailure(date);
    }

    public async Task UpdateEquipmentDailyEnergy(Guid equipmentId, DateTimeOffset date, bool update)
    {
        string message;

        try
        {
            var equipment = await _context.Equipments
                .Include(s => s.Substation)
                .SingleOrDefaultAsync(e => e.Id == equipmentId);

            if (equipment is not null)
            {
                var dailyEnergy = await GetEquipmentDailyEnergy(equipment, date);

                if (dailyEnergy is not null)
                {
                    if (_context.DailyEnergys.Any(e => e.Id == dailyEnergy.Id))
                    {
                        if (update)
                        {
                            var de = await _context.DailyEnergys.FindAsync(dailyEnergy.Id);

                            de.RecordDate = dailyEnergy.RecordDate;
                            de.ImportWatt = dailyEnergy.ImportWatt;
                            de.ExportWatt = dailyEnergy.ExportWatt;
                            de.ImportVar = dailyEnergy.ImportVar;
                            de.ExportVar = dailyEnergy.ExportVar;
                            de.EquipmentId = dailyEnergy.EquipmentId;

                            _context.DailyEnergys.Update(de);
                        }
                    }
                    else
                    {
                        await _context.DailyEnergys.AddAsync(dailyEnergy);
                    }
                }

                await _unitOfWork.SaveChangesAsync();
            }
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, $"DbUpdateException Error:{ex.Message}");
            message = "خطای به روز رسانی جدول انرژی روزانه تجهیز";
            await _messageService.SendMessageAsync(message, MessageScope.UpdateEquipmentEnergyTable, MessageType.Error);
            return;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception Error:{ex.Message}");
            message = "خطای عمومی به روز رسانی جدول انرژی روزانه تجهیز";
            await _messageService.SendMessageAsync(message, MessageScope.UpdateEquipmentEnergyTable, MessageType.Error);
            return;
        }
    }

    public async Task UpdateEquipmentsDailyEnergy(DateTimeOffset date, bool update)
    {
        string message;

        try
        {
            var equipments = await _context.Equipments
                .Include(s => s.Substation)
                .ToListAsync();

            foreach (var equipment in equipments)
            {
                var dailyEnergy = await GetEquipmentDailyEnergy(equipment, date);

                if (dailyEnergy is not null)
                {
                    if (_context.DailyEnergys.Any(e => e.Id == dailyEnergy.Id))
                    {
                        if (update)
                        {
                            var de = await _context.DailyEnergys.FindAsync(dailyEnergy.Id);

                            de.RecordDate = dailyEnergy.RecordDate;
                            de.ImportWatt = dailyEnergy.ImportWatt;
                            de.ExportWatt = dailyEnergy.ExportWatt;
                            de.ImportVar = dailyEnergy.ImportVar;
                            de.ExportVar = dailyEnergy.ExportVar;
                            de.EquipmentId = dailyEnergy.EquipmentId;

                            _context.DailyEnergys.Update(de);
                        }
                    }
                    else
                    {
                        await _context.DailyEnergys.AddAsync(dailyEnergy);
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, $"DbUpdateException Error:{ex.Message}");
            message = "خطای به روز رسانی جدول انرژی روزانه تجهیزات";
            await _messageService.SendMessageAsync(message, MessageScope.UpdateEquipmentEnergyTable, MessageType.Error);
            return;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception Error:{ex.Message}");
            message = "خطای عمومی در به روز رسانی جدول انرژی روزانه تجهیزات";
            await _messageService.SendMessageAsync(message, MessageScope.UpdateEquipmentEnergyTable, MessageType.Error);
            return;
        }

        _logger.LogInformation($"Update DailyEnergy Table at {date.ToString("dddd dd MMMM yyyy")}");
        message = $"به روز رسانی جدول انرژی روزانه تجهیزات در تاریخ {date.ToString("dddd dd MMMM yyyy")}";
        await _messageService.SendMessageAsync(message, MessageScope.UpdateEquipmentEnergyTable, MessageType.Success);
    }

    public async Task UpdateSubstationDailyEnergy(Guid substationId, DateTimeOffset date, bool update)
    {
        var substation = await _context.Substations
            .AsSplitQuery()
            .Include(i => i.Equipments)
            .SingleOrDefaultAsync(w => w.Id == substationId);

        if (substation is null)
        {
            _logger.LogWarning("No Substation Found");
            return;
        }

        foreach (var eq in substation.Equipments)
            await UpdateEquipmentDailyEnergy(eq.Id, date, update);

        _logger.LogInformation($"Update DailyEnergy of {substation.Name} at {date.ToString("dddd dd MMMM yyyy")}");
        string message = $"به روز رسانی جدول انرژی روزانه ایستگاه {substation.Name} در تاریخ {date.ToString("dddd dd MMMM yyyy")}";
        await _messageService.SendMessageAsync(message, MessageScope.UpdateEquipmentEnergyTable, MessageType.Success);
    }

    public async Task UpdateSubstationEquipmentEnergy(Guid substationId, DateTimeOffset date, bool update)
    {
        var substation = await _context.Substations
            .AsSplitQuery()
            .Include(i => i.Equipments)
            .SingleOrDefaultAsync(w => w.Id == substationId);

        if (substation is null)
        {
            _logger.LogWarning("No Substation Found");
            return;
        }

        foreach (var eq in substation.Equipments)
            await UpdateEquipmentEnergy(eq.Id, date, update);

        string message = $"Update EnergyProfile Table in {substation.Name} at Date: {date.ToString("dddd dd MMMM yyyy")}";
        _logger.LogInformation(message);
        message = $"به روز رسانی  جدول انرژی  ایستگاه {substation.Name} در تاریخ {date.ToString("dddd dd MMMM yyyy")}";
        await _messageService.SendMessageAsync(message, MessageScope.UpdateEquipmentEnergyTable, MessageType.Success);
    }

    public async Task ReadFailedMeters()
    {
        var meterFailures = await _context.MeterReadingFailures.ToListAsync();

        foreach (var mf in meterFailures)
        {
            await UpdateEquipmentEnergy(mf.EquipmentId, mf.RecordDate, false);
            await UpdateEquipmentDailyEnergy(mf.EquipmentId, mf.RecordDate, false);
            await CheckMeterFailure(mf.EquipmentId, mf.RecordDate);

            string message = $"به روز رسانی نواقص جدول انرژی تجهیزات در تاریخ {mf.RecordDate.ToString("dddd dd MMMM yyyy")}";
            _logger.LogInformation(message);
            await _messageService.SendMessageAsync(message, MessageScope.UpdateEquipmentEnergyTable, MessageType.Success);
        }
    }

    #region Private Memeber

    private async Task<IList<EnergyProfile>> GetEquipmentEnergy(Equipment equipment, DateTimeOffset date)
    {
        IList<EnergyProfile> equipmentEnergy = null;


        var meter = await FindEquipmentMeter(equipment.Id, date);

        if (meter is null)
        {
            string message = $"میتر برای تجهیز {equipment.Name} در ایستگاه {equipment.Substation.Name} پیدا نشد!!";

            _logger.LogWarning(message);
            await _messageService.SendMessageAsync(message, MessageScope.UpdateEquipmentEnergyTable, MessageType.Warning);
            return equipmentEnergy;
        }

        equipmentEnergy = new List<EnergyProfile>();

        var meterEnergys = await _meterService.GetMeterEnergy(date, meter.SerialNumber);

        foreach (var record in meterEnergys)
        {
            var id = _cryptoService.GetDeterministicGuid($"{equipment.Id}-{record.RecordDate.ToDateTimeOffset().Ticks}");
            var dEnergy = new EnergyProfile(id);

            dEnergy.EquipmentId = equipment.Id;
            dEnergy.RecordDate = record.RecordDate.ToDateTimeOffset();

            if (equipment.Reverse)
            {
                dEnergy.ImportWatt = record.EnergyActiveExport;
                dEnergy.ExportWatt = record.EnergyActiveImport;
                dEnergy.ImportVar = record.EnergyReactiveExport;
                dEnergy.ExportVar = record.EnergyReactiveImport;
            }
            else
            {
                dEnergy.ImportWatt = record.EnergyActiveImport;
                dEnergy.ExportWatt = record.EnergyActiveExport;
                dEnergy.ImportVar = record.EnergyReactiveImport;
                dEnergy.ExportVar = record.EnergyReactiveExport;
            }

            equipmentEnergy.Add(dEnergy);
        }

        return equipmentEnergy;
    }

    private async Task<DailyEnergy> GetEquipmentDailyEnergy(Equipment equipment, DateTimeOffset date)
    {
        int recordCount = 24;
        PersianCalendar pc = new PersianCalendar();

        var farvardin2 = pc.ToDateTime(pc.GetYear(date.Date), 1, 2, 0, 0, 0, 0);

        if (farvardin2.Date == date.Date)
            recordCount = 23;

        var start = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
        var end = start.AddDays(1);

        var sumEnergy = await _context.EnergyProfiles
            .AsNoTracking()
            .Where(w => w.EquipmentId == equipment.Id && w.RecordDate >= start && w.RecordDate < end)
            .ToListAsync();

        DailyEnergy dailyEnergy = null;

        if (sumEnergy.Count == recordCount)
        {
            var id = _cryptoService.GetDeterministicGuid($"{equipment.Id}-{date.Date.Ticks}");
            dailyEnergy = new DailyEnergy(id);

            dailyEnergy.EquipmentId = equipment.Id;
            dailyEnergy.RecordDate = date.Date;
            dailyEnergy.ImportWatt = sumEnergy.Sum(s => s.ImportWatt);
            dailyEnergy.ExportWatt = sumEnergy.Sum(s => s.ExportWatt);
            dailyEnergy.ImportVar = sumEnergy.Sum(s => s.ImportVar);
            dailyEnergy.ExportVar = sumEnergy.Sum(s => s.ExportVar);
        }

        return dailyEnergy;
    }

    private async Task CheckMeterFailure(Guid equipmentId, DateTimeOffset date)
    {
        var equipment = await _context.Equipments.FirstOrDefaultAsync(x => x.Id == equipmentId);

        if (equipment is null)
            return;

        PersianCalendar pc = new PersianCalendar();
        MeterReadingFailure mrf;
        int recordCount = 24;
        string message;

        var farvardin2 = pc.ToDateTime(pc.GetYear(date.Date), 1, 2, 0, 0, 0, 0);

        if (farvardin2.Date == date.Date)
            recordCount = 23;

        var s = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
        var e = s.AddDays(1);

        var records = _context.EnergyProfiles
            .AsNoTracking()
            .Where(w => w.EquipmentId == equipment.Id && w.RecordDate >= s && w.RecordDate < e)
            .Count();

        var id = _cryptoService.GetDeterministicGuid($"{equipment.Id}-{date.Date.Ticks}");

        if (records < recordCount && !_context.MeterReadingFailures.Any(e => e.Id == id))
        {
            mrf = new MeterReadingFailure(id);
            mrf.RecordDate = date.Date;
            mrf.EquipmentId = equipment.Id;
            await _context.MeterReadingFailures.AddAsync(mrf);
        }
        else if (_context.MeterReadingFailures.Any(e => e.Id == id))
        {
            mrf = await _context.MeterReadingFailures.FindAsync(id);
            _context.MeterReadingFailures.Remove(mrf);
        }

        try
        {
            await _unitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, $"CheckMeterFailure DbUpdateException Error:{ex.Message}");
            message = "خطای به روز رسانی جدول نواقص میتر";
            await _messageService.SendMessageAsync(message, MessageScope.UpdateEquipmentEnergyTable, MessageType.Error);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"CheckMeterFailure Exception Error:{ex.Message}");
            message = "خطای عمومی به روز رسانی جدول نواقص میتر";
            await _messageService.SendMessageAsync(message, MessageScope.UpdateEquipmentEnergyTable, MessageType.Error);
        }
    }

    private async Task CheckMetersFailure(DateTimeOffset date)
    {
        PersianCalendar pc = new PersianCalendar();
        MeterReadingFailure mrf;
        int recordCount = 24;
        string message;

        var s = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
        var e = s.AddDays(1);

        var farvardin2 = pc.ToDateTime(pc.GetYear(date.Date), 1, 2, 0, 0, 0, 0);

        if (farvardin2.Date == date.Date)
            recordCount = 23;

        var equipments = await _context.Equipments.ToListAsync();

        foreach (var equipment in equipments)
        {
            var records = _context.EnergyProfiles
                .AsNoTracking()
                .Where(w => w.EquipmentId == equipment.Id && w.RecordDate >= s && w.RecordDate < e)
                .Count();

            var id = _cryptoService.GetDeterministicGuid($"{equipment.Id}-{date.Date.Ticks}");

            if (records < recordCount)
            {
                if (_context.MeterReadingFailures.Any(e => e.Id == id))
                    continue;

                mrf = new MeterReadingFailure(id);
                mrf.RecordDate = date.Date;
                mrf.EquipmentId = equipment.Id;
                await _context.MeterReadingFailures.AddAsync(mrf);
            }
            else if (_context.MeterReadingFailures.Any(e => e.Id == id))
            {
                mrf = await _context.MeterReadingFailures.FindAsync(id);
                _context.MeterReadingFailures.Remove(mrf);
            }
        }

        try
        {
            await _unitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, $"CheckMeterFailures DbUpdateException Error:{ex.Message}");
            message = "خطای به روز رسانی جدول نواقص میتر";
            await _messageService.SendMessageAsync(message, MessageScope.UpdateEquipmentEnergyTable, MessageType.Error);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"CheckMeterFailures Exception Error:{ex.Message}");
            message = "خطای عمومی به روز رسانی جدول نواقص میتر";
            await _messageService.SendMessageAsync(message, MessageScope.UpdateEquipmentEnergyTable, MessageType.Error);
        }
    }

    private async Task<EquipmentMeter> FindEquipmentMeter(Guid equipmentId, DateTimeOffset date)
    {
        var equipment = await _context.Equipments.SingleOrDefaultAsync(w => w.Id == equipmentId);
        return equipment.GetCurrentMeter(date);
    }

    #endregion Private Memeber
}
