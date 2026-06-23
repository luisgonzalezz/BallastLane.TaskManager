import { TestBed } from '@angular/core/testing';
import { CanActivateFn, Router, UrlTree } from '@angular/router';

import { authGuard } from './auth.guard';
import { AuthService } from './auth.service';

describe('authGuard', () => {
  const executeGuard: CanActivateFn = (...guardParameters) =>
    TestBed.runInInjectionContext(() => authGuard(...guardParameters));

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        {
          provide: AuthService,
          useValue: jasmine.createSpyObj<AuthService>('AuthService', ['isAuthenticated']),
        },
        {
          provide: Router,
          useValue: jasmine.createSpyObj<Router>('Router', ['createUrlTree']),
        },
      ],
    });
  });

  it('allows authenticated users to activate protected routes', () => {
    const authService = TestBed.inject(AuthService);
    (authService.isAuthenticated as jasmine.Spy).and.returnValue(true);

    const result = executeGuard({} as never, {} as never);

    expect(result).toBeTrue();
  });

  it('redirects anonymous users to login', () => {
    const authService = TestBed.inject(AuthService);
    const router = TestBed.inject(Router);
    const loginTree = {} as UrlTree;
    (authService.isAuthenticated as jasmine.Spy).and.returnValue(false);
    (router.createUrlTree as jasmine.Spy).and.returnValue(loginTree);

    const result = executeGuard({} as never, {} as never);

    expect(router.createUrlTree).toHaveBeenCalledWith(['/login']);
    expect(result).toBe(loginTree);
  });
});
