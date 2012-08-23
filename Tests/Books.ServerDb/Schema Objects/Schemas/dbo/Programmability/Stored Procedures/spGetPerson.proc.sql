create procedure [dbo].[spGetPerson]
@Id bigInt
as
select Name, City, Id, CompanyId, Country, LastName, Mobile from person where Id = @Id