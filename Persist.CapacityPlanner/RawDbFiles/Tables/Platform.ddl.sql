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
