using sp2mic.WebAPI.Modulo3.Geracao.Services;
using static System.IO.Directory;
using static System.IO.Path;

namespace sp2mic.WebAPI.CrossCutting.Util;

public static class FileUtil
{
  public static string CriarDiretorioProjeto(string diretorioDestinoClasses, string nomeDiretorio)
  {
    var diretorio = Combine(diretorioDestinoClasses, nomeDiretorio.Replace("-", ""));
    diretorio = Combine(GetCurrentDirectory(), diretorio);
    CreateDirectory(diretorio);
    return diretorio;
  }

  public static string CriarDiretorioResources(string diretorioProjeto)
  {
    var diretorioResources = Combine(diretorioProjeto, GeracaoMicrosservicosConstants.RESOURCES_PATH);
    CreateDirectory(Combine(GetCurrentDirectory(), diretorioResources));
    return diretorioResources;
  }

  public static string CriarDiretorioSrcTestJavaPacote(string diretorioEntrada, string nomePacote)
  {
    var pastasPacote = nomePacote.Split("."); // Replace(".", "\\");
    var diretorioPacoteTest = Combine(diretorioEntrada, GeracaoMicrosservicosConstants.SRC_TEST_JAVA);
    var diretorioPacoteTestCompleto = pastasPacote.Aggregate(diretorioPacoteTest, Combine);
    CreateDirectory(Combine(GetCurrentDirectory(), diretorioPacoteTestCompleto));
    return diretorioPacoteTestCompleto;
  }

  public static string CriarDiretorioSrcMainJavaPacote(string diretorioEntrada, string nomePacote)
  {
    var pastasPacote = nomePacote.Split("."); // Replace(".", "\\");
    var diretorioPacoteMain = Combine(diretorioEntrada, GeracaoMicrosservicosConstants.SRC_MAIN_JAVA);
    var diretorioPacoteMainCompleto = pastasPacote.Aggregate(diretorioPacoteMain, Combine);
    CreateDirectory(Combine(GetCurrentDirectory(), diretorioPacoteMainCompleto));
    return diretorioPacoteMainCompleto;
  }

  public static string CriarDiretorioDto(string diretorioDestinoClasses)
  {
    var diretorio = Combine(diretorioDestinoClasses, "dto");
    CreateDirectory(Combine(GetCurrentDirectory(), diretorio));
    return diretorio;
  }

  public static string CriarDiretorioException(string diretorioDestinoClasses)
  {
    var diretorio = Combine(diretorioDestinoClasses, "exception");
    CreateDirectory(Combine(GetCurrentDirectory(), diretorio));
    return diretorio;
  }

  public static string CriarDiretorioRepository(string diretorioDestinoClasses)
  {
    var diretorio = Combine(diretorioDestinoClasses, "repository");
    CreateDirectory(Combine(GetCurrentDirectory(), diretorio));
    return diretorio;
  }

  public static string CriarDiretorioRestClientServices(string diretorioDestinoClasses)
  {
    var diretorio = Combine(diretorioDestinoClasses, "services");
    CreateDirectory(Combine(GetCurrentDirectory(), diretorio));
    return diretorio;
  }

  public static string CriarDiretorioController(string diretorioDestinoClasses)
  {
    var diretorio = Combine(diretorioDestinoClasses, "controller");
    CreateDirectory(Combine(GetCurrentDirectory(), diretorio));
    return diretorio;
  }

  public static void SalvarArquivo(string folder, string fileName, string content)
  {
    using var fs = new FileStream(Combine(GetCurrentDirectory(), folder, fileName.Trim()), FileMode.Create);
    using var str = new StreamWriter(fs);
    str.WriteLine(content);
    str.Flush();
  }

 public static void SaveFile(string folder, string fileName, string content)
  {
    using var fs = new FileStream(Combine(GetCurrentDirectory(), folder, fileName.Trim()), FileMode.Create);
    using var str = new StreamWriter(fs);
    str.WriteLine(content);
    str.Flush();
  }
}
