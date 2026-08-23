import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { tap } from 'rxjs';

export interface AuthResponse { accessToken: string; refreshToken: string; expiresAt: string; }

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly access = signal(sessionStorage.getItem('accessToken'));
  readonly isAuthenticated = computed(() => !!this.access());
  readonly accessToken = this.access.asReadonly();

  login(email: string, password: string) {
    return this.http.post<AuthResponse>('/api/v1/auth/login', { email, password }).pipe(tap(x => this.save(x)));
  }
  register(email: string, password: string) {
    return this.http.post<AuthResponse>('/api/v1/auth/register', { email, password }).pipe(tap(x => this.save(x)));
  }
  refresh(){const refreshToken=sessionStorage.getItem('refreshToken');return this.http.post<AuthResponse>('/api/v1/auth/refresh',{refreshToken}).pipe(tap(x=>this.save(x)));}
  logout() {
    const refreshToken=sessionStorage.getItem('refreshToken');
    if(refreshToken)this.http.post('/api/v1/auth/revoke',{refreshToken}).subscribe();
    sessionStorage.clear(); this.access.set(null); this.router.navigateByUrl('/login');
  }
  private save(response: AuthResponse) {
    sessionStorage.setItem('accessToken', response.accessToken);
    sessionStorage.setItem('refreshToken', response.refreshToken);
    this.access.set(response.accessToken);
  }
}
