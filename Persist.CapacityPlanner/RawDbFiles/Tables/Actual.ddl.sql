-- ======================================================
-- TABLE: Actual
-- ======================================================
-- The Actual table records the realized effort or hours worked by individuals
-- across specific workstreams within defined time periods. Each record represents
-- measured execution data that can be used to compare against planned allocations.
--
-- This table connects directly to:
--   Person: identifies who performed the work.
--   Workstream: identifies the project, enhancement, or support effort
--     where the work occurred.
--
-- Actual data is essential for closing the loop on planning accuracy.
-- By comparing Actuals to Allocations, planners can identify systemic
-- under- or over-utilization, validate capacity assumptions, and adjust
-- demand forecasts accordingly.
--
-- Actual serves as the empirical record of workforce activity, enabling
-- trend analysis, velocity tracking, and variance reporting across periods.
-- ======================================================
CREATE TABLE Actual (
    ActualID UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    PersonID UNIQUEIDENTIFIER NOT NULL,
    WorkstreamID UNIQUEIDENTIFIER NOT NULL,
    PeriodStart DATE NOT NULL,
    PeriodEnd DATE NOT NULL,
    ActualHours DECIMAL(9,2) NULL,
    Source NVARCHAR(100) NULL,
    CONSTRAINT FK_Actual_Person FOREIGN KEY (PersonID) REFERENCES Person(PersonID),
    CONSTRAINT FK_Actual_Workstream FOREIGN KEY (WorkstreamID) REFERENCES Workstream(WorkstreamID),
    Constraint PK_Actual Primary Key (ActualID)
)
