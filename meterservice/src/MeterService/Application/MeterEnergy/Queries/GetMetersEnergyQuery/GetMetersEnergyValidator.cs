using FluentValidation;
namespace MeterService.Application.MeterEnergy.Queries;

public sealed class GetMetersEnergyValidator : AbstractValidator<GetMetersEnergyQuery>
{
    public GetMetersEnergyValidator()
    {
        RuleFor(x => x.RecordDate)
            .LessThanOrEqualTo(DateTime.Now).WithMessage("Date must be Less than Current Day")
            .NotNull().WithMessage("RecordDate is required");
    }
}
