-- ======================================================
-- DROP TABLES (Reverse Dependency Order)
-- ======================================================
IF OBJECT_ID('ImplementationTemplate', 'U') IS NOT NULL DROP TABLE ImplementationTemplate
IF OBJECT_ID('ImplementationPhase', 'U') IS NOT NULL DROP TABLE ImplementationPhase
IF OBJECT_ID('ImplementationPhaseDemand', 'U') IS NOT NULL DROP TABLE ImplementationPhaseDemand
IF OBJECT_ID('ImplementationPhaseDemandSkill', 'U') IS NOT NULL DROP TABLE ImplementationPhaseDemandSkill
IF OBJECT_ID('ForecastResult', 'U') IS NOT NULL DROP TABLE ForecastResult
IF OBJECT_ID('Scenario', 'U') IS NOT NULL DROP TABLE Scenario
IF OBJECT_ID('Actual', 'U') IS NOT NULL DROP TABLE Actual
IF OBJECT_ID('Allocation', 'U') IS NOT NULL DROP TABLE Allocation
IF OBJECT_ID('Capacity', 'U') IS NOT NULL DROP TABLE Capacity
IF OBJECT_ID('DemandSkill', 'U') IS NOT NULL DROP TABLE DemandSkill
IF OBJECT_ID('Demand', 'U') IS NOT NULL DROP TABLE Demand
IF OBJECT_ID('Workstream', 'U') IS NOT NULL DROP TABLE Workstream
IF OBJECT_ID('Implementation', 'U') IS NOT NULL DROP TABLE Implementation
IF OBJECT_ID('Module', 'U') IS NOT NULL DROP TABLE Module
IF OBJECT_ID('Platform', 'U') IS NOT NULL DROP TABLE Platform
IF OBJECT_ID('PersonSkill', 'U') IS NOT NULL DROP TABLE PersonSkill
IF OBJECT_ID('Person', 'U') IS NOT NULL DROP TABLE Person
IF OBJECT_ID('Skill', 'U') IS NOT NULL DROP TABLE Skill
IF OBJECT_ID('Role', 'U') IS NOT NULL DROP TABLE Role
IF OBJECT_ID('Lookup_WorkstreamCategory', 'U') IS NOT NULL DROP TABLE Lookup_WorkstreamCategory
IF OBJECT_ID('Lookup_Status', 'U') IS NOT NULL DROP TABLE Lookup_Status
IF OBJECT_ID('Lookup_ImplementationCategory', 'U') IS NOT NULL DROP TABLE Lookup_ImplementationCategory
IF OBJECT_ID('Lookup_EmploymentType', 'U') IS NOT NULL DROP TABLE Lookup_EmploymentType
IF OBJECT_ID('Lookup_AllocationType', 'U') IS NOT NULL DROP TABLE Lookup_AllocationType

-- ======================================================
-- SCHEMA: Capacity Planning Core
-- ======================================================

-- ======================================================
-- LOOKUP: Workstream Category
-- ======================================================
-- The Lookup_WorkstreamCategory table defines standard categories
-- for workstreams across the organization.
-- Each category represents a high-level classification of work type.
--   ProductDev: Core product feature development
--   Enhancement: Improvements or refactors to existing modules
--   Implementation: Client-specific configuration or deployment
--   Support: Ongoing production maintenance and issue resolution
-- 
-- These categories drive standard reporting and filtering,
-- providing consistent grouping for resource allocation and forecasting.
CREATE TABLE Lookup_WorkstreamCategory (
    WorkstreamCategory NVARCHAR(50) PRIMARY KEY,
    Description NVARCHAR(200) NULL
)
INSERT INTO Lookup_WorkstreamCategory (WorkstreamCategory, Description)
VALUES
('ProductDev', 'Core product feature development'),
('Enhancement', 'Improvements or refactors to existing modules'),
('Implementation', 'Client-specific configuration or deployment'),
('Support', 'Ongoing production maintenance and issue resolution')

-- ======================================================
-- LOOKUP: Status
-- ======================================================
-- The Lookup_Status table defines the standard lifecycle stages
-- used across platforms, modules, implementations, and workstreams.
--   Planned: Defined but not yet started
--   Active: Currently in execution
--   Closed: Completed and inactive
--   Evergreen: Ongoing or continuous with no defined end
--   Retired: Phased out and no longer maintained
CREATE TABLE Lookup_Status (
    Status NVARCHAR(50) PRIMARY KEY,
    Description NVARCHAR(200) NULL
)
INSERT INTO Lookup_Status (Status, Description)
VALUES
('Planned', 'Defined but not yet started'),
('Active', 'Currently in execution'),
('Closed', 'Completed and inactive'),
('Evergreen', 'Ongoing or continuous with no defined end'),
('Retired', 'Phased out and no longer maintained')

-- ======================================================
-- LOOKUP: Implementation Category
-- ======================================================
-- The Lookup_ImplementationCategory table defines high-level categories
-- of implementations or initiatives, aligning templates and reporting.
--   Implementation: Standard client onboarding or deployment
--   Product: Internal core product effort
--   Integration: Cross-system or external connector build
--   POC: Proof of concept or exploratory project
CREATE TABLE Lookup_ImplementationCategory (
    ImplementationCategory NVARCHAR(50) PRIMARY KEY,
    Description NVARCHAR(200) NULL
)
INSERT INTO Lookup_ImplementationCategory (ImplementationCategory, Description)
VALUES
('Implementation', 'Standard client onboarding or deployment'),
('Product', 'Internal core product effort'),
('Integration', 'Cross-system or external connector build'),
('POC', 'Proof of concept or exploratory project')

-- ======================================================
-- LOOKUP: Employment Type
-- ======================================================
-- The Lookup_EmploymentType table defines resource classifications.
--   FTE: Full-time employee of the organization
--   Contingent: Contractor, consultant, or temporary resource
CREATE TABLE Lookup_EmploymentType (
    EmploymentType NVARCHAR(50) PRIMARY KEY,
    Description NVARCHAR(200) NULL
)
INSERT INTO Lookup_EmploymentType (EmploymentType, Description)
VALUES
('FTE', 'Full-time employee of the organization'),
('Contingent', 'Contractor, consultant, or temporary resource')

-- ======================================================
-- LOOKUP: Allocation Type
-- ======================================================
-- The Lookup_AllocationType table defines allocation intent or category.
--   Planned: Forward-looking allocation (forecast)
--   Actual: Recorded actual allocation or utilization
CREATE TABLE Lookup_AllocationType (
    AllocationType NVARCHAR(50) PRIMARY KEY,
    Description NVARCHAR(200) NULL
)
INSERT INTO Lookup_AllocationType (AllocationType, Description)
VALUES
('Planned', 'Forward-looking allocation (forecast)'),
('Actual', 'Recorded actual allocation or utilization')

-- ======================================================
-- TABLES BEGIN BELOW
-- ======================================================

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
('C#', 'General-purpose programming language used for backend development and Azure integration.'),
('FullStack', 'Capable of doing both frontend and backend development tasks.'),
('Python', 'High-level programming language used for data processing, automation, and ML workloads.'),
('SQL', 'Structured Query Language for relational data management and reporting.'),
('T-SQL', 'Microsoft SQL Server dialect used for stored procedures and database logic.'),
('Angular', 'Frontend web framework used in Lyric UI development.'),
('.NET Core', 'Cross-platform framework for building backend services and APIs.'),
('Azure Data Factory', 'ETL and data orchestration service used for claims ingestion and transformation pipelines.'),
('Power BI', 'Business intelligence and data visualization platform for reporting and analytics.'),
('Azure Functions', 'Serverless compute environment for running scalable event-driven workloads.'),
('Healthcare EDI', 'Domain expertise in X12 EDI transactions, claims, and payment integrity processes.'),
('AWS', 'Amazon Web Services cloud environment for hosting and integration.'),
('Kubernetes', 'Container orchestration platform for scaling microservices.'),
('DevOps', 'Practices and tools supporting CI/CD, automation, and infrastructure management.'),
('Machine Learning', 'Applied ML techniques for anomaly detection and data-driven decision support.'),
('RPA', 'Robotic process automation technologies for repetitive workflow tasks.')

-- ======================================================
-- TABLE: Person
-- ======================================================
-- The Person table represents individual resources within the engineering organization.
-- Each record defines a unique person and captures employment type (FTE, Contractor),
-- active status, and general working location (e.g., Onshore, Offshore).
--
-- This table forms the human resource base of the capacity model.
-- It connects directly to:
--   Allocation: defines how a person's time is planned across workstreams.
--   Actual: records actual hours or effort logged by each person.
--   Scenario: identifies the user who created or owns a specific planning scenario.
--
-- Persons represent the real-world supply side of capacity planning.
-- Their aggregate availability, filtered by role and utilization target,
-- is used to calculate total organizational capacity by role and period.
CREATE TABLE Person (
    PersonID UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    Name NVARCHAR(200) NOT NULL,
    EmploymentType NVARCHAR(50) NULL, -- FTE, Contractor, etc.
    IsActive bit,
    Location NVARCHAR(100) NULL, --onshore or offshore
    Constraint PK_Person Primary Key (PersonID)
)
INSERT INTO Person (Name, EmploymentType, IsActive, Location)
VALUES
('Abdul Mosin', 'FTE', 1, 'India'),
('Abhinava Mandikal', 'FTE', 1, 'India'),
('Aishwarya Rajaboina', 'FTE', 1, 'India'),
('Akshith Rachamalla', 'FTE', 1, 'India'),
('Amrutha Vadrevu', 'FTE', 1, 'India'),
('Anil Vidya Sagar Saila', 'FTE', 1, 'India'),
('Apoorba Ganguly', 'FTE', 1, 'India'),
('Ashok Palipi', 'FTE', 1, 'India'),
('Azhar Shaik', 'FTE', 1, 'India'),
('Bala Krishna Vijay Kumar Dwara', 'FTE', 1, 'India'),
('Bala Venkata Dunaboina', 'FTE', 1, 'India'),
('Bhavani Bandaru', 'FTE', 1, 'India'),
('Bimal Kurichiyath', 'FTE', 1, 'India'),
('Chinmayee Achary', 'FTE', 1, 'India'),
('Dilip Bikki', 'FTE', 1, 'India'),
('Durga Jyothi Ketha', 'FTE', 1, 'India'),
('Gaurav Kumar', 'FTE', 1, 'India'),
('Gayatri Chunduru', 'FTE', 1, 'India'),
('Gowtham Chowdary Meda', 'FTE', 1, 'India'),
('Gowthami Kilaru', 'FTE', 1, 'India'),
('Haritha Thoppe', 'FTE', 1, 'India'),
('Himabindu K', 'FTE', 1, 'India'),
('Himabindu T', 'FTE', 1, 'India'),
('Jeevan Kumar Poreddy', 'FTE', 1, 'India'),
('Kalyan Kilaru', 'FTE', 1, 'India'),
('Kalyan Prasad Kancharla', 'FTE', 1, 'India'),
('Karthik Garapati', 'FTE', 1, 'India'),
('Kiran Kumar', 'FTE', 1, 'India'),
('Krishna Teja', 'FTE', 1, 'India'),
('Lavanya Potluri', 'FTE', 1, 'India'),
('Madhuri Godavarthi', 'FTE', 1, 'India'),
('Manisha Chadalavada', 'FTE', 1, 'India'),
('Manoj Kumar', 'FTE', 1, 'India'),
('Manojkumar Golla', 'FTE', 1, 'India'),
('Manvitha Sanikommu', 'FTE', 1, 'India'),
('Mounika Gudapati', 'FTE', 1, 'India'),
('Mounika Patlolla', 'FTE', 1, 'India'),
('Nagamani Pidugu', 'FTE', 1, 'India'),
('Nagaraju Adabala', 'FTE', 1, 'India'),
('Nagendranadh J', 'FTE', 1, 'India'),
('Nagesh P', 'FTE', 1, 'India'),
('Naresh Kumar', 'FTE', 1, 'India'),
('Neelima Bathula', 'FTE', 1, 'India'),
('Niharika Mandadapu', 'FTE', 1, 'India'),
('Niharika Nagisetti', 'FTE', 1, 'India'),
('Nikitha P', 'FTE', 1, 'India'),
('Nikhil Reddy', 'FTE', 1, 'India'),
('Padmaja P', 'FTE', 1, 'India'),
('Pavan Kumar', 'FTE', 1, 'India'),
('Pooja Allu', 'FTE', 1, 'India'),
('Pooja Raj', 'FTE', 1, 'India'),
('Poonam Akula', 'FTE', 1, 'India'),
('Pradeep Danda', 'FTE', 1, 'India'),
('Pranitha Gundu', 'FTE', 1, 'India'),
('Prasanna Kalyani', 'FTE', 1, 'India'),
('Prathyusha P', 'FTE', 1, 'India'),
('Praveen Kumar', 'FTE', 1, 'India'),
('Priyanka D', 'FTE', 1, 'India'),
('Raj Kumar', 'FTE', 1, 'India'),
('Rajasekhar Chinnabathini', 'FTE', 1, 'India'),
('Rajesh Yadav', 'FTE', 1, 'India'),
('Rakesh Ch', 'FTE', 1, 'India'),
('Ramesh G', 'FTE', 1, 'India'),
('Ramu B', 'FTE', 1, 'India'),
('Ranga Reddy', 'FTE', 1, 'India'),
('Ravi Kottapalli', 'FTE', 1, 'India'),
('Ravi Teja', 'FTE', 1, 'India'),
('Rekha Yarramsetti', 'FTE', 1, 'India'),
('Rohith Reddy', 'FTE', 1, 'India'),
('Sai Kumar', 'FTE', 1, 'India'),
('Sai Teja', 'FTE', 1, 'India'),
('Sairam R', 'FTE', 1, 'India'),
('Sandhya B', 'FTE', 1, 'India'),
('Sandeep Reddy', 'FTE', 1, 'India'),
('Sangeetha B', 'FTE', 1, 'India'),
('Santhosh K', 'FTE', 1, 'India'),
('Satish Ch', 'FTE', 1, 'India'),
('Satya Prasad', 'FTE', 1, 'India'),
('Satyavathi P', 'FTE', 1, 'India'),
('Seetha Lakshmi', 'FTE', 1, 'India'),
('Shiva Kiran', 'FTE', 1, 'India'),
('Shravan Kumar', 'FTE', 1, 'India'),
('Siva Krishna', 'FTE', 1, 'India'),
('Sowmya Ch', 'FTE', 1, 'India'),
('Sravani D', 'FTE', 1, 'India'),
('Sreeja Mandadapu', 'FTE', 1, 'India'),
('Srilatha K', 'FTE', 1, 'India'),
('Sruthi K', 'FTE', 1, 'India'),
('Subhasish Pradhan', 'FTE', 1, 'India'),
('Sujatha K', 'FTE', 1, 'India'),
('Suresh P', 'FTE', 1, 'India'),
('Surya Prakash', 'FTE', 1, 'India'),
('Swathi K', 'FTE', 1, 'India'),
('Swetha B', 'FTE', 1, 'India'),
('Teja Reddy', 'FTE', 1, 'India'),
('Thirumal Reddy', 'FTE', 1, 'India'),
('Vamsi Krishna', 'FTE', 1, 'India'),
('Vasantha Kumar', 'FTE', 1, 'India'),
('Veerabhadra Rao', 'FTE', 1, 'India'),
('Venkat Reddy', 'FTE', 1, 'India'),
('Venkata Satish', 'FTE', 1, 'India'),
('Vinay Kumar', 'FTE', 1, 'India'),
('Vivek', 'FTE', 1, 'India'),
('Yeshwanth Reddy', 'FTE', 1, 'India')

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

-- ======================================================
-- TABLE: Platform
-- ======================================================
-- The Platform table represents a major product or technology family within the organization,
-- such as Virtuoso, Replay, or Platform 42.
-- Each platform serves as a top-level container for related modules and client implementations.
--
-- Platforms define the strategic boundaries of engineering investment and capacity tracking.
-- They provide the highest-level linkage between products, clients, and delivery efforts.
-- This table connects to:
--   Module: defines subcomponents or functional areas within the platform.
--   Implementation: links a platform or its modules to client-specific instances.
--
-- Each platform record may also include a descriptive narrative used for reporting,
-- portfolio management, and high-level planning summaries.
CREATE TABLE Platform (
    PlatformID UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    CONSTRAINT UQ_Platform_Name UNIQUE (Name),
    Constraint PK_Platform Primary Key (PlatformID)
)

declare @ClaimSharkID uniqueidentifier = NEWID()
declare @AuditSharkID uniqueidentifier = NEWID()
declare @Platform42ID uniqueidentifier = NEWID()
insert into Platform(PlatformID,Name,Description) values
(@ClaimSharkID,'ClaimShark','ClaimShark is an enterprise payment integrity platform that centralizes audit workflows, vendor management, claim rules, and analytics to help health plans detect overpayments and streamline recovery processes. It provides a scalable, configurable, cloud-native framework for automating audit operations and integrating with core claims systems.'),
(@AuditSharkID,'AuditShark','AuditShark is a specialized module within the ClaimShark platform focused on audit workflow management, compliance, and overpayment detection. It automates audit execution, enforces consistency, and provides end-to-end traceability across payment integrity reviews.'),
(@Platform42ID,'Platform 42','Lyric 42 is Lyric’s next-generation AI-driven platform for payment accuracy and cost-of-care optimization, integrating claim editing, audit, and coordination of benefits into a unified cloud-native solution. It provides modular, extensible architecture designed to reduce operational overhead and improve data-driven decision-making for health plans.')


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

-- ======================================================
-- TABLE: Workstream
-- ======================================================
-- The Workstream table represents a logical grouping of related work within the organization.
-- A workstream may correspond to a feature development effort, client implementation,
-- ongoing support activity, or enhancement initiative.
--
-- Workstreams serve as the operational unit of planning and execution, bridging
-- business initiatives with technical resource allocation.
-- This table connects to:
--   Demand: defines the required FTE effort by role for the workstream.
--   Allocation: records how individual people are assigned to the workstream.
--   Actual: captures realized hours or effort for completed work within the workstream.
--
-- The Category column defines the type of effort (ProductDev, Enhancement, Implementation, Support),
-- and the Status column indicates its current phase (Planned, Active, Closed, or Evergreen).
CREATE TABLE Workstream (
    WorkstreamID UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    Name NVARCHAR(200) NOT NULL,
    Category NVARCHAR(50) NOT NULL, -- ProductDev, Enhancement, Implementation, Support
    ImplementationID UNIQUEIDENTIFIER NULL, -- FK to Implementation if applicable
    StartDate DATE NULL,
    EndDate DATE NULL,
    Status NVARCHAR(50) NULL, -- Planned, Active, Closed
    CONSTRAINT UQ_Workstreams_Name UNIQUE (Name),
    CONSTRAINT FK_Workstream_Implementation FOREIGN KEY (ImplementationID) REFERENCES Implementation(ImplementationID),
    Constraint PK_Workstream Primary Key (WorkstreamID)
)

-- ======================================================
-- TABLE: Demand
-- ======================================================
-- The Demand table captures the required effort, measured in FTE (Full-Time Equivalent),
-- for each role within a specific workstream and time period.
-- It represents the planning side of the capacity equation — what the organization needs.
--
-- Each record defines the expected resource requirement for a given role, period, and workstream,
-- along with a confidence factor indicating estimation certainty.
-- This table connects to:
--   Workstream: identifies the initiative or operational area generating demand.
--   Role: specifies the type of resource or skill being requested.
--
-- Demand records serve as the forecasted inputs against which capacity and allocations are compared,
-- forming the basis for gap analysis, staffing forecasts, and scenario modeling.
CREATE TABLE Demand (
    DemandID UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    WorkstreamID UNIQUEIDENTIFIER NOT NULL,
    RoleID UNIQUEIDENTIFIER NOT NULL,
    PeriodStart DATE NOT NULL,
    PeriodEnd DATE NOT NULL,
    RequiredFTE DECIMAL(9,2) NOT NULL,
    Confidence DECIMAL(5,2) NULL,
    Notes NVARCHAR(MAX) NULL,
    CONSTRAINT FK_Demand_Workstream FOREIGN KEY (WorkstreamID) REFERENCES Workstream(WorkstreamID),
    CONSTRAINT FK_Demand_Role FOREIGN KEY (RoleID) REFERENCES Role(RoleID),
    Constraint PK_Demand Primary Key (DemandID)
)

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

-- ======================================================
-- TABLE: Capacity
-- ======================================================
-- The Capacity table represents the total productive supply available for each role
-- during a defined time period. It defines the upper limit of how much effort
-- the organization can deliver, adjusted for utilization targets and availability.
--
-- Each record expresses the aggregate FTE capacity for a role between PeriodStart and PeriodEnd,
-- forming the supply side of the capacity model.
-- This table connects to:
--   Role: identifies the functional category whose capacity is being measured.
--
-- Capacity represents the pool of effort that can be distributed across multiple workstreams.
-- When compared to Demand, it highlights shortfalls or surpluses by role and timeframe.
-- EffectiveUtilization allows refinement of theoretical capacity by factoring in
-- real-world productivity adjustments or organizational constraints.
CREATE TABLE Capacity (
    CapacityID UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    RoleID UNIQUEIDENTIFIER NOT NULL,
    PeriodStart DATE NOT NULL,
    PeriodEnd DATE NOT NULL,
    AvailableFTE DECIMAL(9,2) NOT NULL,
    EffectiveUtilization DECIMAL(5,2) NULL,
    CONSTRAINT FK_Capacity_Role FOREIGN KEY (RoleID) REFERENCES Role(RoleID),
    Constraint PK_Capacity Primary Key (CapacityID)
)


-- ======================================================
-- TABLE: Allocation
-- ======================================================
-- The Allocation table defines how individual people are assigned
-- to specific workstreams over defined time periods. Each record represents
-- a planned or actual allocation of a person’s available time.
--
-- This table connects directly to:
--   Person: identifies who is being allocated.
--   Workstream: identifies the project, enhancement, or support effort
--     to which the person is assigned.
--
-- Allocation records represent planned or actual utilization,
-- expressed as a percentage of a person’s available capacity (AllocationPct).
-- These values can later be compared against Actuals to measure variance
-- between forecasted and real effort distribution.
--
-- The Allocation table is a key input to understanding real-world workload,
-- resource constraints, and multi-project participation across the organization.
-- ======================================================
CREATE TABLE Allocation (
    AllocationID UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    PersonID UNIQUEIDENTIFIER NOT NULL,
    WorkstreamID UNIQUEIDENTIFIER NOT NULL,
    PeriodStart DATE NOT NULL,
    PeriodEnd DATE NOT NULL,
    AllocationPct DECIMAL(5,2) NOT NULL, -- 0.5 = 50%
    AllocationType NVARCHAR(50) NULL, -- Planned, Actual
    Source NVARCHAR(100) NULL,
    CONSTRAINT FK_Allocation_Person FOREIGN KEY (PersonID) REFERENCES Person(PersonID),
    CONSTRAINT FK_Allocation_Workstream FOREIGN KEY (WorkstreamID) REFERENCES Workstream(WorkstreamID),
    Constraint PK_Allocations Primary Key (AllocationID)
)

-- ======================================================
-- TABLE: Actual
-- ======================================================
-- The Actual table records the realized effort or hours worked by individuals
-- across specific workstreams within defined time periods. Each record represents
-- measured execution data that can be used to compare against planned allocations.
--
-- This table connects directly to:
--   Person: identifies who performed the work.
--   Workstream: identifies the project, enhancement, or support effort
--     where the work occurred.
--
-- Actual data is essential for closing the loop on planning accuracy.
-- By comparing Actuals to Allocations, planners can identify systemic
-- under- or over-utilization, validate capacity assumptions, and adjust
-- demand forecasts accordingly.
--
-- Actual serves as the empirical record of workforce activity, enabling
-- trend analysis, velocity tracking, and variance reporting across periods.
-- ======================================================
CREATE TABLE Actual (
    ActualID UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    PersonID UNIQUEIDENTIFIER NOT NULL,
    WorkstreamID UNIQUEIDENTIFIER NOT NULL,
    PeriodStart DATE NOT NULL,
    PeriodEnd DATE NOT NULL,
    ActualHours DECIMAL(9,2) NULL,
    Source NVARCHAR(100) NULL,
    CONSTRAINT FK_Actual_Person FOREIGN KEY (PersonID) REFERENCES Person(PersonID),
    CONSTRAINT FK_Actual_Workstream FOREIGN KEY (WorkstreamID) REFERENCES Workstream(WorkstreamID),
    Constraint PK_Actual Primary Key (ActualID)
)

-- ======================================================
-- TABLE: Scenario
-- ======================================================
-- The Scenario table represents a named planning or forecasting scenario.
-- Each scenario defines a distinct modeling context used to test assumptions,
-- simulate future staffing needs, or compare alternative capacity plans.
--
-- This table connects directly to:
--   Person: identifies the individual who created or owns the scenario.
--   ForecastResult: stores the computed role-level capacity and demand outcomes
--     generated under this scenario.
--
-- Scenarios serve as sandbox environments for strategic modeling.
-- They allow planners to evaluate the impact of hiring plans, attrition,
-- new project demand, or productivity changes without affecting
-- operational data.
--
-- Only one scenario may be marked active at a time, representing
-- the current planning baseline against which others are compared.
-- ======================================================
CREATE TABLE Scenario (
    ScenarioID UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    Name NVARCHAR(150) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    IsActive BIT NOT NULL DEFAULT 0,
    CreatedBy UNIQUEIDENTIFIER NULL, -- FK to Person
    CreatedOn DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT FK_Scenario_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Person(PersonID),
    CONSTRAINT UQ_Scenario_Name UNIQUE (Name),
    Constraint PK_Scenario Primary Key (ScenarioID)
)

-- ======================================================
-- TABLE: ForecastResult
-- ======================================================
-- The ForecastResult table stores the calculated outcomes of scenario-based
-- capacity modeling. Each record represents the comparison of required versus
-- available FTEs for a given role and time period within a specific scenario.
--
-- This table connects directly to:
--   Scenario: defines the planning model or simulation that produced the result.
--   Role: identifies the job function being analyzed (e.g., QA Engineer, Developer).
--
-- ForecastResult provides the analytical bridge between supply and demand.
-- It captures computed variances, utilization percentages, and balance status
-- (Under, Balanced, Over), allowing planners to pinpoint shortages or excesses
-- in specific skill areas or timeframes.
--
-- These results form the basis for executive dashboards, hiring forecasts,
-- and resource optimization recommendations across the organization.
-- ======================================================
CREATE TABLE ForecastResult (
    ForecastID UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    ScenarioID UNIQUEIDENTIFIER NOT NULL,
    RoleID UNIQUEIDENTIFIER NOT NULL,
    PeriodStart DATE NOT NULL,
    PeriodEnd DATE NOT NULL,
    RequiredFTE DECIMAL(9,2) NOT NULL,
    AvailableFTE DECIMAL(9,2) NOT NULL,
    VarianceFTE AS (AvailableFTE - RequiredFTE) PERSISTED,
    UtilizationPct AS (CASE WHEN AvailableFTE = 0 THEN 0 ELSE RequiredFTE / AvailableFTE END) PERSISTED,
    Status NVARCHAR(20) NULL, -- Under, Balanced, Over
    LastComputed DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT FK_ForecastResult_Scenarios FOREIGN KEY (ScenarioID) REFERENCES Scenario(ScenarioID),
    CONSTRAINT FK_ForecastResult_Roles FOREIGN KEY (RoleID) REFERENCES [Role](RoleID),
    Constraint PK_ForecastResult Primary Key (ForecastID)
)

-- ======================================================
-- END OF SCHEMA
-- ======================================================
