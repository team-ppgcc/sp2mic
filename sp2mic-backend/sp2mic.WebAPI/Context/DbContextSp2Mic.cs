using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using sp2mic.WebAPI.CrossCutting;
using sp2mic.WebAPI.Domain.Entities;
using Endpoint = sp2mic.WebAPI.Domain.Entities.Endpoint;

namespace sp2mic.WebAPI.Context;

public class DbContextSp2Mic : DbContext
{
  private readonly IConfiguration _configuration;

  public DbContextSp2Mic(DbContextOptions<DbContextSp2Mic> options, IConfiguration configuration)
    : base(options)
    => _configuration = configuration;

  public virtual DbSet<Atributo> Atributos {get; set;} = null!;
  public virtual DbSet<Comando> Comandos {get; set;} = null!;
  public virtual DbSet<ComandoVariavel> ComandoVariaveis {get; set;} = null!;
  public virtual DbSet<DtoClasse> DtoClasses {get; set;} = null!;
  public virtual DbSet<Endpoint> Endpoints {get; set;} = null!;
  public virtual DbSet<Expressao> Expressoes {get; set;} = null!;
  public virtual DbSet<Microsservico> Microsservicos {get; set;} = null!;
  public virtual DbSet<Operando> Operandos {get; set;} = null!;
  public virtual DbSet<StoredProcedure> StoredProcedures {get; set;} = null!;
  public virtual DbSet<Tabela> Tabelas {get; set;} = null!;
  public virtual DbSet<Variavel> Variaveis {get; set;} = null!;

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    if (!optionsBuilder.IsConfigured)
    {
      optionsBuilder.EnableDetailedErrors().EnableSensitiveDataLogging()
       .UseNpgsql(_configuration.GetConnectionString("Sp2MicConnection"),
          builder => builder.EnableRetryOnFailure()
           .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
       .ConfigureWarnings(warnings
          => warnings.Throw(RelationalEventId.QueryPossibleUnintendedUseOfEqualsWarning))
       .ConfigureWarnings(w => w.Throw(RelationalEventId.MultipleCollectionIncludeWarning));
    }
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Atributo>(entity =>
    {
      entity.ToTable("Atributo", Constantes.Schema);
      entity.ToTable(t
        => t.HasComment("Tabela contendo as informacões dos atributos de uma classe dto."));
      entity.Property(e => e.Id).IsRequired().ValueGeneratedOnAdd()
       .HasColumnName("Id")
       .HasDefaultValueSql("nextval('sp2mic.\"Atributo_Id_seq\"'::regclass)")
       .HasComment("Sequência que identifica unicamente o registro dessa tabela.");
      entity.HasKey(e => e.Id);
      entity.Property(e => e.NoAtributo).IsRequired()
       .HasMaxLength(200)
       .HasColumnName("No_Atributo")
       .HasComment("Nome do atributo.");
      entity.Property(e => e.CoTipoDado).IsRequired()
       .HasColumnName("Co_TipoDado")
       .HasComment(
          "Tipo de dado do atributo: 1-Tipo não mapeado, 2-void, 3-DTO Classe, 4-String, 5-Integer, 6-Long, 7-Double, 8-Float, 9-Boolean, 10-LocalDate, 11-LocalDateTime, 12-BigDecimal, 13-Object.");
      entity.Property(e => e.IdDtoClasse).IsRequired()
       .HasColumnName("Id_DtoClasse")
       .HasComment("Classe DTO ao qual o atributo pertence.");

      entity.HasOne(d => d.IdDtoClasseNavigation)
       .WithMany(p => p.Atributos)
       .HasForeignKey(d => d.IdDtoClasse)
       .HasConstraintName("fk_atributo_dtoclasse")
       .OnDelete(DeleteBehavior.Cascade);
    });

    modelBuilder.Entity<Comando>(entity =>
    {
      entity.ToTable("Comando", Constantes.Schema);
      entity.ToTable(t
        => t.HasComment("Tabela contendo as informacões dos comandos das stored procedures."));
      entity.Property(e => e.Id).IsRequired().ValueGeneratedOnAdd()
       .HasColumnName("Id")
       .HasDefaultValueSql("nextval('sp2mic.\"Comando_Id_seq\"'::regclass)")
       .HasComment("Sequência que identifica unicamente o registro dessa tabela.");
      entity.HasKey(e => e.Id);
      entity.Property(e => e.CoTipoComando).IsRequired()
       .HasColumnName("Co_TipoComando")
       .HasComment(
          "Tipo da instruçãoo do comando: 1-Tipo não mapeado, 2-endpoint, 3-declaração, 4-atribuição, 5-if, 6-while, 7-Bloco de comandos, 8-Execute Statemen, 9-Begin Transaction Statement.");
      entity.Property(e => e.NuOrdemExecucao).IsRequired()
       .HasColumnName("Nu_OrdemExecucao")
       .HasComment("Ordem de execução do comando na stored procedure.");
      entity.Property(e => e.VlAtribuidoVariavel).IsRequired(false)
       .HasMaxLength(100)
       .HasColumnName("Vl_AtribuidoVariavel")
       .HasComment("Valor atribuido a variável declarada no comando.");
      entity.Property(e => e.IdStoredProcedure).IsRequired()
       .HasColumnName("Id_StoredProcedure")
       .HasComment("Stored procedure que o comando pertence.");
      entity.Property(e => e.IdComandoOrigem).IsRequired(false)
       .HasColumnName("Id_ComandoOrigem")
       .HasComment("Comando que dar origem a outros comando.");
      entity.Property(e => e.IdEndpoint).IsRequired(false)
       .HasColumnName("Id_Endpoint")
       .HasComment("Endpoint chamado no comando.");
      entity.Property(e => e.IdExpressao).IsRequired(false)
       .HasColumnName("Id_Expressao")
       .HasComment("Expressão do comando.");
      entity.Property(e => e.SnCondicaoOrigem).IsRequired(false)
       .HasColumnName("Sn_CondicaoOrigem")
       .HasComment("Indicador da situacao de um comando if");
      entity.Property(e => e.TxComando).IsRequired()
       .HasColumnName("Tx_Comando")
       .HasComment("Texto do comando.");
      entity.Property(e => e.TxComandoTratado).IsRequired()
       .HasColumnName("Tx_ComandoTratado")
       .HasComment("Texto do comando tratado.");

      entity.HasOne(d => d.IdComandoOrigemNavigation)
       .WithMany(p => p.ComandosInternos)
       .HasForeignKey(d => d.IdComandoOrigem)
       .HasConstraintName("fk_comando_comandoorigem")
        .OnDelete(DeleteBehavior.Cascade);
      entity.HasOne(d => d.IdEndpointNavigation)
       .WithMany(p => p.Comandos)
       .HasForeignKey(d => d.IdEndpoint)
       .HasConstraintName("fk_comando_endpoint")
       .OnDelete(DeleteBehavior.Cascade);
      entity.HasOne(d => d.IdExpressaoNavigation)
       .WithMany(p => p.Comandos)
       .HasForeignKey(d => d.IdExpressao)
       .HasConstraintName("fk_comando_expressao")
       .OnDelete(DeleteBehavior.Cascade);
      entity.HasOne(d => d.IdStoredProcedureNavigation)
       .WithMany(p => p.Comandos)
       .HasForeignKey(d => d.IdStoredProcedure)
       .HasConstraintName("fk_comando_storedprocedure")
       .OnDelete(DeleteBehavior.Cascade);
    });

    modelBuilder.Entity<ComandoVariavel>(entity =>
    {
      entity.ToTable("Comando_Variavel", Constantes.Schema);
      entity.ToTable(t => t.HasComment("Tabela contendo relação dos comandos com as variáveis."));
      entity.Property(e => e.Id).IsRequired().ValueGeneratedOnAdd()
       .HasColumnName("Id")
       .HasDefaultValueSql("nextval('sp2mic.\"Comando_Variavel_Id_seq\"'::regclass)")
       .HasComment("Sequência que identifica unicamente o registro dessa tabela.");
      entity.HasKey(e => e.Id);
      entity.Property(e => e.IdComando).IsRequired()
       .HasColumnName("Id_Comando")
       .HasComment("Identificador do comando neste relacionamento.");
      entity.Property(e => e.IdVariavel).IsRequired()
       .HasColumnName("Id_Variavel")
       .HasComment("Identificador da variável neste relacionamento.");
      entity.Property(e => e.NuOrdem).IsRequired()
       .HasColumnName("Nu_Ordem")
       .HasComment("Ordem de execucao da variável.");

      entity.HasOne(d => d.IdComandoNavigation)
       .WithMany(p => p.AsVariaveisDesseComando)
       .HasForeignKey(d => d.IdComando)
       .HasConstraintName("fk_comandovariavel_comando")
       .OnDelete(DeleteBehavior.Cascade);
      entity.HasOne(d => d.IdVariavelNavigation)
       .WithMany(p => p.OsComandosQueContemEssaVariavel)
       .HasForeignKey(d => d.IdVariavel)
       .HasConstraintName("fk_comandovariavel_variavel")
       .OnDelete(DeleteBehavior.Cascade);
    });

    modelBuilder.Entity<DtoClasse>(entity =>
    {
      entity.ToTable("DtoClasse", Constantes.Schema);
      entity.ToTable(t => t.HasComment("Tabela contendo as informacões das classes DTO."));
      entity.Property(e => e.Id).IsRequired().ValueGeneratedOnAdd()
       .HasColumnName("Id")
       .HasDefaultValueSql("nextval('sp2mic.\"DtoClasse_Id_seq\"'::regclass)")
       .HasComment("Sequência que identifica unicamente o registro dessa tabela.");
      entity.HasKey(e => e.Id);
      entity.Property(e => e.NoDtoClasse).IsRequired()
       .HasMaxLength(200)
       .HasColumnName("No_DtoClasse")
       .HasComment("Nome da classe do DTO.");
      entity.Property(e => e.IdStoredProcedure).IsRequired()
       .HasColumnName("Id_StoredProcedure")
       .HasComment("Stored Procedure ao qual a classe DTO pertence.");
      entity.HasOne(d => d.IdStoredProcedureNavigation)
       .WithMany(p => p.DtoClasses)
       .HasForeignKey(d => d.IdStoredProcedure)
       .HasConstraintName("fk_dtoclasse_storedprocedure")
       .OnDelete(DeleteBehavior.Cascade);
    });

    modelBuilder.Entity<Endpoint>(entity =>
    {
      entity.ToTable("Endpoint", Constantes.Schema);
      entity.ToTable(t
        => t.HasComment("Tabela contendo as informações dos endpoints dos microsserviços."));
      entity.Property(e => e.Id).IsRequired().ValueGeneratedOnAdd()
       .HasColumnName("Id")
       .HasDefaultValueSql("nextval('sp2mic.\"Endpoint_Id_seq\"'::regclass)")
       .HasComment("Sequência que identifica unicamente o registro dessa tabela.");
      entity.HasKey(e => e.Id);
      entity.Property(e => e.NoMetodoEndpoint).IsRequired(false)
       .HasMaxLength(200)
       .HasColumnName("No_MetodoEndpoint")
       .HasComment("Nome do metodo do endpoint.");
      entity.Property(e => e.NoPath).IsRequired(false)
       .HasMaxLength(200)
       .HasColumnName("No_Path")
       .HasComment("Path do endpoint.");
      entity.Property(e => e.CoTipoSqlDml).IsRequired()
       .HasColumnName("Co_TipoSqlDml")
       .HasComment("Tipo do endpoint: 1-Tipo não mapeado, 2-Select, 3-Insert, 4-Update, 5-Delete.");
      entity.Property(e => e.CoTipoDadoRetorno).IsRequired()
       .HasColumnName("Co_TipoDadoRetorno")
       .HasComment(
          "Tipo de dado de retorno do metodo: 1-Tipo não mapeado, 2-void, 3-DTO Classe, 4-String, 5-Integer, 6-Long, 7-Double, 8-Float, 9-Boolean, 10-LocalDate, 11-LocalDateTime, 12-BigDecimal, 13-Object.");
      entity.Property(e => e.SnRetornoLista).IsRequired(false)
       .HasColumnName("Sn_RetornoLista")
       .HasComment("Indicador se o retorno e uma lista ou não.");
      entity.Property(e => e.SnAnalisado).IsRequired()
       .HasColumnName("Sn_Analisado")
       .HasComment("Indicador se o endpoint foi analisado ou não pelo especialista.");
      entity.Property(e => e.IdMicrosservico).IsRequired(false)
       .HasColumnName("Id_Microsservico")
       .HasComment("Microsserviço ao qual o endpoint pertence.");
      entity.Property(e => e.IdStoredProcedure).IsRequired()
       .HasColumnName("Id_StoredProcedure")
       .HasComment("Stored procedure a qual o endpoint pertence.");
      entity.Property(e => e.IdDtoClasse).IsRequired(false)
       .HasColumnName("Id_DtoClasse")
       .HasComment("Classe DTO que o endpoint retorna.");
      entity.Property(e => e.IdVariavelRetornada).IsRequired(false)
       .HasColumnName("Id_VariavelRetornada")
       .HasComment("Variavel que o endpoint retorna.");
      entity.Property(e => e.TxEndpoint).IsRequired()
       .HasColumnName("Tx_Endpoint")
       .HasComment("Texto do endpoint original.");
      entity.Property(e => e.TxEndpointTratado).IsRequired()
       .HasColumnName("Tx_EndpointTratado")
       .HasComment("Texto do endpoint tratado.");

      entity.HasOne(d => d.IdMicrosservicoNavigation)
       .WithMany(p => p.Endpoints)
       .HasForeignKey(d => d.IdMicrosservico)
       .HasConstraintName("fk_endpoint_microsservico")
       .OnDelete(DeleteBehavior.SetNull);
      entity.HasOne(d => d.IdStoredProcedureNavigation)
       .WithMany(p => p.Endpoints)
       .HasForeignKey(d => d.IdStoredProcedure)
       .HasConstraintName("fk_endpoint_storedprocedure")
       .OnDelete(DeleteBehavior.Cascade);
      entity.HasOne(d => d.IdDtoClasseNavigation)
       .WithMany(p => p.EndpointsQueRetornamEssaClasse)
       .HasForeignKey(d => d.IdDtoClasse)
       .HasConstraintName("fk_endpoint_dtoclasse")
        .OnDelete(DeleteBehavior.SetNull);
      entity.HasOne(d => d.IdVariavelRetornadaNavigation)
       .WithMany(p => p.EndpointsQueRetornamEssaVariavel)
       .HasForeignKey(d => d.IdVariavelRetornada)
       .HasConstraintName("fk_endpoint_variavel")
       .OnDelete(DeleteBehavior.SetNull);
      entity.HasMany(d => d.TabelasAssociadas)
       .WithMany(p => p.EndpointsAssociados)
       .UsingEntity<Dictionary<string, object>>("EndpointTabela",
          l => l.HasOne<Tabela>()
           .WithMany().HasForeignKey("IdTabela")
           .HasConstraintName("fk_endpointtabela_tabela")
           .OnDelete(DeleteBehavior.Cascade),
          r => r.HasOne<Endpoint>()
           .WithMany().HasForeignKey("IdEndpoint")
           .HasConstraintName("fk_endpointtabela_endpoint")
           .OnDelete(DeleteBehavior.Cascade),
          j =>
          {
            j.HasKey("IdEndpoint", "IdTabela").HasName("pk_endpoint_tabela");
            j.ToTable("Endpoint_Tabela", Constantes.Schema);
            j.ToTable(t => t.HasComment("Tabela contendo relação dos endpoint com as tabelas."));
            j.IndexerProperty<int>("IdEndpoint").IsRequired()
             .HasColumnName("Id_Endpoint")
             .HasComment("Identificador do endpoint neste relacionamento.");
            j.IndexerProperty<int>("IdTabela").IsRequired()
             .HasColumnName("Id_Tabela")
             .HasComment("Identificador da tabela neste relacionamento.");
          });

      entity.HasMany(d => d.Parametros)
       .WithMany(p => p.EndpointsQueContemEssaVariavelComoParametro)
       .UsingEntity<Dictionary<string, object>>("EndpointVariavel",
          l => l.HasOne<Variavel>()
           .WithMany().HasForeignKey("IdVariavel")
           .HasConstraintName("fk_endpointvariavel_variavel")
           .OnDelete(DeleteBehavior.Cascade),
          r => r.HasOne<Endpoint>()
           .WithMany().HasForeignKey("IdEndpoint")
           .HasConstraintName("fk_endpointvariavel_endpoint")
           .OnDelete(DeleteBehavior.Cascade),
          j =>
          {
            j.HasKey("IdEndpoint", "IdVariavel").HasName("pk_endpoint_variavel");
            j.ToTable("Endpoint_Variavel", Constantes.Schema);
            j.ToTable(t => t.HasComment("Tabela contendo relação dos endpoints com as variáveis."));
            j.IndexerProperty<int>("IdEndpoint").IsRequired()
             .HasColumnName("Id_Endpoint")
             .HasComment("Identificador do endpoint neste relacionamento.");
            j.IndexerProperty<int>("IdVariavel").IsRequired()
             .HasColumnName("Id_Variavel")
             .HasComment("Identificador da variável neste relacionamento.");
          });
    });

    modelBuilder.Entity<Expressao>(entity =>
    {
      entity.ToTable("Expressao", Constantes.Schema);
      entity.ToTable(t => t.HasComment("Tabela contendo as informações das expressões."));
      entity.Property(e => e.Id).IsRequired().ValueGeneratedOnAdd()
       .HasColumnName("Id")
       .HasDefaultValueSql("nextval('sp2mic.\"Expressao_Id_seq\"'::regclass)")
       .HasComment("Sequência que identifica unicamente o registro dessa tabela.");
      entity.HasKey(e => e.Id);
      entity.Property(e => e.CoTipoDadoRetorno).IsRequired()
       .HasColumnName("Co_TipoDadoRetorno")
       .HasComment(
          "Tipo de dado de retorno da expressão: 1-Tipo não mapeado, 2-void, 3-DTO Classe, 4-String, 5-Integer, 6-Long, 7-Double, 8-Float, 9-Boolean, 10-LocalDate, 11-LocalDateTime, 12-BigDecimal, 13-Object.");
      entity.Property(e => e.NuOrdemExecucao).IsRequired()
       .HasColumnName("Nu_OrdemExecucao")
       .HasComment("Ordem de execução da expressão.");
      entity.Property(e => e.IdOperandoEsquerda).IsRequired(false)
       .HasColumnName("Id_OperandoEsquerda")
       .HasComment("Operando do lado esquerdo da expressão.");
      entity.Property(e => e.CoOperador).IsRequired(false)
       .HasColumnName("Co_Operador")
       .HasComment(
          "Tipo de operador: 1-Tipo não mapeado, 2-Adição, 3-Subtração, 4-Divisão, 5-Multiplicação, 6-Maior que, 7-Menor que, 8-Maior igual, 9-Menor igual, 10-Igual, 11-Diferente, 12-E, 13-Ou, 14-Exists, 15-Atribuição.");
      entity.Property(e => e.IdOperandoDireita).IsRequired(false)
       .HasColumnName("Id_OperandoDireita")
       .HasComment("Operando do lado direito da expressão.");

      entity.HasOne(d => d.IdOperandoDireitaNavigation)
       .WithMany(p => p.ExpressaoIdOperandoDireitaNavigations)
       .HasForeignKey(d => d.IdOperandoDireita)
       .HasConstraintName("fk_expressao_operandodireita")
       .OnDelete(DeleteBehavior.Cascade);
      entity.HasOne(d => d.IdOperandoEsquerdaNavigation)
       .WithMany(p => p.ExpressaoIdOperandoEsquerdaNavigations)
       .HasForeignKey(d => d.IdOperandoEsquerda)
       .HasConstraintName("fk_expressao_operandoesquerda")
       .OnDelete(DeleteBehavior.Cascade);
    });

    modelBuilder.Entity<Microsservico>(entity =>
    {
      entity.ToTable("Microsservico", Constantes.Schema);
      entity.ToTable(t
        => t.HasComment("Tabela contendo as informações dos microsserviços que serão gerados."));
      entity.HasIndex(e => e.NoMicrosservico, "uk_nomicrosservico").IsUnique();
      entity.Property(e => e.Id).IsRequired().ValueGeneratedOnAdd()
       .HasColumnName("Id")
       .HasDefaultValueSql("nextval('sp2mic.\"Microsservico_Id_seq\"'::regclass)")
       .HasComment("Sequência que identifica unicamente o registro dessa tabela.");
      entity.HasKey(e => e.Id);
      entity.Property(e => e.NoMicrosservico).IsRequired()
       .HasMaxLength(200)
       .HasColumnName("No_Microsservico")
       .HasComment("Nome do microsserviço.");
      entity.Property(e => e.SnProntoParaGerar).IsRequired()
       .HasColumnName("Sn_ProntoParaGerar")
       .HasComment("Indicador se o microsserviço está pronto para ser gerado.");
    });
    modelBuilder.Entity<Microsservico>().Navigation(m => m.Endpoints).AutoInclude();

    modelBuilder.Entity<Operando>(entity =>
    {
      entity.ToTable("Operando", Constantes.Schema);
      entity.ToTable(t
        => t.HasComment("Tabela contendo as informações dos operandos das expressões."));
      entity.Property(e => e.Id).IsRequired().ValueGeneratedOnAdd()
       .HasColumnName("Id")
       .HasDefaultValueSql("nextval('sp2mic.\"Operando_Id_seq\"'::regclass)")
       .HasComment("Sequência que identifica unicamente o registro dessa tabela.");
      entity.HasKey(e => e.Id);
      entity.Property(e => e.CoTipoOperando).IsRequired()
       .HasColumnName("Co_TipoOperando")
       .HasComment(
          "Tipo de operandos: 1-Tipo não mapeado, 2-Constante, 3-Constante String, 4-Variável, 5-Expressão, 6-Endpoint, 7-Método, 8-Constante null.");
      entity.Property(e => e.TxValor).IsRequired(false)
       .HasColumnName("Tx_Valor")
       .HasComment("Valor do operando.");
      entity.Property(e => e.SnNegacao).IsRequired()
       .HasColumnName("Sn_Negacao")
       .HasComment("Indicador se o operando deve ser negado.");
      entity.Property(e => e.IdVariavel).IsRequired(false)
       .HasColumnName("Id_Variavel")
       .HasComment("Variável do operando.");
      entity.Property(e => e.IdExpressao).IsRequired(false)
       .HasColumnName("Id_Expressao")
       .HasComment("Expressão do operando.");
      entity.Property(e => e.IdEndpoint).IsRequired(false)
       .HasColumnName("Id_Endpoint")
       .HasComment("Endpoint do operando.");

      entity.HasOne(d => d.IdEndpointNavigation)
       .WithMany(p => p.Operandos)
       .HasForeignKey(d => d.IdEndpoint)
       .HasConstraintName("fk_operando_endpoint")
       .OnDelete(DeleteBehavior.Cascade);
      entity.HasOne(d => d.IdExpressaoNavigation)
       .WithMany(p => p.Operandos)
       .HasForeignKey(d => d.IdExpressao)
       .HasConstraintName("fk_operando_expressao")
       .OnDelete(DeleteBehavior.Cascade);
      entity.HasOne(d => d.IdVariavelNavigation)
       .WithMany(p => p.Operandos)
       .HasForeignKey(d => d.IdVariavel)
       .HasConstraintName("fk_operando_variavel")
        .OnDelete(DeleteBehavior.Cascade);
    });

    modelBuilder.Entity<StoredProcedure>(entity =>
    {
      entity.ToTable("StoredProcedure", Constantes.Schema);
      entity.ToTable(t => t.HasComment("Tabela contendo as informações das stored procedures."));
      entity.HasIndex(e => new {e.NoSchema, e.NoStoredProcedure}, "uk_noschema_nostoredprocedure")
       .IsUnique();
      entity.Property(e => e.Id).IsRequired().ValueGeneratedOnAdd()
       .HasColumnName("Id")
       .HasDefaultValueSql("nextval('sp2mic.\"StoredProcedure_Id_seq\"'::regclass)")
       .HasComment("Sequência que identifica unicamente o registro dessa tabela.");
      entity.HasKey(e => e.Id);
      entity.Property(e => e.NoStoredProcedure).IsRequired()
       .HasMaxLength(200)
       .HasColumnName("No_StoredProcedure")
       .HasComment("Nome da stored procedure.");
      entity.Property(e => e.NoSchema).IsRequired()
       .HasMaxLength(200)
       .HasColumnName("No_Schema")
       .HasComment("Nome do schema da stored procedure.");
      entity.Property(e => e.CoTipoDadoRetorno).IsRequired(false)
       .HasColumnName("Co_TipoDadoRetorno")
       .HasComment(
          "Tipo de dado retornado pela stored procedure: 1-Tipo não mapeado, 2-void, 3-DTO Classe, 4-String, 5-Integer, 6-Long, 7-Double, 8-Float, 9-Boolean, 10-LocalDate, 11-LocalDateTime, 12-BigDecimal, 13-Object.");
      entity.Property(e => e.SnAnalisada).IsRequired()
       .HasColumnName("Sn_Analisada")
       .HasComment("Indica se a stored procedure já foi analisada.");
      entity.Property(e => e.SnRetornoLista).IsRequired(false)
       .HasColumnName("Sn_RetornoLista")
       .HasComment("Indicador se o retorno da stored procedure é uma lista ou não.");
      entity.Property(e => e.IdDtoClasse).IsRequired(false)
       .HasColumnName("Id_DtoClasse")
       .HasComment("Classe DTO que a stored procedure retorna.");
      entity.Property(e => e.TxResultadoParser).IsRequired()
       .HasColumnName("Tx_ResultadoParser")
       .HasComment("Resultado do parse da stored procedure");
      entity.Property(e => e.SnSucessoParser).IsRequired()
       .HasColumnName("Sn_SucessoParser")
       .HasComment(
          "Indicador se o parser da Stored Procedure foi realizado com sucesso ou não.");
      entity.Property(e => e.TxDefinicao).IsRequired()
       .HasColumnName("Tx_Definicao")
       .HasComment("Texto com a definição da stored procedure.");
      entity.Property(e => e.TxDefinicaoTratada).IsRequired()
       .HasColumnName("Tx_DefinicaoTratada")
       .HasComment("Texto com a definição tratada da stored procedure.");

      entity.HasOne(d => d.IdDtoClasseNavigation)
       .WithMany(p => p.StoredProceduresQueRetornamEssaClasse)
       .HasForeignKey(d => d.IdDtoClasse)
       .HasConstraintName("fk_storedprocedure_dtoclasse")
       .OnDelete(DeleteBehavior.SetNull);
      entity.HasMany(d => d.TabelasAssociadas)
       .WithMany(p => p.StoredProceduresAssociadas)
       .UsingEntity<Dictionary<string, object>>("StoredProcedureTabela",
          l => l.HasOne<Tabela>()
           .WithMany().HasForeignKey("IdTabela")
           .HasConstraintName("fk_storedproceduretabela_tabela")
           .OnDelete(DeleteBehavior.Cascade),
          r => r.HasOne<StoredProcedure>()
           .WithMany().HasForeignKey("IdStoredProcedure")
           .HasConstraintName("fk_storedproceduretabela_storedprocedure")
           .OnDelete(DeleteBehavior.Cascade),
          j =>
          {
            j.HasKey("IdStoredProcedure", "IdTabela").HasName("pk_storedprocedure_tabela");
            j.ToTable("StoredProcedure_Tabela", Constantes.Schema);
            j.ToTable(t
              => t.HasComment("Tabela contendo relação das stored procedures com os tabelas."));
            j.IndexerProperty<int>("IdStoredProcedure").IsRequired()
             .HasColumnName("Id_StoredProcedure")
             .HasComment("Identificador da stored procedure neste relacionamento.");
            j.IndexerProperty<int>("IdTabela").IsRequired()
             .HasColumnName("Id_Tabela")
             .HasComment("Identificador da tabela neste relacionamento.");
          });
    });
    modelBuilder.Entity<StoredProcedure>().Navigation(sp => sp.Endpoints).AutoInclude();

    modelBuilder.Entity<Tabela>(entity =>
    {
      entity.ToTable("Tabela", Constantes.Schema);
      entity.ToTable(t
        => t.HasComment(
          "Tabela contendo as informações das tabelas que as stored proceudres acessam."));
      entity.HasIndex(e => e.NoTabela, "uk_notabela")
       .IsUnique();
      entity.Property(e => e.Id).IsRequired().ValueGeneratedOnAdd()
       .HasColumnName("Id")
       .HasDefaultValueSql("nextval('sp2mic.\"Tabela_Id_seq\"'::regclass)")
       .HasComment("Sequência que identifica unicamente o registro dessa tabela\".");
      entity.HasKey(e => e.Id);
      entity.Property(e => e.NoTabela).IsRequired()
       .HasMaxLength(200)
       .HasColumnName("No_Tabela")
       .HasComment("Nome da tabela.");
    });

    modelBuilder.Entity<Variavel>(entity =>
    {
      entity.ToTable("Variavel", Constantes.Schema);
      entity.ToTable(t
        => t.HasComment("Tabela contendo as informações das variáveis das stored procedures."));
      entity.Property(e => e.Id).IsRequired().ValueGeneratedOnAdd()
       .HasColumnName("Id")
       .HasDefaultValueSql("nextval('sp2mic.\"Variavel_Id_seq\"'::regclass)")
       .HasComment("Sequência que identifica unicamente o registro dessa tabela.");
      entity.HasKey(e => e.Id);
      entity.Property(e => e.NoVariavel).IsRequired()
       .HasMaxLength(200)
       .HasColumnName("No_Variavel")
       .HasComment("Nome da variável.");
      entity.Property(e => e.CoTipoDado).IsRequired()
       .HasColumnName("Co_TipoDado")
       .HasComment(
          "Tipo de dado da variavel: 1-Tipo não mapeado, 2-void, 3-DTO Classe, 4-String, 5-Integer, 6-Long, 7-Double, 8-Float, 9-Boolean, 10-LocalDate, 11-LocalDateTime, 12-BigDecimal, 13-Object.");
      entity.Property(e => e.CoTipoEscopo).IsRequired()
       .HasColumnName("Co_TipoEscopo")
       .HasComment(
          "Tipo do escopo da variável: 1-Parâmetro da Stored Procedure, 2-Variável Local, 3-Parâmetro do Endpoint.");
      entity.Property(e => e.NuTamanho).IsRequired(false)
       .HasColumnName("Nu_Tamanho")
       .HasComment("Tamanho da variável.");
      entity.Property(e => e.IdStoredProcedure).IsRequired()
       .HasColumnName("Id_StoredProcedure")
       .HasComment("Stored procedure ao qual a variável pertence.");

      entity.HasOne(d => d.IdStoredProcedureNavigation)
       .WithMany(p => p.Variaveis)
       .HasForeignKey(d => d.IdStoredProcedure)
       .HasConstraintName("fk_variavel_storedprocedure")
       .OnDelete(DeleteBehavior.Cascade);
    });

    //OnModelCreatingPartial(modelBuilder);
  }

  //partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
