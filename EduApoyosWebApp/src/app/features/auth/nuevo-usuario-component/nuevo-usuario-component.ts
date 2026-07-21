import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

// Material Imports
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

import { UsuarioService } from '../../../core/services/usuario.service';
import { TipoDocumentoService } from '../../../core/services/tipo-documento.service';
import { TipoDocumento } from '../../../core/models/general.models';

@Component({
  selector: 'app-nuevo-usuario-component',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatProgressSpinnerModule,
    MatSnackBarModule
  ],
  templateUrl: './nuevo-usuario-component.html',
  styleUrls: ['./nuevo-usuario-component.css'],
})
export class NuevoUsuarioComponent implements OnInit {
  private fb = inject(FormBuilder);
  private usuarioService = inject(UsuarioService);
  private tipoDocumentoService = inject(TipoDocumentoService);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);

  tiposDocumento = signal<TipoDocumento[]>([]);
  isLoading = signal<boolean>(false);

  // Expresión regular para contraseña segura (Alineada a la regla del backend)
  // Al menos: 8 caracteres, 1 mayúscula, 1 minúscula, 1 número y 1 carácter especial
  private passwordPattern = '^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&._-]).{8,}$';

  registerForm: FormGroup = this.fb.group({
    nombreCompleto: ['', [Validators.required, Validators.minLength(3)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.pattern(this.passwordPattern)]],
    tipoDocumentoId: ['', [Validators.required]],
    numeroDocumento: ['', [Validators.required, Validators.pattern('^[0-9]+$')]],
    programaAcademico: ['', [Validators.required]],
    semestre: ['', [Validators.required, Validators.min(1), Validators.max(12)]]
  });

  ngOnInit(): void {
    this.cargarTiposDocumento();
  }

  private cargarTiposDocumento(): void {
    this.tipoDocumentoService.obtenerTodos().subscribe({
      next: (tipos) => this.tiposDocumento.set(tipos),
      error: () => {
        this.snackBar.open('Error al cargar la lista de tipos de documento.', 'Cerrar', { duration: 4000 });
      }
    });
  }

  onSubmit(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    this.isLoading.set(true);

    this.usuarioService.registrar(this.registerForm.value).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.snackBar.open('Estudiante registrado con éxito. Ya puedes iniciar sesión.', 'Ok', { duration: 4000 });
        this.router.navigate(['/login']);
      },
      error: (err) => {
        this.isLoading.set(false);
        const errorMsg = err.error?.message || 'Error al completar el registro.';
        this.snackBar.open(errorMsg, 'Cerrar', { duration: 4000 });
      }
    });
  }
}
