using FluentValidation;
using sp2mic.WebAPI.Domain.Entities;

namespace sp2mic.WebAPI.Modulo2.Analise.Validators;

public class AtributoValidator : AbstractValidator<Atributo>
{
  public AtributoValidator ()
  {
    RuleFor(x => x.NoAtributo).NotNull().NotEmpty()
     .WithMessage("Attribute Name must be informed.");
    RuleFor(x => x.CoTipoDado).NotNull().NotEmpty()
     .WithMessage("Attribute Data Type must be informed.");
    RuleFor(x => x.IdDtoClasse).NotNull()
     .WithMessage("Dto Class must be informed.");
  }
}
