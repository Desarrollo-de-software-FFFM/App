import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { UserService } from '../../proxy/usuarios/user.service';
import { UserProfileDto } from '../../proxy/usuarios/models';
import { OAuthService } from 'angular-oauth2-oidc';
import { ConfigStateService } from '@abp/ng.core';

@Component({
  selector: 'app-private-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './private-profile.html',
  styleUrls: ['./private-profile.scss']
})
export class PrivateProfile implements OnInit {
  profileForm: FormGroup;
  passwordForm: FormGroup;
  
  userProfile: UserProfileDto | null = null;
  
  isProfileLoading = false;
  isPasswordLoading = false;
  isDeleteLoading = false;

  profileMessage = '';
  profileError = '';
  
  passwordMessage = '';
  passwordError = '';

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private router: Router,
    private oauthService: OAuthService,
    private configState: ConfigStateService
  ) {
    this.profileForm = this.fb.group({
      nombre: ['', Validators.required],
      apellido: ['', Validators.required],
      telefono: [''],
      fotoUrl: ['']
    });

    this.passwordForm = this.fb.group({
      currentPassword: ['', Validators.required],
      newPassword: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile(): void {
    this.userService.getProfile().subscribe({
      next: (profile) => {
        this.userProfile = profile;
        this.profileForm.patchValue({
          nombre: profile.nombre,
          apellido: profile.apellido,
          telefono: profile.telefono,
          fotoUrl: profile.fotoUrl
        });
        localStorage.setItem('currentUser', JSON.stringify(profile));
      },
      error: (err) => {
        this.profileError = 'Error al cargar el perfil.';
      }
    });
  }

  onProfileSubmit(): void {
    if (this.profileForm.invalid) return;
    
    this.isProfileLoading = true;
    this.profileMessage = '';
    this.profileError = '';

    const payload = {
      ...this.profileForm.value,
      userName: this.userProfile?.userName,
      email: this.userProfile?.email
    };

    this.userService.updateProfile(payload).subscribe({
      next: (updatedProfile) => {
        this.isProfileLoading = false;
        this.userProfile = updatedProfile;
        this.profileMessage = 'Perfil actualizado correctamente.';
        localStorage.setItem('currentUser', JSON.stringify(updatedProfile));
        
        // Refrescar el navbar the ABP
        this.configState.refreshAppState().subscribe();

        setTimeout(() => this.profileMessage = '', 3000);
      },
      error: (err) => {
        this.isProfileLoading = false;
        this.profileError = err.error?.error?.message || 'Ocurrió un error al actualizar.';
      }
    });
  }

  onPasswordSubmit(): void {
    if (this.passwordForm.invalid) return;

    this.isPasswordLoading = true;
    this.passwordMessage = '';
    this.passwordError = '';

    this.userService.changePassword(this.passwordForm.value).subscribe({
      next: () => {
        this.isPasswordLoading = false;
        this.passwordMessage = 'Contraseña cambiada exitosamente.';
        this.passwordForm.reset();
        
        setTimeout(() => this.passwordMessage = '', 3000);
      },
      error: (err) => {
        this.isPasswordLoading = false;
        this.passwordError = err.error?.error?.message || 'Error al cambiar la contraseña.';
      }
    });
  }

  onDeleteAccount(): void {
    if (confirm('¿Estás seguro de que deseas eliminar tu cuenta? Esta acción no se puede deshacer.')) {
      this.isDeleteLoading = true;
      this.userService.deleteMyAccount().subscribe({
        next: () => {
          this.oauthService.logOut();
          localStorage.removeItem('currentUser');
          this.router.navigate(['/auth/login']);
        },
        error: (err) => {
          this.isDeleteLoading = false;
          alert('Error al eliminar la cuenta: ' + (err.error?.error?.message || 'Error desconocido'));
        }
      });
    }
  }
}
