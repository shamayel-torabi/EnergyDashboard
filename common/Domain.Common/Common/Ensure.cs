
namespace Domain.Common;

public static class Ensure
{
    public static void NotEmpty(string value, string message, string argumentName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(message, argumentName);
        }
    }

    public static void NotEmpty(Guid value, string message, string argumentName)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException(message, argumentName);
        }
    }

    public static void NotEmpty(DateTime value, string message, string argumentName)
    {
        if (value == default)
        {
            throw new ArgumentException(message, argumentName);
        }
    }

    public static void NotNull<T>(T value, string message, string argumentName)
        where T : class
    {
        if (value is null)
        {
            throw  new ArgumentNullException(argumentName, message);
        }
    }
    public static void DateInRange(DateTimeOffset startDate, DateTimeOffset endDate, string message, string argumentName)
    {
        if (startDate == default)
        {
            throw new ArgumentNullException(message, nameof(startDate));
        }

        if (endDate == default)
        {
            throw new ArgumentNullException(message, nameof(endDate));
        }

        if (startDate >= endDate)
        {
            throw new ArgumentException(message, argumentName);
        }
    }
}
