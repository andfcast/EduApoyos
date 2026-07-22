import { Injectable, PLATFORM_ID, inject } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { Solicitud } from '../models/solicitud.models';
import html2pdf from 'html2pdf.js';

@Injectable({
  providedIn: 'root'
})
export class PdfSolicitudService {
  private platformId = inject(PLATFORM_ID);

  descargarDetallePdf(solicitud: Solicitud): void {
    if (!isPlatformBrowser(this.platformId)) {
      return;
    }

    const contenidoHtml = this.generarPlantillaHtml(solicitud);
    const elemento = document.createElement('div');
    elemento.innerHTML = contenidoHtml;
    document.body.appendChild(elemento);

    const opciones = {
      margin: [15, 15, 15, 15] as [number, number, number, number],
      filename: `Comprobante_Solicitud_${String(solicitud.id ?? 'EduApoyos').substring(0, 8)}.pdf`,
      image: { type: 'jpeg' as const, quality: 0.98 },
      html2canvas: { scale: 2, logging: false },
      jsPDF: { unit: 'mm', format: 'a4', orientation: 'portrait' as const }
    };

    html2pdf().set(opciones).from(elemento).save();

    setTimeout(() => elemento.remove(), 500);
  }

  private generarPlantillaHtml(solicitud: Solicitud): string {
    const fecha = solicitud.fechaSolicitud
      ? new Date(solicitud.fechaSolicitud).toLocaleDateString('es-CO')
      : 'N/A';

    const montoValue = (solicitud as any).montoSolicitado ?? (solicitud as any).monto ?? 0;
    const montoFormatted = new Intl.NumberFormat('es-CO', {
      style: 'currency',
      currency: 'COP',
      maximumFractionDigits: 0
    }).format(montoValue);

    const tipoApoyo = (solicitud as any).tipoApoyo?.nombre ?? (solicitud as any).tipoApoyo ?? 'N/A';
    const estudiante = (solicitud as any).nombreEstudiante ?? 'N/A';
    const estado = (solicitud as any).estado?.nombre ?? (solicitud as any).estado ?? 'Pendiente';
    const descripcion = (solicitud as any).descripcion ?? '';

    return `
      <div style="font-family: Arial, sans-serif; padding: 25px; color: #333; line-height: 1.5;">
        <div style="border-bottom: 2px solid #1976d2; padding-bottom: 12px; margin-bottom: 20px; display: flex; justify-content: space-between; align-items: center;">
          <div>
            <h1 style="color: #1976d2; margin: 0; font-size: 24px;">EduApoyos</h1>
            <p style="margin: 4px 0 0 0; color: #666; font-size: 13px;">Comprobante Oficial de Solicitud</p>
          </div>
          <div style="text-align: right;">
            <p style="margin: 0; font-weight: bold; font-size: 12px; color: #444;">Fecha de descarga:</p>
            <p style="margin: 0; color: #666; font-size: 12px;">${new Date().toLocaleDateString('es-CO')}</p>
          </div>
        </div>

        <h3 style="font-size: 16px; color: #222; margin-bottom: 15px; border-bottom: 1px solid #eee; padding-bottom: 5px;">
          Información General de la Solicitud
        </h3>

        <table style="width: 100%; border-collapse: collapse; margin-bottom: 25px;">
          <tr style="background-color: #f9f9f9;">
            <td style="padding: 10px; border: 1px solid #e0e0e0; font-weight: bold; width: 35%;">ID Solicitud:</td>
            <td style="padding: 10px; border: 1px solid #e0e0e0;">${solicitud.id}</td>
          </tr>
          <tr>
            <td style="padding: 10px; border: 1px solid #e0e0e0; font-weight: bold;">Estudiante:</td>
            <td style="padding: 10px; border: 1px solid #e0e0e0;">${estudiante}</td>
          </tr>
          <tr style="background-color: #f9f9f9;">
            <td style="padding: 10px; border: 1px solid #e0e0e0; font-weight: bold;">Tipo de Apoyo:</td>
            <td style="padding: 10px; border: 1px solid #e0e0e0;">${tipoApoyo}</td>
          </tr>
          <tr>
            <td style="padding: 10px; border: 1px solid #e0e0e0; font-weight: bold;">Monto Solicitado:</td>
            <td style="padding: 10px; border: 1px solid #e0e0e0; font-weight: bold; color: #2e7d32;">${montoFormatted}</td>
          </tr>
          <tr style="background-color: #f9f9f9;">
            <td style="padding: 10px; border: 1px solid #e0e0e0; font-weight: bold;">Fecha de Creación:</td>
            <td style="padding: 10px; border: 1px solid #e0e0e0;">${fecha}</td>
          </tr>
          <tr>
            <td style="padding: 10px; border: 1px solid #e0e0e0; font-weight: bold;">Estado Actual:</td>
            <td style="padding: 10px; border: 1px solid #e0e0e0;">
              <span style="font-weight: bold; padding: 4px 10px; border-radius: 4px; background-color: #e3f2fd; color: #0d47a1;">
                ${estado}
              </span>
            </td>
          </tr>
        </table>

        ${descripcion ? `
          <div style="margin-top: 15px;">
            <h4 style="font-size: 14px; color: #333; margin-bottom: 6px;">Observaciones del Asesor:</h4>
            <div style="background-color: #f5f5f5; border-left: 4px solid #1976d2; padding: 12px; font-style: italic; font-size: 13px; color: #444;">
              ${descripcion}
            </div>
          </div>
        ` : ''}

        <div style="margin-top: 40px; text-align: center; border-top: 1px solid #e0e0e0; padding-top: 15px; font-size: 11px; color: #777;">
          Documento generado automáticamente por el sistema EduApoyos.
        </div>
      </div>
    `;
  }
}