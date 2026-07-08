import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { UserService } from '../../proxy/usuarios/user.service';
import { CommonModule } from '@angular/common';
import { OAuthService } from 'angular-oauth2-oidc';
import { firstValueFrom } from 'rxjs';
import { ConfigStateService } from '@abp/ng.core';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './login.html',
  styleUrls: ['./login.scss']
})
export class Login {
  loginForm: FormGroup;
  isLoading = false;
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private router: Router,
    private oauthService: OAuthService,
    private configState: ConfigStateService
  ) {
    this.loginForm = this.fb.group({
      userNameOrEmail: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  async onSubmit(): Promise<void> {
    if (this.loginForm.invalid) {
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    try {
      // 1. Obtener el Token JWT via Password Flow (se guarda automáticamente)
      await this.oauthService.fetchTokenUsingPasswordFlow(
        this.loginForm.value.userNameOrEmail,
        this.loginForm.value.password
      );

      // 2. Traer el perfil del usuario actual (el token se enviará en los headers gracias al interceptor de ABP)
      const userProfile = await firstValueFrom(this.userService.getProfile());

      // 3. Guardar el perfil localmente para pintar la UI del Navbar / Home
      localStorage.setItem('currentUser', JSON.stringify(userProfile));

      // 4. Refrescar el estado de ABP para que la barra de navegacion se actualice
      await firstValueFrom(this.configState.refreshAppState());

      this.isLoading = false;
      this.router.navigate(['/']); // Redirigir al home
    } catch (err: any) {
      this.isLoading = false;
      this.errorMessage = err?.error?.error_description || 'Error al iniciar sesión. Verifica tus credenciales.';
    }
  }
}
