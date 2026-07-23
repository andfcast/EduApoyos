import { Component, OnInit, ViewChild, inject, signal } from '@angular/core';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatChipsModule } from '@angular/material/chips';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { EstudianteService } from '../../../core/services/estudiante.service';
import { Estudiante } from '../../../core/models/estudiante.models';
import { EstudianteFormDialogComponent } from '../estudiante-form-dialog-component/estudiante-form-dialog-component';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-lista-estudiantes-component',
  standalone: true,
  imports: [MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatDialogModule,    
    MatChipsModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatCardModule,
    MatProgressSpinnerModule],
  templateUrl: './lista-estudiantes-component.html',
  styleUrl: './lista-estudiantes-component.css',
})
export class ListaEstudiantesComponent {
  private estudiantesService = inject(EstudianteService);
  private dialog = inject(MatDialog);  
  isLoading = signal<boolean>(false);
  dataSource = new MatTableDataSource<Estudiante>([]);

  displayedColumns: string[] = [
    'documento',
    'nombreCompleto',
    'correo',
    'programa',
    'semestre',
    'estado',
    'acciones'
  ];

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  ngOnInit(): void {
    this.cargarEstudiantes();
  }

  cargarEstudiantes(): void {
    this.isLoading.set(true);
    this.estudiantesService.obtenerTodos().subscribe({
      next: (estudiantes) => {
        this.dataSource.data = estudiantes;
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
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
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  abrirModalCrear(): void {
    const dialogRef = this.dialog.open(EstudianteFormDialogComponent, { width: '600px' });

    dialogRef.afterClosed().subscribe((nuevoEstudiante) => {
      if (nuevoEstudiante) {
        this.estudiantesService.registrar(nuevoEstudiante).subscribe({
          next: () => {
            Swal.fire({
              icon:'success',              
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
              icon:'success',              
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
