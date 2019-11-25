using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace IntegralHelp
{
    /// <summary>
    /// 后台数据访问类
    /// </summary>
    public class AdminService
    {
        SqlHelper help = new SqlHelper();

        #region 根据商品id添加零元购商品

        public bool AddZeroBuy(int ZeroBuy, int ProductId)
        {
            try
            {
                string sql = "update [dbo].[Hishop_Products] set [ZeroBuy]=@ZeroBuy where [ProductId]=@ProductId";
                SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@ZeroBuy",ZeroBuy),
                new SqlParameter("@ProductId",ProductId)
                };

                int IsSuccess = help.ExecuteNonQuery(sql, CommandType.Text, param);
                if (IsSuccess > 0)
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

        #region 根据商品添加积分兑换值

        public bool AddPointsFor(int IntegralCeiling, int ProductId)
        {
            try
            {
                string sql = "update [dbo].[Hishop_Products] set [IntegralCeiling]=@IntegralCeiling,[IntegralToNow]=1 where [ProductId]=@ProductId";
                SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@IntegralCeiling",IntegralCeiling),
                new SqlParameter("@ProductId",ProductId)
                };

                int IsSuccess = help.ExecuteNonQuery(sql, CommandType.Text, param);
                if (IsSuccess > 0)
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

        #region 根据商品添加购物送积分值

        public bool AddSendsIntegral(int integral, int ProductId)
        {
            try
            {
                string sql = "update [dbo].[Hishop_Products] set [integral]=@integral where [ProductId]=@ProductId";
                SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@integral",integral),
                new SqlParameter("@ProductId",ProductId)
                };

                int IsSuccess = help.ExecuteNonQuery(sql, CommandType.Text, param);
                if (IsSuccess>0)
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

        #region 分页查询未设置购物送积分和未设置积分兑换的商品
        /// <summary>
        /// 分页查询未设置购物送积分和未设置积分兑换的商品
        /// </summary>
        /// <param name="totalCount"></param>
        /// <param name="totalPage"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageInex"></param>
        /// <returns></returns>
        public DataTable NoSelPointsFor(out int totalCount, out int totalPage, int pageSize = 5, int pageInex = 1)
        {
            string sql = "NoSelPointsFor";
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@totalPage",SqlDbType.Int),
                new SqlParameter("@totalCount",SqlDbType.Int),
                new SqlParameter("@pageSize",pageSize),
                new SqlParameter("@pageIndex",pageInex)
            };
            param[0].Direction = ParameterDirection.Output;
            param[1].Direction = ParameterDirection.Output;
            DataTable dt = help.GetTable(sql, CommandType.StoredProcedure, param);
            //获取输出参数的结果值
            totalPage = param[0] == null ? 0 : (int)param[0].Value;
            totalCount = param[1] == null ? 0 : (int)param[1].Value;
            return dt;
        }
        #endregion

        #region 修改积分兑换标识值为0,积分兑换上限为0,零元购为0

        public int DelPointsForProduct(string ProductId)
        {
            int count = 0;
            try
            {
                string sql = @"update [dbo].[Hishop_Products] set [IntegralToNow]=0,[IntegralCeiling]=0,[ZeroBuy]=0 where [ProductId] in(" + ProductId + ")";
                count = help.ExecuteNonQuery(sql);
                return count;
            }
            catch (Exception)
            {
                return count;
                throw;
            }
        }

        #endregion

        #region 修改送商品积分值为0

        public int DelSendsIntegralProduct(string ProductId)
        {
            int count = 0;
            try
            {
                string sql = @"update [dbo].[Hishop_Products] set [integral]=0 where [ProductId] in("+ProductId+")";
                count = help.ExecuteNonQuery(sql);
                return count;
            }
            catch (Exception)
            {
                return count;
                throw;
            }
        }

        #endregion

        #region 查询购物送积分,积分兑换商品数量

        public DataTable  SelSendIntegralCount()
        {
            DataTable count = null;
            try
            {
                string sql = "select (select count([integral]) from [dbo].[Hishop_Products] where integral!=0)integral,(select count([IntegralToNow]) from [dbo].[Hishop_Products] where [IntegralToNow]=1)IntegralToNow,(select count([ZeroBuy]) from [dbo].[Hishop_Products] where [ZeroBuy]=1)ZeroBuy";
                count = help.GetTable(sql);
                return count;
            }
            catch (Exception)
            {
                return count;
                throw;
            }
        }

        #endregion

        #region 查询零元购商品数量

        

        #endregion
    }
}
