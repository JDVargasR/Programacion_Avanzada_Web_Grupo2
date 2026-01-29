/* 
=========================
   CREAR BASE DE DATOS
========================= 
*/
CREATE DATABASE Proyecto_Web_Grupo2;
GO

USE Proyecto_Web_Grupo2;
GO

/* 
=========================
   TABLA: Sources
========================= 
*/
CREATE TABLE Sources (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Url NVARCHAR(500) NOT NULL,          -- URL fuente
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(500) NULL,
    ComponentType NVARCHAR(100) NOT NULL, -- e.g. 'widget', 'api', 'feed'
    RequiresSecret BIT NOT NULL DEFAULT 0 -- 0 = no, 1 = yes
);
GO

/* 
=========================
   TABLA: SourceItems
========================= 
*/
CREATE TABLE SourceItems (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SourceId INT NOT NULL,                -- relación con Sources
    Json NVARCHAR(MAX) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_SourceItems_Sources 
        FOREIGN KEY (SourceId) REFERENCES Sources(Id)
);
GO

/* 
=========================
   TABLA: Secrets / Settings
========================= 
*/
CREATE TABLE Secrets (
   
);
GO
