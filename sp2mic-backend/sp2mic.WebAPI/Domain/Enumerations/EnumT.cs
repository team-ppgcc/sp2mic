namespace sp2mic.WebAPI.Domain.Enumerations;

//     Strongly typed version of Enum with Parsing and performance improvements.
/// <typeparam name="T">Type of Enum</typeparam>
public static class Enum<T> where T : struct, IConvertible
{
  private static readonly IEnumerable<T> All = Enum.GetValues(typeof (T)).Cast<T>();

  private static readonly Dictionary<string, T> InsensitiveNames
    = All.ToDictionary(k => Enum.GetName(typeof (T), k)!.ToUpperInvariant());

  private static readonly Dictionary<string, T> SensitiveNames
    = All.ToDictionary(k => Enum.GetName(typeof (T), k)!);

  private static readonly Dictionary<int, T> Values = All.ToDictionary(k => Convert.ToInt32(k));
  private static readonly Dictionary<T, string> Names = All.ToDictionary(k => k, v => v.ToString())!;

  public static bool IsDefined (T value) => Names.ContainsKey(value);

  public static bool IsDefined (string value) => SensitiveNames.ContainsKey(value);

  public static bool IsDefined (int value) => Values.ContainsKey(value);

  public static IEnumerable<T> GetValues () => All;

  public static string[] GetNames () => Names.Values.ToArray();

  public static string GetName (T value)
  {
    Names.TryGetValue(value, out var name);
    return name!;
  }

  private static T Parse (string value)
  {
    if (!SensitiveNames.TryGetValue(value, out var parsed))
    {
      throw new ArgumentException(
        "Value is not one of the named constants defined for the enumeration", nameof (value));
    }

    return parsed;
  }

  public static T Parse (string value, bool ignoreCase)
  {
    if (!ignoreCase)
    {
      return Parse(value);
    }

    if (!InsensitiveNames.TryGetValue(value.ToUpperInvariant(), out var parsed))
    {
      throw new ArgumentException(
        "Value is not one of the named constants defined for the enumeration", nameof (value));
    }

    return parsed;
  }

  private static bool TryParse (string value, out T returnValue)
    => SensitiveNames.TryGetValue(value, out returnValue);

  public static bool TryParse (string value, bool ignoreCase, out T returnValue)
    => ignoreCase ? InsensitiveNames.TryGetValue(value.ToUpperInvariant(), out returnValue) :
      TryParse(value, out returnValue);

  private static T? ParseOrNull (string value)
  {
    if (string.IsNullOrEmpty(value))
    {
      return null;
    }

    if (SensitiveNames.TryGetValue(value, out var foundValue))
    {
      return foundValue;
    }

    return null;
  }

  public static T? ParseOrNull (string value, bool ignoreCase)
  {
    if (!ignoreCase)
    {
      return ParseOrNull(value);
    }

    if (string.IsNullOrEmpty(value))
    {
      return null;
    }

    if (InsensitiveNames.TryGetValue(value.ToUpperInvariant(), out var foundValue))
    {
      return foundValue;
    }

    return null;
  }

  public static T? CastOrNull (int value)
  {
    if (Values.TryGetValue(value, out var foundValue))
    {
      return foundValue;
    }

    return null;
  }

  public static IEnumerable<T> GetFlags (T flagEnum)
  {
    var flagInt = Convert.ToInt32(flagEnum);
    return All.Where(e => (Convert.ToInt32(e) & flagInt) != 0);
  }

  public static T SetFlags (IEnumerable<T> flags)
  {
    var combined
      = flags.Aggregate(default (int), (current, flag) => current | Convert.ToInt32(flag));

    return Values.TryGetValue(combined, out var result) ? result : default;
  }
}
