using System.Text;
using System.Security.Cryptography;
using MeterFailService.Application.Interfaces;

namespace MeterFailService.Infrastructure.Services;

public sealed class CryptoService: ICryptoService
{
    private readonly MD5 _provider;

    public CryptoService()
    {
        _provider = MD5.Create();
    }

    Guid ICryptoService.GetDeterministicGuid(string serialNumber, long tick)
    {
        string input = $"{serialNumber}-{tick}";
        byte[] hashBytes = _provider.ComputeHash(Encoding.Default.GetBytes(input));
        return new Guid(hashBytes);
    }
}
