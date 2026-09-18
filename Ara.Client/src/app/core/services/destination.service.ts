import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { Destination } from '../models/destination';

@Injectable({ providedIn: 'root' })
export class DestinationService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/destinations';

  getAll(): Observable<Destination[]> {
    return this.http.get<Destination[]>(this.baseUrl);
  }

  getById(id: number): Observable<Destination> {
    return this.http.get<Destination>(`${this.baseUrl}/${id}`);
  }
}
