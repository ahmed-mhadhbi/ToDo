import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class TodoService {
  private baseUrl = `${environment.apiUrl}/todos`;

  constructor(private http: HttpClient) {}

  getMyTodos() {
    return this.http.get<any>(this.baseUrl);
  }
  createTodo(data: { title: string }) {
    return this.http.post<any>(this.baseUrl, data);
  }
  deleteTodo(id: string) {
    return this.http.delete<any>(`${this.baseUrl}/${id}`);
  }

  changePassword(data: {currentPassword: string;newPassword: string;}) {
    return this.http.post(`${this.baseUrl}/change-password`,data);
}
}
