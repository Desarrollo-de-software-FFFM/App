import { Component } from '@angular/core';
import { DynamicLayoutComponent, ReplaceableComponentsService } from '@abp/ng.core';
import { LoaderBarComponent } from '@abp/ng.theme.shared';
import { eAccountComponents } from '@abp/ng.account';
import { PrivateProfile } from './profile/private-profile/private-profile';
import { AuthService, ConfigStateService } from '@abp/ng.core';
import { OAuthService } from 'angular-oauth2-oidc';
import { Router } from '@angular/router';
import { of } from 'rxjs';

@Component({
  selector: 'app-root',
  template: `
    <abp-loader-bar />
    <abp-dynamic-layout />
  `,
  imports: [LoaderBarComponent, DynamicLayoutComponent],
})
export class AppComponent {
  constructor(
    private replaceableComponents: ReplaceableComponentsService,
    private authService: AuthService,
    private oauthService: OAuthService,
    private configState: ConfigStateService,
    private router: Router
  ) {
    // Sobrescribir el componente de perfil de ABP
    this.replaceableComponents.add({
      component: PrivateProfile,
      key: eAccountComponents.ManageProfile,
    });

    // Sobrescribir el comportamiento de "Cerrar sesión" para adaptarlo a Password Flow
    this.authService.logout = () => {
      this.oauthService.logOut(true); // limpia los tokens locales sin redirigir al Identity Server
      localStorage.removeItem('currentUser');
      this.configState.refreshAppState().subscribe(); // actualiza la UI de ABP
      this.router.navigate(['/auth/login']);
      return of(null);
    };

    // Sobrescribir el botón de "Iniciar sesión" del navbar de ABP
    this.authService.navigateToLogin = () => {
      this.router.navigate(['/auth/login']);
    };
  }
}
