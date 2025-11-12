import type { EntityDto } from '@abp/ng.core';

export interface CalificacionDto extends EntityDto<string> {
  destinoId?: string;
  puntaje: number;
  comentario?: string;
  horaDeCreacion?: string;
}

export interface CreacionCalificacionDto {
  destinoId?: string;
  puntaje: number;
  comentario?: string;
}
