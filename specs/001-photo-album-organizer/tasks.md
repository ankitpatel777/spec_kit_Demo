# Tasks: Photo Album Organizer

**Input**: Design documents from `/specs/001-photo-album-organizer/`

**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and local-first app structure

- [X] T001 Create the backend and frontend project folders per the plan in backend/src/, backend/tests/, frontend/src/, and frontend/tests/
- [X] T002 Initialize the Angular frontend project in frontend/ with the required dependencies for the album overview experience
- [X] T003 Initialize the ASP.NET Core backend in backend/ with the required project references and local API startup configuration
- [X] T004 [P] Configure environment settings for local SQL Server access with Windows authentication and local image storage paths in backend/src/PhotoOrganizer.Api/appsettings.Development.json
- [X] T005 [P] Configure linting, formatting, and code quality tooling for the Angular and .NET projects

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Shared models, database access, and service boundaries required before user story work can begin

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [X] T006 Define the core domain model for Photo and Album in backend/src/PhotoOrganizer.Domain/Photo.cs and backend/src/PhotoOrganizer.Domain/Album.cs
- [X] T007 Implement the SQL Server database context and entity mappings in backend/src/PhotoOrganizer.Infrastructure/Data/PhotoOrganizerDbContext.cs
- [X] T008 [P] Create the database schema and migration scripts for Photo, Album, and AlbumPhoto records in backend/database/schema/
- [X] T009 [P] Implement the local image storage abstraction and file metadata access in backend/src/PhotoOrganizer.Infrastructure/FileStorage/
- [X] T010 Implement the application service boundary and dependency injection setup in backend/src/PhotoOrganizer.Application/ and backend/src/PhotoOrganizer.Api/Program.cs
- [X] T011 Add consistent error handling, logging, and validation rules for missing or invalid photo dates in backend/src/PhotoOrganizer.Api/Middleware/ and backend/src/PhotoOrganizer.Application/Validators/

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 1 - View photos organized by date (Priority: P1) 🎯 MVP

**Goal**: Allow users to browse a library grouped by date and view the core album structure without manual sorting.

**Independent Test**: Import sample photos spanning multiple dates and confirm the library displays date-grouped albums in chronological order.

### Implementation for User Story 1

- [X] T012 [P] [US1] Create the photo and album DTOs used by the API in backend/src/PhotoOrganizer.Application/Models/ and backend/src/PhotoOrganizer.Application/DTOs/
- [X] T013 [US1] Implement the date-grouping service that assigns photos to albums and date buckets in backend/src/PhotoOrganizer.Application/Services/AlbumGroupingService.cs
- [X] T014 [US1] Implement the repository methods for album listing and date-group metadata in backend/src/PhotoOrganizer.Infrastructure/Repositories/AlbumRepository.cs
- [X] T015 [P] [US1] Implement the GET /api/albums endpoint in backend/src/PhotoOrganizer.Api/Controllers/AlbumController.cs
- [X] T016 [US1] Implement the Angular album overview page in frontend/src/app/pages/album-overview/album-overview.component.ts and album-overview.component.html
- [X] T017 [P] [US1] Implement reusable album tile UI in frontend/src/app/components/album-tile/album-tile.component.ts and album-tile.component.html
- [X] T018 [US1] Add the album service for fetching overview data and formatting date ranges in frontend/src/app/services/album.service.ts
- [X] T019 [US1] Add fallback handling for unresolved or missing dates in backend/src/PhotoOrganizer.Application/Services/ and frontend/src/app/pages/album-overview/

**Checkpoint**: At this point, User Story 1 should be fully functional and testable independently

---

## Phase 4: User Story 2 - Browse album previews at a glance (Priority: P1)

**Goal**: Present a clear visual summary of each album so users can recognize collections without opening them.

**Independent Test**: Open the overview and verify each album tile includes a representative cover image, album title, date range, and photo count.

### Implementation for User Story 2

- [X] T020 [P] [US2] Create the album summary contract and preview metadata model in backend/src/PhotoOrganizer.Application/DTOs/AlbumSummaryDto.cs and frontend/src/app/models/album-summary.model.ts
- [X] T021 [US2] Implement cover-image selection and photo-count aggregation in backend/src/PhotoOrganizer.Application/Services/AlbumSummaryService.cs
- [X] T022 [P] [US2] Implement the album detail endpoint and photo list endpoint in backend/src/PhotoOrganizer.Api/Controllers/AlbumController.cs
- [X] T023 [US2] Render the album tile preview and empty-state handling in frontend/src/app/components/album-tile/album-tile.component.ts and album-tile.component.css
- [X] T024 [US2] Connect the overview page to the album summary service and ensure sorting remains chronological in frontend/src/app/pages/album-overview/album-overview.component.ts
- [X] T025 [US2] Add UX polish for label formatting, date range display, and album metadata clarity in frontend/src/app/components/album-tile/

**Checkpoint**: At this point, the date-based album overview and tile previews should be independently functional

---

## Phase 5: User Story 3 - Manage album organization over time (Priority: P2)

**Goal**: Let users create, rename, and update albums as the library grows and date grouping changes.

**Independent Test**: Create a new album and rename it after adding new photos; confirm the overview reflects the update without losing date grouping.

### Implementation for User Story 3

- [X] T026 [P] [US3] Add create and update request models for album management in backend/src/PhotoOrganizer.Application/DTOs/CreateAlbumRequest.cs and UpdateAlbumRequest.cs
- [X] T027 [US3] Implement album create/update logic and validation in backend/src/PhotoOrganizer.Application/Services/AlbumManagementService.cs
- [X] T028 [US3] Add POST /api/albums and PATCH /api/albums/{albumId} endpoints in backend/src/PhotoOrganizer.Api/Controllers/AlbumController.cs
- [X] T029 [P] [US3] Build the album create/edit form and validation UX in frontend/src/app/components/album-editor/album-editor.component.ts and album-editor.component.html
- [X] T030 [US3] Connect album create/edit flows to the backend and refresh the overview after successful updates in frontend/src/app/services/album.service.ts and frontend/src/app/pages/album-overview/
- [X] T031 [US3] Add stale data handling and reload flow for albums when underlying metadata changes in frontend/src/app/pages/album-overview/

**Checkpoint**: At this point, User Story 3 should be independently functional and safe for future adjustment as the collection grows

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Final quality improvements and validation across all stories

- [X] T032 [P] Review the local file storage, SQL Server metadata, and date fallback behavior against the spec and data model in backend/src/PhotoOrganizer.Infrastructure/ and backend/src/PhotoOrganizer.Application/
- [X] T033 [P] Add final UI polish for tile spacing, hover states, and date readability in frontend/src/app/components/album-tile/ and frontend/src/app/pages/album-overview/
- [X] T034 Update the quickstart and operational guidance to reflect local setup, SQL Server Windows authentication, and image storage requirements in specs/001-photo-album-organizer/quickstart.md
- [ ] T035 Run the end-to-end validation flow from specs/001-photo-album-organizer/quickstart.md and confirm the album overview, tile previews, and album management flows work together

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Stories (Phase 3-5)**: All depend on Foundational completion
- **Polish (Phase 6)**: Depends on all desired user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational (Phase 2) - no dependencies on other stories
- **User Story 2 (P1)**: Can start after Foundational (Phase 2) and builds on User Story 1 behavior, but should remain independently testable
- **User Story 3 (P2)**: Can start after Foundational (Phase 2) and should remain independently testable as a library-management feature

### Parallel Opportunities

- All tasks marked [P] can run in parallel within their phase when different files are involved
- The Angular and .NET setup tasks can proceed in parallel
- The API and UI tasks for different user stories can proceed in parallel once the foundational layer is complete

---

## Parallel Example: User Story 1

```bash
# Parallel work in the same story:
Task: "Implement the GET /api/albums endpoint in backend/src/PhotoOrganizer.Api/Controllers/AlbumController.cs"
Task: "Implement the album overview page in frontend/src/app/pages/album-overview/album-overview.component.ts"
Task: "Implement the reusable album tile in frontend/src/app/components/album-tile/album-tile.component.ts"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (CRITICAL - blocks all stories)
3. Complete Phase 3: User Story 1
4. Validate the date-grouped library experience independently
5. Stop and confirm the MVP is ready before expanding into previews or management

### Incremental Delivery

1. Complete Setup + Foundational → foundation ready
2. Add User Story 1 → validate date grouping and album overview
3. Add User Story 2 → validate album previews and metadata readability
4. Add User Story 3 → validate create/edit flows and ongoing library management
5. Finish with cross-cutting polish and end-to-end validation

### Parallel Team Strategy

With multiple developers:

1. Team completes Setup + Foundational together
2. Once the foundation is ready:
   - Developer A: User Story 1
   - Developer B: User Story 2
   - Developer C: User Story 3
3. All stories complete and integrate independently with final validation before release

---

## Notes

- [P] tasks = different files, no dependencies
- [Story] labels map work to the corresponding user story for traceability
- Each user story is independently completable and testable
- Avoid broad tasks that mix album grouping, preview UI, and metadata management in one step
- All tasks are intentionally file-scoped and actionable enough to be executed without further clarification

## Phase 7: Convergence

- [X] T036 CRITICAL: Add automated tests for API, persistence, preview, management, and end-to-end flows per Constitution II (partial)
- [X] T037 Wire actual library data through date grouping into the chronologically sorted album overview per FR-001/FR-003/FR-010 and US1/AC1-2 (partial)
- [X] T038 Persist album and photo metadata through the configured SQL Server and repository instead of process-local lists per plan: SQL Server/local storage and T007/T010/T014 (partial)
- [X] T039 Model and persist photo membership in multiple albums without losing associations per FR-009 and T008 (contradicts)
- [X] T040 Implement local photo ingestion and metadata updates, including unresolved-date handling and validation per FR-006/FR-007 and T009/T011 (partial)
- [X] T041 Add GET /api/photos and GET /api/albums/{albumId}/photos endpoints per T022 (missing)
- [X] T042 Connect album tiles to real album summaries and local cover images, removing remote demo URLs per FR-004/FR-005/FR-008, US2/AC1-2, and plan: local-only images (contradicts)
- [X] T043 Complete validated album create, rename, and update flows with an Angular editor and overview refresh per FR-002 and US3/AC1-2 (partial)
- [X] T044 Add a clear empty-album state to album tiles per T023 (missing)
- [X] T045 Configure and document frontend/backend linting and formatting per T005 (missing)
- [X] T046 Validate album browsing and rendering against the planned typical-library and thousands-of-photos performance goals per plan: performance goals and SC-001 (partial)
- [X] T047 Add the planned album-tile hover polish per T033 (missing)
