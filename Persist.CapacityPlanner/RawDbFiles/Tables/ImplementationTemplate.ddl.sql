-- ======================================================
-- TABLE: ImplementationTemplate
-- ======================================================
-- Represents a reusable blueprint for an implementation or initiative.
-- Each template defines a standard set of lifecycle phases (e.g., Discover, Design, Development)
-- and associated default role/skill demands.
-- 
-- This table connects to:
--   ImplementationTemplatePhase: defines the ordered lifecycle phases.
--   ImplementationTemplatePhaseDemand: defines default role requirements for each phase.
-- Templates streamline creation of new implementations and promote delivery consistency.
CREATE TABLE ImplementationTemplate (
    TemplateID UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    Name NVARCHAR(150) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    Category NVARCHAR(50) NULL, -- Implementation, Product, Integration, POC
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT PK_ImplementationTemplate PRIMARY KEY (TemplateID),
    CONSTRAINT UQ_ImplementationTemplate_Name UNIQUE (Name)
)
