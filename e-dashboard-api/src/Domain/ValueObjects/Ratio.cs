using Domain.Common;

namespace EnergyDashboard.Domain.ValueObjects;

public class Ratio : ValueObject
{
    public float Value => Primary / Secondary;
    public ushort Primary { get; }
    public ushort Secondary { get; }

    private Ratio(ushort primary, ushort secondary)
    {
        Primary = primary;
        Secondary = secondary;
    }

    public static Ratio Create(string ratioStr)
    {
        string[] words = ratioStr.Split('/');

        if (words.Length != 2)
            throw new ArgumentException("Ratio format is not valid");

        if (!ushort.TryParse(words[0], out ushort primary))
            throw new ArgumentException("Ratio Primary is not valid number");

        if (!ushort.TryParse(words[1], out ushort secondary))
            throw new ArgumentException("Ratio Secondary is not valid number");

        if (secondary == 0)
            throw new ArgumentException("Ratio Secondary cannot be zero");

        Ratio ratio= new Ratio(primary, secondary);
        return ratio;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Primary;
        yield return Secondary;
    }
}
