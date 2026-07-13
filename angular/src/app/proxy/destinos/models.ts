import type { PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface CalificacionDto {
  id?: string;
  destinoTuristicoId?: string;
  destinoNombre?: string;
  userId?: string;
  userName?: string;
  puntuacion: number;
  comentario?: string;
  creationTime?: string;
}

export interface CityDto {
  id: number;
  name?: string;
  country?: string;
  region?: string;
  population?: number;
  latitude: number;
  longitude: number;
}

export interface CityInformationDto {
  id: number;
  name?: string;
  country?: string;
  region?: string;
  population?: number;
  latitude: number;
  longitude: number;
  timezone?: string;
}

export interface CitySearchRequestDto extends PagedAndSortedResultRequestDto {
  partialName?: string;
  country?: string;
  region?: string;
  minimumPopulation?: number;
}

export interface CitySearchResultDto {
  items: CityDto[];
  totalCount: number;
}

export interface CrearActualizarCalificacionDTO {
  destinoTuristicoId: string;
  puntuacion: number;
  comentario: string;
}

export interface FavoritoDto {
  id?: string;
  destinoTuristicoId?: string;
  userId?: string;
  creationTime?: string;
}
