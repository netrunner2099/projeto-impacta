USE [impacta-credenciamento]
GO

if exists (select * from sysobjects where xtype = 'u' and name = 'person')
	drop table [dbo].[person]
go

create table [dbo].[person]
(
	[personid]			bigint identity(1,1) not null,
	[name]				varchar(255) not null,
	[document]			varchar(12) not null,
	[email]				varchar(255) not null,
	[phone]				varchar(20) not null,
	[zipcode]			varchar(9) not null,
	[address]			varchar(255) not null,
	[number]			varchar(50) not null,
	[complement]		varchar(100) not null,
	[neighborhood]		varchar(100) not null,
	[city]				varchar(100) not null,
	[state]				varchar(2) not null,
	[status]			tinyint not null default 1,
	[createdat]			datetime not null default getdate(),
	[updatedat]			datetime null,
	constraint pk_person primary key (personid)
)
go



