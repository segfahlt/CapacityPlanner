# Roles

User Story
- As a planner, I manage roles with a unique name and a default utilization target.

Acceptance Criteria
- List roles sorted by name.
- Create role with unique `Name` and `DefaultUtilizationTarget` in [0.0, 1.0].
- Update role name and utilization.
- Delete role not referenced by FKs.
- Validation errors return 400 with messages; conflicts return 409.

Test Cases (Service)
- Create succeeds with valid values.
- Create fails when `Name` duplicates existing.
- Update changes persisted values.
- Delete removes the row; deleting non-existent returns not found behavior.

Test Cases (API)
- GET /api/roles returns 200 and list.
- POST /api/roles with duplicate name returns 409.
- PUT /api/roles/{id} returns 204.
- DELETE /api/roles/{id} returns 204; non-existing 404.
