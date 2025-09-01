import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { map, Observable, of, tap } from "rxjs";
import { APP_CONFIG } from "../config.token";
import { API_BASE_URL, SECRET_KEY } from "../constants";

@Injectable({ providedIn: 'root' })

export class AuthService {
  private token: string | null = null;
  private http = inject(HttpClient);
  private config = inject(APP_CONFIG);

  // TO DO: Ispraviti nemogućnost inject-anja config tokena - komentirati ovo
  private get apiUrl(): string {
    return `${API_BASE_URL}/auth`; // workaround
    //return `${this.config.apiUrl}/auth`; 
  }

  getToken(): Observable<string> {
    if (this.token) return of(this.token);

    return this.http.post<{ token: string }>(
      `${this.apiUrl}`,
      { secretKey: SECRET_KEY } // workaround
    ).pipe(
      tap(res => this.token = res.token),
      map(res => res.token)
    );
  }
}
