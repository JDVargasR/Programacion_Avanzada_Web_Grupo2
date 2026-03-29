USE Proyecto_Web_Grupo2;
GO

-- Columnas nuevas en SourceItems (idempotente)
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('SourceItems') AND name = 'Title')
    ALTER TABLE SourceItems ADD Title NVARCHAR(500) NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('SourceItems') AND name = 'ExternalId')
    ALTER TABLE SourceItems ADD ExternalId NVARCHAR(500) NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('SourceItems') AND name = 'IsPinned')
    ALTER TABLE SourceItems ADD IsPinned BIT NOT NULL DEFAULT 0;
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('SourceItems') AND name = 'Category')
    ALTER TABLE SourceItems ADD Category NVARCHAR(200) NULL;
GO

-- Sources semilla (idempotente)
IF NOT EXISTS (SELECT 1 FROM Sources WHERE Name = 'NewsAPI')
    INSERT INTO Sources (Url, Name, ComponentType, RequiresSecret)
    VALUES ('https://newsapi.org/v2/top-headlines', 'NewsAPI', 'api', 1);
GO

IF NOT EXISTS (SELECT 1 FROM Sources WHERE Name = 'Importados')
    INSERT INTO Sources (Url, Name, ComponentType, RequiresSecret)
    VALUES ('internal://import', 'Importados', 'import', 0);
GO
