import { Component, inject, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { Solicitud } from '../../../core/models/solicitud.models';
import { PdfSolicitudService } from '../../../core/services/pdf.service';
import { MatTableModule } from '@angular/material/table';

@Component({
  selector: 'app-detalle-solicitud-dialog-component',
  imports: [
    CommonModule, 
    MatDialogModule, 
    MatTableModule,
    MatButtonModule, 
    MatIconModule, 
    MatDividerModule],
  templateUrl: './detalle-solicitud-dialog-component.html',
  styleUrl: './detalle-solicitud-dialog-component.css',
})
export class DetalleSolicitudDialogComponent {
  columnasHistorial: string[] = ['fecha', 'estadoAnterior', 'estadoNuevo', 'observacion'];
  private pdfService = inject(PdfSolicitudService);
  constructor(
    public dialogRef: MatDialogRef<DetalleSolicitudDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: Solicitud
  ) {}

  descargarPdf(): void {
    this.pdfService.descargarDetallePdf(this.data);
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
