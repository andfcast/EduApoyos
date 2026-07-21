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

@Component({
  selector: 'app-lista-estudiantes-component',
  standalone: true,
  imports: [MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatDialogModule,
    MatSnackBarModule,
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
  private snackBar = inject(MatSnackBar);
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
        this.snackBar.open('Error al obtener estudiantes', 'Cerrar', { duration: 3000 });
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
            this.snackBar.open('Estudiante registrado exitosamente', 'Cerrar', { duration: 3000 });
            this.cargarEstudiantes();
          },
          error: () => this.snackBar.open('Error al crear estudiante', 'Cerrar', { duration: 3000 })
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
            this.snackBar.open('Estudiante actualizado exitosamente', 'Cerrar', { duration: 3000 });
            this.cargarEstudiantes();
          },
          error: () => this.snackBar.open('Error al actualizar estudiante', 'Cerrar', { duration: 3000 })
        });
      }
    });
  }

  // eliminarEstudiante(estudiante: Estudiante): void {
    // if (confirm(`¿Inactivar/Eliminar al estudiante ${estudiante.nombreCompleto}?`)) {
    //   this.estudiantesService.eliminar(estudiante.id).subscribe({
    //     next: () => {
    //       this.snackBar.open('Registro modificado correctamente', 'Cerrar', { duration: 3000 });
    //       this.cargarEstudiantes();
    //     },
    //     error: () => this.snackBar.open('Error al eliminar estudiante', 'Cerrar', { duration: 3000 })
    //   });
    // }
  // }
}
