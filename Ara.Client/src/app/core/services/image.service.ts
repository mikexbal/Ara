import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ImageService {
  private readonly http = inject(HttpClient);

  getHeroImages(): Observable<string[]> {
    return this.http.get<string[]>('/api/images/hero');
  }
}
