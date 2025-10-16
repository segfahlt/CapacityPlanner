-- ======================================================
-- TABLE: Allocation
-- ======================================================
-- The Allocation table defines how individual people are assigned
-- to specific workstreams over defined time periods. Each record represents
-- a planned or actual allocation of a person’s available time.
--
-- This table connects directly to:
--   Person: identifies who is being allocated.
--   Workstream: identifies the project, enhancement, or support effort
--     to which the person is assigned.
--
-- Allocation records represent planned or actual utilization,
-- expressed as a percentage of a person’s available capacity (AllocationPct).
-- These values can later be compared against Actuals to measure variance
-- between forecasted and real effort distribution.
--
-- The Allocation table is a key input to understanding real-world workload,
-- resource constraints, and multi-project participation across the organization.
-- ======================================================
CREATE TABLE Allocation (
    AllocationID UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    PersonID UNIQUEIDENTIFIER NOT NULL,
    WorkstreamID UNIQUEIDENTIFIER NOT NULL,
    PeriodStart DATE NOT NULL,
    PeriodEnd DATE NOT NULL,
    AllocationPct DECIMAL(5,2) NOT NULL, -- 0.5 = 50%
    AllocationType NVARCHAR(50) NULL, -- Planned, Actual
    Source NVARCHAR(100) NULL,
    CONSTRAINT FK_Allocation_Person FOREIGN KEY (PersonID) REFERENCES Person(PersonID),
    CONSTRAINT FK_Allocation_Workstream FOREIGN KEY (WorkstreamID) REFERENCES Workstream(WorkstreamID),
    Constraint PK_Allocations Primary Key (AllocationID)
)
