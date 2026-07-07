import type { AuditedEntityDto } from '@abp/ng.core';

export interface NotificacionDTO extends AuditedEntityDto<string> {
  userId?: string;
  titulo?: string;
  mensaje?: string;
  leida: boolean;
  destinoId?: string;
  fecha?: string;
}
