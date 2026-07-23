import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatChipsModule } from '@angular/material/chips';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';

import { EstudianteService } from '../../../core/services/estudiante.service';
import { Estudiante } from '../../../core/models/estudiante.models';
import { EstudianteFormDialogComponent } from '../estudiante-form-dialog-component/estudiante-form-dialog-component';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-lista-estudiantes-component',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatDialogModule,    
    MatChipsModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatCardModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './lista-estudiantes-component.html',
  styleUrl: './lista-estudiantes-component.css',
})
export class ListaEstudiantesComponent implements OnInit {
  private estudiantesService = inject(EstudianteService);
  private dialog = inject(MatDialog);  

  // Signals para reactividad
  isLoading = signal<boolean>(false);
  estudiantes = signal<Estudiante[]>([]);
  totalRegistros = signal<number>(0);

  // Parámetros de paginación servidor
  paginaActual = 1; // .NET usa base 1
  tamanoPagina = 10;
  textoBusqueda = '';

  // Subject para debounce en la caja de búsqueda
  private buscadorSubject = new Subject<string>();

  displayedColumns: string[] = [
    'documento',
    'nombreCompleto',
    'correo',
    'programa',
    'semestre',
    'estado',
    'acciones'
  ];

  ngOnInit(): void {
    this.cargarEstudiantes();

    // Debounce de 400ms al escribir en el filtro de texto
    this.buscadorSubject.pipe(
      debounceTime(400),
      distinctUntilChanged()
    ).subscribe(texto => {
      this.textoBusqueda = texto;
      this.paginaActual = 1; // Resetear a la primera página tras filtrar
      this.cargarEstudiantes();
    });
  }

  cargarEstudiantes(): void {
    this.isLoading.set(true);

    // Llamada al método paginado de tu servicio
    this.estudiantesService.obtenerPaginado(this.textoBusqueda, this.paginaActual, this.tamanoPagina).subscribe({
      next: (respuesta) => {
        this.estudiantes.set(respuesta.elementos);
        this.totalRegistros.set(respuesta.totalRegistros);
        this.isLoading.set(false);
      },
      error: () => {        
        Swal.fire({
          title: 'Error',
          text: 'Error al obtener estudiantes',
          timer: 3000,
          icon: 'error'
        });
        this.isLoading.set(false);
      }
    });
  }

  aplicarFiltro(event: Event): void {
    const filterValue = (event.target as HTMLInputElement).value;
    this.buscadorSubject.next(filterValue);
  }

  cambiarPagina(event: PageEvent): void {
    this.paginaActual = event.pageIndex + 1; // MatPaginator es base 0, .NET base 1
    this.tamanoPagina = event.pageSize;
    this.cargarEstudiantes();
  }

  abrirModalCrear(): void {
    const dialogRef = this.dialog.open(EstudianteFormDialogComponent, { width: '600px' });

    dialogRef.afterClosed().subscribe((nuevoEstudiante) => {
      if (nuevoEstudiante) {
        this.estudiantesService.registrar(nuevoEstudiante).subscribe({
          next: () => {
            Swal.fire({
              icon: 'success',              
              text: `Estudiante registrado exitosamente`,
              timer: 3000
            });            
            this.cargarEstudiantes();
          },
          error: () => Swal.fire({
            title: 'Error',
            text: 'Error al crear estudiante',
            timer: 3000,
            icon: 'error'
          })                        
        });
      }
    });
  }

  abrirModalEditar(estudiante: Estudiante): void {
    const dialogRef = this.dialog.open(EstudianteFormDialogComponent, {
      width: '600px',
      data: estudiante
    });

    dialogRef.afterClosed().subscribe((estudianteEditado) => {
      if (estudianteEditado) {
        this.estudiantesService.actualizar(estudianteEditado).subscribe({
          next: () => {
            Swal.fire({
              icon: 'success',              
              text: `Estudiante actualizado exitosamente`,
              timer: 3000
            }); 
            this.cargarEstudiantes();
          },
          error: () => Swal.fire({
            title: 'Error',
            text: 'Error al actualizar estudiante',
            timer: 3000,
            icon: 'error'
          }) 
        });
      }
    });
  }  
}