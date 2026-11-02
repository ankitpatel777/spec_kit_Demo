export interface AlbumSummary {
  id: string;
  name: string;
  coverPhotoId: string | null;
  coverImageUrl: string | null;
  startDateUtc: string | null;
  endDateUtc: string | null;
  dateRange: string;
  photoCount: number;
  isEmpty: boolean;
  isDateGroup: boolean;
}

export interface AlbumPhoto {
  id: string;
  fileName: string;
  takenOnUtc: string | null;
  isDateResolved: boolean;
  contentUrl: string;
}

export interface AlbumSaveRequest {
  name: string;
  startDateUtc?: string | null;
  endDateUtc?: string | null;
}
