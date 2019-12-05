--管理员用户表
select * from aspnet_Managers
--会员等级表
select * from aspnet_MemberGrades
--用户表
select * from aspnet_Members
--物流公司表
select * from Hishop_ExpressTemplates
--运费模板表
select * from Hishop_FreightTemplate_Templates
--运费详情表
select * from Hishop_FreightTemplate_SpecifyRegionGroups
--订单详情表
select * from Hishop_OrderItems
--订单记录表
select * from Hishop_Orders
--评论表
select * from Hishop_OrderReturns
--交易成功表
select * from Hishop_OrderSendNote
--积分兑换记录表
select * from Hishop_PointExchange_Changed
--商品表(integral:商品送积分值，IntegralToNow:是否能积分兑换（1能,0否）,IntegralCeiling:积分兑换上限,ZeroBuy:零元购标识（0标识否，1标识是）)
select * from Hishop_Products
--购物车表
select * from Hishop_ShoppingCarts
select * from [dbo].[Hishop_SKUs]
--积分变动表
select * from vshop_IntegralDetail
--购物车地址信息表
select * from Hishop_UserShippingAddresses
--积分兑换内容表
select * from [dbo].[PointsForConten]
--积分兑换金额信息表
select * from [dbo].[PointsForMoney]
--商品分类表
select * from [dbo].[Hishop_Categories]
--商品类型表
select * from [dbo].[Hishop_ProductTypes]
--商品品牌表
select * from [dbo].[Hishop_BrandCategories]
--导航栏表
select * from VShop_NavMenu


--在商品表中添加积分字段
alter table [dbo].[Hishop_Products] add integral		int null  --添加integral（商品送积分值）字段
alter table [dbo].[Hishop_Products] add IntegralToNow	int null  --添加IntegralToNow（是否能积分兑换（1能,0否））字段
alter table [dbo].[Hishop_Products] add IntegralCeiling int null  --添加IntegralCeiling（积分兑换上限）字段
alter table [dbo].[Hishop_Products] add ZeroBuy		    int null  --添加ZeroBuy（零元购标识（0标识否，1标识是））字段
alter table [dbo].[Hishop_Products] add LeaseGood		int null  --添加LeaseGood（租赁商品标识（0标识否，1标识是））字段
go


--新建积分兑换内容表
if exists(select * from sysobjects where name='PointsForConten')
drop table PointsForConten
go
    create table PointsForConten
    (
        id					int             not null    identity(1,1)    primary key ,    
        PointsForConten		varchar(110)		null,		--积分兑换内容
        UserId				int					null,		--用户编号
        ProductId			int			    not null,		--商品编号
        IntegralSet			varchar(50)			null,		--积分兑换值
        SkuId				nvarchar(100)   not null,		--商品唯一标识
        ClosePointsFor		int					null		--是否开启积分兑换（1：是 0：否）
    )
go

--新建积分兑换金额信息表
if exists(select * from sysobjects where name='PointsForMoney')
drop table PointsForMoney
go
    create table PointsForMoney
    (
        id					int             not null    identity(1,1)    primary key ,    
        PointsForMoney		int				not	null,		--积分兑换金额
        UserId				int				not	null,		--用户编号
        [Time]				datetime	    not null,		--入库时间
        [Type]				int				not	null		--购买类型（0：立即购买 1：购物车结算）
    )
go



--修改ss_ShoppingCart_GetItemInfo存储过程
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
-- 会员查询
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
	
 -- 返回商品基本信息
SELECT ProductId, SaleStatus, @SKU as SKU, @Stock as Stock, @Quantity as TotalQuantity, ProductName, CategoryId, @Weight AS [Weight], @SalePrice AS SalePrice, 
	ThumbnailUrl40,ThumbnailUrl60,ThumbnailUrl100,ThumbnailUrl160, ThumbnailUrl180, ThumbnailUrl220,IsfreeShipping,MainCategoryPath,IsSetCommission,ThirdCommission,SecondCommission,FirstCommission,FreightTemplateId,CubicMeter,FreightWeight,[IntegralToNow] FROM Hishop_Products WHERE ProductId = @ProductId AND SaleStatus=1
-- 返回当前规格信息
SELECT s.SkuId, s.SKU, s.ProductId, s.Stock, AttributeName, ValueStr FROM Hishop_SKUs s left join Hishop_SKUItems si on s.SkuId = si.SkuId
left join Hishop_Attributes a on si.AttributeId = a.AttributeId left join Hishop_AttributeValues av on si.ValueId = av.ValueId WHERE s.SkuId = @SkuId
AND s.ProductId IN (SELECT ProductId FROM Hishop_Products WHERE SaleStatus=1)
--返回积分兑换信息
select ProductNumber,PointNumber,ChangedNumber,eachMaxNumber,[status],exChangeId from Hishop_PointExChange_Products where exChangeId=@ExChangeId and ProductId=@ProductId
GO



--添加购物送积分视图
if exists (select * from sysobjects where name = 'vw_Hishop_SendsIntegralProductList')
 drop view vw_Hishop_SendsIntegralProductList
 go
 --创建视图
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


--添加积分兑换视图
if exists (select * from sysobjects where name = 'vw_Hishop_PointsForProductList')
 drop view vw_Hishop_PointsForProductList
 go
 --创建视图
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

--添加零元购视图
if exists (select * from sysobjects where name = 'vw_Hishop_ZeroByProductList')
 drop view vw_Hishop_ZeroByProductList
 go
 --创建视图
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


--分页查询未设置购物送积分和未设置积分兑换的商品
if exists (select * from sys.procedures where name='NoSelPointsFor')
drop proc NoSelPointsFor
go
create proc NoSelPointsFor
@totalPage int output,--总页数
@totalCount int output,--总条数
@pageSize int,--页大小
@pageIndex int--当前页
as

/*获取总条数*/
select @totalCount=count(1) from  [dbo].[Hishop_Products] where [integral]=0 and [IntegralToNow]=0 and  [ZeroBuy]=0

/*计算总页数*/
if @totalCount/@pageSize=0
set @totalPage=@totalCount/@pageSize
else
set @totalPage=@totalCount/@pageSize+1

select top(@pageSize)* from(SELECT row_number() over(order by [ProductId])as Num ,[ProductId],[ProductName],[ThumbnailUrl60],MarketPrice,MinShowPrice AS SalePrice,[IntegralToNow],[IntegralCeiling],[integral],[ZeroBuy]
from [dbo].[Hishop_Products] where [SaleStatus]=1 and ([integral]=0 or [integral] is null)  and ([IntegralToNow]=0 or [IntegralToNow] is null) and ([ZeroBuy] =0 or [ZeroBuy] is  null))as TB where Tb.Num>@pageSize*(@pageIndex-1)
go

/*修改前端分类导航路径*/
update VShop_NavMenu set [Content]='/ProductList.aspx?categoryId=1',[ShopMenuPic]='/Storage/template/20150826/6357619820412031473051320.png' where [MenuId]=2

/*修改前端分销中心导航路径*/
update VShop_NavMenu set [ShopMenuPic]='/Storage/template/20150826/6357619821724642303559837.png'  where [MenuId]=10

/*修改前端我的导航路径*/
update VShop_NavMenu set [ShopMenuPic]='/Storage/template/20150826/6357619821971756441565432.png'  where [MenuId]=11



