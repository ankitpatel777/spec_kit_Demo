import { Component, OnInit } from '@angular/core';
import { AlbumSaveRequest, AlbumSummary } from '../../album.model';
import { AlbumService } from '../../services/album.service';

@Component({
  selector: 'app-album-overview',
  templateUrl: './album-overview.component.html',
  styleUrls: ['./album-overview.component.css'],
})
export class AlbumOverviewComponent implements OnInit {
  albums: AlbumSummary[] = [];
  selectedAlbum: AlbumSummary | null = null;
  showEditor = false;
  loading = false;
  importing = false;
  saving = false;
  errorMessage = '';
  editorError = '';
  notice = '';

  constructor(private readonly albumService: AlbumService) {}

  ngOnInit(): void {
    this.reload();
  }

  reload(): void {
    this.loading = true;
    this.errorMessage = '';
    this.albumService.getAlbums().subscribe({
      next: (albums) => {
        this.albums = [...albums].sort((a, b) => {
          const aDate = a.startDateUtc ? Date.parse(a.startDateUtc) : Number.MAX_SAFE_INTEGER;
          const bDate = b.startDateUtc ? Date.parse(b.startDateUtc) : Number.MAX_SAFE_INTEGER;
          return aDate - bDate || a.name.localeCompare(b.name);
        });
        this.loading = false;
      },
      error: (error: { message?: string }) => {
        this.errorMessage = error.message ?? 'Unable to load albums.';
        this.loading = false;
      },
    });
  }

  importPhotos(): void {
    if (this.importing) return;

    this.importing = true;
    this.errorMessage = '';
    this.notice = '';
    this.albumService.importPhotos().subscribe({
      next: ({ importedCount }) => {
        this.importing = false;
        this.notice = `${importedCount} new ${importedCount === 1 ? 'photo' : 'photos'} imported.`;
        this.reload();
      },
      error: (error: { message?: string }) => {
        this.errorMessage = error.message ?? 'Unable to scan the local photo folder.';
        this.importing = false;
      },
    });
  }

  createAlbum(): void {
    this.selectedAlbum = null;
    this.editorError = '';
    this.showEditor = true;
  }

  editAlbum(album: AlbumSummary): void {
    this.selectedAlbum = album;
    this.editorError = '';
    this.showEditor = true;
  }

  saveAlbum(event: { id: string | null; request: AlbumSaveRequest }): void {
    this.saving = true;
    this.editorError = '';
    const operation = event.id
      ? this.albumService.updateAlbum(event.id, event.request)
      : this.albumService.createAlbum(event.request);

    operation.subscribe({
      next: () => {
        this.saving = false;
        this.showEditor = false;
        this.notice = 'Album saved.';
        this.reload();
      },
      error: (error: { message?: string; error?: { detail?: string } }) => {
        this.saving = false;
        this.editorError = error.error?.detail ?? error.message ?? 'Unable to save album.';
      },
    });
  }

  closeEditor(): void {
    if (!this.saving) this.showEditor = false;
  }
}
