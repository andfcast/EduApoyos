import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ListaEstudiantesComponent } from './lista-estudiantes-component';

describe('ListaEstudiantesComponent', () => {
  let component: ListaEstudiantesComponent;
  let fixture: ComponentFixture<ListaEstudiantesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ListaEstudiantesComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(ListaEstudiantesComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
