using FluentValidation;
using sp2mic.WebAPI.Domain.Entities;

namespace sp2mic.WebAPI.Modulo2.Analise.Validators;

public class TabelaValidator : AbstractValidator<Tabela>
{
  public TabelaValidator ()
  {
    RuleFor(x => x.NoTabela).NotNull().NotEmpty()
     .WithMessage("Table's name must be informed.");
  }
}
