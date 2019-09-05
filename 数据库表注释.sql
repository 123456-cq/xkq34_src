--管理员用户表
select * from aspnet_Managers
--会员表
select * from aspnet_MemberGrades
--用户表
select * from aspnet_Members
--物流公司表
select * from Hishop_ExpressTemplates
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
--商品表(integral:加积分，IntegralToNow:是否能积分兑换（1能,0否）,IntegralCeiling:积分兑换上限)
select * from Hishop_Products
--购物车表
select * from Hishop_ShoppingCarts
select * from [dbo].[Hishop_SKUs]
--积分变动表
select * from vshop_IntegralDetail
--积分兑换内容表
select * from [dbo].[PointsForConten]
--购物车地址信息表
select * from Hishop_UserShippingAddresses

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



--根据商品编号修改/查询送积分
update Hishop_products set [integral]='@integral' where productId='@productId '
select [integral] from [dbo].[Hishop_Products] where [ProductId]='@ProductId'
--根据商品编号修改/查询积分兑换状态(0:否;1:是)
update Hishop_products set [IntegralToNow]='@IntegralToNow' where [ProductId]='@ProductId'
select [integral],[IntegralToNow] from [dbo].[Hishop_Products] where [ProductId]='@ProductId'
--获取总积分
SELECT * FROM vshop_IntegralDetail WHERE Userid = 11729 and IntegralChange>0
update [dbo].[vshop_IntegralDetail] set  [IntegralChange]=10,[IntegralSourceType]=2,[IntegralStatus]=5 WHERE Userid = 11729 and IntegralChange>0

--获取剩余积分(points)
SELECT * FROM aspnet_Members WHERE UserId = @UserId

insert into [dbo].[PointsForConten] values('<li>（北斗复兴手机 摩卡金）积分兑换：<span style=color:#ed5565;font-size:20px>-￥1</span></li>',1,1,'111')
insert into [dbo].[PointsForConten]([PointsForConten], [UserId], [ProductId], [IntegralSet]) values('<li>（北斗复兴手机 摩卡金）积分兑换：<span style=color:#ed5565;font-size:20px>-￥1</span></li>',1,1,'111')
delete from [dbo].[PointsForConten] where [UserId]=@UserId and [ProductId]=@ProductId

select [UserId],[CategoryId] from [dbo].[Hishop_ShoppingCarts]

select * from Hishop_ShoppingCarts

select  [PointsForConten],[IntegralSet],[dbo].[Hishop_SKUs].ProductId,[dbo].[Hishop_SKUs].SkuId  
from [dbo].[PointsForConten],[dbo].[Hishop_SKUs],[dbo].[Hishop_ShoppingCarts] 
where [dbo].[Hishop_SKUs].SkuId=[dbo].[Hishop_ShoppingCarts].SkuId
and [dbo].[PointsForConten].ProductId=[dbo].[Hishop_SKUs].ProductId
and [dbo].[PointsForConten].UserId=[dbo].[Hishop_ShoppingCarts].UserId

select [PointsForConten],[ProductId],[UserId],[IntegralSet] from [dbo].[PointsForConten] where [UserId]=@UserId

select [Points],[IntegralCeiling],[IntegralSet] from [dbo].[aspnet_Members],[dbo].[Hishop_Products],[dbo].[PointsForConten] where [dbo].[Hishop_Products].[ProductId]=@ProductId and [aspnet_Members].[UserId]=@UserId and PointsForConten.[SkuId]=@SkuId
select [PointsForConten],[IntegralSet],[ClosePointsFor] from [dbo].[PointsForConten] where [UserId]=@UserId and [ProductId]=@ProductId and [SkuId]=@SkuId

select [Points],[IntegralCeiling] from [dbo].[aspnet_Members],[dbo].[Hishop_Products],[dbo].[Hishop_SKUs]where [dbo].[Hishop_Products].[ProductId]=@ProductId and [aspnet_Members].[UserId]=@UserId and [dbo].[Hishop_SKUs].[SkuId]=@SkuId

select count(1) from [dbo].[PointsForConten] where [UserId]=@UserId and [ProductId]=@ProductId and SkuId=@SkuId

select * from [dbo].[PointsForConten]

delete from [dbo].[PointsForConten] where [UserId]=2 and [ProductId]=7 and SkuId='7_4'

select [IntegralToNow] from [dbo].[Hishop_Products] where [ProductId]=@ProductId

select [IntegralToNow] from [dbo].[Hishop_Products] where [ProductId]=18

--获取所有用户信息
SELECT * FROM aspnet_Members WHERE UserId = 2

--
SELECT * FROM Hishop_LimitedTimeDiscount WHERE LimitedTimeDiscountId = 0 and Status!=2

select [dbo].[Hishop_SKUs].[ProductId],[dbo].[Hishop_ShoppingCarts].[SkuId],[IntegralSet]
from [dbo].[Hishop_ShoppingCarts],[dbo].[Hishop_SKUs],[dbo].[PointsForConten]
where [dbo].[Hishop_ShoppingCarts].SkuId=[dbo].[Hishop_SKUs].SkuId and [dbo].[PointsForConten].SkuId=[dbo].[Hishop_SKUs].SkuId and [dbo].[Hishop_ShoppingCarts].[UserId]=@UserId

delete from [dbo].[PointsForConten] where [UserId]=@UserId and [SkuId]=@SkuId

select count(1) from [dbo].[PointsForConten] where [UserId]=2 and [ProductId]=0 and SkuId=@SkuId
select * from [dbo].[aspnet_Members] where [UserName]='cq'
update [dbo].[aspnet_Members] set [Points]=200 where [UserName]='cq'

select count(1) from [dbo].[PointsForConten] where [UserId]=11729  and SkuId='7_4'

SELECT * FROM Hishop_UserShippingAddresses WHERE  UserID =11729

SELECT * FROM Hishop_LimitedTimeDiscount WHERE LimitedTimeDiscountId = @ID and Status!=2

SELECT m.*,c.IsAllProduct,c.MemberGrades from  Hishop_Coupon_MemberCoupons as m left join Hishop_Coupon_Coupons as c on c.CouponId=m.CouponId WHERE m.MemberId = 1127 and m.BeginDate<=getdate() and getdate()<=m.EndDate and m.Status=0

select [dbo].[Hishop_SKUs].[ProductId],[dbo].[Hishop_ShoppingCarts].[SkuId],[IntegralSet],PointsForConten.userId 
from [dbo].[Hishop_ShoppingCarts],[dbo].[Hishop_SKUs],[dbo].[PointsForConten]
where [dbo].[Hishop_ShoppingCarts].SkuId=[dbo].[Hishop_SKUs].SkuId and [dbo].[PointsForConten].SkuId=[dbo].[Hishop_SKUs].SkuId 
and [dbo].[Hishop_ShoppingCarts].UserId=PointsForConten.UserId
and [dbo].[Hishop_ShoppingCarts].[UserId]=11729

select [Points],[IntegralCeiling] from [dbo].[aspnet_Members],[dbo].[Hishop_Products],[dbo].[Hishop_SKUs] where   [aspnet_Members].[UserId]=1

select Points from [dbo].[aspnet_Members] where userid=@userid

insert into [dbo].[PointsForConten] values('',1,1,'111')

--根据用户编号修改个人积分
update [dbo].[aspnet_Members] set [Points]=@Points where [UserId]=@UserId

SELECT * from  Hishop_Activities  WHERE  StartTime<=getdate() and getdate()<=EndTIme  



select * from Hishop_Orders where Username='cq'
select * from Hishop_Orders where [OrderId]='190821170418875'


--根据订单编号修改订单总金额
update [dbo].[Hishop_Orders] set [OrderProfit]=123 where Username='cq'
--根据用户编号修改个人积分
update [dbo].[aspnet_Members] set [Points]=@Points where [UserId]=@UserId
--购物送积分
insert into vshop_IntegralDetail values (1,'购物送积分',@IntegralChange,null,@UserId,null,getdate(),1)
--积分记录表添加积分兑换记录
insert into vshop_IntegralDetail values (2,@[IntegralSource],@IntegralChange,null,@UserId,null,getdate(),5)

insert into [dbo].[PointsForMoney] values(@PointsForMoney,@UserId,@SkuId,@ProductId);

update [dbo].[Hishop_Orders] set [OrderTotal]=@OrderTotal and [OrderProfit]=@OrderProfit where [OrderId]=@OrderId

SELECT OrderId,OrderMarking, OrderDate, OrderStatus,PaymentTypeId, OrderTotal,   Gateway,
(SELECT count(0) FROM vshop_OrderRedPager WHERE OrderId = o.OrderId and ExpiryDays<getdate() and AlreadyGetTimes<MaxGetTimes) as HasRedPage,
(SELECT SUM(Quantity) FROM Hishop_OrderItems WHERE OrderId = o.OrderId) as ProductSum FROM Hishop_Orders o WHERE UserId = 11729 ORDER BY OrderDate DESC
sELECT OrderId, ThumbnailsUrl, ItemDescription, SKUContent, SKU,OrderItemsStatus, ProductId,Quantity,ReturnMoney,SkuID FROM Hishop_OrderItems WHERE OrderId IN (SELECT OrderId FROM Hishop_Orders 
WHERE UserId = 11729)

SELECT OrderId,OrderMarking, OrderDate, OrderStatus,PaymentTypeId, OrderTotal,   Gateway,
(SELECT count(0) FROM vshop_OrderRedPager WHERE OrderId = o.OrderId and ExpiryDays<getdate() and AlreadyGetTimes<MaxGetTimes) as HasRedPage,
(SELECT SUM(Quantity) FROM Hishop_OrderItems WHERE OrderId = o.OrderId) as ProductSum FROM Hishop_Orders o WHERE OrderId = '190826202939554' ORDER BY OrderDate DESC
sELECT OrderId, ThumbnailsUrl, ItemDescription, SKUContent, SKU,OrderItemsStatus, ProductId,Quantity,ReturnMoney,SkuID FROM Hishop_OrderItems WHERE OrderId IN (SELECT OrderId FROM Hishop_Orders 
WHERE OrderId = '190826202939554')

select integral,[IntegralToNow],[IntegralCeiling] from Hishop_Products where ProductId=@ProductId

select * from vshop_IntegralDetail where UserId=11729
delete from vshop_IntegralDetail where UserId=11729
update [dbo].[aspnet_Members] set [Points]=200 where UserId=11729

select * from  Hishop_Orders where [UserId]=11729
delete  from Hishop_Orders where [UserId]=11729

select * from [dbo].[PointsForMoney] 
delete from [dbo].[PointsForMoney]


select Points from [dbo].[aspnet_Members] where userid=11729

select * from Hishop_ShoppingCarts

delete from Hishop_ShoppingCarts

delete from [dbo].[PointsForConten] where [UserId]=@UserId

update [dbo].[Hishop_Orders] set [OrderTotal]=1,[OrderProfit]=1 where [OrderId]=190827100905282
select * from  Hishop_Orders where OrderMarking=190827100905282

SELECT OrderId, ThumbnailsUrl, ItemDescription, SKUContent, SKU,OrderItemsStatus, ProductId,Quantity,ReturnMoney,SkuID FROM Hishop_OrderItems WHERE OrderId IN (SELECT OrderId FROM Hishop_Orders WHERE OrderMarking =190827100905282 )
insert into [dbo].[PointsForMoney] values(10,1,2002-01-10,0)

delete from [dbo].[PointsForMoney] where [UserId]=1 and [Type]=0


