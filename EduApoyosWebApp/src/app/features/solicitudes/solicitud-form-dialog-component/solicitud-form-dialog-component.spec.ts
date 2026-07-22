import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SolicitudFormDialogComponent } from './solicitud-form-dialog-component';

describe('SolicitudFormDialogComponent', () => {
  let component: SolicitudFormDialogComponent;
  let fixture: ComponentFixture<SolicitudFormDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SolicitudFormDialogComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(SolicitudFormDialogComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
