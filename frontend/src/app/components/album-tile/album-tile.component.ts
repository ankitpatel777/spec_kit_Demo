import { Component, EventEmitter, Input, Output } from '@angular/core';
import { AlbumSummary } from '../../album.model';

@Component({
  selector: 'app-album-tile',
  templateUrl: './album-tile.component.html',
  styleUrls: ['./album-tile.component.css'],
})
export class AlbumTileComponent {
  @Input() album!: AlbumSummary;
  @Output() edit = new EventEmitter<AlbumSummary>();
}
