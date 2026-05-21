import * as signalR from '@microsoft/signalr';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ChatHubService {
  private hubConnection?: signalR.HubConnection;

  startConnection(token: string): Promise<void> {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5228/chat', {
        accessTokenFactory: () => token,
      })
      .withAutomaticReconnect()
      .build();

    return this.hubConnection.start();
  }

  joinConversation(conversationId: string) {
    this.hubConnection?.invoke('JoinConversation', conversationId);
  }

  leaveConversation(conversationId: string) {
    this.hubConnection?.invoke('LeaveConversation', conversationId);
  }

  onMessageReceived(callback: any) {
    this.hubConnection?.on('ReceiveMessage', callback);
  }

  onConversationCreate(callback: any) {
    this.hubConnection?.on('ConversationUpdate', callback);
  }

  onConversationDelete(callback: any) {
    this.hubConnection?.on('DeleteConversation', callback);
  }
}
