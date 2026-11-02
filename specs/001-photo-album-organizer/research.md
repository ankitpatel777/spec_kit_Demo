# Research: Photo Album Organizer

## Decision: Use a split frontend and backend architecture with local-first storage

**Decision**: Build the feature as an Angular frontend served on the local workstation and an ASP.NET Core API that exposes date-based album data and metadata operations. Store album, photo, and relationship metadata in SQL Server on the local PC while leaving image files in a local directory managed by the application.

**Rationale**: This matches the product need for fast browsing, date-based grouping, and visual thumbnails while keeping image files local and metadata durable. It also respects the project constraint that SQL Server access must use Windows authentication and that the feature should be easy to maintain and test.

**Alternatives considered**:
- SQLite or file-based metadata storage: simpler for local-only apps, but it does not satisfy the explicit requirement to store metadata in SQL Server on the PC.
- Pure client-side processing only: reduces server complexity, but makes metadata persistence and multi-user or later extension scenarios harder to maintain.
- Remote cloud storage: conflicts with the requirement to keep images local and avoid external hosting.

## Decision: Use date-derived album grouping as the primary organizing model

**Decision**: Albums are grouped primarily by date or date range, with support for photo buckets that represent a day, week, or month as needed by the library.

**Rationale**: This directly aligns with the feature requirement that users browse by date and find albums quickly from a visual summary.

**Alternatives considered**:
- Tag-based grouping only: useful for later expansion but weaker for timeline-oriented browsing.
- Folder-based organization only: makes the library structure rigid and does not provide the tile preview model required by this feature.

## Decision: Generate album preview tiles from representative photos and metadata

**Decision**: Each album tile will display a representative cover image, album name, date range, and photo count, with the cover chosen from the first or most relevant photo in the album.

**Rationale**: This provides quick recognition without requiring users to open albums, and it matches the tile overview requirement with minimal cognitive overhead.

**Alternatives considered**:
- Text-only album listings: faster to implement but too weak for visual search and recognition.
- Full-card previews for every album: visually dense and slower to scan for large libraries.

## Decision: Treat missing photo dates as a first-class edge case

**Decision**: When a photo does not have a usable date, the system will place it in an unassigned bucket or a fallback album until the user assigns or corrects the date.

**Rationale**: The feature explicitly calls out incomplete date metadata as a common scenario. The fallback handling prevents the organizer from failing or silently dropping content.

**Alternatives considered**:
- Ignore missing dates entirely: creates incomplete albums and user confusion.
- Block uploads with missing metadata: too restrictive and not aligned with practical collection management.

## Decision: Use Windows authentication for SQL Server access

**Decision**: The application will connect to the local SQL Server instance using Windows authentication, with the local workstation credentials used for identity and access.

**Rationale**: This satisfies the requirement for SQL Server on the PC and avoids storing SQL credentials in the application.

**Alternatives considered**:
- SQL Server authentication with username/password: explicit requirement restricts this approach and increases credential handling complexity.
- Local file storage for metadata: not allowed by the project constraint.

## Decision: Keep image files local and do not exceed local-first constraints

**Decision**: The application manages local image files on disk and only stores metadata in SQL Server; generated thumbnails may be cached locally but should not be treated as the source of truth.

**Rationale**: This preserves a clear separation between durable metadata and the image assets themselves while matching the “keep images local” requirement.

**Alternatives considered**:
- Store images in the database: unnecessary complexity and not aligned with local-file image workflows.
- Download and sync remote copies: violates the local-first requirement.
