using System.ComponentModel.DataAnnotations;

namespace sp2mic.WebAPI.Modulo2.Analise.Validators;

public class PrimeiraLetraMaiusculaAttribute : ValidationAttribute
{
  //Exemplo de como fazer validação manual (substituicao dos [Required], etc) em Produto.cs
  //sobrescrever o metodo IsValid
  protected override ValidationResult? IsValid (object? value, //value será o valor da propriedade
    ValidationContext validationContext) //informacao do contexto onde estamos fazendo a validacao (no caso classeDTO Produto)
  {
    //colocar esse tratamento para dar um bypass no Required
    if (value == null || string.IsNullOrEmpty(value.ToString()))
    {
      return ValidationResult.Success;
    }

    var primeiraLetra = value.ToString()?[0].ToString(); //pega a primeira posição da string e guarda

    return primeiraLetra != primeiraLetra!.ToUpper() ? new ValidationResult("A primeira letra do nome da classeDTO deve ser maiúscula") : ValidationResult.Success;
  }
}
