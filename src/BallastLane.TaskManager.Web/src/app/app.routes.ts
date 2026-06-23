import { Routes } from '@angular/router';

import { authGuard } from './core/auth.guard';
import { LoginComponent } from './features/auth/login.component';
import { RegisterComponent } from './features/auth/register.component';
import { TasksComponent } from './features/tasks/tasks.component';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'tasks', component: TasksComponent, canActivate: [authGuard] },
  { path: '', pathMatch: 'full', redirectTo: 'tasks' },
  { path: '**', redirectTo: 'tasks' },
];
