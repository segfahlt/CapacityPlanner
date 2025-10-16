-- ======================================================
-- TABLE: Implementation
-- ======================================================
-- The Implementation table represents a client-specific deployment or instance
-- of a platform or module. Each record defines where and how a product is delivered
-- to a customer, such as “Healthfirst Replay Implementation” or “Molina ClaimShark.”
--
-- Implementations act as the bridge between product architecture and delivery execution,
-- allowing capacity, demand, and workstream data to be tied to real-world clients.
-- This table connects to:
--   Platform: defines which product family the implementation belongs to.
--   Module: identifies the specific functional area being implemented, if applicable.
--
-- StartDate and GoLiveDate define the implementation lifecycle, while Status
-- tracks whether the implementation is Planned, In Progress, Live, Retired, or Evergreen.
CREATE TABLE Implementation (
    ImplementationID UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    PlatformID UNIQUEIDENTIFIER NOT NULL,
    ModuleID UNIQUEIDENTIFIER NULL,
    ClientName NVARCHAR(200) NOT NULL, -- Healthfirst, Molina, etc.
    Description NVARCHAR(MAX) NULL,
    StartDate DATE NULL,
    GoLiveDate DATE NULL,
    Status NVARCHAR(50) NULL, -- Planned, In_Progress, Live, Retired
    CONSTRAINT FK_Implementation_Platform FOREIGN KEY (PlatformID) REFERENCES Platform(PlatformID),
    CONSTRAINT FK_Implementation_Module FOREIGN KEY (ModuleID) REFERENCES Module(ModuleID),
    CONSTRAINT UQ_Implementation_Client UNIQUE (PlatformID, ClientName),
    Constraint PK_Implementation Primary Key (ImplementationID)
)
