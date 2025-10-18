using FluentValidation;

namespace MeterService.Application.MeterEnergy.Commands;

public sealed class DownloadMetersEnergyCommandValidator : AbstractValidator<DownloadMetersEnergyCommand>
{
    public DownloadMetersEnergyCommandValidator()
    {
        RuleFor(v => v.EndDate)
            .NotEmpty().WithMessage("Date must be valid")
            .LessThan(DateTime.Now).WithMessage("Date must be Less than Current Day");
    }
}
