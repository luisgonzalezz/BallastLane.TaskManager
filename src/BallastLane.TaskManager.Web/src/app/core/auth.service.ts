import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, tap } from 'rxjs';

import { API_BASE_URL } from './api.config';
import { AuthRequest, AuthResponse } from './auth.models';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly tokenKey = 'ballastlane.taskmanager.token';
  private readonly userKey = 'ballastlane.taskmanager.user';

  constructor(private readonly http: HttpClient) {
  }

  login(request: AuthRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${API_BASE_URL}/auth/login`, request)
      .pipe(tap((response) => this.saveSession(response)));
  }

  register(request: AuthRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${API_BASE_URL}/auth/register`, request)
      .pipe(tap((response) => this.saveSession(response)));
  }

  saveSession(response: AuthResponse): void {
    localStorage.setItem(this.tokenKey, response.token);
    localStorage.setItem(this.userKey, JSON.stringify({
      userId: response.userId,
      email: response.email,
    }));
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.userKey);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  isAuthenticated(): boolean {
    return !!this.getToken();
  }
}
