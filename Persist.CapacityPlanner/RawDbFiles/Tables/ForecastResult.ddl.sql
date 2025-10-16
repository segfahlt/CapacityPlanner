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
