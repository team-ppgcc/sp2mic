// using AutoMapper;
// using sp2mic.WebAPI.CrossCutting.Extensions;
// using sp2mic.WebAPI.Domain.Entities;
// using sp2mic.WebAPI.Domain.Enumerations;
// using sp2mic.WebAPI.Modulo2.Analise.Dtos;
//
// namespace sp2mic.WebAPI.Modulo2.Analise.MapperProfiles;
//
// public class AutoMapperProfiles : Profile
// {
//   public AutoMapperProfiles()
//   {
//     // CreateMap<StoredProcedure, StoredProcedureDto>()
//     //  .ForMember(dest
//     //       => dest.NoTipoDadoRetorno,
//     //     opt
//     //       => opt.MapFrom(src => GetNomeTipoDadoRetornoSp(src)))
//     //  .ForMember(dest
//     //       => dest.QtdEndpoints,
//     //     opt
//     //       => opt.MapFrom(src => src.Endpoints.Count))
//     //  .ReverseMap();
//     // CreateMap<StoredProcedure, StoredProcedureUpdateDto>().ReverseMap();
//
//     // CreateMap<Endpoint, EndpointDto>()
//     //  .ForMember(dest
//     //       => dest.NoMetodoEndpoint,
//     //     opt
//     //       => opt.MapFrom(src => src.NoMetodoEndpoint != "nomeAindaNaoDefinido" ? src.NoMetodoEndpoint : ""))
//     //  .ForMember(dest
//     //       => dest.NoPath,
//     //     opt
//     //       => opt.MapFrom(src => src.NoPath != "/path-ainda-nao-definido" ? src.NoPath : ""))
//     //  .ForMember(dest
//     //       => dest.NoTipoDadoRetorno,
//     //     opt
//     //       => opt.MapFrom(src => GetNomeTipoDadoRetornoEp(src)))
//     //  .ForMember(dest
//     //       => dest.NoMicrosservico,
//     //     opt
//     //       => opt.MapFrom(src => src.IdMicrosservicoNavigation != null
//     //         ? src.IdMicrosservicoNavigation.NoMicrosservico : ""))
//     //  .ForMember(dest
//     //       => dest.NoStoredProcedure,
//     //     opt
//     //       => opt.MapFrom(src => src.IdStoredProcedureNavigation != null
//     //         ? src.IdStoredProcedureNavigation.NoStoredProcedure : ""))
//     //  .ForMember(dest
//     //       => dest.NoTipoSqlDml,
//     //     opt
//     //       => opt.MapFrom(src => (src.CoTipoSqlDml).GetNome()))
//     //  .ForMember(dest
//     //       => dest.DtoClassesDaStoredProcedure,
//     //     opt
//     //       => opt.MapFrom(src => src.IdStoredProcedureNavigation.DtoClasses))
//     //  .ForMember(dest
//     //       => dest.NoVariavelRetornda,
//     //     opt
//     //       => opt.MapFrom(src
//     //         => src.IdVariavelRetornadaNavigation == null ? "" :
//     //           src.IdVariavelRetornadaNavigation.NoVariavel))
//     //  .ReverseMap();
//     //
//     // CreateMap<Endpoint, EndpointUpdateDto>().ReverseMap();
//
//     // CreateMap<Atributo, AtributoDto>()
//     //  .ForMember(dest
//     //       => dest.NoTipoDado,
//     //     opt
//     //       => opt.MapFrom(src => src.CoTipoDado.GetNome()))
//     //  .ReverseMap()
//     //  .ForMember(dest
//     //       => dest.NoAtributo,
//     //     opt =>
//     //       opt.MapFrom(src => src.NoAtributo != null
//     //         ? src.NoAtributo.TrimAndRemoveWhiteSpaces() : null));
//     //
//     // CreateMap<Atributo, AtributoUpdateDto>().ReverseMap()
//     //  .ForMember(dest
//     //       => dest.NoAtributo,
//     //     opt =>
//     //       opt.MapFrom(src => src.NoAtributo != null
//     //         ? src.NoAtributo.TrimAndRemoveWhiteSpaces() : null));
//
//     // CreateMap<DtoClasse, DtoClasseDto>()
//     //  .ForMember(dest
//     //       => dest.NoMicrosservico,
//     //     opt
//     //       => opt.MapFrom(src => src.IdMicrosservicoNavigation != null
//     //         ? src.IdMicrosservicoNavigation.NoMicrosservico : ""))
//     //  .ForMember(dest
//     //       => dest.NoStoredProcedure,
//     //     opt
//     //       => opt.MapFrom(src => src.IdStoredProcedureNavigation.NoStoredProcedure))
//     //  .ReverseMap()
//     //  .ForMember(dest
//     //       => dest.NoDtoClasse,
//     //     opt =>
//     //       opt.MapFrom(src => src.NoDtoClasse != null
//     //         ? src.NoDtoClasse.TrimAndRemoveWhiteSpaces().InicialMaiuscula() : null));
//     //
//     // CreateMap<DtoClasse, DtoClasseUpdateDto>()
//     //  .ReverseMap()
//     //  .ForMember(dest
//     //       => dest.NoDtoClasse,
//     //     opt =>
//     //       opt.MapFrom(src => src.NoDtoClasse != null
//     //         ? src.NoDtoClasse.TrimAndRemoveWhiteSpaces().InicialMaiuscula() : null));
//
//     // CreateMap<Microsservico, MicrosservicoDto>()
//     //  .ForMember(dest
//     //       => dest.QtdEndpoints,
//     //     opt
//     //       => opt.MapFrom(src => src.Endpoints.Count))
//     //  .ReverseMap();
//     // CreateMap<Microsservico, MicrosservicoAddUpdateDto>().ReverseMap();
//     //
//     // CreateMap<Variavel, VariavelDto>().ReverseMap();
//     // CreateMap<Variavel, VariavelAddUpdateDto>().ReverseMap();
//   }
//
//   // private static string GetNomeTipoDadoRetornoSp(StoredProcedure src)
//   // {
//   //   if (src.CoTipoDadoRetorno == TipoDadoEnum.TIPO_NAO_MAPEADO)
//   //   {
//   //     return "";
//   //   }
//   //   var nomeRetorno = Util.RecuperarNomeRetornoSp(src.CoTipoDadoRetorno, src.IdDtoClasseNavigation);
//   //   return src.SnRetornoLista == true ? $"List<{nomeRetorno}>" : nomeRetorno;
//   // }
//
//   // private static string GetNomeTipoDadoRetornoEp(Endpoint src)
//   // {
//   //   if (src.CoTipoDadoRetorno == TipoDadoEnum.TIPO_NAO_MAPEADO)
//   //   {
//   //     return "";
//   //   }
//   //   var nomeRetorno = Util.RecuperarNomeRetorno(src.CoTipoDadoRetorno, src.IdDtoClasseNavigation);
//   //   return src.SnRetornoLista == true ? $"List<{nomeRetorno}>" : nomeRetorno;
//   // }
// }
