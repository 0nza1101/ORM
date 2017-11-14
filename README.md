# ORM


Pour pouvoir tester plus rapidement les differents requetes vers les differents type de base de donnée v

### Microsoft SQL requêtes initiales

```
CREATE DATABASE devdb
GO

USE devdb
GO

CREATE TABLE Contacts (
    ContactID int IDENTITY(1,1) PRIMARY KEY,
    Name varchar(255) NOT NULL,
    Address varchar(255) NOT NULL,
    Email varchar(255) NOT NULL);
GO

INSERT INTO Contacts VALUES ('monNom', 'MonAddress', 'monMail@gmail.com');
GO
```

### MySQL Server requêtes initiales

```
CREATE DATABASE devdb

USE devdb


CREATE TABLE Contacts (
    ContactID INT PRIMARY KEY AUTO_INCREMENT,
    Name VARCHAR(255) NOT NULL,
    Address VARCHAR(255) NOT NULL,
    Email VARCHAR(255) NOT NULL
);

INSERT INTO Contacts VALUES ('monNom', 'MonAddress', 'monMail@gmail.com');
```

### PostgreSQL Server requêtes initiales

```
CREATE DATABASE devdb

\c devdb


CREATE TABLE Contacts (
    ContactID INT SERIAL PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Address VARCHAR(255) NOT NULL,
    Email VARCHAR(255) NOT NULL
);

INSERT INTO Contacts VALUES ('monNom', 'MonAddress', 'monMail@gmail.com');
```

