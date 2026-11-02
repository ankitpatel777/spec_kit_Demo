# Idea Research: Offline mode with sync on reconnect

- **Slug**: offline-mode
- **Created**: 2026-11-02
- **Evidence confidence (overall)**: low

All findings come from this repository's own specs and code. No external sources were consulted, and no user data exists, so demand findings are assumptions.

## Users & Demand

- No stated or observed demand for offline use appears in the spec, plan, tasks, or any recorded feedback. — [source: ASSUMPTION; searched `specs/001-photo-album-organizer/` for offline/sync mentions] (confidence: low)
- The product targets "personal or small-team photo collection management on a local workstation". — [source: `specs/001-photo-album-organizer/plan.md`, Scale/Scope] (confidence: high)
- Who would be offline, and when, is unknown. — [source: ASSUMPTION] (confidence: low)

## Prior Art

- No prior offline or sync work exists. The frontend has no service worker, local storage, IndexedDB, or online/offline detection. — [source: code search of `frontend/src`, no matches for `serviceWorker`, `localStorage`, `indexedDB`, `navigator.onLine`, `ngsw`] (confidence: high)
- The earlier feature research rejected remote sync explicitly: "Download and sync remote copies: violates the local-first requirement." — [source: `specs/001-photo-album-organizer/research.md`, line 62] (confidence: high)
- The same research chose SQL Server over SQLite or file-based storage for metadata. — [source: `specs/001-photo-album-organizer/research.md`, line 10] (confidence: high)

## Market & Context

- The app runs the Angular frontend, ASP.NET Core API, SQL Server, and image files all on one local PC, with the browser reaching the API at `http://localhost:5161`. — [source: `plan.md` Technical Context; `frontend/proxy.conf.json`] (confidence: high)
- Because everything is local, the app already works without an internet connection. The only dependency is that the local API and database are running. — [source: `plan.md`, constraints: "no cloud-based storage or remote image hosting"] (confidence: high)
- Alternatives users rely on today are not recorded. — [source: ASSUMPTION] (confidence: low)
- Cost of doing nothing: none identified. — [source: ASSUMPTION] (confidence: low)

## Data & Constraints

- Storage constraint: images stay local; metadata is in SQL Server with Windows authentication; no cloud storage. — [source: `plan.md`, Constraints] (confidence: high)
- No remote service or second device exists to sync to, so there is currently no sync counterpart. — [source: `plan.md` architecture; no remote endpoints in `backend/src/PhotoOrganizer.Api/Controllers`] (confidence: high)
- Library size is described only as "large but manageable", with no numbers. — [source: `plan.md`, Scale/Scope] (confidence: medium)
- Editing is limited to creating and updating albums; photos come in through local-folder import. — [source: API endpoints in `AlbumController.cs` and `PhotoController.cs`] (confidence: high)

## Evidence Against the Idea

- The architecture is already local-first, so "offline" has no clear meaning here. The common failure case is the local API or SQL Server being stopped, not the network being lost.
- Sync requires a second place to sync to (cloud, another PC, or a mobile client). The project has explicitly excluded cloud storage and remote hosting.
- Adding offline queuing, conflict resolution, and sync would add substantial complexity to a small personal-scale app, which the project constitution cautions against without justification.
- No demand signal was found in any project artifact.

## Gaps & Open Questions

- [NEEDS CLARIFICATION: What does "offline" mean for this app: no internet, local API unavailable, or database unavailable?]
- [NEEDS CLARIFICATION: What would users sync to or from (another device, cloud, a shared team store)? That would reverse the "no cloud" constraint.]
- [NEEDS CLARIFICATION: Which operations must work offline, and how should conflicting edits be resolved?]
- [NEEDS CLARIFICATION: Is there real user demand, and from whom?]
- [NEEDS CLARIFICATION: Is the idea intended for a different product direction, such as a mobile or multi-device version?]

## Sources

- `specs/001-photo-album-organizer/plan.md` (repository, read-only)
- `specs/001-photo-album-organizer/research.md` (repository, read-only)
- `frontend/proxy.conf.json` and `frontend/src` code search (repository, read-only)
- `backend/src/PhotoOrganizer.Api/Controllers` (repository, read-only)
- No URLs fetched.
