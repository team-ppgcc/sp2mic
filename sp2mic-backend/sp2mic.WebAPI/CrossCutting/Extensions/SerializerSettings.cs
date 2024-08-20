using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace sp2mic.WebAPI.CrossCutting.Extensions;

public static class SerializerSettings
{
  public static readonly JsonSerializerSettings JsonSerializerSettings = new()
  {
    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
    NullValueHandling = NullValueHandling.Ignore,
   // DefaultValueHandling = DefaultValueHandling.Ignore,
    ContractResolver = new CamelCasePropertyNamesContractResolver()
  };
}
