import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from './auth.service';
import { catchError, switchMap, throwError } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const auth=inject(AuthService);const token = auth.accessToken();
  const authorized=token ? request.clone({ setHeaders: { Authorization: `Bearer ${token}` } }) : request;
  return next(authorized).pipe(catchError(error=>{
    if(error.status!==401||request.url.includes('/auth/'))return throwError(()=>error);
    return auth.refresh().pipe(switchMap(response=>next(request.clone({setHeaders:{Authorization:`Bearer ${response.accessToken}`}}))));
  }));
};
