using FluentValidation;
using sp2mic.WebAPI.Domain.Entities;

namespace sp2mic.WebAPI.Modulo2.Analise.Validators;

public class StoredProcedureValidator : AbstractValidator<StoredProcedure>
{
  public StoredProcedureValidator ()
  {
    RuleFor(x => x.NoStoredProcedure).NotNull().NotEmpty()
     .WithMessage("Stored Procedure name must be informed.");
    RuleFor(x => x.TxResultadoParser).NotNull().NotEmpty()
     .WithMessage("Stored Procedure parser result must be informed.");
  }
}
