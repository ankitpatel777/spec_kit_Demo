import { CommonModule } from '@angular/common';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { of, Subject } from 'rxjs';
import { AlbumSummary } from '../../album.model';
import { AlbumEditorComponent } from '../../components/album-editor/album-editor.component';
import { AlbumTileComponent } from '../../components/album-tile/album-tile.component';
import { AlbumService } from '../../services/album.service';
import { AlbumOverviewComponent } from './album-overview.component';

describe('AlbumOverviewComponent', () => {
  let fixture: ComponentFixture<AlbumOverviewComponent>;
  let component: AlbumOverviewComponent;
  let service: jasmine.SpyObj<AlbumService>;

  beforeEach(async () => {
    service = jasmine.createSpyObj<AlbumService>('AlbumService', [
      'getAlbums',
      'createAlbum',
      'updateAlbum',
      'importPhotos',
    ]);
    service.getAlbums.and.returnValue(
      of([createAlbum('Recent', '2026-06-01'), createAlbum('Past', '2025-01-01')]),
    );
    service.createAlbum.and.returnValue(of(createAlbum('Created', '2026-07-01')));
    service.updateAlbum.and.returnValue(of(createAlbum('Renamed', '2026-07-01')));

    await TestBed.configureTestingModule({
      imports: [CommonModule, FormsModule],
      declarations: [AlbumOverviewComponent, AlbumTileComponent, AlbumEditorComponent],
      providers: [{ provide: AlbumService, useValue: service }],
    }).compileComponents();

    fixture = TestBed.createComponent(AlbumOverviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('sorts albums chronologically and reloads after creating an album', () => {
    const names = Array.from(
      (fixture.nativeElement as HTMLElement).querySelectorAll('.album-tile h2'),
    ).map((heading) => heading.textContent?.trim());
    expect(names).toEqual(['Past', 'Recent']);

    component.createAlbum();
    component.saveAlbum({ id: null, request: { name: 'Created' } });
    fixture.detectChanges();

    expect(service.createAlbum).toHaveBeenCalledWith({ name: 'Created' });
    expect(service.getAlbums).toHaveBeenCalledTimes(2);
    expect(component.showEditor).toBeFalse();
  });

  it('updates existing albums and keeps date-group albums in the date group', () => {
    const dateAlbum = createAlbum('2026-06-01', '2026-06-01');
    component.editAlbum(dateAlbum);
    component.saveAlbum({ id: dateAlbum.id, request: { name: 'Renamed date album' } });

    expect(service.updateAlbum).toHaveBeenCalledWith(dateAlbum.id, { name: 'Renamed date album' });
    expect(component.showEditor).toBeFalse();
    expect(component.notice).toBe('Album saved.');
  });

  it('allows importing photos while albums reload after saving', () => {
    service.getAlbums.and.returnValue(new Subject<AlbumSummary[]>().asObservable());
    service.importPhotos.and.returnValue(of({ importedCount: 0 }));

    component.createAlbum();
    component.saveAlbum({ id: null, request: { name: 'Created' } });
    fixture.detectChanges();

    const importButton = (fixture.nativeElement as HTMLElement).querySelector(
      '.toolbar button:nth-of-type(2)',
    ) as HTMLButtonElement;
    expect(component.loading).toBeTrue();
    expect(importButton.disabled).toBeFalse();

    importButton.click();
    fixture.detectChanges();

    expect(service.importPhotos).toHaveBeenCalledTimes(1);
    expect(component.importing).toBeFalse();
  });
});

function createAlbum(name: string, date: string): AlbumSummary {
  return {
    id: name,
    name,
    coverPhotoId: null,
    coverImageUrl: null,
    startDateUtc: `${date}T00:00:00Z`,
    endDateUtc: `${date}T00:00:00Z`,
    dateRange: date,
    photoCount: 0,
    isEmpty: true,
    isDateGroup: true,
  };
}
