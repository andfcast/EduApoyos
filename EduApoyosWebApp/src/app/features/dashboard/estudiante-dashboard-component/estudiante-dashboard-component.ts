import { Component, inject } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-estudiante-dashboard-component',
  imports: [
    MatCardModule,
    MatButtonModule,
    MatIconModule],
  templateUrl: './estudiante-dashboard-component.html',
  styleUrl: './estudiante-dashboard-component.css',
})
export class EstudianteDashboardComponent {
  authService = inject(AuthService);
}
