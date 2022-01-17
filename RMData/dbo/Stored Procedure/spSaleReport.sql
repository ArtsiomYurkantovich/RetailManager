CREATE PROCEDURE [dbo].[spSaleReport]
   
AS
   begin
   set nocount on;

   select [s].[SaleDate], [s].[SubTotal], [s].[Tax], [s].[Total], [u].[Firstname], [u].[LastName], [u].[EmailAdress]
   from dbo.Sale as s
   inner join dbo.[User] as u on s.CashierId = u.Id;
   end
