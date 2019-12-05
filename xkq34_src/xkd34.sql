--����Ա�û���
select * from aspnet_Managers
--��Ա�ȼ���
select * from aspnet_MemberGrades
--�û���
select * from aspnet_Members
--������˾��
select * from Hishop_ExpressTemplates
--�˷�ģ���
select * from Hishop_FreightTemplate_Templates
--�˷������
select * from Hishop_FreightTemplate_SpecifyRegionGroups
--���������
select * from Hishop_OrderItems
--������¼��
select * from Hishop_Orders
--���۱�
select * from Hishop_OrderReturns
--���׳ɹ���
select * from Hishop_OrderSendNote
--���ֶһ���¼��
select * from Hishop_PointExchange_Changed
--��Ʒ��(integral:��Ʒ�ͻ���ֵ��IntegralToNow:�Ƿ��ܻ��ֶһ���1��,0��,IntegralCeiling:���ֶһ�����,ZeroBuy:��Ԫ����ʶ��0��ʶ��1��ʶ�ǣ�)
select * from Hishop_Products
--���ﳵ��
select * from Hishop_ShoppingCarts
select * from [dbo].[Hishop_SKUs]
--���ֱ䶯��
select * from vshop_IntegralDetail
--���ﳵ��ַ��Ϣ��
select * from Hishop_UserShippingAddresses
--���ֶһ����ݱ�
select * from [dbo].[PointsForConten]
--���ֶһ������Ϣ��
select * from [dbo].[PointsForMoney]
--��Ʒ�����
select * from [dbo].[Hishop_Categories]
--��Ʒ���ͱ�
select * from [dbo].[Hishop_ProductTypes]
--��ƷƷ�Ʊ�
select * from [dbo].[Hishop_BrandCategories]
--��������
select * from VShop_NavMenu


--����Ʒ������ӻ����ֶ�
alter table [dbo].[Hishop_Products] add integral		int null  --���integral����Ʒ�ͻ���ֵ���ֶ�
alter table [dbo].[Hishop_Products] add IntegralToNow	int null  --���IntegralToNow���Ƿ��ܻ��ֶһ���1��,0�񣩣��ֶ�
alter table [dbo].[Hishop_Products] add IntegralCeiling int null  --���IntegralCeiling�����ֶһ����ޣ��ֶ�
alter table [dbo].[Hishop_Products] add ZeroBuy		    int null  --���ZeroBuy����Ԫ����ʶ��0��ʶ��1��ʶ�ǣ����ֶ�
alter table [dbo].[Hishop_Products] add LeaseGood		int null  --���LeaseGood��������Ʒ��ʶ��0��ʶ��1��ʶ�ǣ����ֶ�
go


--�½����ֶһ����ݱ�
if exists(select * from sysobjects where name='PointsForConten')
drop table PointsForConten
go
    create table PointsForConten
    (
        id					int             not null    identity(1,1)    primary key ,    
        PointsForConten		varchar(110)		null,		--���ֶһ�����
        UserId				int					null,		--�û����
        ProductId			int			    not null,		--��Ʒ���
        IntegralSet			varchar(50)			null,		--���ֶһ�ֵ
        SkuId				nvarchar(100)   not null,		--��ƷΨһ��ʶ
        ClosePointsFor		int					null		--�Ƿ������ֶһ���1���� 0����
    )
go

--�½����ֶһ������Ϣ��
if exists(select * from sysobjects where name='PointsForMoney')
drop table PointsForMoney
go
    create table PointsForMoney
    (
        id					int             not null    identity(1,1)    primary key ,    
        PointsForMoney		int				not	null,		--���ֶһ����
        UserId				int				not	null,		--�û����
        [Time]				datetime	    not null,		--���ʱ��
        [Type]				int				not	null		--�������ͣ�0���������� 1�����ﳵ���㣩
    )
go



--�޸�ss_ShoppingCart_GetItemInfo�洢����
USE [xkd34]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[ss_ShoppingCart_GetItemInfo]
(
		@Quantity INT,
		@UserId INT,
		@SkuId NVARCHAR(100),
		@GradeId INT,
		@Type INT
	)
AS

DECLARE @ProductId INT, @Weight INT, @Stock INT, @SalePrice MONEY, @MemberPrice MONEY, @Discount INT, @SKU NVARCHAR(50),@ExChangeId INT 
 
 SELECT @ProductId = ProductId, @SKU = SKU, @Weight = [Weight], @Stock = Stock, @SalePrice = SalePrice FROM Hishop_SKUs WHERE SkuId = @SkuId
-- ��Ա��ѯ
IF @UserId>0 
BEGIN
	SELECT @MemberPrice = MemberSalePrice FROM Hishop_SKUMemberPrice WHERE SkuId = @SkuId AND GradeId = @GradeId
	SELECT @Discount = Discount FROM aspnet_MemberGrades WHERE GradeId = @GradeId		
	SELECT @Quantity=Quantity,@ExChangeId=ExChangeId FROM Hishop_ShoppingCarts WHERE UserId = @UserId AND SkuId = @SkuId And [Type]=@Type
 	IF @MemberPrice IS NOT NULL
		SET @SalePrice = @MemberPrice
	ELSE
		SET @SalePrice = (@SalePrice * @Discount)/100
 END
	
 -- ������Ʒ������Ϣ
SELECT ProductId, SaleStatus, @SKU as SKU, @Stock as Stock, @Quantity as TotalQuantity, ProductName, CategoryId, @Weight AS [Weight], @SalePrice AS SalePrice, 
	ThumbnailUrl40,ThumbnailUrl60,ThumbnailUrl100,ThumbnailUrl160, ThumbnailUrl180, ThumbnailUrl220,IsfreeShipping,MainCategoryPath,IsSetCommission,ThirdCommission,SecondCommission,FirstCommission,FreightTemplateId,CubicMeter,FreightWeight,[IntegralToNow] FROM Hishop_Products WHERE ProductId = @ProductId AND SaleStatus=1
-- ���ص�ǰ�����Ϣ
SELECT s.SkuId, s.SKU, s.ProductId, s.Stock, AttributeName, ValueStr FROM Hishop_SKUs s left join Hishop_SKUItems si on s.SkuId = si.SkuId
left join Hishop_Attributes a on si.AttributeId = a.AttributeId left join Hishop_AttributeValues av on si.ValueId = av.ValueId WHERE s.SkuId = @SkuId
AND s.ProductId IN (SELECT ProductId FROM Hishop_Products WHERE SaleStatus=1)
--���ػ��ֶһ���Ϣ
select ProductNumber,PointNumber,ChangedNumber,eachMaxNumber,[status],exChangeId from Hishop_PointExChange_Products where exChangeId=@ExChangeId and ProductId=@ProductId
GO



--��ӹ����ͻ�����ͼ
if exists (select * from sysobjects where name = 'vw_Hishop_SendsIntegralProductList')
 drop view vw_Hishop_SendsIntegralProductList
 go
 --������ͼ
create view vw_Hishop_SendsIntegralProductList 
as
  SELECT   CategoryId,integral,IntegralToNow,IntegralCeiling, TypeId, BrandId, ProductId, ProductName, ProductShortName, ProductCode, ShortDescription, MarketPrice, 
                ThumbnailUrl40, ThumbnailUrl60, ThumbnailUrl100, ThumbnailUrl160, ThumbnailUrl180, ThumbnailUrl220, 
                ThumbnailUrl310, SaleStatus, DisplaySequence, MainCategoryPath, ExtendCategoryPath, SaleCounts, ShowSaleCounts, 
                AddedDate, VistiCounts, MaxShowPrice, MinShowPrice AS SalePrice,
                    (SELECT   TOP (1) SkuId
                     FROM      dbo.Hishop_SKUs
                     WHERE   (ProductId = p.ProductId)
                     ORDER BY SalePrice) AS SkuId,
                    (SELECT   SUM(Stock) AS Expr1
                     FROM      dbo.Hishop_SKUs AS Hishop_SKUs_2
                     WHERE   (ProductId = p.ProductId)) AS Stock,
                    (SELECT   TOP (1) Weight
                     FROM      dbo.Hishop_SKUs AS Hishop_SKUs_1
                     WHERE   (ProductId = p.ProductId)
                     ORDER BY SalePrice) AS Weight,
                    (SELECT   COUNT(*) AS Expr1
                     FROM      dbo.Taobao_Products
                     WHERE   (ProductId = p.ProductId)) AS IsMakeTaobao
FROM      dbo.Hishop_Products AS p where integral!=0
go


--��ӻ��ֶһ���ͼ
if exists (select * from sysobjects where name = 'vw_Hishop_PointsForProductList')
 drop view vw_Hishop_PointsForProductList
 go
 --������ͼ
create view vw_Hishop_PointsForProductList 
as
  SELECT   CategoryId,integral,IntegralToNow,IntegralCeiling, TypeId, BrandId, ProductId, ProductName, ProductShortName, ProductCode, ShortDescription, MarketPrice, 
                ThumbnailUrl40, ThumbnailUrl60, ThumbnailUrl100, ThumbnailUrl160, ThumbnailUrl180, ThumbnailUrl220, 
                ThumbnailUrl310, SaleStatus, DisplaySequence, MainCategoryPath, ExtendCategoryPath, SaleCounts, ShowSaleCounts, 
                AddedDate, VistiCounts, MaxShowPrice, MinShowPrice AS SalePrice,
                    (SELECT   TOP (1) SkuId
                     FROM      dbo.Hishop_SKUs
                     WHERE   (ProductId = p.ProductId)
                     ORDER BY SalePrice) AS SkuId,
                    (SELECT   SUM(Stock) AS Expr1
                     FROM      dbo.Hishop_SKUs AS Hishop_SKUs_2
                     WHERE   (ProductId = p.ProductId)) AS Stock,
                    (SELECT   TOP (1) Weight
                     FROM      dbo.Hishop_SKUs AS Hishop_SKUs_1
                     WHERE   (ProductId = p.ProductId)
                     ORDER BY SalePrice) AS Weight,
                    (SELECT   COUNT(*) AS Expr1
                     FROM      dbo.Taobao_Products
                     WHERE   (ProductId = p.ProductId)) AS IsMakeTaobao
FROM      dbo.Hishop_Products AS p where IntegralToNow=1
go

--�����Ԫ����ͼ
if exists (select * from sysobjects where name = 'vw_Hishop_ZeroByProductList')
 drop view vw_Hishop_ZeroByProductList
 go
 --������ͼ
create view vw_Hishop_ZeroByProductList 
as
  SELECT   CategoryId,integral,IntegralToNow,IntegralCeiling, TypeId, BrandId, ProductId, ProductName, ProductShortName, ProductCode, ShortDescription, MarketPrice, 
                ThumbnailUrl40, ThumbnailUrl60, ThumbnailUrl100, ThumbnailUrl160, ThumbnailUrl180, ThumbnailUrl220, 
                ThumbnailUrl310, SaleStatus, DisplaySequence, MainCategoryPath, ExtendCategoryPath, SaleCounts, ShowSaleCounts, 
                AddedDate, VistiCounts, MaxShowPrice, MinShowPrice AS SalePrice,
                    (SELECT   TOP (1) SkuId
                     FROM      dbo.Hishop_SKUs
                     WHERE   (ProductId = p.ProductId)
                     ORDER BY SalePrice) AS SkuId,
                    (SELECT   SUM(Stock) AS Expr1
                     FROM      dbo.Hishop_SKUs AS Hishop_SKUs_2
                     WHERE   (ProductId = p.ProductId)) AS Stock,
                    (SELECT   TOP (1) Weight
                     FROM      dbo.Hishop_SKUs AS Hishop_SKUs_1
                     WHERE   (ProductId = p.ProductId)
                     ORDER BY SalePrice) AS Weight,
                    (SELECT   COUNT(*) AS Expr1
                     FROM      dbo.Taobao_Products
                     WHERE   (ProductId = p.ProductId)) AS IsMakeTaobao
FROM      dbo.Hishop_Products AS p where [ZeroBuy]=1 
go


--��ҳ��ѯδ���ù����ͻ��ֺ�δ���û��ֶһ�����Ʒ
if exists (select * from sys.procedures where name='NoSelPointsFor')
drop proc NoSelPointsFor
go
create proc NoSelPointsFor
@totalPage int output,--��ҳ��
@totalCount int output,--������
@pageSize int,--ҳ��С
@pageIndex int--��ǰҳ
as

/*��ȡ������*/
select @totalCount=count(1) from  [dbo].[Hishop_Products] where [integral]=0 and [IntegralToNow]=0 and  [ZeroBuy]=0

/*������ҳ��*/
if @totalCount/@pageSize=0
set @totalPage=@totalCount/@pageSize
else
set @totalPage=@totalCount/@pageSize+1

select top(@pageSize)* from(SELECT row_number() over(order by [ProductId])as Num ,[ProductId],[ProductName],[ThumbnailUrl60],MarketPrice,MinShowPrice AS SalePrice,[IntegralToNow],[IntegralCeiling],[integral],[ZeroBuy]
from [dbo].[Hishop_Products] where [SaleStatus]=1 and ([integral]=0 or [integral] is null)  and ([IntegralToNow]=0 or [IntegralToNow] is null) and ([ZeroBuy] =0 or [ZeroBuy] is  null))as TB where Tb.Num>@pageSize*(@pageIndex-1)
go

/*�޸�ǰ�˷��ർ��·��*/
update VShop_NavMenu set [Content]='/ProductList.aspx?categoryId=1',[ShopMenuPic]='/Storage/template/20150826/6357619820412031473051320.png' where [MenuId]=2

/*�޸�ǰ�˷������ĵ���·��*/
update VShop_NavMenu set [ShopMenuPic]='/Storage/template/20150826/6357619821724642303559837.png'  where [MenuId]=10

/*�޸�ǰ���ҵĵ���·��*/
update VShop_NavMenu set [ShopMenuPic]='/Storage/template/20150826/6357619821971756441565432.png'  where [MenuId]=11



