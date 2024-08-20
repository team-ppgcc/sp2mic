using FluentValidation;
using sp2mic.WebAPI.Domain.Entities;

namespace sp2mic.WebAPI.Modulo2.Analise.Validators;

public class MicrosservicoValidator : AbstractValidator<Microsservico>
{
  public MicrosservicoValidator ()
  {
    RuleFor(x => x.NoMicrosservico).NotNull().NotEmpty()
     .WithMessage("Microservice Name must be informed.");
  }
}
