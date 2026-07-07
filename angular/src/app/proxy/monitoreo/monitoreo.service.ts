import type { MetricasApiExternaDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class MonitoreoService {
  apiName = 'Default';
  

  getMetricas = (desde?: string, hasta?: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, MetricasApiExternaDto[]>({
      method: 'GET',
      url: '/api/app/monitoreo/metricas',
      params: { desde, hasta },
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
