
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 12/04/2015 13:10:05
-- Generated from EDMX file: C:\Users\Ondro\documents\visual studio 2015\Projects\LabourDivider\LabourDivider\LabourModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [labourdividerdatabase];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_JobWork]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Works] DROP CONSTRAINT [FK_JobWork];
GO
IF OBJECT_ID(N'[dbo].[FK_EmployeeWork]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Works] DROP CONSTRAINT [FK_EmployeeWork];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Jobs]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Jobs];
GO
IF OBJECT_ID(N'[dbo].[Employees]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Employees];
GO
IF OBJECT_ID(N'[dbo].[Works]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Works];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Jobs'
CREATE TABLE [dbo].[Jobs] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Employees'
CREATE TABLE [dbo].[Employees] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [FirstName] nvarchar(max)  NOT NULL,
    [LastName] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Works'
CREATE TABLE [dbo].[Works] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [From] datetime  NOT NULL,
    [To] datetime  NOT NULL,
    [JobId] int  NOT NULL,
    [EmployeeId] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Jobs'
ALTER TABLE [dbo].[Jobs]
ADD CONSTRAINT [PK_Jobs]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Employees'
ALTER TABLE [dbo].[Employees]
ADD CONSTRAINT [PK_Employees]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Works'
ALTER TABLE [dbo].[Works]
ADD CONSTRAINT [PK_Works]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [JobId] in table 'Works'
ALTER TABLE [dbo].[Works]
ADD CONSTRAINT [FK_JobWork]
    FOREIGN KEY ([JobId])
    REFERENCES [dbo].[Jobs]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_JobWork'
CREATE INDEX [IX_FK_JobWork]
ON [dbo].[Works]
    ([JobId]);
GO

-- Creating foreign key on [EmployeeId] in table 'Works'
ALTER TABLE [dbo].[Works]
ADD CONSTRAINT [FK_EmployeeWork]
    FOREIGN KEY ([EmployeeId])
    REFERENCES [dbo].[Employees]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_EmployeeWork'
CREATE INDEX [IX_FK_EmployeeWork]
ON [dbo].[Works]
    ([EmployeeId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------