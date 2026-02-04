/* 
=========================
   CREAR BASE DE DATOS
========================= 
*/
IF DB_ID('Proyecto_Web_Grupo2') IS NOT NULL
BEGIN
    ALTER DATABASE Proyecto_Web_Grupo2 SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE Proyecto_Web_Grupo2;
END
GO

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
    Url NVARCHAR(500) NOT NULL,
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(500) NULL,
    ComponentType NVARCHAR(100) NOT NULL,
    RequiresSecret BIT NOT NULL DEFAULT 0
);
GO

/* 
=========================
   TABLA: SourceItems
========================= 
*/
CREATE TABLE SourceItems (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SourceId INT NOT NULL,
    Json NVARCHAR(MAX) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_SourceItems_Sources
        FOREIGN KEY (SourceId) REFERENCES Sources(Id)
);
GO

/* 
=========================
   TABLA: Secrets
========================= 
*/
CREATE TABLE Secrets (

);
GO

/* 
=========================
   TABLA: Estados
========================= 
*/
CREATE TABLE Estados (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(50) NOT NULL UNIQUE,
    Descripcion NVARCHAR(200) NULL
);
GO

INSERT INTO Estados (Nombre)
VALUES
('Activo'),
('Inactivo');
GO


/* 
=========================
   TABLA: Roles
========================= 
*/
CREATE TABLE Roles (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(50) NOT NULL UNIQUE,
    Descripcion NVARCHAR(200) NULL,
    EstadoId INT NOT NULL,

    CONSTRAINT FK_Roles_Estados
        FOREIGN KEY (EstadoId) REFERENCES Estados(Id)
);
GO

INSERT INTO Roles (Nombre, Descripcion, EstadoId)
VALUES
(
    'Administrador',
    'Usuario con control total del sistema',
    (SELECT Id FROM Estados WHERE Nombre = 'Activo')
),
(
    'Visitante',
    'Usuario con acceso limitado de solo lectura',
    (SELECT Id FROM Estados WHERE Nombre = 'Activo')
);
GO

/* 
=========================
   TABLA: Usuarios
========================= 
*/
CREATE TABLE Usuarios (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(150) NOT NULL,
    Correo NVARCHAR(150) NOT NULL UNIQUE,
    Contrasena NVARCHAR(255) NOT NULL,
    RolId INT NOT NULL,
    EstadoId INT NOT NULL,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_Usuarios_Roles
        FOREIGN KEY (RolId) REFERENCES Roles(Id),

    CONSTRAINT FK_Usuarios_Estados
        FOREIGN KEY (EstadoId) REFERENCES Estados(Id)
);
GO
