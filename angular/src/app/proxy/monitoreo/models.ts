
export interface MetricasApiExternaDto {
  nombreApi?: string;
  totalLlamadas: number;
  llamadasExitosas: number;
  llamadasFallidas: number;
  tiempoPromedioMs: number;
  tiempoMaximoMs: number;
  tiempoMinimoMs: number;
  desdeFecha?: string;
  hastaFecha?: string;
}
