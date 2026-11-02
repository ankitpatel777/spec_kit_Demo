# Album API Contract

## Overview

The backend exposes a small, focused API for album browsing and photo metadata management. The contract is designed for the Angular frontend and the local photo organizer workflow.

## Base path

`/api`

## Endpoints

### GET /api/albums

Returns all albums in the current library, with summary data for the tile view.

**Response body**:

```json
[
  {
    "id": "guid",
    "name": "Summer Trip",
    "startDateUtc": "2026-08-01T00:00:00Z",
    "endDateUtc": "2026-08-10T00:00:00Z",
    "coverPhotoId": "guid",
    "photoCount": 42
  }
]
```

### GET /api/albums/{albumId}

Returns detailed album data including the list of photo identifiers and the date range.

### POST /api/albums

Creates a new album.

**Request body**:

```json
{
  "name": "Weekend in Paris",
  "startDateUtc": "2026-06-12T00:00:00Z",
  "endDateUtc": "2026-06-14T00:00:00Z"
}
```

### PATCH /api/albums/{albumId}

Updates album name or date range metadata.

### GET /api/photos

Returns photo metadata and unresolved date records for browsing and review.

**Response body**:

```json
[
  {
    "id": "guid",
    "originalPath": "C:/Photos/2026/June/IMG_001.jpg",
    "fileName": "IMG_001.jpg",
    "takenOnUtc": "2026-06-13T10:42:00Z",
    "isDateResolved": true
  }
]
```

### GET /api/albums/{albumId}/photos

Returns the photo list for a specific album with ordering metadata.

## Error handling

- 400 Bad Request: invalid date range or malformed request body
- 404 Not Found: album or photo does not exist
- 409 Conflict: duplicate or conflicting album metadata assignment
- 500 Internal Server Error: unexpected server failure

## Notes

These contracts deliberately avoid implementation details such as database schema or framework choices. They define the public functionality needed by the Angular UI while leaving the internal implementation flexible.
