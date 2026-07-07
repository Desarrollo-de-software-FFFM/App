import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import type { CalificacionDto, CrearActualizarCalificacionDTO } from '../destinos/models';

@Injectable({
  providedIn: 'root',
})
export class CalificacionService {
  apiName = 'Default';
  

  crearCalificacion = (input: CrearActualizarCalificacionDTO, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CalificacionDto>({
      method: 'POST',
      url: '/api/app/calificacion/crear-calificacion',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  editarCalificacion = (destinoId: string, input: CrearActualizarCalificacionDTO, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CalificacionDto>({
      method: 'POST',
      url: `/api/app/calificacion/editar-calificacion/${destinoId}`,
      body: input,
    },
    { apiName: this.apiName,...config });
  

  eliminarCalificacion = (destinoId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/calificacion/eliminar-calificacion/${destinoId}`,
    },
    { apiName: this.apiName,...config });
  

  listarComentarios = (destinoId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CalificacionDto[]>({
      method: 'POST',
      url: `/api/app/calificacion/listar-comentarios/${destinoId}`,
    },
    { apiName: this.apiName,...config });
  

  obtenerPorUsuario = (usuarioId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CalificacionDto[]>({
      method: 'POST',
      url: `/api/app/calificacion/obtener-por-usuario/${usuarioId}`,
    },
    { apiName: this.apiName,...config });
  

  obtenerPromedio = (destinoId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, number>({
      method: 'POST',
      url: `/api/app/calificacion/obtener-promedio/${destinoId}`,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
