# ORM


Pour pouvoir tester plus rapidement les differents requetes vers les differents type de base de donnée

### Microsoft SQL Server Initial queries

CREATE DATABASE devdb
GO

USE devdb
GO

CREATE TABLE Contacts ( ContactID int IDENTITY(1,1) PRIMARY KEY, Name varchar(255) NOT NULL, Address varchar(255) NOT NULL, Email varchar(255) NOT NULL);
GO

INSERT INTO Contacts VALUES ('monNom', 'MonAddress', 'monMail@gmail.com');
GO

