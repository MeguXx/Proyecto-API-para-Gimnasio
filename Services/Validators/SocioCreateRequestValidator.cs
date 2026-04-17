using FluentValidation;
using GYMAPI.Controllers;

namespace GYMAPI.Services.Validators;

public class SocioCreateRequestValidator : AbstractValidator<SociosController.SocioCreateRequest>
{
    public SocioCreateRequestValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.AlturaCm).GreaterThan(0).When(x => x.AlturaCm.HasValue);
        RuleFor(x => x.PesoKg).GreaterThan(0).When(x => x.PesoKg.HasValue);
    }
}