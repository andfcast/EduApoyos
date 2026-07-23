import { Component,OnInit, inject, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

// Angular Material Imports
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { UsuarioService } from '../../../core/services/usuario.service';
import { RolService } from '../../../core/services/rol.service';
import { Rol } from '../../../core/models/usuario.models';
import { MatIconModule } from '@angular/material/icon';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-registrar-usuario-component',
  imports: [    
    ReactiveFormsModule,
    RouterLink,
    MatCardModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,    
    MatProgressSpinnerModule
  ],
  templateUrl: './registrar-usuario-component.html',
  styleUrl: './registrar-usuario-component.css',
})
export class RegistrarUsuarioComponent implements OnInit {
  private fb = inject(FormBuilder);
  private usuarioService = inject(UsuarioService);
  private rolService = inject(RolService);
  private router = inject(Router);
  
  roles = signal<Rol[]>([]);
  isLoading = signal<boolean>(false);
  hidePassword = signal<boolean>(true);

  // Mismo patrón de contraseña requerido por el backend
  private passwordPattern = '^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&._-]).{8,}$';

  registerForm: FormGroup = this.fb.group({
    nombreCompleto: ['', [Validators.required, Validators.minLength(3)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.pattern(this.passwordPattern)]],
    rolId: ['', [Validators.required]]
  });

  ngOnInit(): void {
    this.cargarRoles();
  }

  togglePasswordVisibility(): void {
    this.hidePassword.update((visible) => !visible);
  }

  private cargarRoles(): void {
    this.rolService.obtenerTodos().subscribe({
      next: (listaRoles) => this.roles.set(listaRoles),
      error: () => {
        Swal.fire({
              title: 'Error',
              text: 'Error al cargar la lista de roles desde el servidor.',
              timer: 4000,
              icon: 'error'
        });          
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
        Swal.fire({
            icon:'success',              
            text: `Usuario creado con éxito. Ya puedes iniciar sesión.`,
            timer: 4000
          });          
        this.router.navigate(['/login']);
      },
      error: (err) => {
        this.isLoading.set(false);
        const errorMsg = err.error?.message || 'Error al completar el registro.';
        Swal.fire({
              title: 'Error',
              text: errorMsg,
              timer: 4000,
              icon: 'error'
        });        
      }
    });
  }
}