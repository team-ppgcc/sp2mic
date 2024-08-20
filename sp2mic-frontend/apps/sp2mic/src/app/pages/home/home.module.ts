import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HomeComponent } from './home.component';
import { SharedModule } from '../../shared/shared.module';
import { MaterialModule } from '../../material.module';

@NgModule({
  declarations: [HomeComponent],
  imports: [
    FormsModule,
    ReactiveFormsModule,
    CommonModule,
    SharedModule,
    MaterialModule,
    RouterModule.forChild([
      {
        path: '',
        data: ['Inicio'],
        component: HomeComponent,
      },
    ]),
  ],
})
export class HomeModule {}
