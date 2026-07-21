import { Component, Inject, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatButtonModule } from '@angular/material/button';
import { RegistroEstudiante } from '../../../core/models/estudiante.models';
import { ProgramaAcademico, TipoDocumento } from '../../../core/models/general.models';
import { MatIconModule } from '@angular/material/icon';
import { CatalogoService } from '../../../core/services/tipo-documento.service';

@Component({
  selector: 'app-estudiante-form-dialog-component',
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatSelectModule,
    MatSlideToggleModule,
    MatButtonModule
  ],
  templateUrl: './estudiante-form-dialog-component.html',
  styleUrl: './estudiante-form-dialog-component.css',
})
export class EstudianteFormDialogComponent implements OnInit {
  private fb = inject(FormBuilder);
  private catalogoService = inject(CatalogoService);
  dialogRef = inject(MatDialogRef<EstudianteFormDialogComponent>);

  isEditMode = false;
  form: FormGroup;
  hidePassword = signal<boolean>(true);
  private passwordPattern = '^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&._-]).{8,}$';

  tiposDocumento = signal<TipoDocumento[]>([]);
  programasAcademicos = signal<ProgramaAcademico[]>([]);

  constructor(@Inject(MAT_DIALOG_DATA) public data?: RegistroEstudiante) {
    this.isEditMode = !!data;

    const passwordValidators = this.isEditMode
      ? [Validators.pattern(this.passwordPattern)]
      : [Validators.required, Validators.pattern(this.passwordPattern)];


    this.form = this.fb.group({
      tipoDocumento: [data?.tipoDocumentoId || 1, [Validators.required]],
      numeroDocumento: [data?.numeroDocumento || '', [Validators.required, Validators.pattern('^[0-9]+$')]],
      nombreCompleto: [data?.nombreCompleto || '', [Validators.required, Validators.minLength(3)]],
      correo: [data?.email || '', [Validators.required, Validators.email]],      
      password: ['', passwordValidators],
      programaAcademico: [data?.programaAcademico || '', [Validators.required]],
      semestre: [data?.semestre || 1, [Validators.required, Validators.min(1), Validators.max(12)]],      
      activo: [data?.activo ?? true, null]      
    });
  }

  ngOnInit(): void {
    this.cargarCatalogos();
  }

  togglePasswordVisibility(): void {
    this.hidePassword.update(value => !value);
  }
  private cargarCatalogos(): void {
    this.catalogoService.obtenerTiposDocumento().subscribe({
      next: (tipos) => this.tiposDocumento.set(tipos),
      error: (err) => console.error('Error cargando tipos de documento', err)
    });

    this.catalogoService.obtenerProgramasAcademicos().subscribe({
      next: (programas) => this.programasAcademicos.set(programas),
      error: (err) => console.error('Error cargando programas académicos', err)
    });
  }

  guardar(): void {
    if (this.form.valid) {
      const formValue = { ...this.form.value };
      if (this.isEditMode && !formValue.password) {
        delete formValue.password;
      } 
      this.dialogRef.close(this.form.value);
    }
  }
}
