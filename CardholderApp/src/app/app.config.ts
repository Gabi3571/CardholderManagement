import { ApplicationConfig, inject, provideAppInitializer, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { HttpClient, provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { AppConfig } from './config.model';
import { firstValueFrom } from 'rxjs';
import { APP_CONFIG } from './config.token';
import { authInterceptor } from './interceptors/auth.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(
      withFetch(),
      withInterceptors([authInterceptor])),
    provideAppInitializer(initConfig()),
    {
      provide: APP_CONFIG,
      useFactory: () => (window as any).__APP_CONFIG__ as AppConfig,
    },
  ]
};

function initConfig() {
  return async () => {
    const http = inject(HttpClient);
    const config = await firstValueFrom(http.get<AppConfig>('/assets/config.json'));
    (window as any).__APP_CONFIG__ = config;
  };
}

