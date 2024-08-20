using FluentValidation;
using Endpoint = sp2mic.WebAPI.Domain.Entities.Endpoint;

namespace sp2mic.WebAPI.Modulo2.Analise.Validators;

public class EndpointValidator : AbstractValidator<Endpoint>
{
  public EndpointValidator ()
  {
    RuleFor(x => x.TxEndpoint).NotNull().NotEmpty()
     .WithMessage("Endpoint text must be informed.");
    RuleFor(x => x.CoTipoSqlDml).NotNull().NotEmpty()
     .WithMessage("Endpoint type must be informed.");
  }
}
