import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatTooltipModule } from '@angular/material/tooltip';
import { AuthService } from '../../../core/services/auth.service';
import { Solicitud } from '../../../core/models/solicitud.models';
import { SolicitudesService } from '../../../core/services/solicitud.service';
import { SolicitudFormDialogComponent } from '../solicitud-form-dialog-component/solicitud-form-dialog-component';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { DetalleSolicitudDialogComponent } from '../detalle-solicitud-dialog-component/detalle-solicitud-dialog-component';

@Component({
  selector: 'app-lista-solicitudes-component',
  imports: [
    CommonModule,
    FormsModule,
    MatTableModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatDialogModule,
    MatTooltipModule,
    MatSnackBarModule
  ],
  templateUrl: './lista-solicitudes-component.html',
  styleUrl: './lista-solicitudes-component.css',
})
export class ListaSolicitudesComponent implements OnInit{


  private solicitudesService = inject(SolicitudesService);
  public authService = inject(AuthService); 
  private dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);

  // Columnas a visibilizar en la tabla
  displayedColumns: string[] = ['estudiante', 'tipoApoyo', 'monto', 'fecha', 'estado', 'acciones'];
  dataSource = signal<Solicitud[]>([]);

  // Signals para los filtros de búsqueda y selección
  estadoSeleccionado = signal<number | null>(null);
  tipoApoyoSeleccionado = signal<number | null>(null);
  busqueda = signal<string>('');

  ngOnInit(): void {
    this.cargarSolicitudes();
  }

  /**
   * Obtiene la lista de solicitudes desde el backend aplicando los filtros vigentes
   */
  cargarSolicitudes(): void {
    // const filtro: FiltroSolicitud = {
    //   estadoId: this.estadoSeleccionado(),
    //   tipoApoyoId: this.tipoApoyoSeleccionado(),
    //   terminoBusqueda: this.busqueda()
    // };

    // this.solicitudesService.listarSolicitudes(filtro).subscribe({
    //   next: (data) => this.dataSource.set(data),
    //   error: (err) => console.error('Error al cargar la lista de solicitudes:', err)
    // });
    this.solicitudesService.obtenerSolicitudes().subscribe({
      next: (data) => this.dataSource.set(data),
      error: (err) => console.error('Error al cargar la lista de solicitudes:', err)
    });
  }

  /**
   * Dispara el recargado de la tabla al cambiar cualquier filtro
   */
  onFiltroChange(): void {
    this.cargarSolicitudes();
  }

  /**
   * Abre el diálogo modal para crear una nueva solicitud (Exclusivo para Asesor)
   */
  abrirModalCrear(): void {
    if (!this.authService.esAsesor()) return;

    const dialogRef = this.dialog.open(SolicitudFormDialogComponent, {
      width: '600px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(dtoResult => {
      if (dtoResult) {
        // Invoca el endpoint de guardado POST /api/solicitudes
        this.solicitudesService.crearSolicitud(dtoResult).subscribe({
          next: () => { 
            this.snackBar.open('Solicitud registrada correctamente', 'OK', { duration: 2500 });
            this.cargarSolicitudes(); 
          },
          error: (err) => console.error('Error al registrar la solicitud:', err)
        });
      }
    });
  }

  cambiarEstado(solicitudId: string, nuevoEstadoId: number): void {
    if (!this.authService.esAsesor()) return;
    let usuarioId = this.authService.getUserId()!;
    this.solicitudesService.cambiarEstadoSolicitud(solicitudId, nuevoEstadoId, usuarioId).subscribe({
      next: () => {
        this.snackBar.open('Estado actualizado correctamente', 'OK', { duration: 2500 });
        this.cargarSolicitudes(); // Recarga y refresca la tabla al instante
      },
      error: (err) => {
        console.error('Error al cambiar el estado:', err);
        this.snackBar.open('Error al actualizar el estado de la solicitud', 'Cerrar', { duration: 3000 });
      }
    });
  }

  verDetalle(solicitud: Solicitud): void {
    this.dialog.open(DetalleSolicitudDialogComponent, {
      width: '500px',
      data: solicitud // Inyectamos toda la data de la fila al modal
    });
  }

  obtenerClaseEstado(estado: string): string {
    switch (estado) {
      case 'Pendiente': return 'pendiente';
      case 'En Revisión': return 'en-revision';
      case 'Aprobada': return 'aprobada';
      case 'Rechazada': return 'rechazada';  
      default: return 'pendiente';   
    }
  }
}
