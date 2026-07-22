import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetalleSolicitudDialogComponent } from './detalle-solicitud-dialog-component';

describe('DetalleSolicitudDialogComponent', () => {
  let component: DetalleSolicitudDialogComponent;
  let fixture: ComponentFixture<DetalleSolicitudDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DetalleSolicitudDialogComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(DetalleSolicitudDialogComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
