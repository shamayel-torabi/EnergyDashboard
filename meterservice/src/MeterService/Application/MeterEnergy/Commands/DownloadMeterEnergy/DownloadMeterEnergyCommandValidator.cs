using FluentValidation;

namespace MeterService.Application.MeterEnergy.Commands;

public sealed class DownloadMeterEnergyCommandValidator : AbstractValidator<DownloadMeterEnergyCommand>
{
    public DownloadMeterEnergyCommandValidator()
    {
        RuleFor(v => v.EndDate)
            .NotEmpty().WithMessage("Date must be valid")
            .LessThan(DateTime.Now).WithMessage("Date must be Less than Current Day");

        RuleFor(x => x.SerialNumber)
            .NotEmpty().WithMessage("SerialNumber is required.")
            .MaximumLength(50).WithMessage("SerialNumber Length must be less than 50 character.");
    }
}
