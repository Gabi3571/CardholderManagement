import { inject } from '@angular/core';
import { HttpRequest, HttpHandlerFn, HttpInterceptorFn } from '@angular/common/http';
import { switchMap, take } from 'rxjs/operators';
import { AuthService } from '../services/auth-service';

export const authInterceptor: HttpInterceptorFn = (req: HttpRequest<unknown>, next: HttpHandlerFn) => {
  if (req.url.includes('/api/auth')) {
    return next(req);
  }
  
  const authService = inject(AuthService);

  return authService.getToken().pipe(
    take(1),
    switchMap(token => {
      const cloned = req.clone({
        setHeaders: { Authorization: `Bearer ${token}` }
      });
      return next(cloned);
    })
  );
};
