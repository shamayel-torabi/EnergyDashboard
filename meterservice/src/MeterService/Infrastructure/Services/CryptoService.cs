using System.Text;
using System.Security.Cryptography;
using MeterService.Application.Interfaces;

namespace MeterService.Infrastructure.Services;

public sealed class CryptoService: ICryptoService
{
    private readonly MD5 _provider;

    public CryptoService()
    {
        _provider = MD5.Create();
    }

    public Guid GetDeterministicGuid(string input)
    {
        var hashBytes = _provider.ComputeHash(Encoding.Default.GetBytes(input));
        return new Guid(hashBytes);
    }
}
