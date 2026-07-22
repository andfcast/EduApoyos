import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ListaSolicitudesComponent } from './lista-solicitudes-component';

describe('ListaSolicitudesComponent', () => {
  let component: ListaSolicitudesComponent;
  let fixture: ComponentFixture<ListaSolicitudesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ListaSolicitudesComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(ListaSolicitudesComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
