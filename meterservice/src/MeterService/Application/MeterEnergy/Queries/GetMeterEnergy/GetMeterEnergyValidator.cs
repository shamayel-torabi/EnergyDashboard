using FluentValidation;
namespace MeterService.Application.MeterEnergy.Queries;

public sealed class GetMeterEnergyValidator : AbstractValidator<GetMeterEnergyQuery>
{
    public GetMeterEnergyValidator()
    {
        RuleFor(x => x.SerialNumber)
            .NotEmpty().WithMessage("SerialNumber is required.")
            .MaximumLength(50).WithMessage("SerialNumber Length must be less than 50 character.");

        RuleFor(x => x.RecordDate)
            .LessThan(DateTime.Now).WithMessage("Date must be Less than Current Day")
            .NotNull().WithMessage("RecordDate is required");
    }
}
