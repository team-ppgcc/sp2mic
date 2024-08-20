import { DadosConexaoDto } from './dados-conexao-dto';
import { ParDto } from './par-dto';
import { TipoBancoDeDados } from '../enums/tipo-banco-de-dados';

export class CargaDto {
  tipoBancoDeDados: TipoBancoDeDados;
  schema?: string;
  nomeProcedure: string;
  nomesProcedures: ParDto[] = [];
  dadosConexao? = new DadosConexaoDto();
  snCarregada?: boolean;
  sendProcedureFile: boolean;
}
