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
('Contingent', 'Contractor, consultant, or temporary resource'),
('FTE', 'Full-time employee of the organization')
