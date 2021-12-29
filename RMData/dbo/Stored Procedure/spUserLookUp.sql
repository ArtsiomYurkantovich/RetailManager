CREATE PROCEDURE [dbo].[spUserLookUp]
   @Id nvarchar(128)
AS
begin
set nocount on;
    SELECT Id, Firstname, LastName, EmailAdress, CreateDate
    From [dbo].[User]
    where Id = @Id;
end
    