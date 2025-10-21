# Skills

User Story
- As a planner, I manage skills with a unique name.

Acceptance Criteria
- List skills sorted by name.
- Create skill with unique `Name`.
- Update skill name.
- Delete skill not referenced by FKs.

Test Cases (Service)
- Create succeeds with valid values.
- Create fails when `Name` duplicates existing.
- Update changes persisted values.
- Delete removes the row.

Test Cases (API)
- GET /api/skills returns 200 and list.
- POST /api/skills with duplicate name returns 409.
- PUT /api/skills/{id} returns 204.
- DELETE /api/skills/{id} returns 204; non-existing 404.
