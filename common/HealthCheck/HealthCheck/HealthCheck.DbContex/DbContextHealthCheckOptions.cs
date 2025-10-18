using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

#nullable disable
#pragma warning disable CS8632 

namespace HealthCheck;

public sealed class DbContextHealthCheckOptions<TContext> where TContext : DbContext
{
    public Func<TContext, CancellationToken, Task<bool>>? CustomTestQuery { get; set; }
}

#pragma warning restore CS8632
