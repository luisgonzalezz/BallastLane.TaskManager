import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';

import { API_BASE_URL } from './api.config';
import { AuthService } from './auth.service';

describe('AuthService', () => {
  let service: AuthService;
  let http: HttpTestingController;

  beforeEach(() => {
    localStorage.clear();

    TestBed.configureTestingModule({
      providers: [
        AuthService,
        provideHttpClient(),
        provideHttpClientTesting(),
      ],
    });

    service = TestBed.inject(AuthService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    http.verify();
    localStorage.clear();
  });

  it('stores the JWT after a successful login', () => {
    service.login({ email: 'ana@example.com', password: 'Password123!' }).subscribe();

    const request = http.expectOne(`${API_BASE_URL}/auth/login`);
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({
      email: 'ana@example.com',
      password: 'Password123!',
    });

    request.flush({
      userId: '9f2fe83e-9f2f-49d0-9448-9d7efc875f69',
      email: 'ana@example.com',
      token: 'jwt-token',
    });

    expect(service.getToken()).toBe('jwt-token');
    expect(service.isAuthenticated()).toBeTrue();
  });

  it('stores the JWT after a successful registration', () => {
    service.register({ email: 'new@example.com', password: 'Password123!' }).subscribe();

    const request = http.expectOne(`${API_BASE_URL}/auth/register`);
    expect(request.request.method).toBe('POST');

    request.flush({
      userId: '01827cf1-d7d8-409b-aeed-fd8f0c65b821',
      email: 'new@example.com',
      token: 'registered-token',
    });

    expect(service.getToken()).toBe('registered-token');
  });

  it('clears the session on logout', () => {
    service.login({ email: 'ana@example.com', password: 'Password123!' }).subscribe();
    http.expectOne(`${API_BASE_URL}/auth/login`).flush({
      userId: '9f2fe83e-9f2f-49d0-9448-9d7efc875f69',
      email: 'ana@example.com',
      token: 'jwt-token',
    });

    service.logout();

    expect(service.getToken()).toBeNull();
    expect(service.isAuthenticated()).toBeFalse();
  });
});
