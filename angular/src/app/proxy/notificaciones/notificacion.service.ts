import type { NotificacionDTO } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class NotificacionService {
  apiName = 'Default';
  

  crearNotificacionCambioDestino = (destinoId: string, detalle: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/notificacion/crear-notificacion-cambio-destino/${destinoId}`,
      params: { detalle },
    },
    { apiName: this.apiName,...config });
  

  getMisNotificaciones = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, NotificacionDTO[]>({
      method: 'GET',
      url: '/api/app/notificacion/mis-notificaciones',
    },
    { apiName: this.apiName,...config });
  

  marcarLeida = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/notificacion/${id}/marcar-leida`,
    },
    { apiName: this.apiName,...config });
  

  marcarNoLeida = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/notificacion/${id}/marcar-no-leida`,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
