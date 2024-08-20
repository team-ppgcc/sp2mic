import { Component, Input } from '@angular/core';

export interface Crumb {
  label: string;
  route: string;
}

@Component({
  selector: 'app-breadcrumb',
  templateUrl: './breadcrumb.component.html',
  styleUrls: ['./breadcrumb.component.scss'],
})
export class BreadcrumbComponent {
  @Input() crumbs: Crumb[] | undefined;
  @Input() currentLabel = 'CONFIGURE O CURRENT LABEL DO BREADCRUMB';

  constructor() {
    //Construtor vazio Intencional. Retirar Comentário somente caso seja inicializado.
  }
}
