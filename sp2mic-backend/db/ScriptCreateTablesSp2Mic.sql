create table "Microsservico"
(
    "Id"                 serial
        constraint pk_microsservico
            primary key,
    "No_Microsservico"   varchar(200) not null
        constraint uk_nomicrosservico
            unique,
    "Sn_ProntoParaGerar" boolean      not null
);

        comment on table "Microsservico" is 'Tabela contendo as informações dos microsserviços que serão gerados.';

        comment on column "Microsservico"."Id" is 'Sequência que identifica unicamente o registro dessa tabela.';

        comment on column "Microsservico"."No_Microsservico" is 'Nome do microsserviço.';

        comment on column "Microsservico"."Sn_ProntoParaGerar" is 'Indicador se o microsserviço está pronto para ser gerado.';

alter table "Microsservico"
    owner to postgres;

create table "Tabela"
(
    "Id"        serial
        constraint pk_tabela
            primary key,
    "No_Tabela" varchar(200) not null
        constraint uk_notabela
            unique
);

        comment on table "Tabela" is 'Tabela contendo as informações das tabelas que as stored proceudres acessam.';

        comment on column "Tabela"."Id" is 'Sequência que identifica unicamente o registro dessa tabela".';

        comment on column "Tabela"."No_Tabela" is 'Nome da tabela.';

        comment on constraint uk_notabela on "Tabela" is 'Nome da tabela é único.';

alter table "Tabela"
    owner to postgres;

create table "DtoClasse"
(
    "Id"                 serial
        constraint pk_dtoclasse
            primary key,
    "No_DtoClasse"       varchar(200) not null,
    "Id_StoredProcedure" integer      not null
);

        comment on table "DtoClasse" is 'Tabela contendo as informações das classes DTO.';

        comment on column "DtoClasse"."Id" is 'Sequência que identifica unicamente o registro dessa tabela.';

        comment on column "DtoClasse"."No_DtoClasse" is 'Nome da classe do DTO.';

        comment on column "DtoClasse"."Id_StoredProcedure" is 'Stored Procedure ao qual a classe DTO pertence.';

alter table "DtoClasse"
    owner to postgres;

create table "Atributo"
(
    "Id"           serial
        constraint pk_atributo
            primary key,
    "No_Atributo"  varchar(200) not null,
    "Co_TipoDado"  integer      not null
        constraint ck_atributo_cotipodado
            check ("Co_TipoDado" = ANY (ARRAY [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13])),
    "Id_DtoClasse" integer      not null
        constraint fk_atributo_dtoclasse
            references "DtoClasse"
            on delete cascade
);

        comment on table "Atributo" is 'Tabela contendo as informações dos atributos de uma classe dto.';

        comment on column "Atributo"."Id" is 'Sequência que identifica unicamente o registro dessa tabela.';

        comment on column "Atributo"."No_Atributo" is 'Nome do atributo.';

        comment on column "Atributo"."Co_TipoDado" is 'Tipo de dado do atributo: 1-Tipo não mapeado, 2-void, 3-DTO Classe, 4-String, 5-Integer, 6-Long, 7-Double, 8-Float, 9-Boolean, 10-LocalDate, 11-LocalDateTime, 12-BigDecimal, 13-Object.';

        comment on constraint ck_atributo_cotipodado on "Atributo" is 'Domínio do campo CoTipoDado: 1-Tipo não mapeado, 2-void, 3-DTO Classe, 4-String, 5-Integer, 6-Long, 7-Double, 8-Float, 9-Boolean, 10-LocalDate, 11-LocalDateTime, 12-BigDecimal, 13-Object.';

        comment on column "Atributo"."Id_DtoClasse" is 'Classe DTO ao qual o atributo pertence.';

alter table "Atributo"
    owner to postgres;

create table "StoredProcedure"
(
    "Id"                  serial
        constraint pk_storedprocedure
            primary key,
    "No_StoredProcedure"  varchar(200)          not null,
    "No_Schema"           varchar(200)          not null,
    "Co_TipoDadoRetorno"  integer
        constraint ck_storedprocedure_cotipodadoretorno
            check ("Co_TipoDadoRetorno" = ANY (ARRAY [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13])),
    "Sn_Analisada"        boolean default false not null,
    "Sn_RetornoLista"     boolean,
    "Id_DtoClasse"        integer
        constraint fk_storedprocedure_dtoclasse
            references "DtoClasse"
            on delete set null,
    "Sn_SucessoParser"    boolean               not null,
    "Tx_ResultadoParser"  text                  not null,
    "Tx_Definicao"        text                  not null,
    "Tx_DefinicaoTratada" text                  not null,
    constraint uk_noschema_nostoredprocedure
        unique ("No_Schema", "No_StoredProcedure")
);

        comment on table "StoredProcedure" is 'Tabela contendo as informações das stored procedures.';

        comment on column "StoredProcedure"."Id" is 'Sequência que identifica unicamente o registro dessa tabela.';

        comment on column "StoredProcedure"."No_StoredProcedure" is 'Nome da stored procedure.';

        comment on column "StoredProcedure"."No_Schema" is 'Nome do schema da stored procedure.';

        comment on column "StoredProcedure"."Co_TipoDadoRetorno" is 'Tipo de dado retornado pela stored procedure: 1-Tipo não mapeado, 2-void, 3-DTO Classe, 4-String, 5-Integer, 6-Long, 7-Double, 8-Float, 9-Boolean, 10-LocalDate, 11-LocalDateTime, 12-BigDecimal, 13-Object.';

        comment on constraint ck_storedprocedure_cotipodadoretorno on "StoredProcedure" is 'Domínio do campo CoTipoDadoRetorno: 1-Tipo não mapeado, 2-void, 3-DTO Classe, 4-String, 5-Integer, 6-Long, 7-Double, 8-Float, 9-Boolean, 10-LocalDate, 11-LocalDateTime, 12-BigDecimal, 13-Object.';

        comment on column "StoredProcedure"."Sn_Analisada" is 'Indicador se a stored procedure já foi analisada.';

        comment on column "StoredProcedure"."Sn_RetornoLista" is 'Indicador se o retorno da stored procedure é uma lista ou não.';

        comment on column "StoredProcedure"."Id_DtoClasse" is 'Classe DTO que a stored procedure retorna.';

        comment on column "StoredProcedure"."Sn_SucessoParser" is 'Indicador se o parser da Stored Procedure foi realizado com sucesso ou não.';

        comment on column "StoredProcedure"."Tx_ResultadoParser" is 'Resultado do parse da stored procedure';

        comment on column "StoredProcedure"."Tx_Definicao" is 'Texto com a definição da stored procedure.';

        comment on column "StoredProcedure"."Tx_DefinicaoTratada" is 'Texto com a definição tratada da stored procedure.';

alter table "StoredProcedure"
    owner to postgres;

alter table "DtoClasse"
    add constraint fk_dtoclasse_storedprocedure
        foreign key ("Id_StoredProcedure") references "StoredProcedure"
            on delete cascade;

create table "StoredProcedure_Tabela"
(
    "Id_StoredProcedure" integer not null
        constraint fk_storedproceduretabela_storedprocedure
            references "StoredProcedure"
            on delete cascade,
    "Id_Tabela"          integer not null
        constraint fk_storedproceduretabela_tabela
            references "Tabela"
            on delete cascade,
    constraint pk_storedprocedure_tabela
        primary key ("Id_StoredProcedure", "Id_Tabela")
);

        comment on table "StoredProcedure_Tabela" is 'Tabela contendo relação das stored procedures com os tabelas.';

        comment on column "StoredProcedure_Tabela"."Id_StoredProcedure" is 'Identificador da stored procedure neste relacionamento.';

        comment on column "StoredProcedure_Tabela"."Id_Tabela" is 'Identificador da tabela neste relacionamento.';

alter table "StoredProcedure_Tabela"
    owner to postgres;

create table "Variavel"
(
    "Id"                 serial
        constraint pk_variavel
            primary key,
    "No_Variavel"        varchar(200) not null,
    "Co_TipoDado"        integer      not null
        constraint ck_variavel_cotipodado
            check ("Co_TipoDado" = ANY (ARRAY [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13])),
    "Co_TipoEscopo"      integer      not null
        constraint ck_variavel_cotipoescopo
            check ("Co_TipoEscopo" = ANY (ARRAY [1, 2, 3])),
    "Nu_Tamanho"         integer,
    "Id_StoredProcedure" integer      not null
        constraint fk_variavel_storedprocedure
            references "StoredProcedure"
            on delete cascade,
    constraint uk_variavel_nome_tipo_procedure
        unique ("No_Variavel", "Co_TipoDado", "Id_StoredProcedure")
);

        comment on table "Variavel" is 'Tabela contendo as informações das variáveis das stored procedures.';

        comment on column "Variavel"."Id" is 'Sequência que identifica unicamente o registro dessa tabela.';

        comment on column "Variavel"."No_Variavel" is 'Nome da variável.';

        comment on column "Variavel"."Co_TipoDado" is 'Tipo de dado da variavel: 1-Tipo não mapeado, 2-void, 3-DTO Classe, 4-String, 5-Integer, 6-Long, 7-Double, 8-Float, 9-Boolean, 10-LocalDate, 11-LocalDateTime, 12-BigDecimal, 13-Object.';

        comment on constraint ck_variavel_cotipodado on "Variavel" is 'Domínio do campo CoTipoDado: 1-Tipo não mapeado, 2-void, 3-DTO Classe, 4-String, 5-Integer, 6-Long, 7-Double, 8-Float, 9-Boolean, 10-LocalDate, 11-LocalDateTime, 12-BigDecimal, 13-Object.';

        comment on column "Variavel"."Co_TipoEscopo" is 'Tipo do escopo da variável: 1-Parâmetro da Stored Procedure, 2-Variável Local, 3-Parâmetro do Endpoint.';

        comment on constraint ck_variavel_cotipoescopo on "Variavel" is 'Domínio do campo CoTipoEscopo: 1-Parâmetro da Stored Procedure, 2-Variável Local, 3-Parâmetro do Endpoint.';

        comment on column "Variavel"."Nu_Tamanho" is 'Tamanho da variável.';

        comment on column "Variavel"."Id_StoredProcedure" is 'Stored procedure ao qual a variável pertence.';

        comment on constraint uk_variavel_nome_tipo_procedure on "Variavel" is 'Nome da variavel e seu tipo de dado é único por procedure.';

alter table "Variavel"
    owner to postgres;

create table "Endpoint"
(
    "Id"                   serial
        constraint pk_endpoint
            primary key,
    "No_MetodoEndpoint"    varchar(200),
    "No_Path"              varchar(200),
    "Co_TipoSqlDml"        integer not null
        constraint ck_endpoint_cotiposqldml
            check ("Co_TipoSqlDml" = ANY (ARRAY [1, 2, 3, 4, 5, 6, 7, 8, 9])),
    "Co_TipoDadoRetorno"   integer not null
        constraint ck_endpoint_cotipodadoretorno
            check ("Co_TipoDadoRetorno" = ANY (ARRAY [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13])),
    "Sn_RetornoLista"      boolean,
    "Sn_Analisado"         boolean not null,
    "Id_Microsservico"     integer
        constraint fk_endpoint_microsservico
            references "Microsservico"
            on delete set null,
    "Id_StoredProcedure"   integer not null
        constraint fk_endpoint_storedprocedure
            references "StoredProcedure"
            on delete cascade,
    "Id_DtoClasse"         integer
        constraint fk_endpoint_dtoclasse
            references "DtoClasse"
            on delete set null,
    "Id_VariavelRetornada" integer
        constraint fk_endpoint_variavel
            references "Variavel"
            on delete set null,
    "Tx_Endpoint"          text    not null,
    "Tx_EndpointTratado"   text    not null
);

        comment on table "Endpoint" is 'Tabela contendo as informações dos endpoints dos microsserviços.';

        comment on column "Endpoint"."Id" is 'Sequência que identifica unicamente o registro dessa tabela.';

        comment on column "Endpoint"."No_MetodoEndpoint" is 'Nome do metodo do endpoint.';

        comment on column "Endpoint"."No_Path" is 'Path do endpoint.';

        comment on column "Endpoint"."Co_TipoSqlDml" is 'Tipo do endpoint: 1-Tipo não mapeado, 2-Select, 3-Insert, 4-Update, 5-Delete, 6-Create, 7-Alter, 8-Drop, 9-Exec.';

        comment on constraint ck_endpoint_cotiposqldml on "Endpoint" is 'Domínio do campo CoTipoSqlDml: 1-Tipo não mapeado, 2-Select, 3-Insert, 4-Update, 5-Delete, 6-Create, 7-Alter, 8-Drop, 9-Exec.';

        comment on column "Endpoint"."Co_TipoDadoRetorno" is 'Tipo de dado de retorno do metodo: 1-Tipo não mapeado, 2-void, 3-DTO Classe, 4-String, 5-Integer, 6-Long, 7-Double, 8-Float, 9-Boolean, 10-LocalDate, 11-LocalDateTime, 12-BigDecimal, 13-Object.';

        comment on constraint ck_endpoint_cotipodadoretorno on "Endpoint" is 'Domínio do campo CoTipoDadoRetorno: 1-Tipo não mapeado, 2-void, 3-DTO Classe, 4-String, 5-Integer, 6-Long, 7-Double, 8-Float, 9-Boolean, 10-LocalDate, 11-LocalDateTime, 12-BigDecimal, 13-Object.';

        comment on column "Endpoint"."Sn_RetornoLista" is 'Indicador se o retorno e uma lista ou não.';

        comment on column "Endpoint"."Sn_Analisado" is 'Indicador se o endpoint foi analisado ou não pelo especialista.';

        comment on column "Endpoint"."Id_Microsservico" is 'Microsserviço ao qual o endpoint pertence.';

        comment on column "Endpoint"."Id_StoredProcedure" is 'Stored procedure a qual o endpoint pertence.';

        comment on column "Endpoint"."Id_DtoClasse" is 'Classe DTO que o endpoint retorna.';

        comment on column "Endpoint"."Id_VariavelRetornada" is 'Variável que o endpoint retorna.';

        comment on column "Endpoint"."Tx_Endpoint" is 'Texto do endpoint original.';

        comment on column "Endpoint"."Tx_EndpointTratado" is 'Texto do endpoint tratado.';

alter table "Endpoint"
    owner to postgres;

create table "Endpoint_Tabela"
(
    "Id_Endpoint" integer not null
        constraint fk_endpointtabela_endpoint
            references "Endpoint"
            on delete cascade,
    "Id_Tabela"   integer not null
        constraint fk_endpointtabela_tabela
            references "Tabela"
            on delete cascade,
    constraint pk_endpoint_tabela
        primary key ("Id_Endpoint", "Id_Tabela")
);

        comment on table "Endpoint_Tabela" is 'Tabela contendo relação dos endpoints com as tabelas.';

        comment on column "Endpoint_Tabela"."Id_Endpoint" is 'Identificador do endpoint neste relacionamento.';

        comment on column "Endpoint_Tabela"."Id_Tabela" is 'Identificador da tabela neste relacionamento.';

alter table "Endpoint_Tabela"
    owner to postgres;

create table "Endpoint_Variavel"
(
    "Id_Endpoint" integer not null
        constraint fk_endpointvariavel_endpoint
            references "Endpoint"
            on delete cascade,
    "Id_Variavel" integer not null
        constraint fk_endpointvariavel_variavel
            references "Variavel"
            on delete cascade,
    constraint pk_endpoint_variavel
        primary key ("Id_Endpoint", "Id_Variavel")
);

        comment on table "Endpoint_Variavel" is 'Tabela contendo relação dos endpoints com as variáveis.';

        comment on column "Endpoint_Variavel"."Id_Endpoint" is 'Identificador do endpoint neste relacionamento.';

        comment on column "Endpoint_Variavel"."Id_Variavel" is 'Identificador da variável neste relacionamento.';

alter table "Endpoint_Variavel"
    owner to postgres;

create table "Operando"
(
    "Id"              serial
        constraint pk_operando
            primary key,
    "Co_TipoOperando" integer not null
        constraint ck_operando_cotipooperando
            check ("Co_TipoOperando" = ANY (ARRAY [1, 2, 3, 4, 5, 6, 7, 8])),
    "Tx_Valor"        text,
    "Sn_Negacao"      boolean not null,
    "Id_Variavel"     integer
        constraint fk_operando_variavel
            references "Variavel"
            on delete cascade,
    "Id_Expressao"    integer,
    "Id_Endpoint"     integer
        constraint fk_operando_endpoint
            references "Endpoint"
            on delete cascade
);

        comment on table "Operando" is 'Tabela contendo as informações dos operandos das expressões.';

        comment on column "Operando"."Id" is 'Sequência que identifica unicamente o registro dessa tabela.';

        comment on column "Operando"."Co_TipoOperando" is 'Tipo de operandos: 1-Tipo não mapeado, 2-Constante, 3-Constante String, 4-Variável, 5-Expressão, 6-Endpoint, 7-Método, 8-Constante null.';

        comment on constraint ck_operando_cotipooperando on "Operando" is 'Domínio do campo CoTipoOperando: 1-Tipo não mapeado, 2-Constante, 3-Constante String, 4-Variável, 5-Expressão, 6-Endpoint, 7-Método, 8-Constante null.';

        comment on column "Operando"."Tx_Valor" is 'Valor do operando.';

        comment on column "Operando"."Sn_Negacao" is 'Indicador se o operando deve ser negado.';

        comment on column "Operando"."Id_Variavel" is 'Variável do operando.';

        comment on column "Operando"."Id_Expressao" is 'Expressão do operando.';

        comment on column "Operando"."Id_Endpoint" is 'Endpoint do operando.';

alter table "Operando"
    owner to postgres;

create table "Expressao"
(
    "Id"                  serial
        constraint pk_expressao
            primary key,
    "Co_TipoDadoRetorno"  integer not null
        constraint ck_expressao_cotipodadoretorno
            check ("Co_TipoDadoRetorno" = ANY (ARRAY [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13])),
    "Nu_OrdemExecucao"    integer not null,
    "Id_OperandoEsquerda" integer
        constraint fk_expressao_operandoesquerda
            references "Operando"
            on delete cascade,
    "Co_Operador"         integer
        constraint ck_expressao_cooperador
            check ("Co_Operador" = ANY (ARRAY [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15])),
    "Id_OperandoDireita"  integer
        constraint fk_expressao_operandodireita
            references "Operando"
            on delete cascade
);

        comment on table "Expressao" is 'Tabela contendo as informações das expressões.';

        comment on column "Expressao"."Id" is 'Sequência que identifica unicamente o registro dessa tabela.';

        comment on column "Expressao"."Co_TipoDadoRetorno" is 'Tipo de dado de retorno da expressão: 1-Tipo não mapeado, 2-void, 3-DTO Classe, 4-String, 5-Integer, 6-Long, 7-Double, 8-Float, 9-Boolean, 10-LocalDate, 11-LocalDateTime, 12-BigDecimal, 13-Object.';

        comment on constraint ck_expressao_cotipodadoretorno on "Expressao" is 'Domínio do campo CoTipoDadoRetorno: 1-Tipo não mapeado, 2-void, 3-DTO Classe, 4-String, 5-Integer, 6-Long, 7-Double, 8-Float, 9-Boolean, 10-LocalDate, 11-LocalDateTime, 12-BigDecimal, 13-Object.';

        comment on column "Expressao"."Nu_OrdemExecucao" is 'Ordem de execução da expressão.';

        comment on column "Expressao"."Id_OperandoEsquerda" is 'Operando do lado esquerdo da expressão.';

        comment on column "Expressao"."Co_Operador" is 'Tipo de operador: 1-Tipo não mapeado, 2-Adição, 3-Subtração, 4-Divisão, 5-Multiplicação, 6-Maior que, 7-Menor que, 8-Maior igual, 9-Menor igual, 10-Igual, 11-Diferente, 12-E, 13-Ou, 14-Exists, 15-Atribuição.';

        comment on constraint ck_expressao_cooperador on "Expressao" is 'Domínio do campo Co_Operador: 1-Tipo não mapeado, 2-Adição, 3-Subtração, 4-Divisão, 5-Multiplicação, 6-Maior que, 7-Menor que, 8-Maior igual, 9-Menor igual, 10-Igual, 11-Diferente, 12-E, 13-Ou, 14-Exists, 15-Atribuição.';

        comment on column "Expressao"."Id_OperandoDireita" is 'Operando do lado direito da expressão.';

alter table "Expressao"
    owner to postgres;

alter table "Operando"
    add constraint fk_operando_expressao
        foreign key ("Id_Expressao") references "Expressao"
            on delete cascade;

create table "Comando"
(
    "Id"                   serial
        constraint pk_comando
            primary key,
    "Co_TipoComando"       integer not null
        constraint ck_comando_cotipocomando
            check ("Co_TipoComando" = ANY (ARRAY [1, 2, 3, 4, 5, 6, 7, 8, 9])),
    "Nu_OrdemExecucao"     integer not null,
    "Vl_AtribuidoVariavel" varchar(100),
    "Id_StoredProcedure"   integer not null
        constraint fk_comando_storedprocedure
            references "StoredProcedure"
            on delete cascade,
    "Id_ComandoOrigem"     integer
        constraint fk_comando_comandoorigem
            references "Comando"
            on delete cascade,
    "Id_Endpoint"          integer
        constraint fk_comando_endpoint
            references "Endpoint"
            on delete cascade,
    "Id_Expressao"         integer
        constraint fk_comando_expressao
            references "Expressao"
            on delete cascade,
    "Sn_CondicaoOrigem"    boolean,
    "Tx_Comando"           text    not null,
    "Tx_ComandoTratado"    text    not null
);

        comment on table "Comando" is 'Tabela contendo as informações dos comandos das stored procedures.';

        comment on column "Comando"."Id" is 'Sequência que identifica unicamente o registro dessa tabela.';

        comment on column "Comando"."Co_TipoComando" is 'Tipo da instrução do comando: 1-Tipo não mapeado, 2-endpoint, 3-declaração, 4-atribuição, 5-if, 6-while, 7-Bloco de comandos, 8-Execute Statemen, 9-Begin Transaction Statement.';

        comment on constraint ck_comando_cotipocomando on "Comando" is 'Domínio do campo CoTipoComando: 1-Tipo não mapeado, 2-endpoint, 3-declaração, 4-atribuição, 5-if, 6-while, 7-Bloco de comandos, 8-Execute Statemen, 9-Begin Transaction Statement.';

        comment on column "Comando"."Nu_OrdemExecucao" is 'Ordem de execução do comando na stored procedure.';

        comment on column "Comando"."Vl_AtribuidoVariavel" is 'Valor atribuido a variável declarada no comando.';

        comment on column "Comando"."Id_StoredProcedure" is 'Stored procedure que o comando pertence.';

        comment on column "Comando"."Id_ComandoOrigem" is 'Comando que dar origem a outros comando.';

        comment on column "Comando"."Id_Endpoint" is 'Endpoint chamado no comando.';

        comment on column "Comando"."Id_Expressao" is 'Expressão do comando.';

        comment on column "Comando"."Sn_CondicaoOrigem" is 'Indicador da situação de um comando if';

        comment on column "Comando"."Tx_Comando" is 'Texto do comando.';

        comment on column "Comando"."Tx_ComandoTratado" is 'Texto do comando tratado.';

alter table "Comando"
    owner to postgres;

create table "Comando_Variavel"
(
    "Id"          serial
        constraint pk_comando_variavel
            primary key,
    "Id_Comando"  integer not null
        constraint fk_comandovariavel_comando
            references "Comando"
            on delete cascade,
    "Id_Variavel" integer not null
        constraint fk_comandovariavel_variavel
            references "Variavel"
            on delete cascade,
    "Nu_Ordem"    integer not null
);

        comment on table "Comando_Variavel" is 'Tabela contendo relação dos comandos com as variáveis.';

        comment on column "Comando_Variavel"."Id" is 'Sequência que identifica unicamente o registro dessa tabela.';

        comment on column "Comando_Variavel"."Id_Comando" is 'Identificador do comando neste relacionamento.';

        comment on column "Comando_Variavel"."Id_Variavel" is 'Identificador da variável neste relacionamento.';

        comment on column "Comando_Variavel"."Nu_Ordem" is 'Ordem de execução da variável.';

alter table "Comando_Variavel"
    owner to postgres;
