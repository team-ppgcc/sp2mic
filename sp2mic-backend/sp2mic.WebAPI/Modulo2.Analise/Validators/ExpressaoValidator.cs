using FluentValidation;
using sp2mic.WebAPI.Domain.Entities;

namespace sp2mic.WebAPI.Modulo2.Analise.Validators;

public class ExpressaoValidator : AbstractValidator<Expressao>
{
  public ExpressaoValidator ()
  { // TODO essa restrição ainda não está no banco
    RuleFor(x => x.NuOrdemExecucao).NotNull().NotEmpty()
     .WithMessage("Expression execution order must be informed.");

    RuleFor(x => x.CoTipoDadoRetorno).NotNull().NotEmpty()
     .WithMessage("Expression return data type must be informed.");
  }
}
