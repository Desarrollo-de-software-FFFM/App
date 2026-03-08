import type { AuditedEntityDto } from '@abp/ng.core';

export interface CrearActualizarDestinoDTO {
  nombre?: string;
  poblacion: number;
  latitud: number;
  longuitud: number;
  imagenUrl?: string;
  calificacionGeneral: number;
  regionId?: string;
}

export interface DestinoTuristicoDTO extends AuditedEntityDto<string> {
  nombre?: string;
  poblacion: number;
  latitud: number;
  longuitud: number;
  imagenUrl?: string;
  calificacionGeneral: number;
  regionId?: string;
}
