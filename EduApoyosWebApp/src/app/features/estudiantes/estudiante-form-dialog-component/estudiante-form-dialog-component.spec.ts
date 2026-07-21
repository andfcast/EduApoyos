import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EstudianteFormDialogComponent } from './estudiante-form-dialog-component';

describe('EstudianteFormDialogComponent', () => {
  let component: EstudianteFormDialogComponent;
  let fixture: ComponentFixture<EstudianteFormDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EstudianteFormDialogComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(EstudianteFormDialogComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
