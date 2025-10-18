using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace MeterService.Application.Interfaces;

public interface ICryptoService
{
    Guid GetDeterministicGuid(string input);
}
