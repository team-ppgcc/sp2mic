using FluentValidation;
using sp2mic.WebAPI.Domain.Entities;

namespace sp2mic.WebAPI.Modulo2.Analise.Validators;

public class OperandoValidator : AbstractValidator<Operando>
{
  public OperandoValidator ()
  {
    RuleFor(x => x.CoTipoOperando).NotNull().NotEmpty().WithMessage("Type of operands must be informed");
  }
}
