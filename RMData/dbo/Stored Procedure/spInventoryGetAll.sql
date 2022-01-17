CREATE PROCEDURE [dbo].[spInventoryGetAll]
   
AS
  begin
  set nocount on;
  select [Id], [ProductId], [Quantity], [PurchasePrice], [PurchaseDate]
  from dbo.Inventory;
  end