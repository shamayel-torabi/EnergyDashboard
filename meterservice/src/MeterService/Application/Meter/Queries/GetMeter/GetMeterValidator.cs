using FluentValidation;

namespace MeterService.Application.Meters.Queries;

public sealed class GetMeterValidator : AbstractValidator<GetMeterQuery>
{
    public GetMeterValidator()
    {
        RuleFor(x => x.SerialNumber)
            .NotEmpty().WithMessage("SerialNumber is required.")
            .MaximumLength(50).WithMessage("SerialNumber Length must be less than 50 character.");
    }
}
