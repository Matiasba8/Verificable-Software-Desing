Create Database InscripcionesBrDbGrupo06
GO

USE [InscripcionesBrDbGrupo06]
GO

CREATE TABLE [dbo].[Persona](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Rut] [nvarchar](10) NULL,
	[Nombre] [nvarchar](50) NOT NULL,
	[FechaNacimiento] [date] NOT NULL,
	[Email] [nchar](50) NOT NULL,
	[Dirección] [nchar](50) NULL,
 CONSTRAINT [PK_Persona] PRIMARY KEY CLUSTERED(
	[Id] ASC
))
GO


USE [InscripcionesBrDbGrupo06]
GO
SET IDENTITY_INSERT [dbo].[Persona] ON 
GO
INSERT [dbo].[Persona] ([Id], [Rut], [Nombre], [FechaNacimiento], [Email], [Dirección]) VALUES (1, N'10915348-6', N'Mario Abellan', CAST(N'1982-01-15' AS Date), N'marioabellan@gmail.com                            ', N'Galvarino Gallardo 1534                           ')
GO
SET IDENTITY_INSERT [dbo].[Persona] OFF
GO

/* Multipropietario (por una extraña razón sale ue ya existe multipropietario y ya lo he intentado borrar varias veces sin exito, por esto es multipropietario1) */

CREATE TABLE [dbo].[Multipropietario1](
	[NumeroAtencion] [int] IDENTITY(1,1) NOT NULL,
	[Comuna] [nvarchar](30) NOT NULL,
	[Manzana] [nvarchar](50) NOT NULL,
	[Predio] [nvarchar](30) NOT NULL,
	[Fojas] [nvarchar](50) NOT NULL,
	[ano_inscripcion] [date] NOT NULL,
	[numero_de_inscripcion] [int] NOT NULL,
 CONSTRAINT [Multipropietario] PRIMARY KEY CLUSTERED(
	[NumeroAtencion] ASC
))

CREATE TABLE Enajenantes (
    [Id] INT PRIMARY KEY,
    NumeroAtencion INT NOT NULL,
    Rut NVARCHAR(10) NOT NULL,
    Derechos DECIMAL(10,2) NOT NULL,
    DerechosNoAcreditados DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (NumeroAtencion) REFERENCES Multipropietario1(NumeroAtencion)
);


CREATE TABLE Adquirentes (
    [Id] INT PRIMARY KEY,
    NumeroAtencion INT NOT NULL,
    Rut NVARCHAR(10) NOT NULL,
    Derechos DECIMAL(10,2) NOT NULL,
    DerechosNoAcreditados DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (NumeroAtencion) REFERENCES Multipropietario1(NumeroAtencion)
);

CREATE TABLE InscripcionesEnajenaciones (
    [Id] INT PRIMARY KEY,
    NumeroAtencion INT NOT NULL,
    Rut NVARCHAR(10) NOT NULL,
    Derechos DECIMAL(10,2) NOT NULL,
    DerechosNoAcreditados DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (NumeroAtencion) REFERENCES Multipropietario1(NumeroAtencion)
);