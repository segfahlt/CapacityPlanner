-- ======================================================
-- TABLE: Demand
-- ======================================================
-- The Demand table captures the required effort, measured in FTE (Full-Time Equivalent),
-- for each role within a specific workstream and time period.
-- It represents the planning side of the capacity equation — what the organization needs.
--
-- Each record defines the expected resource requirement for a given role, period, and workstream,
-- along with a confidence factor indicating estimation certainty.
-- This table connects to:
--   Workstream: identifies the initiative or operational area generating demand.
--   Role: specifies the type of resource or skill being requested.
--
-- Demand records serve as the forecasted inputs against which capacity and allocations are compared,
-- forming the basis for gap analysis, staffing forecasts, and scenario modeling.
CREATE TABLE Demand (
    DemandID UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    WorkstreamID UNIQUEIDENTIFIER NOT NULL,
    RoleID UNIQUEIDENTIFIER NOT NULL,
    PeriodStart DATE NOT NULL,
    PeriodEnd DATE NOT NULL,
    RequiredFTE DECIMAL(9,2) NOT NULL,
    Confidence DECIMAL(5,2) NULL,
    Notes NVARCHAR(MAX) NULL,
    CONSTRAINT FK_Demand_Workstream FOREIGN KEY (WorkstreamID) REFERENCES Workstream(WorkstreamID),
    CONSTRAINT FK_Demand_Role FOREIGN KEY (RoleID) REFERENCES Role(RoleID),
    Constraint PK_Demand Primary Key (DemandID)
)
