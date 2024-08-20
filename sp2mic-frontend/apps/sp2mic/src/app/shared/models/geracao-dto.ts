import { SpringBootDto } from './spring-boot-dto';

export class GeracaoDto extends SpringBootDto {
  orchestratorPort: string;
  gatewayHost: string;
  gatewayPort: string;
  consulHost: string;
  consulPort: string;
  databaseHost: string;
  databasePort: string;
  databaseName: string;
  databaseUserName: string;
  databasePassword: string;
}
