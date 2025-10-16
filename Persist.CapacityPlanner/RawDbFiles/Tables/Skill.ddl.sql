-- ======================================================
-- TABLE: Skill
-- PURPOSE: Master reference for all technical and functional skills
-- ======================================================
-- The Skill table defines the canonical list of skills recognized
-- across the organization. Each skill represents a discrete capability,
-- tool, framework, programming language, or platform area that may be
-- associated with a person or required by a workstream demand.
--
-- Examples:
--   Programming Languages (C#, Python, SQL)
--   Frameworks (Angular, .NET Core, React)
--   Tools (Azure Data Factory, Power BI, Git)
--   Domain Areas (Healthcare EDI, Claims Processing)
--
-- This table acts as the single source of truth for all skill references
-- to maintain naming consistency and avoid duplication across PersonSkill
-- and DemandSkill relationships.
-- ======================================================

CREATE TABLE Skill (
    SkillID UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    CONSTRAINT PK_Skill PRIMARY KEY (SkillID),
    CONSTRAINT UQ_Skill_Name UNIQUE (Name)
)

-- ======================================================
-- SEED DATA
-- ======================================================
-- Baseline list of common skills for Lyric engineering environments.
-- The list can be extended as new technologies or tools are adopted.

INSERT INTO Skill (Name, Description)
VALUES
('.NET Core', 'Cross-platform framework for building backend services and APIs.'),
('Angular', 'Frontend web framework used in Lyric UI development.'),
('AWS', 'Amazon Web Services cloud environment for hosting and integration.'),
('Azure Data Factory', 'ETL and data orchestration service used for claims ingestion and transformation pipelines.'),
('Azure Functions', 'Serverless compute environment for running scalable event-driven workloads.'),
('C#', 'General-purpose programming language used for backend development and Azure integration.'),
('DevOps', 'Practices and tools supporting CI/CD, automation, and infrastructure management.'),
('FullStack', 'Capable of doing both frontend and backend development tasks.'),
('Healthcare EDI', 'Domain expertise in X12 EDI transactions, claims, and payment integrity processes.'),
('Kubernetes', 'Container orchestration platform for scaling microservices.'),
('Machine Learning', 'Applied ML techniques for anomaly detection and data-driven decision support.'),
('Power BI', 'Business intelligence and data visualization platform for reporting and analytics.'),
('Python', 'High-level programming language used for data processing, automation, and ML workloads.'),
('RPA', 'Robotic process automation technologies for repetitive workflow tasks.'),
('SQL', 'Structured Query Language for relational data management and reporting.'),
('T-SQL', 'Microsoft SQL Server dialect used for stored procedures and database logic.')
