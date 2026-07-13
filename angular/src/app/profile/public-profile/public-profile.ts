import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { UserService } from '../../proxy/usuarios/user.service';
import { UserProfileDto } from '../../proxy/usuarios/models';
import { CalificacionService } from '../../proxy/destinos-turisticos/calificacion.service';

@Component({
  selector: 'app-public-profile',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './public-profile.html',
  styleUrls: ['./public-profile.scss']
})
export class PublicProfile implements OnInit {
  userProfile: UserProfileDto | null = null;
  calificaciones: any[] = [];
  isLoading = true;
  isLoadingReviews = false;
  errorMessage = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private userService: UserService,
    private calificacionService: CalificacionService
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const userId = params.get('id');
      if (userId) {
        this.loadPublicProfile(userId);
        this.loadUserReviews(userId);
      } else {
        this.router.navigate(['/']);
      }
    });
  }

  loadPublicProfile(userId: string): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.userService.getPublicProfile(userId).subscribe({
      next: (profile) => {
        this.userProfile = profile;
        this.isLoading = false;
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = 'No se pudo cargar el perfil público. Es posible que el usuario no exista.';
      }
    });
  }

  loadUserReviews(userId: string): void {
    this.isLoadingReviews = true;
    this.calificacionService.obtenerPorUsuario(userId).subscribe({
      next: (res) => {
        this.calificaciones = res;
        this.isLoadingReviews = false;
      },
      error: (err) => {
        this.isLoadingReviews = false;
        console.error('Error loading reviews', err);
      }
    });
  }
}
