import { map, startWith, take, takeUntil } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { DtoClasse } from '../../models/dto-classe';
import { FormControl } from '@angular/forms';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import * as _ from 'lodash';
import { DtoClasseService } from '../../../pages/2-analise/dto-classe/dto-classe.service';
import { Unsub } from '../../util/unsub.class';

@Component({
  selector: 'app-dto-classe-autocomplete',
  templateUrl: './classe-autocomplete.component.html',
  styleUrls: ['./classe-autocomplete.component.scss'],
})
export class ClasseAutocompleteComponent extends Unsub implements OnInit {
  classes: DtoClasse[] = [];
  classeSelecionada = new DtoClasse();
  formControlClasse = new FormControl('');
  classesFiltradas: Observable<DtoClasse[]>;

  @Input() obrigatorio: boolean;
  @Output() classeEvento = new EventEmitter<DtoClasse>();

  constructor(private dtoClasseService: DtoClasseService) {
    super();
  }

  ngOnInit(): void {
    this.recuperaClasses();
  }

  recuperaClasses(): void {
    const classe: DtoClasse = new DtoClasse();

    const apiData = this.dtoClasseService.findByFilter(classe);

    const apiObserver = {
      next: (classes: DtoClasse[]) => {
        this.recuperaClassesFiltradas(classes);
      },
      error: (erro: any) => {
        console.error(erro);
      },
      complete: () => console.log('Observable completed'),
    };

    apiData.pipe(takeUntil(this.unsubscribe$)).subscribe(apiObserver);
  }

  mudarClasse(classeSelecionada: any): void {
    if (classeSelecionada) {
      this.classeSelecionada = classeSelecionada;
      this.classeEvento.emit(this.classeSelecionada);
    } else {
      this.classeSelecionada = null;
      this.formControlClasse.setValue(null);
      this.classeEvento.emit(null);
    }
  }

  alterarClasseInput(): void {
    if (!this.formControlClasse.value) {
      this.classeSelecionada = null;
      this.classeEvento.emit(null);
    }
    if (this.classeSelecionada === null) {
      this.formControlClasse.setValue(null);
    }
  }

  displayClasse(classe: DtoClasse): string {
    this.classeSelecionada = { ...classe };
    return classe && classe.noDtoClasse ? classe.noDtoClasse : '';
  }

  private filtrarClasses(): DtoClasse[] {
    return this.classes.filter((classe) => classe.noDtoClasse.toLowerCase());
  }

  private recuperaClassesFiltradas(classes: DtoClasse[]): void {
    this.classes = classes;
    this.classes = _.orderBy(
      this.classes,
      [(i) => i.noDtoClasse?.toLocaleLowerCase()],
      ['asc'],
    );
    this.classesFiltradas = this.formControlClasse.valueChanges.pipe(
      startWith(''),
      map((value) => {
        const nome =
          typeof value === 'string' ? value : (value as any)?.noDtoClasse;
        return nome
          ? this.filtrarClasses()
          : this.classes.slice();
      }),
    );
  }
}
