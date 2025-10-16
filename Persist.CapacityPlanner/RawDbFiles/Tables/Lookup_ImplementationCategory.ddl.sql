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
('Integration', 'Cross-system or external connector build'),
('POC', 'Proof of concept or exploratory project'),
('Product', 'Internal core product effort')
