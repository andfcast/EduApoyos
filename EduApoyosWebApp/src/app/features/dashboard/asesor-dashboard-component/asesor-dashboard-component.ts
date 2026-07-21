import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';

// Angular Material
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatGridListModule } from '@angular/material/grid-list';

import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-asesor-dashboard-component',
  imports: [
    RouterLink,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatGridListModule
  ],
  templateUrl: './asesor-dashboard-component.html',
  styleUrl: './asesor-dashboard-component.css',
})
export class AsesorDashboardComponent implements OnInit {
  authService = inject(AuthService);

  // Métrica rápida para resumen del panel
  totalEstudiantes = signal<number>(0);
  solicitudesPendientes = signal<number>(0);

  ngOnInit(): void {
    // Aquí puedes invocar métricas iniciales cuando agreguemos las llamadas HTTP correspondientes
  }}
