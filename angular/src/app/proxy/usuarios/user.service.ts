import type { ChangePasswordDto, LoginUserDto, RegisterUserDto, UpdateUserProfileDto, UserProfileDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  apiName = 'Default';
  

  changePassword = (input: ChangePasswordDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/user/change-password',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  deleteMyAccount = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: '/api/app/user/my-account',
    },
    { apiName: this.apiName,...config });
  

  getProfile = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, UserProfileDto>({
      method: 'GET',
      url: '/api/app/user/profile',
    },
    { apiName: this.apiName,...config });
  

  getPublicProfile = (userId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, UserProfileDto>({
      method: 'GET',
      url: `/api/app/user/public-profile/${userId}`,
    },
    { apiName: this.apiName,...config });
  

  login = (input: LoginUserDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, UserProfileDto>({
      method: 'POST',
      url: '/api/app/user/login',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  register = (input: RegisterUserDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, UserProfileDto>({
      method: 'POST',
      url: '/api/app/user/register',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  updateProfile = (input: UpdateUserProfileDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, UserProfileDto>({
      method: 'PUT',
      url: '/api/app/user/profile',
      body: input,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
