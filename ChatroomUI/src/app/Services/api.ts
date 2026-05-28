import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ConversationDTO } from '../Models/Conversation';
import { MessageDTO } from '../Models/Message';

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

  getMessages(convoId: string): Observable<MessageDTO[]> {
    return this.http.get<MessageDTO[]>(`${this.APIURL}/chat/${convoId}/messages`, {
      headers: this.getAuthHeaders(),
    });
  }

  getConversationAdmin(convoId: string): Observable<string> {
    return this.http.get(`${this.APIURL}/chat/${convoId}/admin`, {
      responseType: 'text',
    });
  }

  createConversation(
    targetUsername: string,
    color: string,
    icon: string,
    name: string,
  ): Observable<string> {
    const headers = this.getAuthHeaders();
    return this.http.post<string>(
      `${this.APIURL}/chat/conversation`,
      { targetUsername, color, icon, name },
      { headers },
    );
  }

  updateConversation(
    conversationId: string,
    name: string,
    color: string,
    icon: string,
  ): Observable<void> {
    const headers = this.getAuthHeaders();

    return this.http.put<void>(
      `${this.APIURL}/chat/${conversationId}/update`,
      {
        conversationId,
        name,
        color,
        icon,
      },
      { headers },
    );
  }

  getUsername(): Observable<string> {
    return this.http.get<string>(`${this.APIURL}/auth/username`, {
      responseType: 'text' as 'json',
    });
  }

  createMessage(convoId: string, message: string): Observable<string> {
    const headers = this.getAuthHeaders();
    return this.http.post<string>(
      `${this.APIURL}/chat/${convoId}/message`,
      { message },
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
