using System.Globalization;
using Microsoft.EntityFrameworkCore;
using MeterService.Application.Interfaces;
using MeterService.Domain.Entities;
using MediatR;

#nullable disable

namespace MeterService.Application.Meters.Commands;

public sealed record UploadMeterFilesCommand : IRequest<bool>
{
    public List<IFormFile> Files { get; set; }
}

public sealed class UploadMeterFilesCommandHandler : IRequestHandler<UploadMeterFilesCommand,bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ICryptoService _cryptoService;

    private string[] formats = {
        // Basic formats
        @"yyyy/MM/dd\THH:mm:ss",
        @"MM/dd/yyyy\THH:mm:ss",
        @"yyyy/MM/dd\THH:mm",
        @"MM/dd/yyyy\THH:mm",
        @"yyyy/MM/dd\THH",
        @"MM/dd/yyyy\THH",
        @"dd/MM/yyyy HH:mm"
    };

    private enum RecordInterval
    {
        Quarter = 1,
        Hour = 2
    }

    public UploadMeterFilesCommandHandler(IApplicationDbContext context, ICryptoService cryptoService)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _cryptoService = cryptoService ?? throw new ArgumentNullException(nameof(cryptoService));
    }

    public async Task<bool> Handle(UploadMeterFilesCommand request, CancellationToken cancellationToken)
    {
        string text;
        int nFile = request.Files.Count();
        int i = 0, progress = 0;


        DateTime startTime = DateTime.Now;
        text = $"بارگذاری اطلاعات {request.Files.Count()} فایل در بانک اطلاعاتی.";
        //await _messageService.SendMessage(text, MessageType.Info, MessageScope.UploadMeterFile);

        foreach (var file in request.Files)
        {
            if (file.Length > 0)
            {
                string fileName = Path.GetFileName(file.FileName);
                string extension = Path.GetExtension(file.FileName);

                text = $"فایل {fileName} باز شد";
                //await _messageService.SendMessage(text, MessageType.Info, MessageScope.UploadMeterFile);

                RecordInterval interval = await CheckTimeInterval(file);

                if (extension.ToLower() == ".ls2".ToLower())
                {
                    if (interval == RecordInterval.Quarter)
                        await AddLS2RecordQuarterlyAsync(file);
                    else
                        await AddLS2RecordHourlyAsync(file);
                }
                else if (extension.ToLower() == ".csv".ToLower())
                    await AddCSVRecordAsync(file);

                i++;
                progress = 100 * i / nFile;

                //await _messageService.SendProgress(progress, MessageScope.UploadMeterFile);
            }
        }
        DateTime endTime = DateTime.Now;
        TimeSpan ts = startTime - endTime;

        text = $"به روز رسانی در مدت زمان {ts.ToString(@"hh\:mm\:ss")} به پایان رسید";
        //await _messageService.SendMessage(text, MessageType.Info, MessageScope.UploadMeterFile);

        return await Task.FromResult(true);
    }

    private async Task AddLS2RecordQuarterlyAsync(IFormFile file)
    {
        int recNo = 0;
        string str;
        string fName = file.FileName;
        char[] delimiterChars = { ' ', ',', '\t' };

        using (var reader = new StreamReader(file.OpenReadStream()))
        {
            await reader.ReadLineAsync();
            await reader.ReadLineAsync();
            string stime = await reader.ReadLineAsync();
            string[] stoken = stime.Split('=');
            string[] time = stoken[1].Split(' ');
            stime = time[0] + "T" + time[1];
            DateTime startTime;

            if (!DateTime.TryParseExact(stime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out startTime))
            {
                str = string.Format("خطا در تاریخ قرائت فایل!!");
                //await _messageService.SendMessage(str, MessageType.Info, MessageScope.UploadMeterFile);
                return;
                //return Ok(sb.ToString());
            }

            await reader.ReadLineAsync();
            await reader.ReadLineAsync();
            await reader.ReadLineAsync();
            string serialNumber = Path.GetFileNameWithoutExtension(file.FileName);

            var meter = await _context.Meters.Where(w => w.SerialNumber == serialNumber).SingleOrDefaultAsync();
            if (meter == null)
            {
                str = $"کنتور به شماره سریال {serialNumber} در بانک اطلاعاتی وجود ندارد.";
                //await _messageService.SendMessage(str, MessageType.Info, MessageScope.UploadMeterFile);
                return;
            }

            int recordNo = 0;
            int count = 0;
            decimal ImportWhTotal = 0m;
            decimal ExportWhTotal = 0m;
            decimal ImportVarhTotal = 0m;
            decimal ExportVarhTotal = 0m;
            bool start = true;

            while (!reader.EndOfStream)
            {
                decimal ImportWatt, ImportVar, ExportWatt, ExportVar, ImportTotalWatt, ExportTotalWatt;
                DateTime dt;
                bool dataConversionError = false;

                string line = await reader.ReadLineAsync();
                string[] token = line.Split(delimiterChars);

                if (!int.TryParse(token[0], out recordNo))
                    dataConversionError = true;

                string date = token[1] + "T" + token[2];
                if (!DateTime.TryParseExact(date, formats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dt))
                    dataConversionError = true;

                if (dt.Minute != 15 && start)
                    continue;

                start = false;

                if (!decimal.TryParse(token[3], out ExportWatt))
                    dataConversionError = true;

                if (!decimal.TryParse(token[4], out ImportWatt))
                    dataConversionError = true;

                if (!decimal.TryParse(token[5], out ExportVar))
                    dataConversionError = true;

                if (!decimal.TryParse(token[6], out ImportVar))
                    dataConversionError = true;

                if (!decimal.TryParse(token[7], out ImportTotalWatt))
                    dataConversionError = true;

                if (!decimal.TryParse(token[8], out ExportTotalWatt))
                    dataConversionError = true;

                if (dataConversionError)
                {
                    str = $"خطای تبدیل در فایل {fName} در رکورد {recordNo} وجود دارد";
                    //await _messageService.SendMessage(str, MessageType.Info, MessageScope.UploadMeterFile);
                }
                else
                {
                    count++;

                    ImportWhTotal += ImportWatt;
                    ExportWhTotal += ExportWatt;
                    ImportVarhTotal += ImportVar;
                    ExportVarhTotal += ExportVar;

                    if (count == 4)
                    {
                        var guid = _cryptoService.GetDeterministicGuid($"{meter.SerialNumber}-{dt.Ticks}");

                        if (_context.MeterEnergys.Any(e => e.MeterEnergyId == guid))
                            continue;

                        var mId = await GetMeterId(meter.SerialNumber);
                        MeterEnergyEntity dEnergy = new MeterEnergyEntity();
                        dEnergy.MeterEnergyId = guid;
                        dEnergy.MeterId = mId;
                        dEnergy.RecordDate = dt;
                        dEnergy.EnergyActiveExport = ExportWhTotal;
                        dEnergy.EnergyActiveImport = ImportWhTotal;
                        dEnergy.EnergyReactiveExport = ExportVarhTotal;
                        dEnergy.EnergyReactiveImport = ImportVarhTotal;


                        await _context.MeterEnergys.AddAsync(dEnergy);
                        recNo++;

                        ImportWhTotal = 0m;
                        ExportWhTotal = 0m;
                        ImportVarhTotal = 0m;
                        ExportVarhTotal = 0m;
                        count = 0;
                    }
                }
            }
        }


        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            str = $"خطا در به روز رسانی فایل {fName} وجود دارد";
            //await _messageService.SendMessage(str, MessageType.Error, MessageScope.UploadMeterFile);
        }
        finally
        {
            str = $"محتوای فایل {fName} خوانده شد. تعداد {recNo} رکورد به بانک افزوده شد.";
            //await _messageService.SendMessage(str, MessageType.Info, MessageScope.UploadMeterFile);
        }

        return;
    }

    private async Task AddLS2RecordHourlyAsync(IFormFile file)
    {
        int recNo = 0;
        string str;
        string fName = file.FileName;
        char[] delimiterChars = { ' ', ',', '\t' };

        using (var reader = new StreamReader(file.OpenReadStream()))
        {
            await reader.ReadLineAsync();
            await reader.ReadLineAsync();
            string stime = await reader.ReadLineAsync();
            string[] stoken = stime.Split('=');
            string[] time = stoken[1].Split(' ');
            stime = time[0] + "T" + time[1];
            DateTime startTime;

            if (!DateTime.TryParseExact(stime, formats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out startTime))
            {
                str = string.Format("خطا در تاریخ قرائت فایل!!");
                //await _messageService.SendMessage(str, MessageType.Info, MessageScope.UploadMeterFile);
                return;
            }

            await reader.ReadLineAsync();
            await reader.ReadLineAsync();
            await reader.ReadLineAsync();
            string serialNumber = Path.GetFileNameWithoutExtension(file.FileName);

            var meter = await _context.Meters.Where(w => w.SerialNumber == serialNumber).SingleOrDefaultAsync();
            if (meter == null)
            {
                str = $"کنتور به شماره سریال {serialNumber} در بانک اطلاعاتی وجود ندارد";
                //await _messageService.SendMessage(str, MessageType.Info, MessageScope.UploadMeterFile);
                return;
            }

            int recordNo = 0;

            while (!reader.EndOfStream)
            {
                decimal ImportWatt, ImportVar, ExportWatt, ExportVar;
                DateTime dt;
                bool dataConversionError = false;

                string line = await reader.ReadLineAsync();
                string[] token = line.Split(delimiterChars);

                if (!int.TryParse(token[0], out recordNo))
                    dataConversionError = true;

                string date = token[1] + "T" + token[2];
                if (!DateTime.TryParseExact(date, formats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dt))
                    dataConversionError = true;

                if (!decimal.TryParse(token[3], out ExportWatt))
                    dataConversionError = true;

                if (!decimal.TryParse(token[4], out ImportWatt))
                    dataConversionError = true;

                if (!decimal.TryParse(token[5], out ExportVar))
                    dataConversionError = true;

                if (!decimal.TryParse(token[6], out ImportVar))
                    dataConversionError = true;

                if (dataConversionError)
                {
                    str = $"خطای تبدیل در فایل {fName} در رکورد {recordNo} وجود دارد";
                    //await _messageService.SendMessage(str, MessageType.Info, MessageScope.UploadMeterFile);
                }
                else
                {
                    var guid = _cryptoService.GetDeterministicGuid($"{meter.SerialNumber}-{dt.Ticks}");

                    if (_context.MeterEnergys.Any(e => e.MeterEnergyId == guid))
                        continue;

                    var mId = await GetMeterId(meter.SerialNumber);
                    MeterEnergyEntity dEnergy = new MeterEnergyEntity();
                    dEnergy.MeterEnergyId = guid;
                    dEnergy.MeterId = mId;
                    dEnergy.RecordDate = dt;
                    dEnergy.EnergyActiveExport = ExportWatt;
                    dEnergy.EnergyActiveImport = ImportWatt;
                    dEnergy.EnergyReactiveExport = ExportVar;
                    dEnergy.EnergyReactiveImport = ImportVar;


                    await _context.MeterEnergys.AddAsync(dEnergy);
                    recNo++;
                }
            }
        }


        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            str = $"خطا در به روز رسانی فایل {fName} وجود دارد";
            //await _messageService.SendMessage(str, MessageType.Error, MessageScope.UploadMeterFile);
        }
        finally
        {
            str = $"محتوای فایل {fName} خوانده شد. تعداد {recNo} رکورد به بانک افزوده شد.";
            //await _messageService.SendMessage(str, MessageType.Info, MessageScope.UploadMeterFile);
        }

        return;
    }

    private async Task AddCSVRecordAsync(IFormFile file)
    {
        int recNo = 0;
        string str;
        string fName = file.FileName;
        char[] delimiterChars = { ';' };

        using (var reader = new StreamReader(file.OpenReadStream()))
        {

            string meterId = Path.GetFileNameWithoutExtension(file.FileName);
            string[] f = meterId.Split('_');
            meterId = f[1];
            string sd = f[3];
            string year = sd.Substring(0, 4);
            string month = sd.Substring(4, 2);
            string day = sd.Substring(6, 2);

            string stime = day + "/" + month + "/" + year + " 00:00";

            DateTime startTime;
            if (!DateTime.TryParseExact(stime, formats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out startTime))
            {
                str = $"خطا در تاریخ قرائت فایل!!";
                //await _messageService.SendMessage(str, MessageType.Info, MessageScope.UploadMeterFile);
                return;
            }

            var meter = await _context.Meters.Where(w => w.SerialNumber == meterId).SingleOrDefaultAsync();
            if (meter == null)
            {
                str = $"تجهیزی با کنتور به شماره سریال {meterId} در بانک اطلاعاتی وجود ندارد";
                //await _messageService.SendMessage(str, MessageType.Info, MessageScope.UploadMeterFile);
                return;
            }


            int recordNo = 0;
            int count = 0;
            decimal ImportWhTotal = 0m;
            decimal ExportWhTotal = 0m;
            decimal ImportVarhTotal = 0m;
            decimal ExportVarhTotal = 0m;

            string line = "";
            while (!line.StartsWith("Date"))
                line = reader.ReadLine();

            while (!reader.EndOfStream)
            {
                recordNo++;

                line = reader.ReadLine();

                string[] token = line.Split(delimiterChars);

                if (token.Length < 7)
                    break;

                decimal ImportWatt, ExportWatt, ImportVar, ExportVar, ImportTotalWatt, ExportTotalWatt;

                DateTime dt;
                bool dataConversionError = false;

                string date = token[0];
                if (!DateTime.TryParseExact(date, formats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dt))
                    dataConversionError = true;

                if (!decimal.TryParse(token[1].Replace(" ", String.Empty), out ImportWatt))
                    dataConversionError = true;

                if (!decimal.TryParse(token[2].Replace(" ", String.Empty), out ExportWatt))
                    dataConversionError = true;

                if (!decimal.TryParse(token[3].Replace(" ", String.Empty), out ImportVar))
                    dataConversionError = true;

                if (!decimal.TryParse(token[4].Replace(" ", String.Empty), out ExportVar))
                    dataConversionError = true;

                if (!decimal.TryParse(token[5].Replace(" ", String.Empty), out ImportTotalWatt))
                    dataConversionError = true;

                if (!decimal.TryParse(token[6].Replace(" ", String.Empty), out ExportTotalWatt))
                    dataConversionError = true;


                if (dataConversionError)
                {
                    str = $"خطای تبدیل در فایل {fName} در رکورد {recordNo} وجود دارد";
                    //await _messageService.SendMessage(str, MessageType.Info, MessageScope.UploadMeterFile);
                }
                else
                {
                    count++;

                    ImportWhTotal += ImportWatt * 1000;
                    ExportWhTotal += ExportWatt * 1000;
                    ImportVarhTotal += ImportVar * 1000;
                    ExportVarhTotal += ExportVar * 1000;

                    if (count == 4)
                    {
                        var guid = _cryptoService.GetDeterministicGuid($"{meter.SerialNumber}-{dt.Ticks}");

                        if (_context.MeterEnergys.Any(e => e.MeterEnergyId == guid))
                            continue;
                        var mId = await GetMeterId(meter.SerialNumber);
                        MeterEnergyEntity dEnergy = new MeterEnergyEntity();
                        dEnergy.MeterEnergyId = guid;
                        dEnergy.MeterId = mId;
                        dEnergy.RecordDate = dt;
                        dEnergy.EnergyActiveExport = ExportWhTotal;
                        dEnergy.EnergyActiveImport = ImportWhTotal;
                        dEnergy.EnergyReactiveExport = ExportVarhTotal;
                        dEnergy.EnergyReactiveImport = ImportVarhTotal;

                        await _context.MeterEnergys.AddAsync(dEnergy);
                        recNo++;

                        ImportWhTotal = 0m;
                        ExportWhTotal = 0m;
                        ImportVarhTotal = 0m;
                        ExportVarhTotal = 0m;
                        count = 0;
                    }
                }
            }
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            str = $"خطا در  به روز رسانی فایل {fName} وجود دارد";
            //await _messageService.SendMessage(str, MessageType.Error, MessageScope.UploadMeterFile);
        }
        finally
        {
            str = $"محتوای فایل {fName} خوانده شد. تعداد {recNo} رکورد به بانک افزوده شد.";
            //await _messageService.SendMessage(str, MessageType.Info, MessageScope.UploadMeterFile);
        }

        return;
    }

    private async Task<RecordInterval> CheckTimeInterval(IFormFile file)
    {
        RecordInterval ret = RecordInterval.Hour;

        char[] delimiterChars = { ' ', ',', '\t' };


        using (var reader = new StreamReader(file.OpenReadStream()))
        {
            await reader.ReadLineAsync();
            await reader.ReadLineAsync();
            string stime = await reader.ReadLineAsync();
            string[] stoken = stime.Split('=');
            string[] time = stoken[1].Split(' ');
            stime = time[0] + "T" + time[1];


            await reader.ReadLineAsync();
            await reader.ReadLineAsync();
            await reader.ReadLineAsync();
            string meterId = Path.GetFileNameWithoutExtension(file.FileName);


            bool start = true;

            while (!reader.EndOfStream)
            {
                DateTimeOffset dt;

                string line = await reader.ReadLineAsync();
                string[] token = line.Split(delimiterChars);

                string date = token[1] + "T" + token[2];
                DateTimeOffset.TryParseExact(date, formats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dt);

                if (dt.Minute != 15 && start)
                    continue;

                ret = RecordInterval.Quarter;
                break;
            }
        }

        return ret;
    }

    private async Task<int> GetMeterId(string serialNumber)
    {
        var meter = await _context.Meters.SingleOrDefaultAsync(x => x.SerialNumber == serialNumber);
        if (meter == null)
            return 0;
        return meter.MeterId;
    }
}
