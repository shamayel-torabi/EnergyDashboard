
namespace MeterFailService.Application.Interfaces;

public interface ICryptoService
{
    Guid GetDeterministicGuid(string serialNumber, long tick);
}
