-- ======================================================
-- TABLE: Role
-- ======================================================
-- The Role table defines each distinct functional position within the engineering organization
-- (e.g., Backend Developer, QA Engineer, Project Manager).
-- Each role establishes a DefaultUtilizationTarget, representing the expected percentage
-- of productive time that can be allocated to projects after accounting for overhead such as
-- meetings, communication, and non-billable activities.
--
-- This table serves as the categorical foundation for all capacity planning, linking to:
--   Demand: to define required FTEs by role and period.
--   Capacity: to define total available FTEs by role and period.
--   ForecastResult: to measure utilization and balance at the role level.
--
-- Roles are seeded with common engineering functions and their typical utilization rates,
-- enabling consistent planning assumptions across all workstreams and scenarios.

CREATE TABLE Role (
    RoleID UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    Name NVARCHAR(100) NOT NULL,
    DefaultUtilizationTarget DECIMAL(5,2) NULL, -- e.g. 0.85
    CONSTRAINT UQ_Role_Name UNIQUE (Name),
    Constraint PK_Role PRIMARY KEY CLUSTERED (RoleID)
)
-- Utilization targets are set below 100% because no engineer delivers full billable output time lost to communication, interruptions, and operational overhead must be baked into the model.
INSERT INTO [Role] (Name, DefaultUtilizationTarget) VALUES
('Architect', 0.80),
('Automation Engineer', 0.85),
('Backend Developer', 0.85),
('Business Analyst', 0.75),
('Database Administrator', 0.80),
('Database Developer', 0.85),
('Database Lead Developer', 0.80),
('Frontend Developer', 0.85),
('Frontend Lead Developer', 0.80),
('Full Stack Developer', 0.85),
('Infrastructure Engineer', 0.85),
('Lead Developer', 0.85),
('Level 1 Support Engineer', 0.90),
('Level 2 Support Engineer', 0.85),
('Level 3 Support Engineer', 0.80),
('Level 4 Support Engineer', 0.80),
('Machine Learning Engineer', 0.85),
('Manager', 0.75),
('Power BI Developer', 0.85),
('Product Owner', 0.70),
('Project Manager', 0.70),
('Python Developer', 0.85),
('QA Analyst', 0.85),
('QA Engineer', 0.85),
('QA Engineering Lead', 0.80),
('RPA Developer', 0.85),
('RPA Lead Developer', 0.80),
('Rule Translator', 0.85),
('Scrum Master', 0.75),
('Senior Developer', 0.85),
('Solution Architect', 0.80),
('SysAdmin', 0.85),
('Team Lead', 0.80),
('Tech Lead', 0.85),
('Technical Writer', 0.75)
