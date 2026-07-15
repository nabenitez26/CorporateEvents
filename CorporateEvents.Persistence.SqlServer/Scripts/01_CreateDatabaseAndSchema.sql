-- ============================================================
-- CorporateEvents - Script de creación de base de datos y schema
-- Ejecutar contra el servidor SQL Server (Express, LocalDB, etc.)
-- que se vaya a usar. Es idempotente: se puede correr varias veces.
-- ============================================================

-- 1. Crear la base de datos si no existe
IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = 'CorporateEventsDb')
BEGIN
    CREATE DATABASE CorporateEventsDb;
END
GO

-- 2. Cambiar de contexto a la base de datos
USE CorporateEventsDb;
GO

-- 3. Crear el schema si no existe
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'EventBus')
    EXEC('CREATE SCHEMA EventBus');
GO

-- 4. Tabla de eventos fallidos (Dead Letter Queue)
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'DeadLetterEvents' AND schema_id = SCHEMA_ID('EventBus'))
BEGIN
    CREATE TABLE EventBus.DeadLetterEvents (
        Id            BIGINT IDENTITY(1,1) PRIMARY KEY,
        EventId       UNIQUEIDENTIFIER NOT NULL,
        CorrelationId UNIQUEIDENTIFIER NOT NULL,
        EventType     NVARCHAR(200)    NOT NULL,
        HandlerName   NVARCHAR(200)    NOT NULL,
        PayloadJson   NVARCHAR(MAX)    NOT NULL,
        ErrorMessage  NVARCHAR(MAX)    NOT NULL,
        FailedAt      DATETIME2        NOT NULL
    );
END
GO

-- 5. Tabla de idempotencia (eventos ya procesados)
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'EventProcessing' AND schema_id = SCHEMA_ID('EventBus'))
BEGIN
    CREATE TABLE EventBus.EventProcessing (
		EventId UNIQUEIDENTIFIER NOT NULL,
        CorrelationId       UNIQUEIDENTIFIER NOT NULL,
		EventType     NVARCHAR(200)    NOT NULL,
		[Status] TINYINT NOT NULL,
	    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
		ProcessedAt DATETIME2 NULL,
		ErrorMessage NVARCHAR(MAX) NULL,
        CONSTRAINT PK_ProcessedEvents PRIMARY KEY (EventId, EventType)
    );
END
GO


