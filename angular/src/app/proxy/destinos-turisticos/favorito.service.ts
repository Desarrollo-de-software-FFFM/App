import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import type { FavoritoDto } from '../destinos/models';

@Injectable({
  providedIn: 'root',
})
export class FavoritoService {
  apiName = 'Default';
  

  agregarFavorito = (destinoTuristicoId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, FavoritoDto>({
      method: 'POST',
      url: `/api/app/favorito/agregar-favorito/${destinoTuristicoId}`,
    },
    { apiName: this.apiName,...config });
  

  eliminarFavorito = (destinoTuristicoId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/favorito/eliminar-favorito/${destinoTuristicoId}`,
    },
    { apiName: this.apiName,...config });
  

  obtenerMisFavoritos = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, FavoritoDto[]>({
      method: 'POST',
      url: '/api/app/favorito/obtener-mis-favoritos',
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
