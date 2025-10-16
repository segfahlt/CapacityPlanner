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
('Actual', 'Recorded actual allocation or utilization'),
('Planned', 'Forward-looking allocation (forecast)')
