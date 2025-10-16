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
