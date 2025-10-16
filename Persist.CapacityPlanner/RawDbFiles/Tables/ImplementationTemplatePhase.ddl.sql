-- ======================================================
-- TABLE: ImplementationTemplatePhase
-- ======================================================
-- Represents a distinct phase within an implementation template.
-- Each phase defines the sequence, name, and typical duration.
--
-- This table connects to:
--   ImplementationTemplate: establishes the parent template relationship.
--   ImplementationTemplatePhaseDemand: defines default role requirements for this phase.
--
-- These records act as lifecycle scaffolding for new implementations,
-- allowing Workstreams to be auto-generated with appropriate phase and demand context.
CREATE TABLE ImplementationTemplatePhase (
    PhaseID UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    TemplateID UNIQUEIDENTIFIER NOT NULL,
    PhaseName NVARCHAR(100) NOT NULL,  -- Discover, Design, Development, etc.
    Sequence INT NOT NULL,
    DefaultDurationWeeks INT NULL,
    DefaultStartOffsetWeeks INT NULL,
    DefaultStatus NVARCHAR(50) NULL, -- Planned, Active, Complete
    Notes NVARCHAR(MAX) NULL,
    CONSTRAINT PK_ImplementationTemplatePhase PRIMARY KEY (PhaseID),
    CONSTRAINT FK_ImplementationTemplatePhase_Template FOREIGN KEY (TemplateID)
        REFERENCES ImplementationTemplate(TemplateID)
)
