import { Component, OnInit, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { ConversationDTO } from './Models/Conversation';
import { ApiService } from './Services/api';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CommonModule, HttpClientModule],
  providers: [ApiService],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App implements OnInit {
  protected readonly title = signal('ChatroomUI');

  public signedIn: boolean = false;
  public errorMessage: string = '';
  public conversations: ConversationDTO[] = [];

  constructor(private apiService: ApiService) {}

  ngOnInit() {
    const token = localStorage.getItem('auth_token');
    if (token) {
      this.signedIn = true;
      console.log('User is already logged in (token exists)');
      this.loadConversations();
    } else {
      this.signedIn = false;
      console.log('No token found, user is not logged in');
    }
  }

  Login(username: string, password: string) {
    this.apiService.login(username, password).subscribe({
      next: (response: any) => {
        localStorage.setItem('auth_token', response.token);
        console.log('Login successful: ', response);
        this.signedIn = true;
        this.loadConversations();
        this.errorMessage = '';
      },
      error: (error) => {
        console.error('Login failed: ', error);
        this.errorMessage = error.error.split('\n')[0] || 'Unknown error';
      },
    });
  }

  Register(username: string, password: string) {
    this.apiService.register(username, password).subscribe({
      next: (response: any) => {
        console.log('Registration successful: ', response);
        this.Login(username, password);
        this.errorMessage = '';
      },
      error: (error) => {
        console.error('Registration failed: ', error);
        this.errorMessage = error.error.split('\n')[0] || 'Unknown error';
      },
    });
  }

  DeleteConversation(id: string) {
    this.apiService.deleteConversation(id).subscribe({
      next: () => {},
    });
  }
  LeaveConversation(id: string) {
    this.apiService.leaveConversation(id).subscribe({
      next: () => {},
    });
  }

  Logout() {
    localStorage.removeItem('auth_token');
    this.signedIn = false;
    this.conversations = [];
  }

  private loadConversations() {
    this.apiService.getConversations().subscribe({
      next: (data: ConversationDTO[]) => {
        console.log('Conversations loaded: ', data);
        this.conversations = data;
      },
      error: (error) => {
        console.error('Failed to load conversations: ', error);
        this.errorMessage = 'Could not load conversations';
      },
    });
  }

  CreateConversation(targetUserId: string) {
    this.apiService.createConversation(targetUserId).subscribe({
      next: (conversationId: string) => {
        console.log('Conversation created: ', conversationId);
        this.loadConversations();
      },
      error: (error) => {
        console.error('Failed to create conversation: ', error);
        this.errorMessage = 'Could not create conversation';
      },
    });
  }
}
