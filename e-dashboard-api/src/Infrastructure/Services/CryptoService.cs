using System;
using System.Text;
using System.Security.Cryptography;
using EnergyDashboard.Application.Interfaces;

namespace EnergyDashboard.Infrastructure.Services;

public class CryptoService: ICryptoService
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
