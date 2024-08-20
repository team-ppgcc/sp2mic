namespace sp2mic.WebAPI.CrossCutting.Extensions;

public static class EnumerationsExtensions
{
  public static T? ToEnum<T> (this int @this)
  {
    var codigo = @this;
    var enumType = typeof (T);

    if (codigo == 0)
      // throw new ArgumentException("Not found.", codigo.ToString());
    {
      return default;
    }

    foreach (int i in Enum.GetValues(enumType))
    {
      if (codigo == i)
      {
        return(T) Enum.Parse(enumType, codigo.ToString().AsSpan());
      }
    }

    return default;
    //throw new ArgumentException("Not found.", codigo.ToString());
  }

  // TODO ajeitar
  public static int GetCodigo (this Enum @this) => ToCodigo(@this);

  public static string GetNome (this Enum @this) => ToNome(@this);

  public static string GetDescricao (this Enum @this) => ToDescricao(@this);

  public static string GetDescricaoComplementar (this Enum @this) => ToDescricaoComplementar(@this);

  public static string GetPacoteImport (this Enum @this) => ToPacoteImport(@this);

  // This extension method is broken out so you can use a similar pattern with
  // other MetaData elements in the future. This is your base method for each.
  public static T? GetAttribute<T> (this Enum value) where T : Attribute
  {
    var type = value.GetType();
    var memberInfo = type.GetMember(value.ToString());
    object[] attributes; // = memberInfo[0].GetCustomAttributes(typeof(T), false);

    try
    {
      attributes = memberInfo[0].GetCustomAttributes(typeof (T), false);
    }
    catch (IndexOutOfRangeException)
    {
      return null;
    }

    // check if no attributes have been specified.
    if (attributes.Length > 0)
    {
      return(T) attributes[0];
    }

    return null;
  }

  public static int ToCodigo (this Enum value)
  {
    var attribute = value.GetAttribute<EnumInformationAttribute>();
    return attribute?.Codigo ?? 0;
  }

  public static string ToNome (this Enum value)
  {
    var attribute = value.GetAttribute<EnumInformationAttribute>();
    return attribute == null ? value.ToString() : attribute.Nome;
  }

  public static string ToPacoteImport (this Enum value)
  {
    var attribute = value.GetAttribute<EnumInformationAttribute>();
    return attribute == null ? value.ToString() : attribute.PacoteImport;
  }

  public static string ToDescricao (this Enum value)
  {
    var attribute = value.GetAttribute<EnumInformationAttribute>();
    return attribute == null ? value.ToString() : attribute.Descricao;
  }

  public static string ToDescricaoComplementar (this Enum value)
  {
    var attribute = value.GetAttribute<EnumInformationAttribute>();
    return attribute == null ? value.ToString() : attribute.DescricaoComplementar;
  }

  //     Find the enum from the description attribute.
  /// <typeparam name="T"></typeparam>
  /// <param name="desc"></param>
  /// <returns></returns>
  public static T FromName<T> (this string desc) where T : struct
  {
    string attr;
    var found = false;
    var result = (T) Enum.GetValues(typeof (T)).GetValue(0)!;

    foreach (var enumVal in Enum.GetValues(typeof (T)))
    {
      attr = ((Enum) enumVal).ToNome();

      if (attr == desc)
      {
        result = (T) enumVal;
        found = true;
        break;
      }
    }

    if (!found)
      //throw new Exception(); TODO
      // throw new ArgumentException("Not found.", desc);
    {
      return default;
    }

    return result;
  }
}
