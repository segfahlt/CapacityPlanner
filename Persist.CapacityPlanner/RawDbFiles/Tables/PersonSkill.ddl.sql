-- ======================================================
-- TABLE: PersonSkill
-- ======================================================
-- The PersonSkill table defines the skills associated with each person
-- in the engineering organization. Each record represents a mapping between
-- a person and a skill, optionally including a proficiency level or qualitative notes.
--
-- This table connects directly to:
--   Person: identifies the individual who holds the skill.
--   Skill: defines the specific capability, tool, or technology.
--
-- PersonSkill enables the system to evaluate workforce capability depth
-- and perform skill-based matching for resource planning, role alignment,
-- and demand fulfillment analysis.
-- ======================================================

CREATE TABLE PersonSkill (
    PersonSkillID UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    PersonID UNIQUEIDENTIFIER NOT NULL,
    SkillID UNIQUEIDENTIFIER NOT NULL,
    ProficiencyLevel NVARCHAR(50) NULL, -- Beginner, Intermediate, Advanced, Expert
    Notes NVARCHAR(MAX) NULL,
    CONSTRAINT PK_PersonSkill PRIMARY KEY (PersonSkillID),
    CONSTRAINT FK_PersonSkill_Person FOREIGN KEY (PersonID) REFERENCES Person(PersonID),
    CONSTRAINT FK_PersonSkill_Skill FOREIGN KEY (SkillID) REFERENCES Skill(SkillID),
    CONSTRAINT UQ_PersonSkill UNIQUE (PersonID, SkillID)
)
