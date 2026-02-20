import type { PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface CalificacionDto {
  destinoTuristicoId?: string;
  userId?: string;
  puntuacion: number;
  comentario?: string;
  creationTime?: string;
}

export interface CityDto {
  name?: string;
  country?: string;
  latitude: number;
  longitude: number;
}

export interface CitySearchRequestDto extends PagedAndSortedResultRequestDto {
  partialName?: string;
  country?: string;
}

export interface CrearActualizarCalificacionDTO {
  destinoTuristicoId: string;
  puntuacion: number;
  comentario: string;
}
