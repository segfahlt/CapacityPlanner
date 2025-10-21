# Model Runner & ForecastResult

User Story
- As a planner, I can run a model for a scenario to compute role/period utilization and variance.

Acceptance Criteria (initial)
- For each Role and Period, compute:
  - RequiredFTE = sum Demand.RequiredFTE
  - AvailableFTE = sum Capacity.AvailableFTE adjusted by utilization (Role.DefaultUtilizationTarget or Capacity.EffectiveUtilization if provided)
  - VarianceFTE = AvailableFTE - RequiredFTE
  - UtilizationPct = RequiredFTE / AvailableFTE (0 if AvailableFTE == 0)
- Persist results in `ForecastResult` with `LastComputed` timestamp.

Test Cases (Service)
- Run computes expected numbers for a simple dataset.
- AvailableFTE == 0 => UtilizationPct == 0.
- Negative variance flagged as Under, near-zero as Balanced, positive as Over (status rules TBD).
- Multiple roles and periods aggregate correctly.

Test Cases (API)
- POST /api/scenarios/{id}/run returns 202/200 and persists results.
- GET /api/scenarios/{id}/results returns paged results.
