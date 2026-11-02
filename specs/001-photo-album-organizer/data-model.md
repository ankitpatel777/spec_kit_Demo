# Data Model: Photo Album Organizer

## Overview

The model is designed around a local photo library with date-based organization and an album preview layer. The main entities are Photo, Album, and the relationship between the two, plus derived metadata needed for sorting and preview generation.

## Entities

### Photo

Represents a user photo or image asset in the library.

| Field | Type | Description |
|-------|------|-------------|
| Id | GUID | Unique identifier for the photo |
| OriginalPath | string | Local filesystem path to the source image |
| FileName | string | Original file name |
| TakenOnUtc | DateTimeOffset? | Capture date from metadata when available |
| ImportedAtUtc | DateTimeOffset | Timestamp when the photo was added to the library |
| CreatedAtUtc | DateTimeOffset | System timestamp for record creation |
| UpdatedAtUtc | DateTimeOffset | Most recent metadata update |
| FileModifiedAtUtc | DateTimeOffset | Last filesystem modification observed during import |
| IsDateResolved | bool | Indicates whether the date could be determined reliably |
| Hash | string | Optional hash used to detect duplicates or repeated imports |

**Validation rules**:
- OriginalPath must be a non-empty local file path.
- Either a valid TakenOnUtc date or a fallback bucket assignment must exist.
- If the date is missing, the record must still be retained with an explicit unresolved state.
- Re-imports compare file size and modification time to refresh changed photo metadata.

### Album

Represents a date-based collection of photos.

| Field | Type | Description |
|-------|------|-------------|
| Id | GUID | Unique identifier for the album |
| Name | string | User-visible album name |
| StartDateUtc | DateTimeOffset? | Earliest photo date in the album |
| EndDateUtc | DateTimeOffset? | Latest photo date in the album |
| CreatedAtUtc | DateTimeOffset | When the album was created |
| UpdatedAtUtc | DateTimeOffset | Last update timestamp |
| CoverPhotoId | GUID? | Representative photo used for the album tile |
| PhotoCount | int | Current number of photos in the album |

**Validation rules**:
- Name must be non-empty after trimming.
- Album date range can be null until photos are assigned.
- CoverPhotoId may be null until the album has a photo.

### AlbumPhoto

Many-to-many relationship between albums and photos.

| Field | Type | Description |
|-------|------|-------------|
| AlbumId | GUID | Parent album |
| PhotoId | GUID | Photo included in the album |
| SortOrder | int | Ordering within the album |
| AssignDateUtc | DateTimeOffset? | Date used for album membership when relevant |

**Validation rules**:
- AlbumId and PhotoId must both reference existing records.
- SortOrder must be non-negative.
- A photo can belong to more than one album if the user intentionally assigns it.

### DateGroup

Derived grouping used for browsing and timeline display.

| Field | Type | Description |
|-------|------|-------------|
| Id | GUID | Unique grouping identifier |
| GroupKey | string | Key representing the day, week, or month |
| Label | string | Human-readable title displayed to the user |
| Type | enum | Day, Week, Month |

**Validation rules**:
- GroupKey must be deterministic and consistent with the date normalization rules.
- Labels are derived, not user-edited.

## Relationships

- One Album contains many AlbumPhoto records.
- One Photo belongs to many AlbumPhoto records.
- One Photo may have a single default cover relationship for zero or many albums.
- DateGroup is derived from Photo.TakenOnUtc and is not a direct user-managed entity.

## State transitions

**Photo lifecycle**:
- Imported -> date resolved or unresolved -> assigned to album or date group -> updated or reclassified.

**Album lifecycle**:
- Created -> populated with photos -> date range updated -> cover photo selected -> edited or renamed.

## Notes

This model keeps local image paths separate from the SQL Server metadata model. It supports quick album rendering, date grouping, and fallback handling when metadata is incomplete without requiring a hard requirement that every photo already has a valid date.
