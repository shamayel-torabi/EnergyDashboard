using FluentValidation;

namespace EnergyDashboard.Application.EnergyTariffs.Commands;

public class CreateEnergyTariffCommandValidator : AbstractValidator<CreateEnergyTariffCommand>
{
    public CreateEnergyTariffCommandValidator()
    {
        RuleFor(e => e.Id).NotEmpty();
        RuleFor(x => x.Name).MaximumLength(50).WithMessage("Name Length must be less than 50 character.");
    }
}
