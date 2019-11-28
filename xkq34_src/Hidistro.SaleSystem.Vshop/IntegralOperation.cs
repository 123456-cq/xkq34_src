using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Hidistro.SaleSystem.Vshop
{
    public class IntegralOperation
    {
        SqlHelper help = new SqlHelper();

        #region 查询商品积分信息
        public DataTable GetProductIntegralInfo(int productId)
        {
            DataTable dt = null;
            try
            {
                string sql = "select [integral],[IntegralToNow],[IntegralCeiling],[FristPrice] from [dbo].[Hishop_Products] left join [dbo].[Hishop_FreightTemplate_SpecifyRegionGroups] on [TemplateId]=[FreightTemplateId]  where [ProductId]=@ProductId ";
                SqlParameter[] param = new SqlParameter[]{
                             new SqlParameter("@ProductId",productId)
                        };
                dt = help.GetTable(sql,CommandType.Text,param);
            }
            catch (Exception)
            {
                return dt;
                throw;
            }
            return dt;
        }
        #endregion

    }
}
