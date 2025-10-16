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
('Active', 'Currently in execution'),
('Closed', 'Completed and inactive'),
('Evergreen', 'Ongoing or continuous with no defined end'),
('Planned', 'Defined but not yet started'),
('Retired', 'Phased out and no longer maintained')
