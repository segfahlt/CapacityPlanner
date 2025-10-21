# Scenario

User Story
- As a planner, I can create scenarios and view previously created scenarios.

Acceptance Criteria
- List scenarios ordered by `CreatedOn` desc.
- Create scenario with name unique.
- Toggle active scenario (only one active allowed) – later.

Test Cases (Service)
- Create persists scenario with timestamps.
- Create duplicate name fails.
- List returns latest first.

Test Cases (API)
- GET /api/scenarios returns 200 in desc order.
- POST /api/scenarios returns 201 with location header; duplicate returns 409.
