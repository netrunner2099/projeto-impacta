USE [impacta-credenciamento]
GO

if exists (select * from sysobjects where xtype = 'u' and name = 'user')
	drop table [dbo].[user]
go

if exists (select * from sysobjects where xtype = 'u' and name = 'ticket')
	drop table [dbo].[ticket]
go

if exists (select * from sysobjects where xtype = 'u' and name = 'person')
	drop table [dbo].[person]
go

if exists (select * from sysobjects where xtype = 'u' and name = 'event')
	drop table [dbo].[event]
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
	[complement]		varchar(100) null,
	[neighborhood]		varchar(100) not null,
	[city]				varchar(100) not null,
	[state]				varchar(2) not null,
	[status]			tinyint not null default 1,
	[createdat]			datetime not null default getdate(),
	[updatedat]			datetime null,
	constraint pk_person primary key (personid)
)
go

create table [dbo].[event]
(
	[eventid]			bigint identity(1,1) not null,
	[name]				varchar(100) not null,
	[description]		varchar(500) not null,
	[begin]				datetime not null,
	[end]				datetime not null,
	[price]				numeric(18,6) not null,
	[status]			tinyint not null default 1,
	[createdat]			datetime not null default getdate(),
	[updatedat]			datetime null,
	constraint pk_event primary key (eventid)
)
go

create table [dbo].[ticket]
(
	[ticketid]			bigint identity(1,1) not null,
	[personid]			bigint  not null,
	[eventid]			bigint not null,
	[price]				numeric(18,6) not null,
	[status]			tinyint not null default 1,
	[createdat]			datetime not null default getdate(),
	[updatedat]			datetime null,
	constraint pk_ticket primary key (ticketid),
	constraint fk_ticket_person foreign key (personid) references [dbo].[person](personid),
	constraint fk_ticket_event foreign key (eventid) references [dbo].[event](eventid)
)
go


create table [dbo].[user]
(
	[userid]			bigint identity(1,1) not null,
	[name]				varchar(255) not null,
	[email]				varchar(255) not null,
	[password]			varchar(255) not null,
	[role]				tinyint not null default 1,
	[status]			tinyint not null default 1,
	[createdat]			datetime not null default getdate(),
	[updatedat]			datetime null,
	constraint pk_user primary key (userid)
)
go




