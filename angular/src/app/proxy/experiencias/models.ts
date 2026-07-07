import type { TipoValoracion } from './tipo-valoracion.enum';
import type { AuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface CrearActualizarExperienciaDto {
  destinoId: string;
  comentario: string;
  valoracion: TipoValoracion;
}

export interface ExperienciaDto extends AuditedEntityDto<string> {
  destinoId?: string;
  comentario?: string;
  valoracion?: TipoValoracion;
}

export interface GetExperienciasInput extends PagedAndSortedResultRequestDto {
  destinoId?: string;
  valoracion?: TipoValoracion;
  palabrasClave?: string;
}
