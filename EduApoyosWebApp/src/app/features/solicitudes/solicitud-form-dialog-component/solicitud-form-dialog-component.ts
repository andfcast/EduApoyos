import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

import { CatalogoService } from '../../../core/services/catalogo.service';
import { EstudianteService } from '../../../core/services/estudiante.service';
import { AuthService } from '../../../core/services/auth.service';
import { EstudianteCombo } from '../../../core/models/estudiante.models';
import { TipoApoyo } from '../../../core/models/general.models';

@Component({
  selector: 'app-solicitud-form-dialog-component',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,    
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './solicitud-form-dialog-component.html',
  styleUrl: './solicitud-form-dialog-component.css',
})
export class SolicitudFormDialogComponent implements OnInit {
  private fb = inject(FormBuilder);
  private catalogosService = inject(CatalogoService);
  private estudiantesService = inject(EstudianteService);
  public authService = inject(AuthService);
  dialogRef = inject(MatDialogRef<SolicitudFormDialogComponent>);

  form!: FormGroup;

  // Signals para poblar los combox
  estudiantes = signal<EstudianteCombo[]>([]);
  tiposApoyo = signal<TipoApoyo[]>([]);

  ngOnInit(): void {
    this.initForm();
    this.cargarCombos();
  }

  private initForm(): void {    
    const asesorId = this.authService.esAsesor() ? this.authService.getUserId() : '00000000-0000-0000-0000-000000000000';
    const estudianteId = this.authService.esEstudiante() ? this.authService.getUserId() : '';
    
    this.form = this.fb.group({
      id: [crypto.randomUUID()],
      estudianteId: [estudianteId, [Validators.required]],
      tipoApoyoId: ['', [Validators.required]],
      montoSolicitado: [0, [Validators.required, Validators.min(1)]],
      descripcion: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(1000)]],
      estadoId: [1],
      asesorId: [asesorId]
    });
  }

  private cargarCombos(): void {
    // Cargar Estudiantes
    if(this.authService.esAsesor()) {
      this.estudiantesService.obtenerTodosCombo().subscribe({
        next: (list) => this.estudiantes.set(list),
        error: (err) => console.error('Error cargando estudiantes', err)
      });
    }

    // Cargar Tipos de Apoyo
    this.catalogosService.obtenerTiposApoyo().subscribe({
      next: (list) => this.tiposApoyo.set(list),
      error: (err) => console.error('Error cargando tipos de apoyo', err)
    });
  }

  guardar(): void {
    if (this.form.valid) {
      this.dialogRef.close(this.form.value);
    }
  }}
