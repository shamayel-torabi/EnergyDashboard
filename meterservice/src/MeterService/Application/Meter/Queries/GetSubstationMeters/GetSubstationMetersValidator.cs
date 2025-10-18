using FluentValidation;

namespace MeterService.Application.Meters.Queries;

public sealed class GetSubstationMetersValidator : AbstractValidator<GetSubstationMetersQuery>
{
    public GetSubstationMetersValidator()
    {
        RuleFor(x => x.SubstationId).NotNull().WithMessage("SubstationId is required.");
    }
}
