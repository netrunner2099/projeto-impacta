use [master]
go

if exists(select * from sysdatabases where name = 'impacta-credenciamento')
	drop database [impacta-credenciamento]
go

CREATE DATABASE [impacta-credenciamento]
go

