# Problem Definition: Working through loss of connectivity

- **Slug**: offline-mode
- **Created**: 2026-11-02
- **Inputs used**: intake.md, research.md

## Problem Statement

Users of the photo album organizer may be unable to keep working when something they depend on becomes unavailable, and any changes made during that time may be lost or inconsistent once service returns. The research found no evidence that this happens today or what "unavailable" means for an application that runs entirely on one local PC.

## Affected Users & Stakeholders

- **Users**: Individuals or small teams managing photo collections on a local workstation. [source: research.md, Users & Demand] Whether any of them experience interruptions is [NEEDS CLARIFICATION: no demand evidence found].
- **Stakeholders**: Project owner/maintainer, who decides whether the local-only constraint is relaxed. [NEEDS CLARIFICATION: confirm who owns this decision]

## Goals

- Users do not lose work or get stuck when a dependency they rely on is unavailable. (Dependent on clarifying what "unavailable" means.)
- Changes made during an interruption end up correctly reflected once normal service returns, without manual rework.
- Users can tell whether the application is currently usable and whether their changes have been saved.

## Non-Goals

- Moving photo storage to the cloud or hosting images remotely, which the project explicitly excludes. [source: research.md, Data & Constraints]
- Building multi-device or mobile access, unless the clarifications below show it is the real intent.
- Changing how photos are imported from the local folder or how date-group albums are built.

## Success Metrics

- Share of album changes made during an interruption that are retained afterwards. (baseline: unknown; no interruptions are recorded)
- Number of user-reported lost or duplicated edits after an interruption. (baseline: unknown)
- Qualitative: users can tell at a glance whether they are working normally or during an interruption.

## Cost of Inaction

The application already works without an internet connection because the frontend, API, database and images are all local. If nothing is built, users are affected only when the local API or SQL Server is stopped, where they currently see an error and cannot proceed. No data-loss incidents or user complaints are recorded. [source: research.md] The cost is unknown and appears low, though it would be higher if the app is later used across multiple devices.

## Open Questions

- [NEEDS CLARIFICATION: What does "offline" mean here: no internet, local API unavailable, or database unavailable?]
- [NEEDS CLARIFICATION: What would changes sync to or from (another device, cloud, shared team store), given the "no cloud" constraint?]
- [NEEDS CLARIFICATION: Which actions must keep working during an interruption?]
- [NEEDS CLARIFICATION: How should conflicting changes be resolved?]
- [NEEDS CLARIFICATION: Is there real user demand, and who raised it?]
- [NEEDS CLARIFICATION: Is this meant for a different product direction, such as a mobile or multi-device version?]
