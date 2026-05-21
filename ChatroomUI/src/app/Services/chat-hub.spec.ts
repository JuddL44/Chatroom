import { TestBed } from '@angular/core/testing';

import { ChatHubService } from './chat-hub';

describe('ChatHub', () => {
  let service: ChatHubService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ChatHubService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
