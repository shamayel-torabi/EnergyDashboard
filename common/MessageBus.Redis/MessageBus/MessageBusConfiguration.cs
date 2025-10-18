
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MessageBus;

public class MessageBusConfiguration
{
    public List<Assembly> AssembliesToRegister { get; } = new();

    public string? ConnectionString { get; set; }

    public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Singleton;


    public MessageBusConfiguration RegisterServicesFromAssemblyContaining<T>()
    => RegisterServicesFromAssemblyContaining(typeof(T));

    public MessageBusConfiguration RegisterServicesFromAssemblyContaining(Type type)
    => RegisterServicesFromAssembly(type.Assembly);

    public MessageBusConfiguration RegisterServicesFromAssembly(Assembly assembly)
    {
        AssembliesToRegister.Add(assembly);
        return this;
    }

    public MessageBusConfiguration RegisterServicesFromAssemblies(params Assembly[] assemblies)
    {
        AssembliesToRegister.AddRange(assemblies);
        return this;
    }
}