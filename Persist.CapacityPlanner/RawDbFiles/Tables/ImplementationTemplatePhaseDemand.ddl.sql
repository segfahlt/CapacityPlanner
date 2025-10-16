-- ======================================================
-- TABLE: ImplementationTemplatePhaseDemand
-- ======================================================
-- Represents the role-level resource demand pattern within a specific template phase.
-- Each record defines a default FTE requirement for a role within that phase.
--
-- This table connects to:
--   ImplementationTemplatePhase: defines the parent lifecycle phase.
--   Role: identifies the functional role being modeled.
--   ImplementationTemplatePhaseDemandSkill: specifies any skill-level requirements.
--
-- These defaults are used to auto-generate live Demand records
-- when creating new Implementations from templates.
CREATE TABLE ImplementationTemplatePhaseDemand (
    PhaseDemandID UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    PhaseID UNIQUEIDENTIFIER NOT NULL,
    RoleID UNIQUEIDENTIFIER NOT NULL,
    DefaultFTE DECIMAL(9,2) NULL,
    Notes NVARCHAR(MAX) NULL,
    CONSTRAINT PK_ImplementationTemplatePhaseDemand PRIMARY KEY (PhaseDemandID),
    CONSTRAINT FK_ImplementationTemplatePhaseDemand_Phase FOREIGN KEY (PhaseID)
        REFERENCES ImplementationTemplatePhase(PhaseID),
    CONSTRAINT FK_ImplementationTemplatePhaseDemand_Role FOREIGN KEY (RoleID)
        REFERENCES Role(RoleID),
    CONSTRAINT UQ_ImplementationTemplatePhaseDemand UNIQUE (PhaseID, RoleID)
)
