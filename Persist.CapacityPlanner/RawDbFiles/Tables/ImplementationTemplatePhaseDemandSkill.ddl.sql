-- ======================================================
-- TABLE: ImplementationTemplatePhaseDemandSkill
-- ======================================================
-- Represents specific skills required for a given role demand
-- within a template phase. Mirrors the live DemandSkill table.
--
-- This table connects to:
--   ImplementationTemplatePhaseDemand: defines the parent demand record.
--   Skill: defines the required capability.
--
-- Template-level skills allow fine-grained modeling of capability needs
-- even before a project is instantiated, ensuring accurate future demand forecasting.
CREATE TABLE ImplementationTemplatePhaseDemandSkill (
    PhaseDemandSkillID UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    PhaseDemandID UNIQUEIDENTIFIER NOT NULL,
    SkillID UNIQUEIDENTIFIER NOT NULL,
    PriorityLevel NVARCHAR(50) NULL, -- Core, Nice-to-Have, Optional
    Notes NVARCHAR(MAX) NULL,
    CONSTRAINT PK_ImplementationTemplatePhaseDemandSkill PRIMARY KEY (PhaseDemandSkillID),
    CONSTRAINT FK_ImplementationTemplatePhaseDemandSkill_PhaseDemand FOREIGN KEY (PhaseDemandID)
        REFERENCES ImplementationTemplatePhaseDemand(PhaseDemandID),
    CONSTRAINT FK_ImplementationTemplatePhaseDemandSkill_Skill FOREIGN KEY (SkillID)
        REFERENCES Skill(SkillID),
    CONSTRAINT UQ_ImplementationTemplatePhaseDemandSkill UNIQUE (PhaseDemandID, SkillID)
)
