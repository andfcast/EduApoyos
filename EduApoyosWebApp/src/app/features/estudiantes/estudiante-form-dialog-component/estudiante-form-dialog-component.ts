import { Component, Inject, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatButtonModule } from '@angular/material/button';
import { EstudianteService } from '../../../core/services/estudiante.service';
import { RegistroEstudiante } from '../../../core/models/estudiante.models';

@Component({
  selector: 'app-estudiante-form-dialog-component',
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatSlideToggleModule,
    MatButtonModule
  ],
  templateUrl: './estudiante-form-dialog-component.html',
  styleUrl: './estudiante-form-dialog-component.css',
})
export class EstudianteFormDialogComponent {
  private fb = inject(FormBuilder);
  dialogRef = inject(MatDialogRef<EstudianteFormDialogComponent>);

  isEditMode = false;
  form: FormGroup;

  constructor(@Inject(MAT_DIALOG_DATA) public data?: RegistroEstudiante) {
    this.isEditMode = !!data;

    this.form = this.fb.group({
      tipoDocumento: [data?.tipoDocumentoId || 1, [Validators.required]],
      numeroDocumento: [data?.numeroDocumento || '', [Validators.required, Validators.pattern('^[0-9]+$')]],
      nombreCompleto: [data?.nombreCompleto || '', [Validators.required, Validators.minLength(3)]],
      correo: [data?.email || '', [Validators.required, Validators.email]],      
      programaAcademico: [data?.programaAcademico || '', [Validators.required]],
      semestre: [data?.semestre || 1, [Validators.required, Validators.min(1), Validators.max(12)]],      
    });
  }

  guardar(): void {
    if (this.form.valid) {
      this.dialogRef.close(this.form.value);
    }
  }
}
