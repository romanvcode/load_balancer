import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TaskState } from '../models/task-state.model';

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  private baseUrl = 'http://localhost/api/tasks';

  constructor(private http: HttpClient) {}

  submitTask(size: number): Observable<TaskState> {
    return this.http.post<TaskState>(`${this.baseUrl}/start`, size);
  }

  getTaskProgress(): Observable<number> {
    return this.http.get<number>(`${this.baseUrl}/progress`);
  }

  getTaskHistory(): Observable<TaskState[]> {
    return this.http.get<TaskState[]>(`${this.baseUrl}/history`);
  }

  cancelTask(id: number): Observable<any> {
    return this.http.post(`${this.baseUrl}/cancel/${id}`, {});
  }
}
