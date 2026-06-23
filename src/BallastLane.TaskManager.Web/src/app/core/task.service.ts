import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { API_BASE_URL } from './api.config';

export enum TaskItemStatus {
  Pending = 1,
  InProgress = 2,
  Completed = 3,
}

export interface TaskItem {
  id: string;
  userId: string;
  title: string;
  description: string;
  status: TaskItemStatus;
  dueDate: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateTaskRequest {
  title: string;
  description: string | null;
  dueDate: string;
}

export interface UpdateTaskRequest extends CreateTaskRequest {
  status: TaskItemStatus;
}

@Injectable({
  providedIn: 'root',
})
export class TaskService {
  constructor(private readonly http: HttpClient) {
  }

  getTasks(): Observable<TaskItem[]> {
    return this.http.get<TaskItem[]>(`${API_BASE_URL}/tasks`);
  }

  createTask(request: CreateTaskRequest): Observable<TaskItem> {
    return this.http.post<TaskItem>(`${API_BASE_URL}/tasks`, request);
  }

  updateTask(id: string, request: UpdateTaskRequest): Observable<TaskItem> {
    return this.http.put<TaskItem>(`${API_BASE_URL}/tasks/${id}`, request);
  }

  deleteTask(id: string): Observable<void> {
    return this.http.delete<void>(`${API_BASE_URL}/tasks/${id}`);
  }
}
