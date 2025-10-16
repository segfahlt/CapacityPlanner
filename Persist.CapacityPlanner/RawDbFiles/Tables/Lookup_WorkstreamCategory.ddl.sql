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
('Enhancement', 'Improvements or refactors to existing modules'),
('Implementation', 'Client-specific configuration or deployment'),
('ProductDev', 'Core product feature development'),
('R&D', 'Research and development of new technologies or prototypes'),
('Support', 'Ongoing production maintenance and issue resolution')
