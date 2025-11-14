import type { PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface CityDto {
  name?: string;
  country?: string;
  latitude: number;
  longitude: number;
}

export interface CitySearchRequestDto extends PagedAndSortedResultRequestDto {
  partialName?: string;
  country?: string;
}
