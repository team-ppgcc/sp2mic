import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-crumb',
  templateUrl: './crumb.component.html',
  styleUrls: ['./crumb.component.scss'],
})
export class CrumbComponent implements OnInit {
  @Input() order = '1';
  @Input() label = '## CONFIGURE O CRUMB ##';
  @Input() route = '';

  constructor() {
    //Construtor vazio Intencional. Retirar Comentário somente caso seja inicializado.
  }

  ngOnInit(): void {
    //Construtor vazio Intencional. Retirar Comentário somente caso seja inicializado.
  }
}
