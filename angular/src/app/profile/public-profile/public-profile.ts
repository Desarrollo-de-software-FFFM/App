import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from '../../proxy/usuarios/user.service';
import { UserProfileDto } from '../../proxy/usuarios/models';

@Component({
  selector: 'app-public-profile',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './public-profile.html',
  styleUrls: ['./public-profile.scss']
})
export class PublicProfile implements OnInit {
  userProfile: UserProfileDto | null = null;
  isLoading = true;
  errorMessage = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private userService: UserService
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const userId = params.get('id');
      if (userId) {
        this.loadPublicProfile(userId);
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
}
