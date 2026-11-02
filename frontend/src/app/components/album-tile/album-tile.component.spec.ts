import { CommonModule } from '@angular/common';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AlbumSummary } from '../../album.model';
import { AlbumTileComponent } from './album-tile.component';

describe('AlbumTileComponent', () => {
  let fixture: ComponentFixture<AlbumTileComponent>;
  let component: AlbumTileComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CommonModule],
      declarations: [AlbumTileComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(AlbumTileComponent);
    component = fixture.componentInstance;
  });

  it('renders the cover, date context, and photo count', () => {
    component.album = createAlbum({
      coverImageUrl: 'data:image/gif;base64,R0lGODlhAQABAAD/ACwAAAAAAQABAAACADs=',
      dateRange: '2026-06-01 to 2026-06-03',
      photoCount: 3,
    });
    fixture.detectChanges();

    expect(fixture.nativeElement.querySelector('img').getAttribute('src')).toBe(
      'data:image/gif;base64,R0lGODlhAQABAAD/ACwAAAAAAQABAAACADs=',
    );
    expect(fixture.nativeElement.textContent).toContain('2026-06-01 to 2026-06-03');
    expect(fixture.nativeElement.textContent).toContain('3 photos');
  });

  it('shows an accessible empty-album state without a cover image', () => {
    component.album = createAlbum({ coverImageUrl: null, isEmpty: true, photoCount: 0 });
    fixture.detectChanges();

    expect(fixture.nativeElement.querySelector('img')).toBeNull();
    expect(fixture.nativeElement.textContent).toContain('This album has no photos yet.');
    expect(fixture.nativeElement.textContent).toContain('No cover photo');
  });
});

function createAlbum(overrides: Partial<AlbumSummary> = {}): AlbumSummary {
  return {
    id: 'album-1',
    name: 'Summer trip',
    coverPhotoId: 'photo-1',
    coverImageUrl: '/api/photos/photo-1/content',
    startDateUtc: '2026-06-01T00:00:00Z',
    endDateUtc: '2026-06-03T00:00:00Z',
    dateRange: '2026-06-01 to 2026-06-03',
    photoCount: 3,
    isEmpty: false,
    isDateGroup: true,
    ...overrides,
  };
}
