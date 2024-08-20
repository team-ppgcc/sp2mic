import { Injectable } from '@angular/core';

interface MenuItem {
  id: string;
  label: string;
  title: string;
  route: string;
}

interface Submenu {
  id: string;
  label: string;
  icon: any;
  title: string;
  items: MenuItem[];
  active?: false;
}

interface MenuData {
  submenus: Submenu[];
}

@Injectable({
  providedIn: 'root',
})
export class MainMenuComponent {
  public expanded = false;

  public menuData: MenuData;

  ouvintes: MenuOuvinte[] = [];

  constructor() {}

  public show() {
    this.expanded = true;
    this.ouvintes.forEach((o) => o.notificar());
  }

  public hide() {
    this.expanded = false;
    this.ouvintes.forEach((o) => o.notificar());
  }

  public toggle() {
    this.expanded = !this.expanded;
    this.ouvintes.forEach((o) => o.notificar());
  }

  init() {
    this.criarMenuData();
  }

  criarMenuData() {
    this.menuData = {
      submenus: [
        {
          // SubMenu Carga
          icon: 'cloud_download',
          title: 'Carga',
          id: 'modulo-carga',
          label: 'Carga',
          items: [
            {
              id: 'carregar-procedure',
              label: 'Stored Procedures',
              title: 'Stored Procedures',
              route: '/load/database-connection',
            },
          ],
        },
        {
          // Submenu Análise
          id: 'modulo-analise',
          label: 'Análise',
          icon: 'assessment',
          title: 'Análise',
          items: [
            {
              id: 'stored-procedures',
              label: 'Stored Procedures',
              title: 'Features of stored procedure',
              route: '/analysis/stored-procedure',
            },
            {
              id: 'endpoints',
              label: 'Endpoints',
              title: 'Features of endpoint',
              route: '/analysis/endpoint',
            },
            {
              id: 'microsservicos',
              label: 'Microservices',
              title: 'Features of microservice',
              route: '/analysis/microsservico',
            },
            {
              id: 'tabelas',
              label: 'Tables',
              title: 'Features of tables',
              route: '/analysis/requisito',
            },
            {
              id: 'classes',
              label: 'Classes',
              title: 'Features of DTO Classes',
              route: '/analysis/dto-classe',
            },
            {
              id: 'atributos',
              label: 'Attributes',
              title: 'Features of attributes',
              route: '/analysis/atributo',
            }, // {
            //   id: 'comandos',
            //   label: 'Comandos',
            //   title: 'Features of do Comando',
            //   route: '/analysis/comando',
            // },
            // {
            //   id: 'expressao',
            //   label: 'Expressões',
            //   title: 'Features of do Expressao',
            //   route: '/analysis/expressao',
            // },
            // {
            //   id: 'operando',
            //   label: 'Operandos',
            //   title: 'Features of do Operando',
            //   route: '/analysis/operando',
            // },
            // {
            //   id: 'variavel',
            //   label: 'Variável',
            //   title: 'Features of da Variável',
            //   route: '/analysis/variavel',
            // }
          ],
        },
        {
          // SubMenu Geracao
          // icon: 'grid_view',
          icon: 'class',
          title: 'Features of generation',
          id: 'modulo-geracao',
          label: 'Generation',
          items: [
            {
              id: 'gerar-microsservicos',
              label: 'Microsserviços',
              title: 'Microsserviços',
              route: '/generation/microsservicos',
            },
          ],
        },
      ],
    };
  }

  public registrarOuvinte(ouvinte: MenuOuvinte) {
    this.ouvintes.push(ouvinte);
  }
}

export interface MenuOuvinte {
  notificar(): void;
}
