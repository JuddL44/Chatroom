import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  protected readonly title = signal('ChatroomUI');
  private readonly APIURL = 'http://localhost:5228/api/auth';
  public errorMessage: string = '';
  constructor(private http: HttpClient) {}
  signedIn: boolean = false;

  ngOnInit() {
    const token = localStorage.getItem('auth_token');
    if (token) {
      this.signedIn = true;
      console.log('User is already logged in (token exists)');
    } else {
      this.signedIn = false;
      console.log('No token found, user is not logged in');
    }
  }

  Login(username: string, password: string) {
    const loginData = { username, password };
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });

    this.http.post(`${this.APIURL}/login`, loginData, { headers }).subscribe({
      next: (response: any) => {
        localStorage.setItem('auth_token', response.token);
        console.log('Login successful: ', response);
        this.signedIn = true;
      },
      error: (error) => {
        console.error('Login failed: ', error);
        this.errorMessage = error.error.split('\n')[0] || 'Unknown error';
      },
    });
  }

  Register(username: string, password: string) {
    const loginData = { username, password };
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });

    this.http.post(`${this.APIURL}/register`, loginData, { headers }).subscribe({
      next: (response: any) => {
        this.Login(username, password);
        console.log('Registration successful: ', response);
        this.signedIn = true;
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
  }
}
