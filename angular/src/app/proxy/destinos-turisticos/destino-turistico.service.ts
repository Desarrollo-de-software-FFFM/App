import type { CrearActualizarDestinoDTO, DestinoTuristicoDTO } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import type { CitySearchRequestDto, CitySearchResultDto } from '../destinos/models';

@Injectable({
  providedIn: 'root',
})
export class DestinoTuristicoService {
  apiName = 'Default';
  

  create = (input: CrearActualizarDestinoDTO, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DestinoTuristicoDTO>({
      method: 'POST',
      url: '/api/app/destino-turistico',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/destino-turistico/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DestinoTuristicoDTO>({
      method: 'GET',
      url: `/api/app/destino-turistico/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<DestinoTuristicoDTO>>({
      method: 'GET',
      url: '/api/app/destino-turistico',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  searchCities = (request: CitySearchRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CitySearchResultDto>({
      method: 'POST',
      url: '/api/app/destino-turistico/search-cities',
      body: request,
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CrearActualizarDestinoDTO, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DestinoTuristicoDTO>({
      method: 'PUT',
      url: `/api/app/destino-turistico/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
