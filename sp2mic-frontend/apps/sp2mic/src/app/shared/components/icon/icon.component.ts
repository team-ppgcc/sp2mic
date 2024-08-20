import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-icon',
  templateUrl: './icon.component.html',
})
export class IconComponent implements OnInit {
  static readonly LIBS = ['fa', 'far', 'fas', 'fal', 'fab', 'ion'];

  @Input() lib = 'fas';
  @Input() name: string | undefined;
  @Input() styleClass: string | undefined;
  @Input() style: any;

  constructor() {
    //Construtor vazio Intencional. Retirar Comentário somente caso seja inicializado.
  }

  ngOnInit(): void {
    if (IconComponent.LIBS.indexOf(this.lib) === -1) {
      this.lib = 'fas';
    }
  }
}
