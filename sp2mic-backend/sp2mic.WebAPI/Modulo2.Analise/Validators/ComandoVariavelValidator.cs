using FluentValidation;
using sp2mic.WebAPI.Domain.Entities;

namespace sp2mic.WebAPI.Modulo2.Analise.Validators;

public class ComandoVariavelValidator : AbstractValidator<ComandoVariavel>
{
  public ComandoVariavelValidator ()
  { // TODO essa restrição ainda não está no banco
    RuleFor(x => x.NuOrdem).NotNull().NotEmpty()
     .WithMessage("Variable execution order must be informed.");
  }
}
