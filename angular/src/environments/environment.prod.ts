import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:4200';

const oAuthConfig = {
  issuer: 'https://localhost:44391/',
  redirectUri: baseUrl,
  clientId: 'ExploraYa1_App',
  responseType: 'code',
  scope: 'offline_access ExploraYa1',
  requireHttps: true,
};

export const environment = {
  production: true,
  application: {
    baseUrl,
    name: 'ExploraYa1',
  },
  oAuthConfig,
  apis: {
    default: {
      url: 'https://localhost:44391',
      rootNamespace: 'ExploraYa1',
    },
    AbpAccountPublic: {
      url: oAuthConfig.issuer,
      rootNamespace: 'AbpAccountPublic',
    },
  },
  remoteEnv: {
    url: '/getEnvConfig',
    mergeStrategy: 'deepmerge'
  }
} as Environment;
