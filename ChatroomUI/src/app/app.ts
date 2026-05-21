import { Component, OnInit, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { ConversationDTO } from './Models/Conversation';
import { ApiService } from './Services/api';
import { ChatHubService } from './Services/chat-hub';
import { firstValueFrom, forkJoin } from 'rxjs';
import { tap } from 'rxjs/operators';
import { MessageDTO } from './Models/Message';
import { ConversationUpdate } from './Models/ConversationUpdate';

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

  messagesMap = new Map<string, MessageDTO[]>();
  constructor(
    private apiService: ApiService,
    private chatHub: ChatHubService,
  ) {}

  async ngOnInit() {
    const token = localStorage.getItem('auth_token');
    if (token) {
      try {
        await this.chatHub.startConnection(token);

        console.log('SignalR connected');

        this.signedIn = true;

        this.loadConversations();
      } catch (err) {
        console.error(err);
      }
    } else {
      this.signedIn = false;
      console.log('No token found, user is not logged in');
    }
    this.chatHub.onMessageReceived((message: MessageDTO) => {
      const existingMessages = this.messagesMap.get(message.conversationId) || [];

      existingMessages.push(message);

      this.messagesMap.set(message.conversationId, existingMessages);
    });
    this.chatHub.onConversationCreate((data: ConversationUpdate) => {
      console.log('ConversationCreate:', data);

      this.conversations.push({
        id: data.conversationId,
      } as ConversationDTO);
      this.chatHub.joinConversation(data.conversationId);
      this.messagesMap.set(data.conversationId, []);
    });
    this.chatHub.onConversationDelete((data: { conversationId: string }) => {
      console.log('ConversationDelete:', data.conversationId);
      this.conversations = this.conversations.filter((c) => c.id !== data.conversationId);
      this.messagesMap.delete(data.conversationId);
    });
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
  Logout() {
    localStorage.removeItem('auth_token');
    this.signedIn = false;
    this.conversations = [];
  }

  DeleteConversation(id: string) {
    this.apiService.deleteConversation(id).subscribe({
      next: () => {},
    });
  }
  LeaveConversation(id: string) {
    this.apiService.leaveConversation(id).subscribe({
      next: () => {
        this.conversations = this.conversations.filter((c) => c.id !== id);
      },
    });
  }

  private loadConversations() {
    this.apiService.getConversations().subscribe({
      next: (data: ConversationDTO[]) => {
        console.log('Conversations loaded: ', data);
        this.conversations = data;
        this.loadAllMessages(this.conversations);
        this.conversations.forEach((convo) => {
          this.chatHub.joinConversation(convo.id);
        });
      },
      error: (error) => {
        console.error('Failed to load conversations: ', error);
        this.errorMessage = 'Could not load conversations';
      },
    });
  }
  loadAllMessages(conversations: ConversationDTO[]) {
    const requests = conversations.map((convo) =>
      this.apiService
        .getMessages(convo.id)
        .pipe(tap((messages) => this.messagesMap.set(convo.id, messages))),
    );
    forkJoin(requests).subscribe();
  }

  CreateMessage(convoId: string, message: string) {
    this.apiService.createMessage(convoId, message).subscribe();
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
