USE [master]
GO

-- Fecha todas as conexões e drop database se existir
IF EXISTS(SELECT * FROM sys.databases WHERE name = 'impacta-credenciamento')
BEGIN
    ALTER DATABASE [impacta-credenciamento] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
    DROP DATABASE [impacta-credenciamento]
END
GO

-- Cria o database
CREATE DATABASE [impacta-credenciamento]
GO

-- Drop user se existir no database
USE [impacta-credenciamento]
GO
IF EXISTS (SELECT * FROM sys.database_principals WHERE name = 'impacta_user')
    DROP USER [impacta_user]
GO

-- Drop login se existir
USE [master]
GO
IF EXISTS (SELECT * FROM sys.server_principals WHERE name = 'impacta_user')
    DROP LOGIN [impacta_user]
GO

-- Cria o login
CREATE LOGIN [impacta_user] WITH PASSWORD=N'vvsD%u3y!5s5m#EI', 
    DEFAULT_DATABASE=[master], 
    CHECK_EXPIRATION=OFF, 
    CHECK_POLICY=OFF
GO

-- Cria o user no database
USE [impacta-credenciamento]
GO
CREATE USER [impacta_user] FOR LOGIN [impacta_user]
GO

-- Define schema padrão
ALTER USER [impacta_user] WITH DEFAULT_SCHEMA=[dbo]
GO

-- Adiciona à role db_owner
ALTER ROLE [db_owner] ADD MEMBER [impacta_user]
GO