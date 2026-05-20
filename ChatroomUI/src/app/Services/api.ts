import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ConversationDTO } from '../Models/Conversation';

@Injectable({
  providedIn: 'root',
})
export class ApiService {
  private readonly APIURL = 'http://localhost:5228/api';
  constructor(private http: HttpClient) {}

  private getAuthToken(): string {
    return localStorage.getItem('auth_token') || '';
  }

  private getAuthHeaders(): HttpHeaders {
    return new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${this.getAuthToken()}`,
    });
  }

  login(username: string, password: string): Observable<any> {
    const loginData = { username, password };
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this.http.post(`${this.APIURL}/auth/login`, loginData, { headers });
  }

  register(username: string, password: string): Observable<any> {
    const registerData = { username, password };
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this.http.post(`${this.APIURL}/auth/register`, registerData, { headers });
  }

  getConversations(): Observable<ConversationDTO[]> {
    return this.http.get<ConversationDTO[]>(`${this.APIURL}/chat/conversations`, {
      headers: this.getAuthHeaders(),
    });
  }

  createConversation(targetUserId: string): Observable<string> {
    const headers = this.getAuthHeaders();
    return this.http.post<string>(
      `${this.APIURL}/chat/conversation`,
      { targetUserId },
      { headers },
    );
  }

  deleteConversation(convoId: string): Observable<void> {
    const headers = this.getAuthHeaders();
    return this.http.delete<void>(`${this.APIURL}/chat/${convoId}/delete`, { headers });
  }

  leaveConversation(convoId: string): Observable<void> {
    const headers = this.getAuthHeaders();
    return this.http.delete<void>(`${this.APIURL}/chat/${convoId}/leave`, { headers });
  }
}
