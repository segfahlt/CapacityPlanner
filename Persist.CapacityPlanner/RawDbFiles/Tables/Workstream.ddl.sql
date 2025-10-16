-- ======================================================
-- TABLE: Workstream
-- ======================================================
-- The Workstream table represents a logical grouping of related work within the organization.
-- A workstream may correspond to a feature development effort, client implementation,
-- ongoing support activity, or enhancement initiative.
--
-- Workstreams serve as the operational unit of planning and execution, bridging
-- business initiatives with technical resource allocation.
-- This table connects to:
--   Demand: defines the required FTE effort by role for the workstream.
--   Allocation: records how individual people are assigned to the workstream.
--   Actual: captures realized hours or effort for completed work within the workstream.
--
-- The Category column defines the type of effort (ProductDev, Enhancement, Implementation, Support),
-- and the Status column indicates its current phase (Planned, Active, Closed, or Evergreen).
CREATE TABLE Workstream (
    WorkstreamID UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    Name NVARCHAR(200) NOT NULL,
    Category NVARCHAR(50) NOT NULL, -- ProductDev, Enhancement, Implementation, Support
    ImplementationID UNIQUEIDENTIFIER NULL, -- FK to Implementation if applicable
    StartDate DATE NULL,
    EndDate DATE NULL,
    Status NVARCHAR(50) NULL, -- Planned, Active, Closed
    CONSTRAINT UQ_Workstreams_Name UNIQUE (Name),
    CONSTRAINT FK_Workstream_Implementation FOREIGN KEY (ImplementationID) REFERENCES Implementation(ImplementationID),
    Constraint PK_Workstream Primary Key (WorkstreamID)
)
