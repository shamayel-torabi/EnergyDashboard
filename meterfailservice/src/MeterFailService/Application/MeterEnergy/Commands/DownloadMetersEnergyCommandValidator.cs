using FluentValidation;

namespace MeterFailService.Application.MeterEnergy.Commands;

public sealed class DownloadMetersEnergyCommandValidator : AbstractValidator<DownloadMetersEnergyCommand>
{
    public DownloadMetersEnergyCommandValidator()
    {
        RuleFor(v => v.RecordDate)
            .NotEmpty().WithMessage("Date must be valid")
            .LessThan(DateTime.Now).WithMessage("Date must be Less than Current Day");
    }
}
