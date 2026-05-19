import { bootstrapApplication } from '@angular/platform-browser';
import { provideHttpClient } from '@angular/common/http';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { appConfig } from './app/app.config';
import { App } from './app/app';
import { AuthInterceptor } from './app/auth.interceptor';

bootstrapApplication(App, {
  ...appConfig,
  providers: [
    provideHttpClient(),
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true,
    },
    ...(appConfig.providers || []),
  ],
}).catch((err) => console.error(err));
