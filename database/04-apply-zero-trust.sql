if exists (select * from syscolumns where id in (select id from sysobjects where xtype = 'u' and name = 'user') and name = 'otp_code')
	alter table [dbo].[user] drop column [otp_code]
go

alter table [dbo].[user] add [otp_code] [varchar](255) null
go

if exists (select * from syscolumns where id in (select id from sysobjects where xtype = 'u' and name = 'user') and name = 'otp_expiration')
	alter table [dbo].[user] drop column [otp_expiration]
go

alter table [dbo].[user] add [otp_expiration] datetime null
go
