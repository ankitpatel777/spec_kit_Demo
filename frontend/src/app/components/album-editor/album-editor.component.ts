import { Component, EventEmitter, Input, Output, OnChanges, SimpleChanges } from '@angular/core';
import { AlbumSummary, AlbumSaveRequest } from '../../album.model';

@Component({
  selector: 'app-album-editor',
  templateUrl: './album-editor.component.html',
  styleUrls: ['./album-editor.component.css'],
})
export class AlbumEditorComponent implements OnChanges {
  @Input() album: AlbumSummary | null = null;
  @Input() saving = false;
  @Input() errorMessage = '';
  @Output() save = new EventEmitter<{ id: string | null; request: AlbumSaveRequest }>();
  @Output() cancel = new EventEmitter<void>();

  name = '';
  startDate = '';
  endDate = '';

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['album']) {
      this.name = this.album?.name ?? '';
      this.startDate = this.album?.startDateUtc?.slice(0, 10) ?? '';
      this.endDate = this.album?.endDateUtc?.slice(0, 10) ?? '';
    }
  }

  submit(): void {
    const request: AlbumSaveRequest = {
      name: this.name.trim(),
      startDateUtc: this.startDate ? new Date(`${this.startDate}T00:00:00Z`).toISOString() : null,
      endDateUtc: this.endDate ? new Date(`${this.endDate}T23:59:59Z`).toISOString() : null,
    };
    this.save.emit({ id: this.album?.id ?? null, request });
  }
}
