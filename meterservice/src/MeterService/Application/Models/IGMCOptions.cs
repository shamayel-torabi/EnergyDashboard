
namespace MeterService.Application.Models;

public sealed class IGMCOptions
{
    //public string Username { get; set; }
    //public string Password { get; set; }
    public int Timeout { get; set; }
    public int RequestDays { get; set; }
    public int MeterSaveCount  { get; set; }
    public string ModamUrl { get; set; }
}
