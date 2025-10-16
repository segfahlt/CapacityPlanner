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
