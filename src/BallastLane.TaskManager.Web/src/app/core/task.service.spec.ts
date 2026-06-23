import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';

import { API_BASE_URL } from './api.config';
import { CreateTaskRequest, TaskItemStatus, TaskService, UpdateTaskRequest } from './task.service';

describe('TaskService', () => {
  let service: TaskService;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        TaskService,
        provideHttpClient(),
        provideHttpClientTesting(),
      ],
    });

    service = TestBed.inject(TaskService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    http.verify();
  });

  it('loads tasks for the current user', () => {
    service.getTasks().subscribe((tasks) => {
      expect(tasks.length).toBe(1);
      expect(tasks[0].title).toBe('Prepare interview');
    });

    const request = http.expectOne(`${API_BASE_URL}/tasks`);
    expect(request.request.method).toBe('GET');
    request.flush([
      {
        id: '22222222-2222-2222-2222-222222222222',
        userId: '11111111-1111-1111-1111-111111111111',
        title: 'Prepare interview',
        description: 'Review the project.',
        status: TaskItemStatus.Pending,
        dueDate: '2026-07-01T00:00:00Z',
        createdAt: '2026-06-22T00:00:00Z',
        updatedAt: '2026-06-22T00:00:00Z',
      },
    ]);
  });

  it('creates a task', () => {
    const payload: CreateTaskRequest = {
      title: 'Build UI',
      description: 'Connect Angular to the API.',
      dueDate: '2026-07-03',
    };

    service.createTask(payload).subscribe();

    const request = http.expectOne(`${API_BASE_URL}/tasks`);
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(payload);
    request.flush({});
  });

  it('updates a task', () => {
    const payload: UpdateTaskRequest = {
      title: 'Build UI',
      description: 'Connect Angular to the API.',
      dueDate: '2026-07-03',
      status: TaskItemStatus.InProgress,
    };

    service.updateTask('task-id', payload).subscribe();

    const request = http.expectOne(`${API_BASE_URL}/tasks/task-id`);
    expect(request.request.method).toBe('PUT');
    expect(request.request.body).toEqual(payload);
    request.flush({});
  });

  it('deletes a task', () => {
    service.deleteTask('task-id').subscribe();

    const request = http.expectOne(`${API_BASE_URL}/tasks/task-id`);
    expect(request.request.method).toBe('DELETE');
    request.flush(null);
  });
});
