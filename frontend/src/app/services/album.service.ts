import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AlbumSaveRequest, AlbumSummary } from '../album.model';

@Injectable({ providedIn: 'root' })
export class AlbumService {
  private readonly albumsUrl = '/api/albums';

  constructor(private readonly http: HttpClient) {}

  getAlbums(): Observable<AlbumSummary[]> {
    return this.http.get<AlbumSummary[]>(this.albumsUrl);
  }

  createAlbum(request: AlbumSaveRequest): Observable<AlbumSummary> {
    return this.http.post<AlbumSummary>(this.albumsUrl, request);
  }

  updateAlbum(id: string, request: Partial<AlbumSaveRequest>): Observable<AlbumSummary> {
    return this.http.patch<AlbumSummary>(`${this.albumsUrl}/${id}`, request);
  }

  importPhotos(): Observable<{ importedCount: number }> {
    return this.http.post<{ importedCount: number }>('/api/photos/import', {});
  }
}
