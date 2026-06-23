import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';

import { authInterceptor } from './auth.interceptor';
import { AuthService } from './auth.service';

describe('authInterceptor', () => {
  let httpClient: HttpClient;
  let http: HttpTestingController;
  let authService: AuthService;

  beforeEach(() => {
    localStorage.clear();

    TestBed.configureTestingModule({
      providers: [
        AuthService,
        provideHttpClient(withInterceptors([authInterceptor])),
        provideHttpClientTesting(),
      ],
    });

    httpClient = TestBed.inject(HttpClient);
    http = TestBed.inject(HttpTestingController);
    authService = TestBed.inject(AuthService);
  });

  afterEach(() => {
    http.verify();
    localStorage.clear();
  });

  it('adds the bearer token when the user is authenticated', () => {
    authService.saveSession({
      userId: '9f2fe83e-9f2f-49d0-9448-9d7efc875f69',
      email: 'ana@example.com',
      token: 'jwt-token',
    });

    httpClient.get('/api/tasks').subscribe();

    const request = http.expectOne('/api/tasks');
    expect(request.request.headers.get('Authorization')).toBe('Bearer jwt-token');
    request.flush([]);
  });

  it('leaves requests unchanged when there is no token', () => {
    httpClient.get('/api/tasks').subscribe();

    const request = http.expectOne('/api/tasks');
    expect(request.request.headers.has('Authorization')).toBeFalse();
    request.flush([]);
  });
});
