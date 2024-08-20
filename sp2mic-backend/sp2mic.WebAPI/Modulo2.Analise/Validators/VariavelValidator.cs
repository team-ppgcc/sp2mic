using FluentValidation;
using sp2mic.WebAPI.Domain.Entities;

namespace sp2mic.WebAPI.Modulo2.Analise.Validators;

public class VariavelValidator : AbstractValidator<Variavel>
{
  public VariavelValidator ()
  {
    RuleFor(x => x.NoVariavel).NotNull().NotEmpty()
     .WithMessage("Variable Name must be informed.");
  }
}
