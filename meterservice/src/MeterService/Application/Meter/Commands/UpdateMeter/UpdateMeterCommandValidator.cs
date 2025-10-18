using FluentValidation;

namespace MeterService.Application.Meters.Commands;

public sealed class UpdateMeterCommandValidator : AbstractValidator<UpdateMeterCommand>
{
    public UpdateMeterCommandValidator()
    {
        RuleFor(x => x.SerialNumber)
            .NotEmpty().WithMessage("SerialNumber is required.")
            .MaximumLength(50).MaximumLength(50).WithMessage("SerialNumber Length must be less than 50 character.");

        RuleFor(x => x.Name).MaximumLength(100).WithMessage("Name Length must be less than 100 character.");
        RuleFor(x => x.StationName).MaximumLength(100).WithMessage("StationName Length must be less than 100 character.");
    }
}
