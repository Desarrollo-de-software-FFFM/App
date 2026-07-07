import { mapEnumToOptions } from '@abp/ng.core';

export enum TipoValoracion {
  Positiva = 1,
  Neutral = 2,
  Negativa = 3,
}

export const tipoValoracionOptions = mapEnumToOptions(TipoValoracion);
