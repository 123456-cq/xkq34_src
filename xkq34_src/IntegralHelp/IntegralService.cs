using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;


namespace IntegralHelp
{
    /// <summary>
    /// 积分数据访问类
    /// </summary>
    public class IntegralService
    {
        SqlHelper help = new SqlHelper();

        #region 根据UserId查询个人现有积分

        public int Points(int UserId)
        {
            int num = 0;
            try
            {
                string sql = "select Points from [dbo].[aspnet_Members] where userid=@userid";
                SqlParameter[] param = new SqlParameter[]
                {
                     new SqlParameter("@UserId",UserId)
                };
                num = help.ExecuteScalar(sql, CommandType.Text, param);
            }
            catch (Exception)
            {
                throw;
            }
            return num;
        }

        #endregion

        #region 查询购物车中商品的积分兑换值

        public DataTable SelIntegralSetInShopCar(int UserId)
        {
            DataTable dt = null;
            try
            {
                string sql = "select [dbo].[Hishop_SKUs].[ProductId],[dbo].[Hishop_ShoppingCarts].[SkuId],[IntegralSet]"
                       + "from [dbo].[Hishop_ShoppingCarts],[dbo].[Hishop_SKUs],[dbo].[PointsForConten]"
                       + "where [dbo].[Hishop_ShoppingCarts].SkuId=[dbo].[Hishop_SKUs].SkuId and [dbo].[PointsForConten].SkuId=[dbo].[Hishop_SKUs].SkuId and [dbo].[Hishop_ShoppingCarts].UserId=PointsForConten.UserId and  [dbo].[Hishop_ShoppingCarts].[UserId]=@UserId";
                SqlParameter[] param = new SqlParameter[]
                {
                     new SqlParameter("@UserId",UserId)
                };
                dt = help.GetTable(sql, CommandType.Text, param);
                return dt;
            }
            catch (Exception)
            {
                return dt;
                throw;
            }
        }

        #endregion

        #region 根据商品编号查询改商品是否能积分兑换

        public bool SelIntegralToNow(int ProductId)
        {
            try
            {
                string sql = "select [IntegralToNow] from [dbo].[Hishop_Products] where [ProductId]=@ProductId";
                SqlParameter[] param = new SqlParameter[]
                {
                    new SqlParameter("@ProductId",ProductId)
                };
                int num = help.ExecuteScalar(sql, CommandType.Text, param);
                if (num == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        #endregion

        #region 查询商品最大兑换积分,现有积分
        /// <summary>
        /// 查询商品最大兑换积分,现有积分
        /// </summary>
        /// <param name="UserId">用户编号</param>
        /// <param name="ProductId">商品编号</param>
        /// <returns></returns>
        public DataTable SelMaxIntegral(int UserId, int ProductId, string SkuId)
        {
            DataTable dt = new DataTable();
            try
            {
                string sql = "select [Points],[IntegralCeiling] from [dbo].[aspnet_Members],[dbo].[Hishop_Products],[dbo].[Hishop_SKUs] where [dbo].[Hishop_Products].[ProductId]=@ProductId and [aspnet_Members].[UserId]=@UserId and [dbo].[Hishop_SKUs].[SkuId]=@SkuId";
                SqlParameter[] param = new SqlParameter[]
                {
                    new SqlParameter("@ProductId",ProductId),
                    new SqlParameter("@UserId",UserId),
                    new SqlParameter("@SkuId",SkuId)
                };
                dt = help.GetTable(sql, CommandType.Text, param);
            }
            catch (Exception)
            {
                return dt;
                throw;
            }
            return dt;
        }

        #endregion

        #region 根据商品编号，商品唯一标识，用户编号，查询积分内容

        public DataTable GetPointsForByPSU(int UserId, int ProductId, string SkuId)
        {
            DataTable dt = new DataTable();
            try
            {
                string sql = "select [PointsForConten],[IntegralSet],[ClosePointsFor] from [dbo].[PointsForConten] where [UserId]=@UserId and [ProductId]=@ProductId and [SkuId]=@SkuId";
                SqlParameter[] param = new SqlParameter[]
                {
                    new SqlParameter("@ProductId",ProductId),
                    new SqlParameter("@UserId",UserId),
                    new SqlParameter("@SkuId",SkuId)
                };
                dt = help.GetTable(sql, CommandType.Text, param);
            }
            catch (Exception)
            {
                return dt;
                throw;
            }
            return dt;
        }

        #endregion

        #region 添加积分兑换内容
        /// <summary>
        /// 添加积分兑换内容
        /// </summary>
        /// <param name="UserId">用户编号</param>
        /// <param name="ProductId">商品编号</param>
        /// <param name="PointsForConten">兑换内容</param>
        /// <param name="IntegralSet">积分兑换值</param>
        /// <returns></returns>
        public bool SaveContent(int UserId, int ProductId, string PointsForConten, string IntegralSet, string SkuId)
        {
            try
            {
                string sq1 = "select count(1) from [dbo].[PointsForConten] where [UserId]=@UserId and [ProductId]=@ProductId and SkuId=@SkuId";//数据库是否已存在此积分兑换
                SqlParameter[] param1 = new SqlParameter[]{
                         new SqlParameter("@UserId",UserId),
                          new SqlParameter("@ProductId",ProductId),
                          new SqlParameter("@SkuId",SkuId)
                };
                int count = help.ExecuteScalar(sq1, CommandType.Text, param1);
                if (count > 0)
                {
                    string sq2 = "delete from [dbo].[PointsForConten] where [UserId]=@UserId and [ProductId]=@ProductId and SkuId=@SkuId";
                    SqlParameter[] param2 = new SqlParameter[]{
                         new SqlParameter("@UserId",UserId),
                          new SqlParameter("@ProductId",ProductId),
                            new SqlParameter("@SkuId",SkuId)
                };
                    int counts = help.ExecuteScalar(sq2, CommandType.Text, param2);
                }
                string sql = "insert into [dbo].[PointsForConten] values(@PointsForConten,@UserId,@ProductId,@IntegralSet,@SkuId,1)";
                SqlParameter[] param = new SqlParameter[]{
                        new SqlParameter("@PointsForConten",PointsForConten),
                         new SqlParameter("@UserId",UserId),
                          new SqlParameter("@ProductId",ProductId),
                           new SqlParameter("@IntegralSet",IntegralSet),
                            new SqlParameter("@SkuId",SkuId)
                };
                int num = help.ExecuteNonQuery(sql, CommandType.Text, param);
                if (num > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        #endregion

        #region 查询积分兑换内容

        public DataTable SelPointsForConten(int UserId)
        {
            DataTable dt = null;
            try
            {
                string sql = "select [PointsForConten],[ProductId],[UserId],[IntegralSet] from [dbo].[PointsForConten] where [UserId]=@UserId";
                SqlParameter[] param = new SqlParameter[]{
                      new SqlParameter("@UserId",UserId)
                };
                dt = help.GetTable(sql, CommandType.Text, param);
            }
            catch (Exception)
            {
                return dt;
                throw;
            }
            return dt;
        }

        #endregion

        #region 删除积分兑换

        public bool DeletePointsFor(int UserId, string SkuId)
        {
            try
            {
                string sql = "select count(1) from [dbo].[PointsForConten] where [UserId]=@UserId  and SkuId=@SkuId";
                SqlParameter[] param = new SqlParameter[]{
                         new SqlParameter("@UserId",UserId),
                            new SqlParameter("@SkuId",SkuId)
                };
                int num = help.ExecuteScalar(sql, CommandType.Text, param);
                if (num > 0)
                {
                    string sq2 = "delete from [dbo].[PointsForConten] where [UserId]=@UserId  and SkuId=@SkuId";
                    SqlParameter[] param2 = new SqlParameter[]{
                         new SqlParameter("@UserId",UserId),
                            new SqlParameter("@SkuId",SkuId)
                };
                    int counts = help.ExecuteNonQuery(sq2, CommandType.Text, param2);
                    if (counts > 0)
                    {
                        return true;
                    }
                    return false;
                }
                else
                {
                    return true;
                }


            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        #endregion

        #region 根据订单编号修改金额

        public bool UpdateMoneyByOrderId(string OrderId, decimal OrderTotal)
        {
            try
            {
                string sql = "update [dbo].[Hishop_Orders] set [OrderTotal]=@OrderTotal where [OrderId]=@OrderId";
                SqlParameter[] param2 = new SqlParameter[]{
                         new SqlParameter("@OrderTotal",OrderTotal),
                            new SqlParameter("@OrderId",OrderId)
                };
                int counts = help.ExecuteNonQuery(sql, CommandType.Text, param2);
                if (counts > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        #endregion

        #region 根据用户编号修改个人积分

        public bool UpdatePointByUserId(int Points, int UserId)
        {
            try
            {
                string sql = "update [dbo].[aspnet_Members] set [Points]=@Points where [UserId]=@UserId";
                SqlParameter[] param = new SqlParameter[]{
                  new SqlParameter("@Points",Points),
                  new SqlParameter("@UserId",UserId)
                 };
                int counts = help.ExecuteNonQuery(sql, CommandType.Text, param);
                if (counts > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        #endregion

        #region 添加购物送积分记录(收入)
        /// <summary>
        /// 添加购物送积分记录(收入)
        /// </summary>
        /// <param name="IntegralChange">送积分值</param>
        /// <param name="UserId">用户编号</param>
        /// <returns></returns>
        public bool ShopSendIntegral(decimal IntegralChange, int UserId)
        {
            try
            {
                string sql = "insert into vshop_IntegralDetail values (1,'购物送积分',@IntegralChange,null,@UserId,null,getdate(),1)";
                SqlParameter[] param = new SqlParameter[]{
                  new SqlParameter("@IntegralChange",IntegralChange),
                  new SqlParameter("@UserId",UserId)
                 };
                int counts = help.ExecuteNonQuery(sql, CommandType.Text, param);
                if (counts > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        #endregion

        #region 添加积分兑换记录（支出）
        /// <summary>
        /// 添加积分兑换记录
        /// </summary>
        /// <param name="orderId">订单号</param>
        /// <param name="IntegralChange">积分兑换值</param>
        /// <param name="UserId">用户编号</param>
        /// <returns></returns>
        public bool InsertIntegral(string orderId, decimal IntegralChange, int UserId)
        {
            try
            {
                string sql = "insert into vshop_IntegralDetail values (2,@IntegralSource,@IntegralChange,null,@UserId,null,getdate(),5)";
                SqlParameter[] param = new SqlParameter[]{
                  new SqlParameter("@IntegralSource","积分兑换-订单号："+orderId),
                  new SqlParameter("@IntegralChange",IntegralChange),
                  new SqlParameter("@UserId",UserId)
                 };
                int counts = help.ExecuteNonQuery(sql, CommandType.Text, param);
                if (counts > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        #endregion

        #region 添加积分兑换金额记录

        public bool InsertPointsForMoney(int PointsForMoney, int UserId, int type)
        {
            try
            {
                string sql = "insert into [dbo].[PointsForMoney] values(@PointsForMoney,@UserId,@time,@type)";
                SqlParameter[] param = new SqlParameter[]{
                  new SqlParameter("@PointsForMoney",PointsForMoney),
                  new SqlParameter("@UserId",UserId),
                  new SqlParameter("@time",DateTime.Now.ToString()),
                      new SqlParameter("@type",type)
                 };
                int counts = help.ExecuteNonQuery(sql, CommandType.Text, param);
                if (counts > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        #endregion

        #region 根据用户编号查询积分兑换金额记录

        public int SelPointsForMoneyById(int UserId)
        {
            try
            {
                string sql = "select PointsForMoney from [dbo].[PointsForMoney] where [UserId]=@UserId";
                SqlParameter[] param = new SqlParameter[]{
                  new SqlParameter("@UserId",UserId)
                 };
                int num = help.ExecuteScalar(sql, CommandType.Text, param);
                return num;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region 删除积分金额记录

        public bool DeletePointsFor(int UserId)
        {
            try
            {
                string sql = "delete from [dbo].[PointsForMoney] where UserId=@UserId";
                SqlParameter[] param = new SqlParameter[]{
                  new SqlParameter("@UserId",UserId)
                 };
                int num = help.ExecuteNonQuery(sql, CommandType.Text, param);
                if (num >= 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        #endregion

        #region 根据订单编号修改订单金额

        public bool UpdateOrderTotal(string OrderId, decimal OrderTotal)
        {
            try
            {
                string sql = "update [dbo].[Hishop_Orders] set [OrderTotal]=@OrderTotal,[OrderProfit]=@OrderProfit where OrderMarking=@OrderId";
                SqlParameter[] param = new SqlParameter[]{
                    new SqlParameter("@OrderId",OrderId),
                    new SqlParameter("@OrderTotal",OrderTotal),
                  new SqlParameter("@OrderProfit",OrderTotal)
                 };
                int num = help.ExecuteNonQuery(sql, CommandType.Text, param);
                if (num > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
                throw;
            }

        }

        #endregion

        #region 根据订单号查询商品信息

        public DataTable SelProductInfoByOrderId(string OrderId)
        {
            DataTable dt = null;
            try
            {
                string sql = "SELECT OrderId, ThumbnailsUrl, ItemDescription, SKUContent, SKU,OrderItemsStatus, ProductId,Quantity,ReturnMoney,SkuID FROM Hishop_OrderItems WHERE OrderId IN (SELECT OrderId FROM Hishop_Orders WHERE OrderMarking = @OrderId)";
                SqlParameter[] param = new SqlParameter[]{
                    new SqlParameter("@OrderId",OrderId)
                 };
                dt = help.GetTable(sql, CommandType.Text, param);
                return dt;
            }
            catch (Exception)
            {
                return dt;
                throw;
            }
        }

        #endregion

        #region 根据商品编号查询送积分值

        public int SelIntegralById(int ProductId)
        {
            try
            {
                string sql = "select integral from Hishop_Products where ProductId=@ProductId";
                SqlParameter[] param = new SqlParameter[]{
                    new SqlParameter("@ProductId",ProductId)
                 };
                int num = help.ExecuteScalar(sql, CommandType.Text, param);
                return num;
            }
            catch (Exception)
            {
                return 0;
                throw;
            }
        }

        #endregion

        #region 删除积分兑换内容

        public void ClearPoints(int UserId)
        {
            try
            {
                string sql = "delete from [dbo].[PointsForConten] where [UserId]=@UserId";
                SqlParameter[] param = new SqlParameter[]{
                    new SqlParameter("@UserId",UserId)
                 };
                int num = help.ExecuteNonQuery(sql, CommandType.Text, param);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region 删除立即购买积分兑换金额

        public bool DelShoppointdfor(int UserId)
        {
            try
            {
                string sql = "delete from [dbo].[PointsForMoney] where [UserId]=1 and [Type]=0";
                SqlParameter[] param = new SqlParameter[]{
                  new SqlParameter("@UserId",UserId)
                 };
                int num = help.ExecuteNonQuery(sql, CommandType.Text, param);
                if (num >= 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        #endregion
    }
}
