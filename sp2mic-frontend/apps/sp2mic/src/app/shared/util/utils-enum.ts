import _ from 'lodash';
import { TipoDadoEnum } from '../enums/tipo-dado-enum';
import { TipoEndpointEnum } from '../enums/tipo-endpoint-enum';
import { ComboBoxDto } from '../models/combo-box-dto';

export class UtilsEnum {
  /**
   * Retorna a lista de números ordenada pelo nome do rótulo fornecido pela função.
   * @param original o array de números original.
   * @param fn
   */
  public static ordenarNumerosPorNomeDaFuncao(
    original: number[],
    fn: (n: number) => string,
  ): number[] {
    let retorno: { nome: string; valor: number }[];
    const makeMap = (m: number) => {
      return { nome: fn(m), valor: m };
    };
    retorno = original.map(makeMap);
    retorno = _.orderBy(
      retorno,
      [(m: { nome: string; valor: number }) => m.nome?.toLocaleLowerCase()],
      ['asc'],
    );

    return retorno.map((r) => r.valor);
  }

  public static retornaRotuloTipoSqlDml(tipo: number): string {
    if (tipo === undefined) {
      return '';
    }
    return TipoEndpointEnum.SELECT === tipo
      ? 'SELECT'
      : TipoEndpointEnum.INSERT === tipo
        ? 'INSERT'
        : TipoEndpointEnum.UPDATE === tipo
          ? 'UPDATE'
          : TipoEndpointEnum.DELETE === tipo
            ? 'DELETE'
            : '';
  }

  /**
   * Obtem o rotulo pertinente situação do Arquivo de Provisão para apresentação na tela.
   * @param tipoDado o identificador da situação do Arquivo de Provisão.
   */
  public static retornaRotuloTipoDado(tipoDado: number): string {
    if (tipoDado === undefined) {
      return '';
    }
    return TipoDadoEnum.DTO === tipoDado
      ? 'DTO Class'
      : TipoDadoEnum.VOID === tipoDado
        ? 'void'
        : TipoDadoEnum.STRING === tipoDado
          ? 'String'
          : TipoDadoEnum.INTEGER === tipoDado
            ? 'Integer'
            : TipoDadoEnum.LONG === tipoDado
              ? 'Long'
              : TipoDadoEnum.DOUBLE === tipoDado
                ? 'Double'
                : TipoDadoEnum.FLOAT === tipoDado
                  ? 'Float'
                  : TipoDadoEnum.BOOLEAN === tipoDado
                    ? 'Boolean'
                    : TipoDadoEnum.LOCAL_DATE === tipoDado
                      ? 'LocalDate'
                      : TipoDadoEnum.LOCAL_DATE_TIME === tipoDado
                        ? 'LocalDateTime'
                        : TipoDadoEnum.BIG_DECIMAL === tipoDado
                          ? 'BigDecimal'
                          : TipoDadoEnum.OBJECT === tipoDado
                            ? 'Object'
                            : 'tipoDado = ' + tipoDado;
  }

  public static recuperarValoresComboTipoDado(): ComboBoxDto[] {
    let valoresComboTipoDado: ComboBoxDto[];
    let valoresTipoDado = Object.values(TipoDadoEnum).filter(
      Number,
    ) as number[];
    const makeMap = (i: number) => {
      return new ComboBoxDto(i, this.retornaRotuloTipoDado(i));
    };
    valoresComboTipoDado = valoresTipoDado.map(makeMap);
    valoresComboTipoDado = _.orderBy(
      valoresComboTipoDado,
      [(tipo: ComboBoxDto) => tipo.nome?.toLocaleLowerCase()],
      ['asc'],
    );
    return valoresComboTipoDado;
  }
}
