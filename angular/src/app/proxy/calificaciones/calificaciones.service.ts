import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import type { CalificacionDto, CreacionCalificacionDto } from '../calificaciones-dto/models';

@Injectable({
  providedIn: 'root',
})
export class CalificacionesService {
  apiName = 'Default';
  

  create = (input: CreacionCalificacionDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CalificacionDto>({
      method: 'POST',
      url: '/api/app/calificaciones',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  getMyRatings = (destinoId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CalificacionDto[]>({
      method: 'GET',
      url: `/api/app/calificaciones/my-ratings/${destinoId}`,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
