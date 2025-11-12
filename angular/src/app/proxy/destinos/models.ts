
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

export interface CitySearchRequestDto {
  partialName?: string;
}

export interface CitySearchResultDto {
  cities: CityDto[];
}

export interface CrearActualizarCalificacionDTO {
  destinoTuristicoId: string;
  puntuacion: number;
  comentario: string;
}
