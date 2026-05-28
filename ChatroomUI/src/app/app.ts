import { Component, OnInit, signal, ɵbypassSanitizationTrustScript } from '@angular/core';
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
import { FormsModule } from '@angular/forms';
import { InputType } from './Models/InputType';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CommonModule, HttpClientModule, FormsModule],
  providers: [ApiService],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App implements OnInit {
  InputType = InputType;
  protected readonly title = signal('ChatroomUI');

  // Html State Indicators
  public creatingConversation: boolean = false;
  public editingConversation: boolean = false;
  public creatingAccount: boolean = false;
  public loading: boolean = false;

  // States
  public conversationAdmin: boolean = false;
  public signedIn: boolean = false;

  // Backend Saved Inputs
  public focusedConversation: string = '';
  public focusedConversationName: string = '';
  public errorMessage: string = '';
  public currentName: string = '';
  public currentIcon: string = '';
  public currentColor: string = '';
  public usernameCreation: string = '';
  public passwordCreation: string = '';
  public messageCreation: string = '';
  public conversationCreation: string = '';
  public participantCreation: string = '';
  public displayNameCreation: string = '';

  // Storage
  public conversations: ConversationDTO[] = [];
  public focusedMessages: MessageDTO[] = [];
  messagesMap = new Map<string, MessageDTO[]>();
  public icons: string[] = [
    '📖',
    '💯',
    '🎮',
    '🪜',
    '🗿',
    '⚒️',
    '🍪',
    '🌍',
    '🔒',
    '💎',
    '🎁',
    '⚓',
    '🖋️',
    '🎲',
    '🍔',
    '⛱️',
    '🐸',
    '🫂',
    '💀',
    '❤️‍🔥',
    '🪼',
    '🦋',
    '🚕',
    '🪙',
  ];
  public colors: string[] = [
    '#FF312E',
    '#35A7FF',
    '#FADB12',
    '#52AA5E',
    '#E28151',
    '#9565BB',
    '#0B111D',
    '#DFE5F1',
    '#64686F',
    '#594037',
    '#0013E8',
    '#F2B5D4',
  ];
  constructor(
    private apiService: ApiService,
    private chatHub: ChatHubService,
  ) {}

  // Input Checking Methods
  LimitInput(
    event: Event,
    chars: number,
    lowercase: boolean,
    specialCharacters: boolean,
    tag: boolean,
    type: InputType,
  ) {
    const input = event.target as HTMLInputElement;
    let value = input.value;
    value = value.slice(0, chars);
    value = lowercase ? value.toLowerCase() : value;
    value = !specialCharacters ? value.replace(/[^a-z0-9]/g, '') : value;
    value = tag ? '@' + value.replace(/^@+/, '') : value;
    input.value = value;
    switch (type) {
      case InputType.Username:
        this.usernameCreation = value;
        break;
      case InputType.Password:
        this.passwordCreation = value;
        break;
      case InputType.DisplayName:
        this.displayNameCreation = value;
        break;
      case InputType.Conversation:
        this.conversationCreation = value;
        break;
      case InputType.Participant:
        this.participantCreation = value;
        break;
    }
  }
  confirmContentLimitations(username: string, password: string): string {
    if (username.length <= 2) {
      return 'Username must be more than 3 characters!';
    }
    if (password.length <= 6) {
      return 'Password must be more than 6 characters!';
    }
    return '';
  }

  // Lifecycle Logic
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
    this.apiService.getUsername().subscribe((name) => {
      this.currentName = name;
      console.log(name);
    });
  }

  //
  // API Calling Methods
  //

  // Users
  Login(username: string, password: string) {
    this.loading = true;
    this.apiService.login(username, password).subscribe({
      next: (response: any) => {
        localStorage.setItem('auth_token', response.token);
        console.log('Login successful: ', response);
        this.signedIn = true;
        this.loadConversations();
        this.errorMessage = '';
        this.loading = false;
      },
      error: (error) => {
        console.error('Login failed: ', error);
        this.errorMessage = error.error.split('\n')[0] || 'Unknown error';
        this.loading = false;
      },
    });
  }
  Register(username: string, password: string) {
    var result = this.confirmContentLimitations(username, password);
    if (result != '') {
      this.errorMessage = result;
      return;
    }
    this.loading = true;
    this.apiService.register(username, password).subscribe({
      next: (response: any) => {
        console.log('Registration successful: ', response);
        this.Login(username, password);
        this.errorMessage = '';
        this.loading = false;
      },
      error: (error) => {
        console.error('Registration failed: ', error);
        this.errorMessage = error.error.split('\n')[0] || 'Unknown error';
        this.loading = false;
      },
    });
  }
  Logout() {
    localStorage.removeItem('auth_token');
    this.signedIn = false;
    this.conversations = [];
  }

  // Conversations

  CreateConversation(targetUserId: string, color: string, icon: string, name: string) {
    targetUserId = targetUserId.slice(1);
    this.apiService.createConversation(targetUserId, color, icon, name).subscribe({
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
  FocusConversation(id: string, name: string) {
    const messages = this.messagesMap.get(id);
    this.focusedConversation = id;
    this.focusedConversationName = name;
    this.conversationAdmin = false;
    this.apiService.getConversationAdmin(id).subscribe({
      next: (data: string) => {
        console.log(data);

        if (data === this.currentName) {
          this.conversationAdmin = true;
        }
      },

      error: (err) => {
        console.error(err);
      },
    });
    if (!messages) return;
    this.focusedMessages = messages;
  }
  LeaveConversation(id: string) {
    this.apiService.leaveConversation(id).subscribe({
      next: () => {
        this.conversations = this.conversations.filter((c) => c.id !== id);
      },
    });
  }
  DeleteConversation(id: string) {
    this.apiService.deleteConversation(id).subscribe({
      next: () => {},
    });
  }
  loadConversations() {
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
  UpdateConversation(convoId: string, name: string, color: string, icon: string) {
    this.apiService.updateConversation(convoId, name, color, icon).subscribe({
      next: () => {
        console.log('Conversation updated');
      },
      error: (error) => {
        console.error('Failed to update conversation: ', error);
        this.errorMessage = 'Could not update conversation';
      },
    });
  }

  // Messages
  CreateMessage(convoId: string, message: string) {
    this.messageCreation = '';
    this.apiService.createMessage(convoId, message).subscribe();
  }
  loadAllMessages(conversations: ConversationDTO[]) {
    const requests = conversations.map((convo) =>
      this.apiService
        .getMessages(convo.id)
        .pipe(tap((messages) => this.messagesMap.set(convo.id, messages))),
    );
    forkJoin(requests).subscribe();
  }
}
