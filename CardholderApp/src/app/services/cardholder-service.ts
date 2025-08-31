import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { APP_CONFIG } from '../config.token';
import { Observable } from 'rxjs';
import { Cardholder } from '../models/cardholder';
import { PagedResult } from '../models/pagedresult';

@Injectable({
  providedIn: 'root'
})
export class CardholderService {
  private http = inject(HttpClient);
  private config = inject(APP_CONFIG);

  //private apiUrl = `${this.config.apiUrl}/cardholders`;
  private apiUrl = 'https://localhost:7276/api/cardholders'; // TO DO: Ispraviti nemogućnost inject-anja config tokena

  getAll(): Observable<Cardholder[]> {
    return this.http.get<Cardholder[]>(this.apiUrl);
  }

  getPaged(page: number, pageSize: number, sortOrder: 'asc' | 'desc' = 'desc') {
    return this.http.get<PagedResult<Cardholder>>(
      `${this.apiUrl}?page=${page}&pageSize=${pageSize}&sortOrder=${sortOrder}`
    );
  }

  create(cardholder: Cardholder): Observable<Cardholder> {
    return this.http.post<Cardholder>(this.apiUrl, cardholder);
  }

  update(cardholder: Cardholder): Observable<Cardholder> {
    return this.http.put<Cardholder>(`${this.apiUrl}/${cardholder.id}`, cardholder);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
