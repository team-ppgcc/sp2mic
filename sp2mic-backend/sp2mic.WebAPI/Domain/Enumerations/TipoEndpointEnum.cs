using sp2mic.WebAPI.CrossCutting.Extensions;

namespace sp2mic.WebAPI.Domain.Enumerations;

public enum TipoEndpointEnum
{
  [EnumInformation(1, "", "", "")]
  TIPO_NAO_MAPEADO = 1,

  [EnumInformation(2, "Select", "org.springframework.web.bind.annotation.GetMapping", "@GetMapping", "getResultList")]
  SELECT = 2,

  [EnumInformation(3, "Insert","org.springframework.web.bind.annotation.PostMapping", "@PostMapping", "executeUpdate")]
  INSERT = 3,

  [EnumInformation(4, "Update", "org.springframework.web.bind.annotation.PutMapping","@PutMapping", "executeUpdate")]
  UPDATE = 4,

  [EnumInformation(5, "Delete", "org.springframework.web.bind.annotation.DeleteMapping","@DeleteMapping", "executeUpdate")]
  DELETE = 5,

  [EnumInformation(6, "Create", "org.springframework.web.bind.annotation.GetMapping","@GetMapping", "executeUpdate")]
  CREATE = 6,

  [EnumInformation(7, "Alter", "org.springframework.web.bind.annotation.GetMapping","@GetMapping", "executeUpdate")]
  ALTER  = 7,

  [EnumInformation(8, "Drop", "org.springframework.web.bind.annotation.GetMapping","@GetMapping", "executeUpdate")]
  DROP  = 8,

  [EnumInformation(9, "Exec", "org.springframework.web.bind.annotation.GetMapping","@GetMapping", "executeUpdate")]
  EXEC  = 9
}
