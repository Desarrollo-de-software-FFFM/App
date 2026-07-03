import { Injectable } from '@angular/core';
import { RestService } from '@abp/ng.core';
import { Observable } from 'rxjs';
import { FavoritoDto } from './favorito.model';

@Injectable({ providedIn: 'root' })
export class FavoritoService {
  private readonly apiName = 'Default';
  private readonly baseUrl = '/api/app/favorito';

  constructor(private rest: RestService) {}

  /** POST /api/app/favorito/agregar-favorito?destinoTuristicoId={id} */
  addFavorito(destinoTuristicoId: string): Observable<FavoritoDto> {
    return this.rest.request<void, FavoritoDto>(
      {
        method: 'POST',
        url: `${this.baseUrl}/agregar-favorito`,
        params: { destinoTuristicoId },
      },
      { apiName: this.apiName }
    );
  }

  /** DELETE /api/app/favorito/eliminar-favorito?destinoTuristicoId={id} */
  removeFavorito(destinoTuristicoId: string): Observable<void> {
    return this.rest.request<void, void>(
      {
        method: 'DELETE',
        url: `${this.baseUrl}/eliminar-favorito`,
        params: { destinoTuristicoId },
      },
      { apiName: this.apiName }
    );
  }

  /** GET /api/app/favorito/obtener-mis-favoritos */
  getMyFavoritos(): Observable<FavoritoDto[]> {
    return this.rest.request<void, FavoritoDto[]>(
      {
        method: 'GET',
        url: `${this.baseUrl}/obtener-mis-favoritos`,
      },
      { apiName: this.apiName }
    );
  }
}
