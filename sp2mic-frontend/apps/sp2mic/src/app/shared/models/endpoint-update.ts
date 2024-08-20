import { DtoClasseUpdateDto } from './dto-classe-update-dto';
import { TipoDadoEnum } from '../enums/tipo-dado-enum';

export class EndpointUpdate {
  noMetodoEndpoint: string;
  noPath: string;
  txEndpointTratado: string;
  coTipoDadoRetorno: TipoDadoEnum;
  snRetornoLista: boolean;
  snAnalisado: boolean;
  idMicrosservico: number;
  idDtoClasse?: number;
  idDtoClasseNavigation?: DtoClasseUpdateDto;
}
