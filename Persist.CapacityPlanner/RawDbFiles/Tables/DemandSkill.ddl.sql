-- ======================================================
-- TABLE: DemandSkill
-- ======================================================
-- The DemandSkill table defines the specific skills required
-- to fulfill a given demand record. Each entry links a skill
-- to a demand, expressing the technical or functional capabilities
-- needed for that portion of work.
--
-- This table connects directly to:
--   Demand: identifies the workload or role-based requirement that drives the need.
--   Skill: defines the specific capability, tool, or technology required.
--
-- DemandSkill enables more granular matching between workforce supply
-- and demand by skill rather than only by role. It allows planning tools
-- to analyze gaps between available skills (from PersonSkill)
-- and required skills (from DemandSkill) across active workstreams.
-- ======================================================
CREATE TABLE DemandSkill (
    DemandSkillID UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    DemandID UNIQUEIDENTIFIER NOT NULL,
    SkillID UNIQUEIDENTIFIER NOT NULL,
    PriorityLevel NVARCHAR(50) NULL, -- Optional: Core, Nice-to-Have, Optional
    Notes NVARCHAR(MAX) NULL,
    CONSTRAINT PK_DemandSkill PRIMARY KEY (DemandSkillID),
    CONSTRAINT FK_DemandSkill_Demand FOREIGN KEY (DemandID) REFERENCES Demand(DemandID),
    CONSTRAINT FK_DemandSkill_Skill FOREIGN KEY (SkillID) REFERENCES Skill(SkillID),
    CONSTRAINT UQ_DemandSkill UNIQUE (DemandID, SkillID)
)
