
using Domain.Common;

namespace EnergyDashboard.Domain.ValueObjects;

public class SerialNumber: ValueObject
{
    public string Value { get; private set; }

    private SerialNumber(string value)
    {
        Value = value;
    }

    public static SerialNumber Create(string serialNumber)
    {
        Ensure.NotEmpty(serialNumber, "SerialNumber cannot be null", nameof(serialNumber));

        if(serialNumber.Length > 50)
            throw new ArgumentException("Length of SerialNumber cannot be greather than 50 character", nameof(serialNumber));

        return new SerialNumber(serialNumber);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
