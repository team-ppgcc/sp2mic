using FluentValidation;
using sp2mic.WebAPI.Domain.Entities;

namespace sp2mic.WebAPI.Modulo2.Analise.Validators;

public class DtoClasseValidator : AbstractValidator<DtoClasse>
{
  public DtoClasseValidator ()
  {
    RuleFor(x => x.NoDtoClasse).NotNull().NotEmpty().WithMessage("DTO class name must be informed.");
  }
}
