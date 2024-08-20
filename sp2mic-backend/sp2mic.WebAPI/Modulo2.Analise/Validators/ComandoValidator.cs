using FluentValidation;
using sp2mic.WebAPI.Domain.Entities;

namespace sp2mic.WebAPI.Modulo2.Analise.Validators;

public class ComandoValidator : AbstractValidator<Comando>
{
  public ComandoValidator ()
  {
    RuleFor(x => x.CoTipoComando).NotNull().NotEmpty()
     .WithMessage("Tipo da instrução do comando deve ser preenchida");
  }
}
