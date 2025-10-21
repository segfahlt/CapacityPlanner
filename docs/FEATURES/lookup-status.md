# Lookups: Status

User Story
- As an admin, I manage lifecycle statuses.

Acceptance Criteria
- CRUD with unique `Status` key and optional description.

Test Cases
- Create succeeds; duplicate key fails.
- Update description.
- Delete not referenced by FKs.
