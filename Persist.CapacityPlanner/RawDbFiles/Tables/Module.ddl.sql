-- ======================================================
-- TABLE: Module
-- ======================================================
-- The Module table represents a distinct subsystem, feature set, or functional area
-- within a parent platform. Modules organize work and capacity around logical product
-- components, such as “Prepay Coding Review” or “Audit Engine” and have their own development lifecycle
-- and team structure.
--
-- Modules provide a mid-level structure between platform and implementation,
-- enabling more granular tracking of demand, development, and support activity.
-- This table connects to:
--   Platform: establishes the parent platform relationship.
--   Implementation: links client-specific deployments or efforts tied to this module.
--
-- The Status column identifies whether a module is Active, Planned, or Legacy,
-- supporting portfolio visibility and lifecycle management across product families.
CREATE TABLE Module (
    ModuleID UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    PlatformID UNIQUEIDENTIFIER NOT NULL,
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    Status NVARCHAR(50) NULL, -- Active, Legacy, Planned
    CONSTRAINT FK_Module_Platform FOREIGN KEY (PlatformID) REFERENCES Platform(PlatformID),
    CONSTRAINT UQ_Module_Platform_Name UNIQUE (PlatformID, Name),
    Constraint PK_Module Primary Key (ModuleID)
)
