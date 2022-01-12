CREATE PROCEDURE [dbo].[spProductGetById]
   @Id int
As
begin
    set nocount on;

     select Id, ProductName, [Description], RetailPrice, QuantityInStock, IsTaxable
     from dbo.Product
     where Id = @Id;
end
