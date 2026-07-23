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
import { AuthService } from '../../../core/services/auth.service';
import { FiltroSolicitud, Solicitud } from '../../../core/models/solicitud.models';
import { SolicitudesService } from '../../../core/services/solicitud.service';
import { SolicitudFormDialogComponent } from '../solicitud-form-dialog-component/solicitud-form-dialog-component';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { DetalleSolicitudDialogComponent } from '../detalle-solicitud-dialog-component/detalle-solicitud-dialog-component';
import { EstudianteService } from '../../../core/services/estudiante.service';
import { CatalogoService } from '../../../core/services/catalogo.service';
import { EstadoSolicitud, TipoApoyo } from '../../../core/models/general.models';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-lista-solicitudes-component',
  imports: [
    CommonModule,
    FormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatFormFieldModule,
    MatSelectModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatDialogModule
  ],
  templateUrl: './lista-solicitudes-component.html',
  styleUrl: './lista-solicitudes-component.css',
})
export class ListaSolicitudesComponent implements OnInit{


  private solicitudesService = inject(SolicitudesService);
  private catalogoService = inject(CatalogoService);
  private estudianteService = inject(EstudianteService);
  public authService = inject(AuthService); 
  private dialog = inject(MatDialog);
  
  // Columnas a visibilizar en la tabla
  displayedColumns: string[] = ['estudiante', 'tipoApoyo', 'monto', 'fecha', 'estado', 'acciones'];
  dataSource = signal<Solicitud[]>([]);

  tiposApoyo = signal<TipoApoyo[]>([]);
  estados = signal<TipoApoyo[]>([]);

  // Signals para los filtros de búsqueda y selección
  tipoApoyoSeleccionado = signal<number | null>(null);
  fechaSeleccionada = signal<Date | null>(null);
  estadoSeleccionado = signal<number | null>(null);
  busqueda = signal<string>('');

  totalRegistros = signal<number>(0);
  paginaActual = signal<number>(0); // 0-based index para MatPaginator
  tamanoPagina = signal<number>(5);
  opcionesTamanoPagina = [5, 10, 20, 50];

  ngOnInit(): void {
    this.cargarCatalogos();
    this.cargarSolicitudes();
  }

  private cargarCatalogos(): void {
    this.catalogoService.obtenerTiposApoyo().subscribe({
      next: (tipos) => this.tiposApoyo.set(tipos),
      error: (err) => console.error('Error cargando tipos de apoyo', err)
    });

    this.catalogoService.obtenerEstadosSolicitud().subscribe({
      next: (estados) => this.estados.set(estados),
      error: (err) => console.error('Error cargando estados de solicitud', err)
    });
  }
  cargarSolicitudes(): void {    
    if(this.authService.esAsesor()) {
      const fechaFormateada = this.fechaSeleccionada() 
        ? this.fechaSeleccionada()!.toISOString().split('T')[0] 
      :null;
      const filtro: FiltroSolicitud = {
        tipoApoyoId: this.tipoApoyoSeleccionado(),
        fecha: fechaFormateada,
        estadoId: this.estadoSeleccionado(),
        pagina: this.paginaActual() + 1,
        tamanoPagina: this.tamanoPagina()        
      }
      this.solicitudesService.filtrarSolicitudes(filtro).subscribe({
        next: (respuesta) => {
          this.dataSource.set(respuesta.elementos);
          this.totalRegistros.set(respuesta.totalRegistros);
        },
        error: (err) => console.error('Error al cargar solicitudes:', err)
      });      
    }
    else{
      this.estudianteService.obtenerSolicitudesXEstudiante(this.authService.getUserId()!).subscribe({
        next: (data) => this.dataSource.set(data),
        error: (err) => console.error('Error al cargar la lista de solicitudes:', err)
      });
    }
  }

  onFiltroChange(): void {
    this.cargarSolicitudes();
  }

  aplicarFiltros(): void {
    this.paginaActual.set(0);
    this.cargarSolicitudes();
  }

  limpiarFiltros(): void {
    this.tipoApoyoSeleccionado.set(null);
    this.fechaSeleccionada.set(null);
    this.estadoSeleccionado.set(null);
    this.aplicarFiltros();
  }

  cambiarPagina(event: PageEvent): void {
    this.paginaActual.set(event.pageIndex);
    this.tamanoPagina.set(event.pageSize);
    this.cargarSolicitudes();
  }

  abrirModalCrear(): void {    
    const dialogRef = this.dialog.open(SolicitudFormDialogComponent, {
      width: '600px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(dtoResult => {
      if (dtoResult) {
        // Invoca el endpoint de guardado POST /api/solicitudes
        this.solicitudesService.crearSolicitud(dtoResult).subscribe({
          next: () => {
            Swal.fire({
              icon:'success',              
              text: `Solicitud registrada correctamente`,
              timer: 2500
            });                
            this.cargarSolicitudes(); 
          },
          error: (err) => {
                console.error('Error al registrar la solicitud:', err);
                Swal.fire({
                      title: 'Error',
                      text: 'Error al registrar la solicitud',
                      timer: 3000,
                      icon: 'error'
                }) 
          }
        });
      }
    });
  }

  cambiarEstado(solicitudId: string, nuevoEstadoId: number): void {
    if (!this.authService.esAsesor()) return;
    let verbo = '';
    switch(nuevoEstadoId){
      case 2: verbo = 'avanzar en'; break;
      case 3: verbo = 'aprobar'; break;
      case 4: verbo = 'rechazar'; break;
    }
    //let usuarioId = this.authService.getUserId()!;
    Swal.fire({
      title: "¿Está seguro de " + verbo + " la solicitud?",
      text: "Esta acción no se puede revertir",
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "Sí",
      cancelButtonText: "No"
    }).then((result) =>{
      if(result.isConfirmed){
        this.aplicarCambioEstado(solicitudId,nuevoEstadoId)
      }
    });    
  }

  private aplicarCambioEstado(solicitudId: string, nuevoEstadoId: number){
    let usuarioId = this.authService.getUserId()!;
    this.solicitudesService.cambiarEstadoSolicitud(solicitudId, nuevoEstadoId, usuarioId).subscribe({
      next: () => {        
        Swal.fire({
          icon:'success',              
          text: `Estado actualizado correctamente`,
          timer: 2500
        });
        this.cargarSolicitudes();
      },
      error: (err) => {
        console.error('Error al cambiar el estado:', err);
        Swal.fire({
              title: 'Error',
              text: 'Error al actualizar el estado de la solicitud',
              timer: 3000,
              icon: 'error'
        });        
      }
    });
  }

  verDetalle(solicitud: Solicitud): void {
    this.dialog.open(DetalleSolicitudDialogComponent, {
      width: '500px',
      data: solicitud
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
