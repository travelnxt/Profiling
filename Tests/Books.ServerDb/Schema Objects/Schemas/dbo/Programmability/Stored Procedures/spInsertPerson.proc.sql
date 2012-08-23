Create procedure [dbo].[spInsertPerson]
@Name nvarchar(50),
@LastName nvarchar(50),
@City nvarchar(50),
@Country nvarchar(50),
@Mobile nvarchar(50),
@CompanyId bigint
as
insert into person(Name, City, CompanyId, Country, LastName, Mobile) values
(@Name, @City, @CompanyId, @Country, @LastName, @Mobile)