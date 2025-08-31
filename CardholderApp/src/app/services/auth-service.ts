import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { map, Observable, of, tap } from "rxjs";
import { APP_CONFIG } from "../config.token";

@Injectable({ providedIn: 'root' })

export class AuthService {
  private token: string | null = null;
  private http = inject(HttpClient);
  private config = inject(APP_CONFIG);

  private secretKey = 'a97cc3e0ef9c047d57c9348fdd8c78fef2a7aace505521caee700601b6633a84b491e08';

  private get apiUrl(): string {
    return `https://localhost:7276/api/auth`;
    //return `${this.config.apiUrl}/auth`; // TO DO: Ispraviti nemogućnost inject-anja config tokena
  }

  getToken(): Observable<string> {
    if (this.token) return of(this.token);

    return this.http.post<{ token: string }>(
      `${this.apiUrl}`,
      { secretKey: this.secretKey }
    ).pipe(
      tap(res => this.token = res.token),
      map(res => res.token)
    );
  }
}
